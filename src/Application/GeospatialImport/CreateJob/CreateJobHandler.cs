using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.GeospatialImportJob;
using EdgIQ.MurimiOS.Application.Common.Imports;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.GeospatialImport.CreateJob;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using GeospatialImportFeature = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportFeature;
using GeospatialImportFormat = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportFormat;
using GeospatialImportJob = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportJob;
using GeospatialImportStatus = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportStatus;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;

public sealed class CreateJobHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IMediaStorage storage, GeospatialImportParser parser, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock) : IRequestHandler<CreateJobCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateJobCommand message, CancellationToken cancellationToken) => CreateJobAsync(message.farmId, message.request, db, scope, storage, parser, tenant, actor, clock, cancellationToken);

    private static async Task<UseCaseResult> CreateJobAsync(Guid farmId, IncomingContent request, IPlatformDbContext db,
        IOrganisationScopeAccess scope, IMediaStorage storage, GeospatialImportParser parser,
        ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, CancellationToken cancellationToken)
    {
        if (!await scope.CanAccessFarmAsync(farmId, cancellationToken)
            || !await db.Farms.AnyAsync(farm => farm.Id == farmId, cancellationToken)) return UseCaseResults.NotFound();
        string formatValue = request.FirstQuery("format") ?? request.FirstHeader("X-Import-Format") ?? string.Empty;
        if (!Enum.TryParse(formatValue, true, out GeospatialImportFormat format) || !Enum.IsDefined(format))
            return Bad("invalid_import_format", "Use GeoJson, Kml, Shapefile, or IsoXml.");
        string fileName = SanitizeFileName(request.FirstQuery("fileName")
            ?? request.FirstHeader("X-File-Name"));
        if (fileName.Length == 0) return Bad("file_name_required", "Supply fileName or the X-File-Name header.");
        if (request.ContentLength is > MaximumBytes) return TooLarge();

        byte[] bytes;
        try { bytes = await ReadLimitedAsync(request.Body, MaximumBytes, cancellationToken); }
        catch (ImportTooLargeException) { return TooLarge(); }
        if (bytes.Length == 0) return Bad("empty_import", "The request body is empty.");
        string sha256 = Convert.ToHexStringLower(SHA256.HashData(bytes));
        GeospatialImportJob? existing = await db.GeospatialImportJobs.AsNoTracking()
            .FirstOrDefaultAsync(job => job.FarmId == farmId && job.Sha256 == sha256 && job.Format == format, cancellationToken);
        if (existing is not null) return UseCaseResults.Ok(ToResponse(existing));

        Guid jobId = Guid.NewGuid();
        string contentType = (request.ContentType ?? ContentTypeFor(format)).Split(';')[0].Trim().ToLowerInvariant();
        await using var source = new MemoryStream(bytes, writable: false);
        string objectKey = await storage.PutImportAsync(tenant.TenantId, jobId, contentType, source, bytes.Length, cancellationToken);
        var job = new GeospatialImportJob(jobId, farmId, format, fileName, contentType, bytes.Length, sha256,
            objectKey, GeospatialImportParser.Version);
        db.GeospatialImportJobs.Add(job);

        DateTimeOffset now = clock.GetUtcNow();
        GeospatialParseResult parsed = parser.Parse(format, bytes);
        if (parsed.FileIssues.Count > 0)
        {
            GeospatialValidationIssue issue = parsed.FileIssues[0];
            job.Fail(issue.Code, issue.Message, now);
        }
        else
        {
            int warningCount = 0;
            int errorCount = 0;
            foreach (ParsedGeospatialFeature parsedFeature in parsed.Features)
            {
                warningCount += parsedFeature.Issues.Count(issue => issue.Severity == "warning");
                errorCount += parsedFeature.Issues.Count(issue => issue.Severity == "error");
                db.GeospatialImportFeatures.Add(new GeospatialImportFeature(Guid.NewGuid(), job.Id,
                    parsedFeature.Ordinal, parsedFeature.Name, parsedFeature.Boundary, parsedFeature.PropertiesJson,
                    JsonSerializer.Serialize(parsedFeature.Issues, JsonOptions), parsedFeature.IsEligible));
            }
            job.CompleteValidation(parsed.Features.Count, parsed.Features.Count(feature => feature.IsEligible),
                warningCount, errorCount, now);
        }
        AddAudit(db, tenant, actor, now, "Validated", job.Id, farmId, new
        {
            job.Format, job.FileName, job.Sha256, job.Status, job.FeatureCount, job.ErrorCount, job.WarningCount
        });
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, "GeospatialImport.Validated",
            JsonSerializer.Serialize(new { job.Id, job.FarmId, job.Format, job.Status, job.FeatureCount }), now));
        await db.SaveChangesAsync(cancellationToken);
        return UseCaseResults.Created($"/api/v1/imports/geospatial/jobs/{job.Id}", ToResponse(job));
    }

    private const long MaximumBytes = 52_428_800;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);


    private static GeospatialImportJobResponse ToResponse(GeospatialImportJob job) => new(job.Id, job.FarmId,
        job.Format, job.FileName, job.ContentType, job.ByteSize, job.Sha256, job.ParserVersion, job.Status,
        job.FeatureCount, job.EligibleFeatureCount, job.WarningCount, job.ErrorCount, job.FailureCode,
        job.FailureDetail, job.ValidatedAt, job.AppliedAt, job.AppliedBy, job.CreatedAtUtc);


    private static void AddAudit(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor,
        DateTimeOffset at, string action, Guid entityId, Guid farmId, object changes) =>
        db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, at, actor.Subject, actor.MembershipId,
            action, typeof(GeospatialImportJob).FullName!, entityId, farmId, null,
            JsonSerializer.Serialize(changes, JsonOptions), System.Diagnostics.Activity.Current?.TraceId.ToString()));


    private static async Task<byte[]> ReadLimitedAsync(Stream input, long maximum, CancellationToken cancellationToken)
    {
        using var output = new MemoryStream();
        byte[] buffer = new byte[64 * 1024];
        while (true)
        {
            int read = await input.ReadAsync(buffer, cancellationToken);
            if (read == 0) break;
            if (output.Length + read > maximum) throw new ImportTooLargeException();
            await output.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
        }
        return output.ToArray();
    }


    private static string SanitizeFileName(string? value) => string.Concat(Path.GetFileName(value?.Trim() ?? string.Empty)
        .Where(character => !char.IsControl(character))).Trim();

    private static string ContentTypeFor(GeospatialImportFormat format) => format switch
    {
        GeospatialImportFormat.GeoJson => "application/geo+json",
        GeospatialImportFormat.Kml => "application/vnd.google-earth.kml+xml",
        GeospatialImportFormat.Shapefile => "application/zip",
        GeospatialImportFormat.IsoXml => "application/zip",
        _ => "application/octet-stream"
    };

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult TooLarge() => UseCaseResults.Problem(statusCode: 413,
        title: "Import file too large", extensions: new Dictionary<string, object?> { ["code"] = "import_too_large", ["maximumBytes"] = MaximumBytes });

    private sealed class ImportTooLargeException : Exception;

}
