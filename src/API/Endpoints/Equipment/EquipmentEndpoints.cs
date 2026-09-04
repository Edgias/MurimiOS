using EdgIQ.MurimiOS.API.Infrastructure.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Equipment;
using EquipmentRequest = global::EdgIQ.MurimiOS.API.Endpoints.Equipment.Shared.EquipmentRequest;


internal static class EquipmentEndpoints
{
    public static RouteGroupBuilder MapMachineryCategories(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/equipments");
        group.WithTags("Equipments");
        group.WithParameterValidation(typeof(EquipmentRequest));
        global::EdgIQ.MurimiOS.API.Endpoints.Equipment.GetGroup.GetGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Equipment.GetGroupId.GetGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Equipment.PostGroup.PostGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Equipment.PutGroupId.PutGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Equipment.DeleteGroupId.DeleteGroupIdEndpoint.Map(group);

        return group;
    }

}

