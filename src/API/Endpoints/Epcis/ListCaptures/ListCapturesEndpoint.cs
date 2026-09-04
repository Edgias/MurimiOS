using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.Epcis.EpcisEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Epcis.ListCaptures;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListCapturesEndpoint
{
    internal static void Map(IEndpointRouteBuilder epcis)
    {
        epcis.MapGet("/capture", async Task<IResult> (Guid farmId, int? perPage, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Epcis.ListCaptures.ListCapturesQuery(farmId, perPage), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.TraceabilityRead);

    }
}
