using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.CompleteJob;
using static EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ProviderIntegrationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.CompleteJob;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CompleteJobEndpoint
{
    internal static void Map(IEndpointRouteBuilder integrations)
    {
        integrations.MapPost("/jobs/{jobId:guid}/complete", async Task<IResult> (Guid jobId, CompleteIngestionJobRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ProviderIntegration.CompleteJob.CompleteJobCommand(jobId, new global::EdgIQ.MurimiOS.Application.ProviderIntegration.CompleteJob.CompleteIngestionJobRequest(request.LeaseId, request.Checkpoint, request.HttpStatusCode, request.RequestUnits, request.Cost, request.Currency, request.RateLimitRemaining, request.RateLimitResetAt)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.IntegrationsOperate);

    }
}
