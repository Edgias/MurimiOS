using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.FailJob;
using static EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ProviderIntegrationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.FailJob;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class FailJobEndpoint
{
    internal static void Map(IEndpointRouteBuilder integrations)
    {
        integrations.MapPost("/jobs/{jobId:guid}/fail", async Task<IResult> (Guid jobId, FailIngestionJobRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ProviderIntegration.FailJob.FailJobCommand(jobId, new global::EdgIQ.MurimiOS.Application.ProviderIntegration.FailJob.FailIngestionJobRequest(request.LeaseId, request.ErrorCode, request.ErrorMessage, request.RetryAt, request.HttpStatusCode, request.RequestUnits, request.Cost, request.Currency)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.IntegrationsOperate);

    }
}
