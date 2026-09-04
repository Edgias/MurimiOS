using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.RegionalFoundation.PutSettings;
using CurrencyCode = global::EdgIQ.MurimiOS.Domain.Common.CurrencyCode;
using ReferenceCatalogue = global::EdgIQ.MurimiOS.Domain.ReferenceCatalogue.ReferenceCatalogue;
using ReferenceCatalogueItem = global::EdgIQ.MurimiOS.Domain.ReferenceCatalogue.ReferenceCatalogueItem;
using TenantRegionalSettings = global::EdgIQ.MurimiOS.Domain.TenantRegionalSettings.TenantRegionalSettings;

public sealed class PutSettingsHandler(IPlatformDbContext db) : IRequestHandler<PutSettingsCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(PutSettingsCommand message, CancellationToken cancellationToken) => PutSettingsAsync(message.request, db, cancellationToken);

    private static async Task<UseCaseResult> PutSettingsAsync(
        UpdateRegionalSettingsRequest request,
        IPlatformDbContext db,
        CancellationToken cancellationToken)
    {
        try
        {
            TenantRegionalSettings? settings = await db.TenantRegionalSettings.SingleOrDefaultAsync(cancellationToken);
            if (settings is null)
            {
                settings = new TenantRegionalSettings(
                    Guid.NewGuid(), request.CountryCode, request.CurrencyCode, request.Locale, request.TimeZone,
                    request.CropYearStartMonth, request.AreaUnit, request.MassUnit, request.VolumeUnit, request.TemperatureUnit);
                db.TenantRegionalSettings.Add(settings);
            }
            else
            {
                settings.Update(
                    request.CountryCode, request.CurrencyCode, request.Locale, request.TimeZone,
                    request.CropYearStartMonth, request.AreaUnit, request.MassUnit, request.VolumeUnit, request.TemperatureUnit);
            }

            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Ok(ToResponse(settings));
        }
        catch (ArgumentException exception)
        {
            return UseCaseResults.BadRequest(new { code = "regional_settings_invalid", message = exception.Message });
        }
    }


    private static RegionalSettingsResponse ToResponse(TenantRegionalSettings settings) => new(
        settings.Id, settings.CountryCode, settings.CurrencyCode, settings.Locale, settings.TimeZone,
        settings.CropYearStartMonth, settings.AreaUnit, settings.MassUnit, settings.VolumeUnit,
        settings.TemperatureUnit, settings.Version);


    private static ReferenceCatalogueResponse ToResponse(ReferenceCatalogue catalogue) => new(
        catalogue.Id, catalogue.Key, catalogue.Name, catalogue.CatalogueVersion, catalogue.CountryCode,
        catalogue.Authority, catalogue.EffectiveFrom, catalogue.EffectiveTo, catalogue.Version);


    private static ReferenceCatalogueItemResponse ToResponse(ReferenceCatalogueItem item)
    {
        using JsonDocument attributes = JsonDocument.Parse(item.AttributesJson);
        return new ReferenceCatalogueItemResponse(
            item.Id, item.CatalogueId, item.Code, item.Name, item.Description,
            attributes.RootElement.Clone(), item.EffectiveFrom, item.EffectiveTo, item.Version);
    }

}
