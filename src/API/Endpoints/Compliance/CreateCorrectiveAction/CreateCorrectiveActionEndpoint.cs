using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Compliance.CreateCorrectiveAction;
using static EdgIQ.MurimiOS.API.Endpoints.Compliance.ComplianceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Compliance.CreateCorrectiveAction;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateCorrectiveActionEndpoint
{
    internal static void Map(IEndpointRouteBuilder findings)
    {
        findings.MapPost("/{findingId:guid}/actions", async Task<IResult> (Guid findingId, CreateCorrectiveActionRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Compliance.CreateCorrectiveAction.CreateCorrectiveActionCommand(findingId, new global::EdgIQ.MurimiOS.Application.Compliance.CreateCorrectiveAction.CreateCorrectiveActionRequest(request.Id, request.Action, request.Owner, request.DueOn)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.ComplianceAssess);

    }
}
