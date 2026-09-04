using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using EdgIQ.MurimiOS.API.Endpoints.Notification.Get;
using static EdgIQ.MurimiOS.API.Endpoints.ComplianceAuditPack.ComplianceAuditPackEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ComplianceAuditPack.Get;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetEndpoint
{
    internal static void Map(IEndpointRouteBuilder farms)
    {
        farms.MapGet("/audit-packs/{exportId:guid}", async Task<IResult> (Guid farmId, Guid exportId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ComplianceAuditPack.Get.GetQuery(farmId, exportId), cancellationToken)).ToHttpResult())
            .RequireAuthorization(PlatformPermissions.ComplianceRead);

    }
}
