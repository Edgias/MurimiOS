using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.Submit;
using static EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.LivestockExchangeEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.Submit;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class SubmitEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapPost("/livestock/exchange-submissions/{submissionId:guid}/submit", async Task<IResult> (Guid submissionId, SubmitLivestockExchangeRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.LivestockExchange.Submit.SubmitCommand(submissionId, new global::EdgIQ.MurimiOS.Application.LivestockExchange.Submit.SubmitLivestockExchangeRequest(request.ConnectionId)), cancellationToken)).ToHttpResult())
            .WithTags("Livestock external exchange").RequireAuthorization(PlatformPermissions.LivestockWrite);

    }
}
