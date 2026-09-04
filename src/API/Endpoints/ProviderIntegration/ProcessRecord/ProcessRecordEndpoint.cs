using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ProviderIntegrationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ProcessRecord;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ProcessRecordEndpoint
{
    internal static void Map(IEndpointRouteBuilder integrations)
    {
        integrations.MapPost("/records/{recordId:guid}/processed", async Task<IResult> (Guid recordId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ProviderIntegration.ProcessRecord.ProcessRecordCommand(recordId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.IntegrationsOperate);

    }
}
