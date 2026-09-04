using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Traceability.Shared;
using EdgIQ.MurimiOS.API.Endpoints.Traceability.CreateRecall;
using static EdgIQ.MurimiOS.API.Endpoints.Traceability.TraceabilityEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Traceability.CreateRecall;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using TraceObjectReference = global::EdgIQ.MurimiOS.API.Endpoints.Traceability.Shared.TraceObjectReference;
internal static class CreateRecallEndpoint
{
    internal static void Map(IEndpointRouteBuilder recalls)
    {
        recalls.MapPost("/", async Task<IResult> (CreateRecallRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Traceability.CreateRecall.CreateRecallCommand(new global::EdgIQ.MurimiOS.Application.Traceability.CreateRecall.CreateRecallRequest(request.Id, request.Reference, request.Reason, request.InitiatedAt, request.Sources.Select(item => new global::EdgIQ.MurimiOS.Application.Traceability.CreateRecall.TraceObjectReference(item.ObjectType, item.ObjectId)).ToArray())), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.RecallManage);

    }
}
