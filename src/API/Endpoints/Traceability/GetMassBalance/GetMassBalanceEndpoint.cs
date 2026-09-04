using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Traceability.GetMassBalance;
using static EdgIQ.MurimiOS.API.Endpoints.Traceability.TraceabilityEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Traceability.GetMassBalance;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetMassBalanceEndpoint
{
    internal static void Map(IEndpointRouteBuilder trace)
    {
        trace.MapGet("/objects/{objectType}/{objectId:guid}/mass-balance", async Task<IResult> (string objectType, Guid objectId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Traceability.GetMassBalance.GetMassBalanceQuery(objectType, objectId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.TraceabilityRead);

    }
}
