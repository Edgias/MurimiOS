using System.Data;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.CropPlanning.CreateApplicationRule;
using AgriculturalInput = global::EdgIQ.MurimiOS.Domain.AgriculturalInput.AgriculturalInput;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using CropPlan = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlan;
using CropProductApplicationRule = global::EdgIQ.MurimiOS.Domain.CropProductApplicationRule.CropProductApplicationRule;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;
public sealed record ProductApplicationRuleResponse(Guid Id, Guid InputId, string Crop, string Jurisdiction,
    decimal MaximumRatePerHa, string RateUnit, decimal EnteredMaximumRatePerHa, string EnteredRateUnit,
    int ReEntryHours, int PreHarvestIntervalDays, DateOnly EffectiveFrom, DateOnly? EffectiveTo,
    string AuthorizationReference);
