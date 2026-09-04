using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.Application.Common.Compliance;
public sealed record ComplianceStandardPack(string Key, string Code, string Name, string Scheme, string VersionLabel,
    int Revision, DateOnly EffectiveFrom, string? Jurisdiction, string SourceUri, string Notice, ComplianceConformancePolicy Policy,
    IReadOnlyList<StandardPackControl> Controls)
{
    [System.Text.Json.Serialization.JsonIgnore]
    public string ManifestSha256 => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(
        JsonSerializer.Serialize(this, StandardPackJson.Options)))).ToLowerInvariant();
}
