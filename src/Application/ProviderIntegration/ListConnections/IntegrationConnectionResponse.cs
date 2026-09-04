using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.IntegrationConnection;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ProviderIntegration.ListConnections;
using IntegrationFeed = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeed;

using IntegrationCategory = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationCategory;
using IntegrationConnection = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnection;
using IntegrationConnectionStatus = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnectionStatus;
using IntegrationCredentialStatus = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationCredentialStatus;
using IntegrationHealth = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationHealth;
using IntegrationFeedStatus = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedStatus;

public sealed record IntegrationConnectionResponse(Guid Id, string ProviderCode, string Name,
    IntegrationCategory Category, IntegrationConnectionStatus Status, IntegrationHealth Health,
    DateTimeOffset CreatedAt, string CreatedBy, DateTimeOffset? ActivatedAt, DateTimeOffset? LastSucceededAt,
    DateTimeOffset? LastFailedAt, string? LastErrorCode, string? LastErrorMessage, int? RateLimitRemaining,
    DateTimeOffset? RateLimitResetAt, DateTimeOffset? DisabledAt, int? ActiveCredentialVersion,
    string? ActiveCredentialFingerprint, DateTimeOffset? ActiveCredentialExpiresAt, int ActiveFeedCount, uint Version);
