using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.CreateItem;
using static EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.RegionalFoundationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.CreateItem;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateItemEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapPost("/reference-catalogues/{catalogueId:guid}/items", async Task<IResult> (Guid catalogueId, CreateReferenceCatalogueItemRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.RegionalFoundation.CreateItem.CreateItemCommand(catalogueId, new global::EdgIQ.MurimiOS.Application.RegionalFoundation.CreateItem.CreateReferenceCatalogueItemRequest(request.Code, request.Name, request.Description, request.Attributes, request.EffectiveFrom, request.EffectiveTo)), cancellationToken)).ToHttpResult())
            .WithTags("Reference data")
            .RequireAuthorization(PlatformPermissions.ReferenceDataManage);

    }
}
