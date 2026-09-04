using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.Epcis.EpcisEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Epcis.Mappings;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class MappingsEndpoint
{
    internal static void Map(IEndpointRouteBuilder epcis)
    {
        epcis.MapGet("/cbv/mappings", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Epcis.Mappings.MappingsQuery(), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.TraceabilityRead);

    }
}
