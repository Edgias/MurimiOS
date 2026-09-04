using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Customer.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Customer.CustomerEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Customer.PostGroup;
using CustomerRequest = global::EdgIQ.MurimiOS.API.Endpoints.Customer.Shared.CustomerRequest;
using CustomerResponse = global::EdgIQ.MurimiOS.API.Endpoints.Customer.Shared.CustomerResponse;
internal static class PostGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPost("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, CustomerRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Customer.PostGroup.PostGroupCommand(new global::EdgIQ.MurimiOS.Application.Customer.PostGroup.CustomerRequest { Name = request.Name, Email = request.Email, Phone = request.Phone, Website = request.Website, ContactPerson = request.ContactPerson, ContactPersonEmail = request.ContactPersonEmail, ContactPersonPhone = request.ContactPersonPhone, BillingAddress = request.BillingAddress }), cancellationToken)).ToHttpResult()).Produces<CustomerResponse>(201);

    }
}
