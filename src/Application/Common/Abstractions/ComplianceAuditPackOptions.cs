using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public sealed class ComplianceAuditPackOptions
{
    public string SigningKeyBase64 { get; set; } = "";
    public string SigningKeyId { get; set; } = "";
    public long MaximumArtifactBytes { get; set; } = 268_435_456;
    public Dictionary<string, string> VerificationKeys { get; set; } = new(StringComparer.Ordinal);

    public byte[] ValidateAndGetKey()
    {
        byte[] key;
        try { key = Convert.FromBase64String(SigningKeyBase64); }
        catch (FormatException exception) { throw new InvalidOperationException("Compliance audit-pack signing key must be base64.", exception); }
        if (key.Length < 32) throw new InvalidOperationException("Compliance audit-pack signing key must contain at least 32 bytes.");
        if (string.IsNullOrWhiteSpace(SigningKeyId) || SigningKeyId.Length > 160)
            throw new InvalidOperationException("Compliance audit-pack signing key id is required and cannot exceed 160 characters.");
        if (MaximumArtifactBytes is < 1_048_576 or > 536_870_912)
            throw new InvalidOperationException("Compliance audit-pack maximum size must be between 1 MiB and 512 MiB.");
        return key;
    }

    public byte[]? GetVerificationKey(string keyId)
    {
        if (keyId.Equals(SigningKeyId, StringComparison.Ordinal)) return ValidateAndGetKey();
        if (!VerificationKeys.TryGetValue(keyId, out string? encoded)) return null;
        try
        {
            byte[] key = Convert.FromBase64String(encoded);
            return key.Length >= 32 ? key : null;
        }
        catch (FormatException) { return null; }
    }
}
