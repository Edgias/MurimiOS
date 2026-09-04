using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.TenantMigrationJob;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicEnterpriseEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.ListMigrationJobs;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using TenantMigrationStatus = global::EdgIQ.MurimiOS.Domain.TenantMigrationJob.TenantMigrationStatus;
internal static class ListMigrationJobsEndpoint
{
    internal static void Map(IEndpointRouteBuilder enterprise)
    {
        enterprise.MapGet("/migration-jobs", async Task<IResult> (TenantMigrationStatus? status, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.PublicEnterprise.ListMigrationJobs.ListMigrationJobsQuery(status), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.TenantMigrationManage);

    }
}
