using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.LivestockStatutoryEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.ListCropFeedTransfers;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListCropFeedTransfersEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapGet("/farms/{farmId:guid}/livestock/crop-feed-transfers", async Task<IResult> (Guid farmId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.LivestockStatutory.ListCropFeedTransfers.ListCropFeedTransfersQuery(farmId), cancellationToken)).ToHttpResult())
            .WithTags("Livestock feed traceability").RequireAuthorization(PlatformPermissions.LivestockRead);

    }
}
