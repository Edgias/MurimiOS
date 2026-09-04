using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.CreateItem;

public sealed record CreateReferenceCatalogueItemRequest(
    string Code,
    string Name,
    string? Description,
    JsonElement? Attributes,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo);
