namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public interface IIntegrationCredentialProtector
{
    (string ProtectedReference, string Fingerprint) Protect(string secretReference);
    string Unprotect(string protectedReference);
}
