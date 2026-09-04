using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using static EdgIQ.MurimiOS.API.Endpoints.ComplianceAuditPack.ComplianceAuditPackEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ComplianceAuditPack.Verify;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class VerifyEndpoint
{
    internal static void Map(IEndpointRouteBuilder farms)
    {
        farms.MapGet("/audit-packs/{exportId:guid}/verify", async Task<IResult> (Guid farmId, Guid exportId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ComplianceAuditPack.Verify.VerifyQuery(farmId, exportId), cancellationToken)).ToHttpResult())
            .RequireAuthorization(PlatformPermissions.ComplianceExport);

    }
}
