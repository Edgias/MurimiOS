using EdgIQ.MurimiOS.API.Infrastructure;
namespace EdgIQ.MurimiOS.API.Endpoints.Farms.GetFarms;
using FarmResponse = global::EdgIQ.MurimiOS.API.Endpoints.Farms.Shared.FarmResponse;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetFarmsEndpoint
{
    internal static void Map(IEndpointRouteBuilder farms)
    {

        farms.MapGet("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => 
        (await sender.Send(new global::EdgIQ.MurimiOS.Application.Farms.GetFarms.GetFarmsQuery(), cancellationToken))
        .ToHttpResult())
            .Produces<FarmResponse[]>(200).RequireAuthorization(PlatformPermissions.FarmRead);

    }
}
