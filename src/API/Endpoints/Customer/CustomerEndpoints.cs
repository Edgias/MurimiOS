using EdgIQ.MurimiOS.API.Infrastructure.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Customer;
using CustomerRequest = global::EdgIQ.MurimiOS.API.Endpoints.Customer.Shared.CustomerRequest;


internal static class CustomerEndpoints
{
    public static RouteGroupBuilder MapCustomers(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/customers");
        group.WithTags("Customers");
        group.WithParameterValidation(typeof(CustomerRequest));
        global::EdgIQ.MurimiOS.API.Endpoints.Customer.GetGroup.GetGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Customer.GetGroupId.GetGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Customer.PostGroup.PostGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Customer.PutGroupId.PutGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Customer.DeleteGroupId.DeleteGroupIdEndpoint.Map(group);

        return group;
    }

}

