using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;
using EdgIQ.MurimiOS.Domain.ComplianceAssessment;
using EdgIQ.MurimiOS.Domain.ComplianceFinding;
using EdgIQ.MurimiOS.Domain.EvidenceAttachment;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EdgIQ.MurimiOS.Infrastructure.Storage.Compliance;
using ComplianceAssessment = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessment;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;

using ComplianceAssessmentStatus = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessmentStatus;
using ComplianceControlEvaluation = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceControlEvaluation;
using ComplianceEvidenceLink = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceEvidenceLink;
using ComplianceCorrectiveAction = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceCorrectiveAction;
using ComplianceFinding = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceFinding;
using ComplianceControlPoint = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceControlPoint;
using ComplianceEvidenceRequirement = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceEvidenceRequirement;
using EvidenceAttachment = global::EdgIQ.MurimiOS.Domain.EvidenceAttachment.EvidenceAttachment;
using ComplianceAuditPackArtifact = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ComplianceAuditPackArtifact;
using ComplianceAuditPackOptions = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ComplianceAuditPackOptions;
using IComplianceAuditPackGenerator = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IComplianceAuditPackGenerator;
using IComplianceAuditPackSignatureService = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IComplianceAuditPackSignatureService;
using IMediaStorage = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IMediaStorage;
using PlatformDbContext = global::EdgIQ.MurimiOS.Infrastructure.Data.PlatformDbContext;

public sealed class ComplianceAuditPackSignatureService(IOptions<ComplianceAuditPackOptions> configured)
:EdgIQ.MurimiOS.Application.Common.Abstractions.IComplianceAuditPackSignatureService{
    private readonly ComplianceAuditPackOptions options = configured.Value;
    public string KeyId => options.SigningKeyId;
    public string Sign(byte[] content) => Convert.ToBase64String(HMACSHA256.HashData(options.ValidateAndGetKey(), content));
    public bool Verify(string keyId, byte[] content, string signatureBase64)
    {
        byte[]? key = options.GetVerificationKey(keyId);
        if (key is null) return false;
        byte[] supplied;
        try { supplied = Convert.FromBase64String(signatureBase64); }
        catch (FormatException) { return false; }
        byte[] expected = HMACSHA256.HashData(key, content);
        return supplied.Length == expected.Length && CryptographicOperations.FixedTimeEquals(supplied, expected);
    }
}

public sealed class ComplianceAuditPackGenerator(
    PlatformDbContext db,
    IMediaStorage storage,
    IOptions<ComplianceAuditPackOptions> configured,
    ComplianceAuditPackSignatureService signer)
