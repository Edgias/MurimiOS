using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.RegionalFoundationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.GetCatalogues;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetCataloguesEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {

        api.MapGet("/reference-catalogues", async Task<IResult> (DateOnly? effectiveOn, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.RegionalFoundation.GetCatalogues.GetCataloguesQuery(effectiveOn), cancellationToken)).ToHttpResult())
            .WithTags("Reference data")
            .RequireAuthorization(PlatformPermissions.ReferenceDataRead);

    }
}
