using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Compliance.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Compliance.ComplianceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Compliance.ChangeCorrectiveAction;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using CorrectiveActionCommandRequest = global::EdgIQ.MurimiOS.API.Endpoints.Compliance.Shared.CorrectiveActionCommandRequest;
internal static class ChangeCorrectiveActionEndpoint
{
    internal static void Map(IEndpointRouteBuilder findings)
    {
        findings.MapPost("/{findingId:guid}/actions/{actionId:guid}/{command:regex(^start|submit|verify|reject|cancel$)}", async Task<IResult> (Guid findingId, Guid actionId, string command, CorrectiveActionCommandRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Compliance.ChangeCorrectiveAction.ChangeCorrectiveActionCommand(findingId, actionId, command, new global::EdgIQ.MurimiOS.Application.Compliance.ChangeCorrectiveAction.CorrectiveActionCommandRequest(request.Notes)), cancellationToken)).ToHttpResult())
            .RequireAuthorization(PlatformPermissions.ComplianceAssess);

    }
}
