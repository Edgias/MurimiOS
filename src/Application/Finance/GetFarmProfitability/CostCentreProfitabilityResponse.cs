using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.CostCentre;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Finance.GetFarmProfitability;

using CurrencyCode = global::EdgIQ.MurimiOS.Domain.Common.CurrencyCode;
using CostCentre = global::EdgIQ.MurimiOS.Domain.CostCentre.CostCentre;
using CostCentreType = global::EdgIQ.MurimiOS.Domain.CostCentre.CostCentreType;
using CostAllocation = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.CostAllocation;
using FinanceEntryType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceEntryType;
using FinanceTransaction = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceTransaction;
using LivestockSubjectKind = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubjectKind;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;

public sealed record CostCentreProfitabilityResponse(Guid CostCentreId, string Code, string Name,
    CostCentreType Type, Guid? TargetId, decimal Expenses, decimal Revenue, decimal GrossMargin,
    decimal? AreaHa, decimal YieldKg, decimal? HeadCount, decimal? CostPerHa,
    decimal? CostPerTonne, decimal? MarginPerHa, decimal? CostPerHead, decimal? MarginPerHead);
