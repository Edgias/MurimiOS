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
namespace EdgIQ.MurimiOS.Application.RegionalFoundation.GetItems;
using CurrencyCode = global::EdgIQ.MurimiOS.Domain.Common.CurrencyCode;
using ReferenceCatalogue = global::EdgIQ.MurimiOS.Domain.ReferenceCatalogue.ReferenceCatalogue;
using ReferenceCatalogueItem = global::EdgIQ.MurimiOS.Domain.ReferenceCatalogue.ReferenceCatalogueItem;
using TenantRegionalSettings = global::EdgIQ.MurimiOS.Domain.TenantRegionalSettings.TenantRegionalSettings;

public sealed class GetItemsHandler(IPlatformDbContext db) : IRequestHandler<GetItemsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetItemsQuery message, CancellationToken cancellationToken) => GetItemsAsync(message.catalogueId, message.effectiveOn, db, cancellationToken);

    private static async Task<UseCaseResult> GetItemsAsync(
        Guid catalogueId,
        DateOnly? effectiveOn,
        IPlatformDbContext db,
        CancellationToken cancellationToken)
    {
        if (!await db.ReferenceCatalogues.AnyAsync(x => x.Id == catalogueId, cancellationToken))
            return UseCaseResults.NotFound();
        IQueryable<ReferenceCatalogueItem> query = db.ReferenceCatalogueItems
            .AsNoTracking()
            .Where(x => x.CatalogueId == catalogueId);
        if (effectiveOn is { } date)
            query = query.Where(x => x.EffectiveFrom <= date && (x.EffectiveTo == null || x.EffectiveTo >= date));
        ReferenceCatalogueItem[] items = await query.OrderBy(x => x.Code).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(items.Select(ToResponse));
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
