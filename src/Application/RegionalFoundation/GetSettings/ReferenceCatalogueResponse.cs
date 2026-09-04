using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.RegionalFoundation.GetSettings;
using CurrencyCode = global::EdgIQ.MurimiOS.Domain.Common.CurrencyCode;
using ReferenceCatalogue = global::EdgIQ.MurimiOS.Domain.ReferenceCatalogue.ReferenceCatalogue;
using ReferenceCatalogueItem = global::EdgIQ.MurimiOS.Domain.ReferenceCatalogue.ReferenceCatalogueItem;
using TenantRegionalSettings = global::EdgIQ.MurimiOS.Domain.TenantRegionalSettings.TenantRegionalSettings;

public sealed record ReferenceCatalogueResponse(
    Guid Id,
    string Key,
    string Name,
    string CatalogueVersion,
    string? CountryCode,
    string? Authority,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    uint Version);
