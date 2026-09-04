using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CreateMigration;
using static EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicEnterpriseEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CreateMigration;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateMigrationEndpoint
{
    internal static void Map(IEndpointRouteBuilder enterprise)
    {
        enterprise.MapPost("/migration-jobs", async Task<IResult> (CreateTenantMigrationRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.PublicEnterprise.CreateMigration.CreateMigrationCommand(new global::EdgIQ.MurimiOS.Application.PublicEnterprise.CreateMigration.CreateTenantMigrationRequest(request.Id, request.Kind, request.SourcePlacementId, request.DestinationRegion, request.DestinationDatabaseKey, request.DestinationMode)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.TenantMigrationManage);

    }
}
