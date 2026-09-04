using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.LivestockExchangeEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.CreateMilkSubmission;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using CreateLivestockExchangeSubmissionRequest = global::EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.Shared.CreateLivestockExchangeSubmissionRequest;
internal static class CreateMilkSubmissionEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapPost("/livestock/milk-records/{milkRecordId:guid}/recording-submissions", async Task<IResult> (Guid milkRecordId, CreateLivestockExchangeSubmissionRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.LivestockExchange.CreateMilkSubmission.CreateMilkSubmissionCommand(milkRecordId, new global::EdgIQ.MurimiOS.Application.LivestockExchange.CreateMilkSubmission.CreateLivestockExchangeSubmissionRequest(request.Id, request.ProfileId)), cancellationToken)).ToHttpResult())
            .WithTags("Livestock external exchange").RequireAuthorization(PlatformPermissions.LivestockWrite);

    }
}
