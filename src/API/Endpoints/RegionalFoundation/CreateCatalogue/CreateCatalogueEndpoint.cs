using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.CreateCatalogue;
using static EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.RegionalFoundationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.CreateCatalogue;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateCatalogueEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapPost("/reference-catalogues", async Task<IResult> (CreateReferenceCatalogueRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.RegionalFoundation.CreateCatalogue.CreateCatalogueCommand(new global::EdgIQ.MurimiOS.Application.RegionalFoundation.CreateCatalogue.CreateReferenceCatalogueRequest(request.Key, request.Name, request.Version, request.CountryCode, request.Authority, request.EffectiveFrom, request.EffectiveTo)), cancellationToken)).ToHttpResult())
            .WithTags("Reference data")
            .RequireAuthorization(PlatformPermissions.ReferenceDataManage);

    }
}
