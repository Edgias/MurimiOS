using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Customer.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Customer.CustomerEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Customer.PutGroupId;
using CustomerRequest = global::EdgIQ.MurimiOS.API.Endpoints.Customer.Shared.CustomerRequest;
internal static class PutGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPut("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id, CustomerRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Customer.PutGroupId.PutGroupIdCommand(id, new global::EdgIQ.MurimiOS.Application.Customer.PutGroupId.CustomerRequest { Name = request.Name, Email = request.Email, Phone = request.Phone, Website = request.Website, ContactPerson = request.ContactPerson, ContactPersonEmail = request.ContactPersonEmail, ContactPersonPhone = request.ContactPersonPhone, BillingAddress = request.BillingAddress }), cancellationToken)).ToHttpResult()).Produces(200).Produces(404).Produces(400);

    }
}
