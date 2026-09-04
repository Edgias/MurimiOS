using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.Compliance.ComplianceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Compliance.ChangeStandard;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ChangeStandardEndpoint
{
    internal static void Map(IEndpointRouteBuilder standards)
    {
        standards.MapPost("/{standardId:guid}/{command:regex(^publish|retire$)}", async Task<IResult> (Guid standardId, string command, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Compliance.ChangeStandard.ChangeStandardCommand(standardId, command), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.ComplianceManage);

    }
}
