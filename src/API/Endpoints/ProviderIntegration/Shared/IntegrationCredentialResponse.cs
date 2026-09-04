using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.IntegrationConnection;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.Shared;
using IntegrationCredentialStatus = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationCredentialStatus;
public sealed record IntegrationCredentialResponse(Guid Id, Guid ConnectionId, int CredentialVersion,
    string SecretProvider, string Fingerprint, DateTimeOffset ValidFrom, DateTimeOffset? ExpiresAt,
    DateTimeOffset CreatedAt, string CreatedBy, Guid? PreviousCredentialId, IntegrationCredentialStatus Status,
    DateTimeOffset? SupersededAt, Guid? SupersededByCredentialId, DateTimeOffset? RevokedAt, string? RevokedBy);
