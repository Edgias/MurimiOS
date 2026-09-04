using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.Epcis.EpcisEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Epcis.Capture;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using IncomingContentAdapter = global::EdgIQ.MurimiOS.API.Infrastructure.IncomingContentAdapter;
internal static class CaptureEndpoint
{
    internal static void Map(IEndpointRouteBuilder epcis)
    {
        epcis.MapPost("/capture", async Task<IResult> (Guid farmId, HttpRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Epcis.Capture.CaptureCommand(farmId, EdgIQ.MurimiOS.API.Infrastructure.IncomingContentAdapter.Bind(request)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.TraceabilityWrite);

    }
}
