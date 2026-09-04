using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.EnqueueJob;
using static EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ProviderIntegrationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.EnqueueJob;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class EnqueueJobEndpoint
{
    internal static void Map(IEndpointRouteBuilder integrations)
    {
        integrations.MapPost("/feeds/{feedId:guid}/jobs", async Task<IResult> (Guid feedId, EnqueueIngestionJobRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ProviderIntegration.EnqueueJob.EnqueueJobCommand(feedId, new global::EdgIQ.MurimiOS.Application.ProviderIntegration.EnqueueJob.EnqueueIngestionJobRequest(request.Id, request.Kind, request.IdempotencyKey, request.AvailableAt, request.MaximumAttempts, request.WindowStart, request.WindowEnd)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.IntegrationsOperate);

    }
}
