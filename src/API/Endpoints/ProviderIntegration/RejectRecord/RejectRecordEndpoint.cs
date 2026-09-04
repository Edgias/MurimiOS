using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.RejectRecord;
using static EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ProviderIntegrationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.RejectRecord;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class RejectRecordEndpoint
{
    internal static void Map(IEndpointRouteBuilder integrations)
    {
        integrations.MapPost("/records/{recordId:guid}/reject", async Task<IResult> (Guid recordId, RejectIngestionRecordRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ProviderIntegration.RejectRecord.RejectRecordCommand(recordId, new global::EdgIQ.MurimiOS.Application.ProviderIntegration.RejectRecord.RejectIngestionRecordRequest(request.Code, request.Reason)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.IntegrationsOperate);

    }
}
