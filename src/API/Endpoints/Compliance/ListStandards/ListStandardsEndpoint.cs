using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.Compliance.ComplianceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Compliance.ListStandards;
using ComplianceStandardStatus = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardStatus;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListStandardsEndpoint
{
    internal static void Map(IEndpointRouteBuilder standards)
    {
        standards.MapGet("/", async Task<IResult> (ComplianceStandardStatus? status, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Compliance.ListStandards.ListStandardsQuery(status), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.ComplianceRead);

    }
}
