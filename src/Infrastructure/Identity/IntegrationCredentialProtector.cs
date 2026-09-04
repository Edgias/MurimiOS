using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.DataProtection;

namespace EdgIQ.MurimiOS.Infrastructure.Identity;

public sealed class IntegrationCredentialProtector(IDataProtectionProvider provider) : EdgIQ.MurimiOS.Application.Common.Abstractions.IIntegrationCredentialProtector
{
    private readonly IDataProtector protector = provider.CreateProtector("MurimiOS.IntegrationCredentialReference.v1");

    public (string ProtectedReference, string Fingerprint) Protect(string secretReference)
    {
        string normalized = secretReference?.Trim() ?? string.Empty;
        if (normalized.Length is 1 or > 2_000)
            throw new ArgumentException("A credential secret reference between 2 and 2000 characters is required.", nameof(secretReference));
        string fingerprint = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(normalized)))[..16].ToLowerInvariant();
        return (protector.Protect(normalized), fingerprint);
    }

    public string Unprotect(string protectedReference)
    {
        string normalized = protectedReference?.Trim() ?? string.Empty;
        if (normalized.Length is 1 or > 4_000)
            throw new ArgumentException("A protected credential reference is required.", nameof(protectedReference));
        return protector.Unprotect(normalized);
    }
}
