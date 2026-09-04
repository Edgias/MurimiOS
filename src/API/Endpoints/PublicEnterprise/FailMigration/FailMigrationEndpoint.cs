using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.FailMigration;
using static EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicEnterpriseEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.FailMigration;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class FailMigrationEndpoint
{
    internal static void Map(IEndpointRouteBuilder enterprise)
    {
        enterprise.MapPost("/migration-jobs/{jobId:guid}/fail", async Task<IResult> (Guid jobId, FailTenantMigrationRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.PublicEnterprise.FailMigration.FailMigrationCommand(jobId, new global::EdgIQ.MurimiOS.Application.PublicEnterprise.FailMigration.FailTenantMigrationRequest(request.Code, request.Message)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.TenantMigrationManage);

    }
}
