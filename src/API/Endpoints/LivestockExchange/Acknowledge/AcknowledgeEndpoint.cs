using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.Acknowledge;
using static EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.LivestockExchangeEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.Acknowledge;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class AcknowledgeEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapPost("/livestock/exchange-submissions/{submissionId:guid}/acknowledge", async Task<IResult> (Guid submissionId, AcknowledgeLivestockExchangeRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.LivestockExchange.Acknowledge.AcknowledgeCommand(submissionId, new global::EdgIQ.MurimiOS.Application.LivestockExchange.Acknowledge.AcknowledgeLivestockExchangeRequest(request.Accepted, request.ExternalReference, request.ErrorCode, request.ErrorMessage, request.ResponseJson)), cancellationToken)).ToHttpResult())
            .WithTags("Livestock external exchange").RequireAuthorization(PlatformPermissions.LivestockWrite);

    }
}
