using System.Data;
using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.CropPlanning.CreateApplicationRule;
public sealed record CreateProductApplicationRuleRequest(Guid? Id, string Crop, string Jurisdiction,
    decimal MaximumRatePerHa, string RateUnit, int ReEntryHours, int PreHarvestIntervalDays,
    DateOnly EffectiveFrom, DateOnly? EffectiveTo, string AuthorizationReference);
