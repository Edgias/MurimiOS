using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.CreateProfile;
using EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.CreateProfile;
using static EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.LivestockExchangeEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.CreateProfile;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateProfileEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapPost("/livestock/regulatory-profiles", async Task<IResult> (CreateLivestockRegulatoryProfileRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.LivestockExchange.CreateProfile.CreateProfileCommand(new global::EdgIQ.MurimiOS.Application.LivestockExchange.CreateProfile.CreateLivestockRegulatoryProfileRequest(request.Id, request.ProfileKey, request.Version, request.Name, request.ExchangeType, request.Jurisdiction, request.Species, request.ProviderCode, request.SchemaVersion, request.MediaType, request.ReportingDeadlineHours, request.RegisterDeadlineHours, request.EffectiveFrom, request.EffectiveTo, request.SourceUri)), cancellationToken)).ToHttpResult())
            .WithTags("Livestock external exchange").RequireAuthorization(PlatformPermissions.LivestockWrite);

    }
}
