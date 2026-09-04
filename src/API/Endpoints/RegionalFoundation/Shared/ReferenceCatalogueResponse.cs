using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.Shared;

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
