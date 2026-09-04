using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.FarmField;
using EdgIQ.MurimiOS.Domain.GeospatialImportJob;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.GeospatialImport.Apply;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
using FieldBoundaryVersion = global::EdgIQ.MurimiOS.Domain.FarmField.FieldBoundaryVersion;
using GeospatialImportFeature = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportFeature;
using GeospatialImportFormat = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportFormat;
using GeospatialImportJob = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportJob;
using GeospatialImportStatus = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportStatus;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class ApplyHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock) : IRequestHandler<ApplyCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ApplyCommand message, CancellationToken cancellationToken) => ApplyAsync(message.jobId, message.request, db, scope, tenant, actor, clock, cancellationToken);

    private static async Task<UseCaseResult> ApplyAsync(Guid jobId, ApplyGeospatialImportRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor,
        TimeProvider clock, CancellationToken cancellationToken)
    {
        GeospatialImportJob? job = await db.GeospatialImportJobs.SingleOrDefaultAsync(value => value.Id == jobId, cancellationToken);
        if (job is null || !await scope.CanAccessFarmAsync(job.FarmId, cancellationToken)) return UseCaseResults.NotFound();
        if (job.Status == GeospatialImportStatus.Applied) return UseCaseResults.Ok(ToResponse(job));
        if (job.Status != GeospatialImportStatus.Validated)
            return UseCaseResults.Conflict(new { code = "import_not_validated", message = "Only a validated import can be applied." });

        GeospatialImportFeature[] features = await db.GeospatialImportFeatures
            .Where(feature => feature.JobId == jobId && feature.IsEligible).OrderBy(feature => feature.Ordinal)
            .ToArrayAsync(cancellationToken);
        if (request.Features.Count != features.Length
            || request.Features.Select(mapping => mapping.FeatureId).Distinct().Count() != features.Length
            || features.Any(feature => request.Features.All(mapping => mapping.FeatureId != feature.Id)))
            return Bad("incomplete_feature_mapping", "Map every eligible preview feature exactly once.");

        ApplyGeospatialImportFeature[] mappings = request.Features.ToArray();
        if (mappings.Any(mapping => mapping.ValidFrom == default))
            return Bad("valid_from_required", "Every imported boundary requires a valid-from date.");
        Guid[] existingFieldIds = mappings.Where(mapping => mapping.FieldId.HasValue)
            .Select(mapping => mapping.FieldId!.Value).Distinct().ToArray();
        int accessibleFieldCount = await db.FarmFields.CountAsync(
            field => existingFieldIds.Contains(field.Id) && field.FarmId == job.FarmId, cancellationToken);
        if (accessibleFieldCount != existingFieldIds.Length)
            return Bad("field_scope_invalid", "Every mapped field must belong to the import farm.");
        IReadOnlyDictionary<Guid, GeospatialImportFeature> featuresById = features.ToDictionary(feature => feature.Id);
        string[] newFieldNames = mappings.Where(mapping => !mapping.FieldId.HasValue)
            .Select(mapping => string.IsNullOrWhiteSpace(mapping.FieldName)
                ? featuresById[mapping.FeatureId].SourceName
                : mapping.FieldName.Trim())
            .ToArray();
        if (newFieldNames.Distinct(StringComparer.OrdinalIgnoreCase).Count() != newFieldNames.Length)
            return Bad("duplicate_field_name", "New field names must be unique within the import.");

        DateTimeOffset now = clock.GetUtcNow();
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        foreach (GeospatialImportFeature feature in features)
        {
            ApplyGeospatialImportFeature mapping = request.Features.Single(value => value.FeatureId == feature.Id);
            FarmField field;
            if (mapping.FieldId is { } fieldId)
            {
                field = await db.FarmFields.SingleAsync(value => value.Id == fieldId && value.FarmId == job.FarmId, cancellationToken);
                FieldBoundaryVersion? current = await db.FieldBoundaryVersions
                    .SingleOrDefaultAsync(value => value.FieldId == field.Id && value.ValidTo == null, cancellationToken);
                if (current is not null)
                {
                    if (mapping.ValidFrom <= current.ValidFrom)
                        return Bad("boundary_date_conflict", $"The new boundary for '{field.Name}' must start after {current.ValidFrom:yyyy-MM-dd}.");
                    current.Close(mapping.ValidFrom.AddDays(-1));
                }
            }
            else
            {
                string name = mapping.FieldName?.Trim() ?? feature.SourceName;
                if (string.IsNullOrWhiteSpace(name)) return Bad("field_name_required", "A new field requires a name.");
                if (await db.FarmFields.AnyAsync(value => value.FarmId == job.FarmId && value.Name == name, cancellationToken))
                    return UseCaseResults.Conflict(new { code = "field_name_exists", message = $"Field name '{name}' already exists." });
                field = new FarmField(Guid.NewGuid(), job.FarmId, name);
                db.FarmFields.Add(field);
            }
            Guid boundaryId = Guid.NewGuid();
            var boundary = new FieldBoundaryVersion(boundaryId, field.Id, (MultiPolygon)feature.Boundary.Copy(),
                mapping.ValidFrom, $"import-{job.Format.ToString().ToLowerInvariant()}");
            db.FieldBoundaryVersions.Add(boundary);
            feature.MarkApplied(field.Id, boundaryId, now);
            string payload = JsonSerializer.Serialize(new
            {
                ImportJobId = job.Id, FeatureId = feature.Id, FieldId = field.Id, BoundaryVersionId = boundaryId,
                field.Name, mapping.ValidFrom, job.Format, job.Sha256
            }, JsonOptions);
            db.SyncChanges.Add(new SyncChange(tenant.TenantId, job.FarmId, "FieldBoundaryVersion", boundaryId,
                "GeoImportApplied", payload, now));
            db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId,
                "FieldBoundary.Imported", payload, now));
        }
        job.MarkApplied(actor.Subject, now);
        AddAudit(db, tenant, actor, now, "Applied", job.Id, job.FarmId,
            new { job.Id, job.Format, job.FeatureCount, job.Sha256 });
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, "GeospatialImport.Applied",
            JsonSerializer.Serialize(new { job.Id, job.FarmId, job.FeatureCount, job.AppliedAt, job.AppliedBy }), now));
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return UseCaseResults.Ok(ToResponse(job));
    }

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

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}
