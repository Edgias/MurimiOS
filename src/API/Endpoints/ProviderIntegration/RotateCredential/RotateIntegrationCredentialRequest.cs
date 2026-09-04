using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.RotateCredential;
public sealed record RotateIntegrationCredentialRequest(Guid? Id, string SecretProvider, string SecretReference,
    DateTimeOffset? ValidFrom, DateTimeOffset? ExpiresAt);
