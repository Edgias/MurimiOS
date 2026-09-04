using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.GetConnection;
using IntegrationConnectionResponse = global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.Shared.IntegrationConnectionResponse;
using IntegrationCredentialResponse = global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.Shared.IntegrationCredentialResponse;
using IntegrationFeedResponse = global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.Shared.IntegrationFeedResponse;
public sealed record IntegrationConnectionDetailResponse(IntegrationConnectionResponse Connection,
    JsonElement Configuration, IReadOnlyList<IntegrationCredentialResponse> Credentials,
    IReadOnlyList<IntegrationFeedResponse> Feeds);
