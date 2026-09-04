using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.Shared;

public sealed record ReferenceCatalogueItemResponse(
    Guid Id,
    Guid CatalogueId,
    string Code,
    string Name,
    string? Description,
    JsonElement Attributes,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    uint Version);
