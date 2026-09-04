using EdgIQ.MurimiOS.API.Infrastructure.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Asset;
using AssetModel = global::EdgIQ.MurimiOS.API.Endpoints.Asset.Shared.AssetModel;


internal static class AssetEndpoints
{
    public static RouteGroupBuilder MapAssets(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/assets");
        group.WithTags("Assets");
        group.WithParameterValidation(typeof(AssetModel));
        global::EdgIQ.MurimiOS.API.Endpoints.Asset.GetGroup.GetGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Asset.GetGroupId.GetGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Asset.PostGroup.PostGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Asset.PutGroupId.PutGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Asset.DeleteGroupId.DeleteGroupIdEndpoint.Map(group);

        return group;
    }

}

