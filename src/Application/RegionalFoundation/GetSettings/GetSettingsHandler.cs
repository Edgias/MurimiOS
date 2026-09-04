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
namespace EdgIQ.MurimiOS.Application.RegionalFoundation.GetSettings;
using CurrencyCode = global::EdgIQ.MurimiOS.Domain.Common.CurrencyCode;
using ReferenceCatalogue = global::EdgIQ.MurimiOS.Domain.ReferenceCatalogue.ReferenceCatalogue;
using ReferenceCatalogueItem = global::EdgIQ.MurimiOS.Domain.ReferenceCatalogue.ReferenceCatalogueItem;
using TenantRegionalSettings = global::EdgIQ.MurimiOS.Domain.TenantRegionalSettings.TenantRegionalSettings;

public sealed class GetSettingsHandler(IPlatformDbContext db) : IRequestHandler<GetSettingsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetSettingsQuery message, CancellationToken cancellationToken) => GetSettingsAsync(db, cancellationToken);

    private static async Task<UseCaseResult> GetSettingsAsync(IPlatformDbContext db, CancellationToken cancellationToken)
    {
        TenantRegionalSettings? settings = await db.TenantRegionalSettings.SingleOrDefaultAsync(cancellationToken);
        return settings is null
            ? UseCaseResults.NotFound(new { code = "regional_settings_not_configured", message = "Configure regional settings before recording regionalized business data." })
            : UseCaseResults.Ok(ToResponse(settings));
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
