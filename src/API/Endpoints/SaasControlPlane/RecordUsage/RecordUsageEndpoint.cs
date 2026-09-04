using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.RecordUsage;
using static EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.SaasControlPlaneEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.RecordUsage;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class RecordUsageEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {
        group.MapPost("/usage", async Task<IResult> (RecordUsageRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SaasControlPlane.RecordUsage.RecordUsageCommand(new global::EdgIQ.MurimiOS.Application.SaasControlPlane.RecordUsage.RecordUsageRequest(request.Id, request.IdempotencyKey, request.MetricCode, request.Quantity, request.Unit, request.OccurredAt, request.Source, request.Dimensions)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SaasUsageWrite);

    }
}
