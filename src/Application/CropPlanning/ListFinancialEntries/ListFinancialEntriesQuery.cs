using System.Data;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.CropPlanning.ListFinancialEntries;
using CropPlan = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlan;
using CropPlanFinancialEntry = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanFinancialEntry;
using FinancialEntryType = global::EdgIQ.MurimiOS.Domain.CropPlan.FinancialEntryType;

public sealed record ListFinancialEntriesQuery(Guid planId) : IRequest<UseCaseResult>;
