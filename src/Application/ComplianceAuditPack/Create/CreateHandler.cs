using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.ComplianceAssessment;
using EdgIQ.MurimiOS.Domain.ComplianceAuditPackExport;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
namespace EdgIQ.MurimiOS.Application.ComplianceAuditPack.Create;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using ComplianceAssessment = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessment;
using ComplianceAuditPackExport = global::EdgIQ.MurimiOS.Domain.ComplianceAuditPackExport.ComplianceAuditPackExport;
using ComplianceAuditPackFormat = global::EdgIQ.MurimiOS.Domain.ComplianceAuditPackExport.ComplianceAuditPackFormat;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;

public sealed class CreateHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IComplianceAuditPackGenerator generator, ITenantContext tenant, IRequestActorContext actor, UseCaseContext context, TimeProvider clock) : IRequestHandler<CreateCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateCommand message, CancellationToken cancellationToken) => CreateAsync(message.farmId, message.assessmentId, message.request, db, scope, generator, tenant, actor, context, clock, cancellationToken);

    private static async Task<UseCaseResult> CreateAsync(Guid farmId, Guid assessmentId,
        CreateComplianceAuditPackRequest request, IPlatformDbContext db, IOrganisationScopeAccess scope,
        IComplianceAuditPackGenerator generator, ITenantContext tenant, IRequestActorContext actor,
        UseCaseContext context, TimeProvider clock, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound();
        string scopeStatement = request.ScopeStatement?.Trim() ?? "";
        if (scopeStatement.Length is 0 or > 4_000)
            return Bad("audit_pack_scope_invalid", "A scope statement containing 1-4000 characters is required.");
        Guid exportId = request.Id is { } supplied && supplied != Guid.Empty ? supplied : Guid.NewGuid();
        int revision = (await db.ComplianceAuditPackExports.AsNoTracking()
            .Where(x => x.AssessmentId == assessmentId).MaxAsync(x => (int?)x.Revision, ct) ?? 0) + 1;
        DateTimeOffset now = clock.GetUtcNow();
        try
        {
            ComplianceAuditPackArtifact artifact = await generator.GenerateAsync(exportId, tenant.TenantId, farmId,
                assessmentId, revision, scopeStatement, request.IncludeEvidenceFiles, actor.Subject, now, ct);
            ComplianceAssessment? assessment = await db.ComplianceAssessments.AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == assessmentId && x.FarmId == farmId, ct);
            if (assessment is null) return UseCaseResults.NotFound();
            var export = new ComplianceAuditPackExport(exportId, assessmentId, farmId, assessment.StandardVersionId,
                revision, 1, scopeStatement, request.IncludeEvidenceFiles, ComplianceAuditPackFormat.Zip,
                artifact.ObjectKey, artifact.Bytes.LongLength, artifact.ContentEntryCount, artifact.EvidenceFileCount,
                artifact.ArtifactSha256, artifact.ManifestSha256, "HMAC-SHA256", artifact.SignatureKeyId,
                artifact.SignatureBase64, artifact.ManifestJson, now, actor.Subject);
            db.ComplianceAuditPackExports.Add(export);
            string payload = JsonSerializer.Serialize(new
            {
                export.Id, export.AssessmentId, export.FarmId, export.StandardVersionId, export.Revision,
                export.ContentSchemaVersion, export.IncludesEvidenceFiles, export.ByteSize,
                export.ContentEntryCount, export.EvidenceFileCount, export.ArtifactSha256,
                export.ManifestSha256, export.SignatureAlgorithm, export.SignatureKeyId, export.GeneratedAt
            });
            db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId,
                "Exported", nameof(ComplianceAuditPackExport), export.Id, farmId, actor.ScopeOrganisationNodeId,
                payload, Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId));
            db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId,
                "Compliance.AuditPackExported", payload, now));
            await db.SaveChangesAsync(ct);
            context.Response["Location"] = $"/api/v1/farms/{farmId}/compliance/audit-packs/{export.Id}";
            return UseCaseResults.Created(context.Response["Location"]!, Metadata(export));
        }
        catch (KeyNotFoundException) { return UseCaseResults.NotFound(); }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException or InvalidDataException)
        { return Conflict("audit_pack_generation_failed", exception.Message); }
    }


    private static object Metadata(ComplianceAuditPackExport export) => new
    {
        export.Id, export.AssessmentId, export.FarmId, export.StandardVersionId, export.Revision,
        export.ContentSchemaVersion, export.ScopeStatement, export.IncludesEvidenceFiles, export.Format,
        export.ByteSize, export.ContentEntryCount, export.EvidenceFileCount, export.ArtifactSha256,
        export.ManifestSha256, export.SignatureAlgorithm, export.SignatureKeyId, export.SignatureBase64,
        export.GeneratedAt, export.GeneratedBy
    };

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
