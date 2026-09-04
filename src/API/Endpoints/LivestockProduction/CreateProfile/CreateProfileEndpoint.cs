using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.CreateProfile;
using EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.CreateProfile;
using static EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.LivestockProductionEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.CreateProfile;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateProfileEndpoint
{
    internal static void Map(IEndpointRouteBuilder livestock)
    {
        livestock.MapPost("/subjects/{subjectId:guid}/biological-profile", async Task<IResult> (Guid subjectId, CreateBiologicalProfileRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.LivestockProduction.CreateProfile.CreateProfileCommand(subjectId, new global::EdgIQ.MurimiOS.Application.LivestockProduction.CreateProfile.CreateBiologicalProfileRequest(request.Sex, request.BornOn, request.Breed, request.DamId, request.SireId)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.LivestockWrite);

    }
}
