using Microsoft.AspNetCore.DataProtection;

namespace EdgIQ.MurimiOS.Worker;

public sealed class ProviderCredentialReader(IDataProtectionProvider provider)
{
    private readonly IDataProtector protector = provider.CreateProtector("MurimiOS.IntegrationCredentialReference.v1");
    public string Unprotect(string protectedReference) => protector.Unprotect(protectedReference);
}
