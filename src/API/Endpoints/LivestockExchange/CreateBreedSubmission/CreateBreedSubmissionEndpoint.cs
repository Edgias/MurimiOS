using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.LivestockExchangeEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.CreateBreedSubmission;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using CreateLivestockExchangeSubmissionRequest = global::EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.Shared.CreateLivestockExchangeSubmissionRequest;
internal static class CreateBreedSubmissionEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapPost("/livestock/subjects/{subjectId:guid}/breed-registry-submissions", async Task<IResult> (Guid subjectId, CreateLivestockExchangeSubmissionRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.LivestockExchange.CreateBreedSubmission.CreateBreedSubmissionCommand(subjectId, new global::EdgIQ.MurimiOS.Application.LivestockExchange.CreateBreedSubmission.CreateLivestockExchangeSubmissionRequest(request.Id, request.ProfileId)), cancellationToken)).ToHttpResult())
            .WithTags("Livestock external exchange").RequireAuthorization(PlatformPermissions.LivestockWrite);

    }
}
