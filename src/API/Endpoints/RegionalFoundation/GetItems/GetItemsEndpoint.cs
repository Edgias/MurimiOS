using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.RegionalFoundationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.GetItems;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetItemsEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapGet("/reference-catalogues/{catalogueId:guid}/items", async Task<IResult> (Guid catalogueId, DateOnly? effectiveOn, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.RegionalFoundation.GetItems.GetItemsQuery(catalogueId, effectiveOn), cancellationToken)).ToHttpResult())
            .WithTags("Reference data")
            .RequireAuthorization(PlatformPermissions.ReferenceDataRead);

    }
}
