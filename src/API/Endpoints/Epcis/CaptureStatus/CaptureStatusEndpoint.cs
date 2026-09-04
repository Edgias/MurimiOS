using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.Epcis.EpcisEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Epcis.CaptureStatus;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CaptureStatusEndpoint
{
    internal static void Map(IEndpointRouteBuilder epcis)
    {
        epcis.MapGet("/capture/{captureId:guid}", async Task<IResult> (Guid captureId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Epcis.CaptureStatus.CaptureStatusQuery(captureId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.TraceabilityRead);

    }
}
