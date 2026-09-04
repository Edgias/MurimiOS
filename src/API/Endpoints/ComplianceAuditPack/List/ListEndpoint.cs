using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.API.Endpoints.Audit.List;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using static EdgIQ.MurimiOS.API.Endpoints.ComplianceAuditPack.ComplianceAuditPackEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ComplianceAuditPack.List;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListEndpoint
{
    internal static void Map(IEndpointRouteBuilder farms)
    {
        farms.MapGet("/assessments/{assessmentId:guid}/audit-packs", async Task<IResult> (Guid farmId, Guid assessmentId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ComplianceAuditPack.List.ListQuery(farmId, assessmentId), cancellationToken)).ToHttpResult())
            .RequireAuthorization(PlatformPermissions.ComplianceRead);

    }
}
