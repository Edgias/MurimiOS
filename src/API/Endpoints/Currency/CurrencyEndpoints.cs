using EdgIQ.MurimiOS.API.Infrastructure.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Currency;
using CurrencyRequest = global::EdgIQ.MurimiOS.API.Endpoints.Currency.Shared.CurrencyRequest;


internal static class CurrencyEndpoints
{
    public static RouteGroupBuilder MapCurrencies(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/currencies");
        group.WithTags("Currencies");
        group.WithParameterValidation(typeof(CurrencyRequest));
        global::EdgIQ.MurimiOS.API.Endpoints.Currency.GetGroup.GetGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Currency.GetGroupId.GetGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Currency.PostGroup.PostGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Currency.PutGroupId.PutGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Currency.DeleteGroupId.DeleteGroupIdEndpoint.Map(group);

        return group;
    }

}

