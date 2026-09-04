using EdgIQ.MurimiOS.Application.Common.Abstractions;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.Application.Common.Compliance;
using ComplianceControlPoint = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceControlPoint;
using ComplianceEvidenceRequirement = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceEvidenceRequirement;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;

public sealed class ComplianceStandardPackInstaller(IPlatformDbContext db)
{
    public async Task<(ComplianceStandardVersion Standard, bool Created)> InstallAsync(ComplianceStandardPack pack,
        string actor, DateTimeOffset now, CancellationToken cancellationToken)
    {
        ComplianceStandardVersion? existing = await db.ComplianceStandardVersions
            .SingleOrDefaultAsync(x => x.PackKey == pack.Key && x.ManifestSha256 == pack.ManifestSha256, cancellationToken);
        if (existing is not null) return (existing, false);
        Guid seriesId = StableGuid($"standard-pack-series:{pack.Key}");
        var standard = new ComplianceStandardVersion(StableGuid($"standard-pack:{pack.Key}:{pack.ManifestSha256}"), seriesId,
            pack.Code, pack.Name, pack.Scheme, pack.VersionLabel, pack.Revision, pack.EffectiveFrom, null, pack.Jurisdiction,
            pack.Key, pack.SourceUri, pack.ManifestSha256, JsonSerializer.Serialize(pack.Policy, StandardPackJson.Options));
        db.ComplianceStandardVersions.Add(standard);
        int order = 0;
        foreach (StandardPackControl definition in pack.Controls)
        {
            Guid controlId = StableGuid($"standard-pack-control:{pack.Key}:{pack.ManifestSha256}:{definition.Code}");
            db.ComplianceControlPoints.Add(new ComplianceControlPoint(controlId, standard.Id, definition.Code, definition.Title,
                definition.Description, definition.Applicability, definition.Weight, definition.Critical, definition.EvaluatorKey, "1.0.0", order++));
            foreach (StandardPackRequirement requirement in definition.Requirements)
                db.ComplianceEvidenceRequirements.Add(new ComplianceEvidenceRequirement(
                    StableGuid($"standard-pack-requirement:{pack.Key}:{pack.ManifestSha256}:{definition.Code}:{requirement.Code}"),
                    controlId, requirement.Code, requirement.Name, requirement.Type, requirement.Required, requirement.MinimumCount,
                    requirement.MaximumAgeDays, requirement.Instructions));
        }
        standard.Publish(now, actor);
        return (standard, true);
    }

    private static Guid StableGuid(string value)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        bytes[7] = (byte)((bytes[7] & 0x0f) | 0x40); bytes[8] = (byte)((bytes[8] & 0x3f) | 0x80);
        return new Guid(bytes.AsSpan(0, 16));
    }
}
