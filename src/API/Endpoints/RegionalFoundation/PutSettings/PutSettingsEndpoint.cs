using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.PutSettings;
using static EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.RegionalFoundationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.PutSettings;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class PutSettingsEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapPut("/settings/regional", async Task<IResult> (UpdateRegionalSettingsRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.RegionalFoundation.PutSettings.PutSettingsCommand(new global::EdgIQ.MurimiOS.Application.RegionalFoundation.PutSettings.UpdateRegionalSettingsRequest(request.CountryCode, request.CurrencyCode, request.Locale, request.TimeZone, request.CropYearStartMonth, request.AreaUnit, request.MassUnit, request.VolumeUnit, request.TemperatureUnit)), cancellationToken)).ToHttpResult())
            .WithTags("Settings")
            .RequireAuthorization(PlatformPermissions.SettingsManage);

    }
}
