using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.CreateCropFeedTransfer;
using static EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.LivestockStatutoryEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.CreateCropFeedTransfer;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateCropFeedTransferEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapPost("/farms/{farmId:guid}/livestock/crop-feed-transfers", async Task<IResult> (Guid farmId, CreateCropFeedTransferRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.LivestockStatutory.CreateCropFeedTransfer.CreateCropFeedTransferCommand(farmId, new global::EdgIQ.MurimiOS.Application.LivestockStatutory.CreateCropFeedTransfer.CreateCropFeedTransferRequest(request.Id, request.FeedBatchId, request.SourceType, request.SourceId, request.BatchCode, request.FeedName, request.Quantity, request.Unit, request.TransferredAt, request.ExpiresOn, request.Reference, request.Notes)), cancellationToken)).ToHttpResult())
            .WithTags("Livestock feed traceability").RequireAuthorization(PlatformPermissions.LivestockWrite);

    }
}
