using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.RegionalFoundation.CreateItem;
using CurrencyCode = global::EdgIQ.MurimiOS.Domain.Common.CurrencyCode;
using ReferenceCatalogue = global::EdgIQ.MurimiOS.Domain.ReferenceCatalogue.ReferenceCatalogue;
using ReferenceCatalogueItem = global::EdgIQ.MurimiOS.Domain.ReferenceCatalogue.ReferenceCatalogueItem;
using TenantRegionalSettings = global::EdgIQ.MurimiOS.Domain.TenantRegionalSettings.TenantRegionalSettings;

public sealed record RegionalSettingsResponse(
    Guid Id,
    string CountryCode,
    string CurrencyCode,
    string Locale,
    string TimeZone,
    int CropYearStartMonth,
    string AreaUnit,
    string MassUnit,
    string VolumeUnit,
    string TemperatureUnit,
    uint Version);
