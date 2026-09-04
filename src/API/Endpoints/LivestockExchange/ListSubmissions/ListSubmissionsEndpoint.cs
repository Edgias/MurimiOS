using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile;
using EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.LivestockExchangeEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.ListSubmissions;
using LivestockExchangeSubmissionStatus = global::EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission.LivestockExchangeSubmissionStatus;
using LivestockRegulatoryExchangeType = global::EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile.LivestockRegulatoryExchangeType;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListSubmissionsEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapGet("/livestock/exchange-submissions", async Task<IResult> (LivestockExchangeSubmissionStatus? status, LivestockRegulatoryExchangeType? exchangeType, Guid? farmId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.LivestockExchange.ListSubmissions.ListSubmissionsQuery(status, exchangeType, farmId), cancellationToken)).ToHttpResult())
            .WithTags("Livestock external exchange").RequireAuthorization(PlatformPermissions.LivestockRead);

    }
}
