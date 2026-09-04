using System.Data;
using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.CropPlanning.Shared;
public sealed record ProductApplicationRuleResponse(Guid Id, Guid InputId, string Crop, string Jurisdiction,
    decimal MaximumRatePerHa, string RateUnit, decimal EnteredMaximumRatePerHa, string EnteredRateUnit,
    int ReEntryHours, int PreHarvestIntervalDays, DateOnly EffectiveFrom, DateOnly? EffectiveTo,
    string AuthorizationReference);
