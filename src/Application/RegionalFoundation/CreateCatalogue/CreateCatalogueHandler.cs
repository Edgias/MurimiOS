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
namespace EdgIQ.MurimiOS.Application.RegionalFoundation.CreateCatalogue;
using CurrencyCode = global::EdgIQ.MurimiOS.Domain.Common.CurrencyCode;
using ReferenceCatalogue = global::EdgIQ.MurimiOS.Domain.ReferenceCatalogue.ReferenceCatalogue;
using ReferenceCatalogueItem = global::EdgIQ.MurimiOS.Domain.ReferenceCatalogue.ReferenceCatalogueItem;
using TenantRegionalSettings = global::EdgIQ.MurimiOS.Domain.TenantRegionalSettings.TenantRegionalSettings;

public sealed class CreateCatalogueHandler(IPlatformDbContext db) : IRequestHandler<CreateCatalogueCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateCatalogueCommand message, CancellationToken cancellationToken) => CreateCatalogueAsync(message.request, db, cancellationToken);

    private static async Task<UseCaseResult> CreateCatalogueAsync(
        CreateReferenceCatalogueRequest request,
        IPlatformDbContext db,
        CancellationToken cancellationToken)
    {
        try
        {
            var catalogue = new ReferenceCatalogue(
                Guid.NewGuid(), request.Key, request.Name, request.Version, request.CountryCode,
                request.Authority, request.EffectiveFrom, request.EffectiveTo);
            if (await db.ReferenceCatalogues.AnyAsync(
                    x => x.Key == catalogue.Key && x.CatalogueVersion == catalogue.CatalogueVersion, cancellationToken))
                return UseCaseResults.Conflict(new { code = "reference_catalogue_exists", message = "This catalogue key and version already exist." });
            db.ReferenceCatalogues.Add(catalogue);
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/reference-catalogues/{catalogue.Id}", ToResponse(catalogue));
        }
        catch (ArgumentException exception)
        {
            return UseCaseResults.BadRequest(new { code = "reference_catalogue_invalid", message = exception.Message });
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
