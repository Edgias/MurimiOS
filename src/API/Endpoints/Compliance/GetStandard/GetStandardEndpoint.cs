using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.Compliance.ComplianceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Compliance.GetStandard;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetStandardEndpoint
{
    internal static void Map(IEndpointRouteBuilder standards)
    {
        standards.MapGet("/{standardId:guid}", async Task<IResult> (Guid standardId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Compliance.GetStandard.GetStandardQuery(standardId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.ComplianceRead);

    }
}
