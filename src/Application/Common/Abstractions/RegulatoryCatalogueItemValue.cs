using System.Text.Json;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public sealed record RegulatoryCatalogueItemValue(
    string CatalogueVersion,
    string Code,
    string Name,
    string? Description,
    string AttributesJson,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo) : ProviderNormalizedRecord;
