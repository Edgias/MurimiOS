using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.IntegrationConnection;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
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

public sealed record ListConnectionsQuery(IntegrationCategory? category, IntegrationConnectionStatus? status) : IRequest<UseCaseResult>;