:EdgIQ.MurimiOS.Application.Common.Abstractions.IComplianceAuditPackGenerator{
    private const int SchemaVersion = 1;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };
    private readonly ComplianceAuditPackOptions options = configured.Value;

    public async Task<ComplianceAuditPackArtifact> GenerateAsync(Guid exportId, Guid tenantId, Guid farmId,
        Guid assessmentId, int revision, string scopeStatement, bool includeEvidenceFiles, string actor,
        DateTimeOffset generatedAt, CancellationToken ct)
    {
        options.ValidateAndGetKey();
        var context = await (
            from assessment in db.ComplianceAssessments.AsNoTracking()
            join farm in db.Farms.AsNoTracking() on assessment.FarmId equals farm.Id
            join standard in db.ComplianceStandardVersions.AsNoTracking() on assessment.StandardVersionId equals standard.Id
            where assessment.Id == assessmentId && assessment.FarmId == farmId
            select new { Assessment = assessment, Farm = farm, Standard = standard })
            .SingleOrDefaultAsync(ct);
        if (context is null) throw new KeyNotFoundException("The compliance assessment is unavailable.");
        if (context.Assessment.Status != ComplianceAssessmentStatus.Completed)
            throw new InvalidOperationException("Only a completed compliance assessment can be exported as an audit pack.");

        ComplianceControlPoint[] controls = await db.ComplianceControlPoints.AsNoTracking()
            .Where(x => x.StandardVersionId == context.Standard.Id).OrderBy(x => x.SortOrder).ThenBy(x => x.Code).ToArrayAsync(ct);
        Guid[] controlIds = controls.Select(x => x.Id).ToArray();
        ComplianceEvidenceRequirement[] requirements = await db.ComplianceEvidenceRequirements.AsNoTracking()
            .Where(x => controlIds.Contains(x.ControlPointId)).OrderBy(x => x.Code).ToArrayAsync(ct);
        ComplianceControlEvaluation[] evaluations = await db.ComplianceControlEvaluations.AsNoTracking()
            .Where(x => x.AssessmentId == assessmentId).OrderBy(x => x.EvaluatedAt).ToArrayAsync(ct);
        Guid[] evaluationIds = evaluations.Select(x => x.Id).ToArray();
        ComplianceEvidenceLink[] links = await db.ComplianceEvidenceLinks.AsNoTracking()
            .Where(x => evaluationIds.Contains(x.EvaluationId)).OrderBy(x => x.LinkedAt).ToArrayAsync(ct);
        Guid[] evidenceIds = links.Select(x => x.EvidenceAttachmentId).Distinct().ToArray();
        EvidenceAttachment[] evidence = await db.EvidenceAttachments.AsNoTracking()
            .Where(x => evidenceIds.Contains(x.Id)).OrderBy(x => x.CapturedAt).ThenBy(x => x.Id).ToArrayAsync(ct);
        ComplianceFinding[] findings = await db.ComplianceFindings.AsNoTracking()
            .Where(x => x.AssessmentId == assessmentId).OrderBy(x => x.OpenedAt).ToArrayAsync(ct);
        Guid[] findingIds = findings.Select(x => x.Id).ToArray();
        ComplianceCorrectiveAction[] actions = await db.ComplianceCorrectiveActions.AsNoTracking()
            .Where(x => findingIds.Contains(x.FindingId)).OrderBy(x => x.DueOn).ToArrayAsync(ct);

        var content = new List<PackEntry>();
        AddJson(content, "scope.json", new
        {
            scopeStatement,
            assessmentScope = ParseJson(context.Assessment.ScopeJson),
            farm = new { context.Farm.Id, context.Farm.Name, context.Farm.Code, context.Farm.CountryCode, context.Farm.TimeZone }
        });
        AddJson(content, "standard.json", new
        {
            standard = new
            {
                context.Standard.Id, context.Standard.SeriesId, context.Standard.Code, context.Standard.Name,
                context.Standard.Scheme, context.Standard.VersionLabel, context.Standard.Revision,
                context.Standard.EffectiveFrom, context.Standard.EffectiveTo, context.Standard.Jurisdiction,
                context.Standard.PackKey, context.Standard.SourceUri, context.Standard.ManifestSha256,
                context.Standard.ConformancePolicyJson, context.Standard.Status, context.Standard.PublishedAt,
                context.Standard.PublishedBy
            },
            controls = controls.Select(control => new
            {
                control.Id, control.Code, control.Title, control.Description, control.Applicability, control.Weight,
                control.Critical, control.EvaluatorKey, control.EvaluatorVersion, control.SortOrder,
                requirements = requirements.Where(x => x.ControlPointId == control.Id).Select(requirement => new
                {
                    requirement.Id, requirement.Code, requirement.Name, requirement.EvidenceType,
                    requirement.Required, requirement.MinimumCount, requirement.MaximumAgeDays, requirement.Instructions
                })
            })
        });
        AddJson(content, "assessment.json", new
        {
            context.Assessment.Id, context.Assessment.AssessmentCode, context.Assessment.Status,
            context.Assessment.ScheduledOn, context.Assessment.Auditor, context.Assessment.StartedAt,
            context.Assessment.SubmittedAt, context.Assessment.CompletedAt, context.Assessment.ScorePercent,
            context.Assessment.Conforms
        });
        AddJson(content, "evaluations.json", evaluations.Select(evaluation => new
        {
            evaluation.Id, evaluation.ControlPointId, evaluation.Result, evaluation.ScorePercent,
            evaluation.EvaluatorVersion, evaluation.EvaluatedBy, evaluation.EvaluatedAt, evaluation.Notes,
            evidenceSnapshot = ParseJson(evaluation.EvidenceSnapshotJson),
            evidenceAttachmentIds = links.Where(x => x.EvaluationId == evaluation.Id).Select(x => x.EvidenceAttachmentId)
        }));
        AddJson(content, "findings-and-actions.json", findings.Select(finding => new
        {
            finding.Id, finding.ControlPointId, finding.Severity, finding.Title, finding.Description,
            finding.DueOn, finding.OpenedAt, finding.Status, finding.ClosedAt, finding.ClosedBy, finding.WaiverReason,
            actions = actions.Where(x => x.FindingId == finding.Id).Select(action => new
            {
                action.Id, action.Action, action.Owner, action.DueOn, action.Status, action.CompletionNotes,
                action.SubmittedAt, action.VerifiedAt, action.VerifiedBy, action.RejectionReason
            })
        }));
        AddJson(content, "evidence-index.json", evidence.Select(item => new
        {
            item.Id, item.CropZoneId, item.FileName, item.ContentType, item.ByteSize, item.Sha256,
            item.CapturedAt, item.Latitude, item.Longitude, item.AccuracyMetres,
            linkedEvaluationIds = links.Where(x => x.EvidenceAttachmentId == item.Id).Select(x => x.EvaluationId),
            includedPath = includeEvidenceFiles ? EvidencePath(item) : null
        }));
        content.Add(PackEntry.Text("README.txt",
            "MurimiOS controlled compliance audit pack\n" +
            "This export freezes the listed records at the generation time. It supports audit review but does not itself constitute certification or legal advice.\n"));

        if (includeEvidenceFiles)
        {
            foreach (EvidenceAttachment item in evidence)
            {
                await using Stream source = await storage.OpenReadAsync(item.ObjectKey, ct);
                await using var copy = new MemoryStream();
                await source.CopyToAsync(copy, ct);
                byte[] bytes = copy.ToArray();
                string digest = Sha(bytes);
                if (bytes.LongLength != item.ByteSize || !digest.Equals(item.Sha256, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidDataException($"Evidence {item.Id} no longer matches its retained size or SHA-256 digest.");
                content.Add(new PackEntry(EvidencePath(item), item.ContentType, bytes, digest, item.Id));
            }
        }

        long unpackedBytes = content.Sum(x => x.Bytes.LongLength);
        if (unpackedBytes > options.MaximumArtifactBytes)
            throw new InvalidDataException("The audit-pack content exceeds the configured maximum artifact size.");

        object manifest = new
        {
            schema = "murimios.compliance.audit-pack",
            schemaVersion = SchemaVersion,
            exportId,
            revision,
            generatedAt = generatedAt.ToUniversalTime(),
            generatedBy = actor,
            tenantId,
            farm = new { context.Farm.Id, context.Farm.Name, context.Farm.Code },
            assessment = new { context.Assessment.Id, context.Assessment.AssessmentCode, context.Assessment.CompletedAt },
            standard = new
            {
                context.Standard.Id, context.Standard.Code, context.Standard.Scheme, context.Standard.VersionLabel,
                context.Standard.Revision, context.Standard.ManifestSha256
            },
            scopeStatement,
            includesEvidenceFiles = includeEvidenceFiles,
            entries = content.OrderBy(x => x.Path, StringComparer.Ordinal).Select(x => new
            {
                path = x.Path, contentType = x.ContentType, byteSize = x.Bytes.LongLength,
                sha256 = x.Sha256, sourceEvidenceAttachmentId = x.SourceEvidenceId
            })
        };
        byte[] manifestBytes = JsonSerializer.SerializeToUtf8Bytes(manifest, JsonOptions);
        string manifestJson = Encoding.UTF8.GetString(manifestBytes);
        string manifestSha = Sha(manifestBytes);
        string signature = signer.Sign(manifestBytes);
        byte[] signatureBytes = JsonSerializer.SerializeToUtf8Bytes(new
        {
            algorithm = "HMAC-SHA256", keyId = signer.KeyId, manifestSha256 = manifestSha,
            signatureBase64 = signature
        }, JsonOptions);

        await using var artifact = new MemoryStream();
        using (var zip = new ZipArchive(artifact, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (PackEntry entry in content.OrderBy(x => x.Path, StringComparer.Ordinal))
                await WriteAsync(zip, entry.Path, entry.Bytes, ct);
            await WriteAsync(zip, "manifest.json", manifestBytes, ct);
            await WriteAsync(zip, "signature.json", signatureBytes, ct);
        }
        if (artifact.Length > options.MaximumArtifactBytes)
            throw new InvalidDataException("The compressed audit pack exceeds the configured maximum artifact size.");
        byte[] artifactBytes = artifact.ToArray();
        string artifactSha = Sha(artifactBytes);
        artifact.Position = 0;
        string objectKey = await storage.PutExportAsync(tenantId, exportId, "compliance-audit-packs", "zip",
            "application/zip", artifact, artifact.Length, ct);
        return new ComplianceAuditPackArtifact(objectKey, artifactBytes, artifactSha, manifestJson, manifestSha,
            signature, signer.KeyId, content.Count + 2, includeEvidenceFiles ? evidence.Length : 0);
    }

    private static void AddJson(List<PackEntry> target, string path, object value)
    {
        byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(value, JsonOptions);
        target.Add(new PackEntry(path, "application/json", bytes, Sha(bytes), null));
    }

    private static async Task WriteAsync(ZipArchive archive, string path, byte[] bytes, CancellationToken ct)
    {
        ZipArchiveEntry entry = archive.CreateEntry(path, CompressionLevel.SmallestSize);
        entry.LastWriteTime = new DateTimeOffset(1980, 1, 1, 0, 0, 0, TimeSpan.Zero);
        await using Stream output = entry.Open();
        await output.WriteAsync(bytes, ct);
    }

    private static string EvidencePath(EvidenceAttachment item)
    {
        string safe = new(item.FileName.Select(x => char.IsAsciiLetterOrDigit(x) || x is '.' or '-' or '_' ? x : '_').ToArray());
        if (safe.Length == 0) safe = "evidence";
        if (safe.Length > 160) safe = safe[..160];
        return $"evidence/{item.Id:N}-{safe}";
    }

    private static JsonElement? ParseJson(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        using JsonDocument document = JsonDocument.Parse(value);
        return document.RootElement.Clone();
    }

    private static string Sha(byte[] bytes) => Convert.ToHexStringLower(SHA256.HashData(bytes));

    private sealed record PackEntry(string Path, string ContentType, byte[] Bytes, string Sha256, Guid? SourceEvidenceId)
    {
        public static PackEntry Text(string path, string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            return new PackEntry(path, "text/plain; charset=utf-8", bytes, Sha(bytes), null);
        }
    }
}
