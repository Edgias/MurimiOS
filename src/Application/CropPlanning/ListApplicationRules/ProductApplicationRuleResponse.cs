using System.Data;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.CropPlanning.ListApplicationRules;
using CropProductApplicationRule = global::EdgIQ.MurimiOS.Domain.CropProductApplicationRule.CropProductApplicationRule;
public sealed record ProductApplicationRuleResponse(Guid Id, Guid InputId, string Crop, string Jurisdiction,
    decimal MaximumRatePerHa, string RateUnit, decimal EnteredMaximumRatePerHa, string EnteredRateUnit,
    int ReEntryHours, int PreHarvestIntervalDays, DateOnly EffectiveFrom, DateOnly? EffectiveTo,
    string AuthorizationReference);
