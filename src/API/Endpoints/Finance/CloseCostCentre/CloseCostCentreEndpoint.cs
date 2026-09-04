using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.Finance.FinanceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Finance.CloseCostCentre;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CloseCostCentreEndpoint
{
    internal static void Map(IEndpointRouteBuilder finance)
    {
        finance.MapPost("/cost-centres/{costCentreId:guid}/close", async Task<IResult> (Guid costCentreId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Finance.CloseCostCentre.CloseCostCentreCommand(costCentreId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.FinanceWrite);

    }
}
