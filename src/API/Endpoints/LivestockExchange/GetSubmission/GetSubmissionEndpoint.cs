using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.LivestockExchangeEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.GetSubmission;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetSubmissionEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapGet("/livestock/exchange-submissions/{submissionId:guid}", async Task<IResult> (Guid submissionId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.LivestockExchange.GetSubmission.GetSubmissionQuery(submissionId), cancellationToken)).ToHttpResult())
            .WithTags("Livestock external exchange").RequireAuthorization(PlatformPermissions.LivestockRead);

    }
}
