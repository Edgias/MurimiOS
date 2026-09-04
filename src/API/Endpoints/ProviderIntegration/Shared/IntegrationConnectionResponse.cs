using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.IntegrationConnection;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.Shared;
using IntegrationCategory = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationCategory;
using IntegrationConnectionStatus = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnectionStatus;
using IntegrationHealth = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationHealth;

public sealed record IntegrationConnectionResponse(Guid Id, string ProviderCode, string Name,
    IntegrationCategory Category, IntegrationConnectionStatus Status, IntegrationHealth Health,
    DateTimeOffset CreatedAt, string CreatedBy, DateTimeOffset? ActivatedAt, DateTimeOffset? LastSucceededAt,
    DateTimeOffset? LastFailedAt, string? LastErrorCode, string? LastErrorMessage, int? RateLimitRemaining,
    DateTimeOffset? RateLimitResetAt, DateTimeOffset? DisabledAt, int? ActiveCredentialVersion,
    string? ActiveCredentialFingerprint, DateTimeOffset? ActiveCredentialExpiresAt, int ActiveFeedCount, uint Version);
