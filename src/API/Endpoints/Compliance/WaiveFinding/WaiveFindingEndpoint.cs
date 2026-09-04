using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Compliance.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Compliance.ComplianceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Compliance.WaiveFinding;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using CorrectiveActionCommandRequest = global::EdgIQ.MurimiOS.API.Endpoints.Compliance.Shared.CorrectiveActionCommandRequest;
internal static class WaiveFindingEndpoint
{
    internal static void Map(IEndpointRouteBuilder findings)
    {
        findings.MapPost("/{findingId:guid}/waive", async Task<IResult> (Guid findingId, CorrectiveActionCommandRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Compliance.WaiveFinding.WaiveFindingCommand(findingId, new global::EdgIQ.MurimiOS.Application.Compliance.WaiveFinding.CorrectiveActionCommandRequest(request.Notes)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.ComplianceVerify);

    }
}
