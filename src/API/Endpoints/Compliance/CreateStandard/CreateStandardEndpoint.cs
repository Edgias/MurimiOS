using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Compliance.CreateStandard;
using static EdgIQ.MurimiOS.API.Endpoints.Compliance.ComplianceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Compliance.CreateStandard;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateStandardEndpoint
{
    internal static void Map(IEndpointRouteBuilder standards)
    {
        standards.MapPost("/", async Task<IResult> (CreateComplianceStandardRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Compliance.CreateStandard.CreateStandardCommand(new global::EdgIQ.MurimiOS.Application.Compliance.CreateStandard.CreateComplianceStandardRequest(request.Id, request.SeriesId, request.Code, request.Name, request.Scheme, request.VersionLabel, request.Revision, request.EffectiveFrom, request.EffectiveTo, request.Jurisdiction)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.ComplianceManage);

    }
}
