using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.EnterpriseConsolidationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.GetConsolidation;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetConsolidationEndpoint
{
    internal static void Map(IEndpointRouteBuilder enterprise)
    {
        enterprise.MapGet("/consolidations/{id:guid}", async Task<IResult> (Guid id, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.EnterpriseConsolidation.GetConsolidation.GetConsolidationQuery(id), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.FinanceRead);

    }
}
