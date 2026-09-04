using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.CreateCatalogue;

public sealed record CreateReferenceCatalogueRequest(
    string Key,
    string Name,
    string Version,
    string? CountryCode,
    string? Authority,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo);
