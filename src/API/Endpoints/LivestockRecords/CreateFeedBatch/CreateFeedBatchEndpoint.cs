using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.CreateFeedBatch;
using static EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.LivestockRecordsEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.CreateFeedBatch;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateFeedBatchEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapPost("/farms/{farmId:guid}/livestock/feed-batches", async Task<IResult> (Guid farmId, CreateLivestockFeedBatchRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.LivestockRecords.CreateFeedBatch.CreateFeedBatchCommand(farmId, new global::EdgIQ.MurimiOS.Application.LivestockRecords.CreateFeedBatch.CreateLivestockFeedBatchRequest(request.Id, request.BatchCode, request.FeedName, request.Quantity, request.Unit, request.ReceivedOn, request.ExpiresOn, request.Supplier)), cancellationToken)).ToHttpResult()).WithTags("Livestock").RequireAuthorization(PlatformPermissions.LivestockWrite);

    }
}
