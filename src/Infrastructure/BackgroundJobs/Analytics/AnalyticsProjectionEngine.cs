using System.Text.Json;
using EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition;
using EdgIQ.MurimiOS.Domain.PredictiveModelDefinition;
using EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun;
using EdgIQ.MurimiOS.Domain.ComplianceAssessment;
using EdgIQ.MurimiOS.Domain.CostCentre;
using EdgIQ.MurimiOS.Domain.FinanceTransaction;
using EdgIQ.MurimiOS.Domain.FieldRiskRule;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.LivestockFeedBatch;
using EdgIQ.MurimiOS.Domain.LivestockMilkRecord;
using EdgIQ.MurimiOS.Domain.LivestockMeasurementRecord;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using EdgIQ.MurimiOS.Domain.CropPlan;
using EdgIQ.MurimiOS.Domain.HarvestLot;
using EdgIQ.MurimiOS.Domain.GrowingSeason;
using EdgIQ.MurimiOS.Domain.CropZone;
using EdgIQ.MurimiOS.Domain.HostedSupplierAccount;
using EdgIQ.MurimiOS.Domain.SupplierEvidenceRequest;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Analytics;
using Crop = global::EdgIQ.MurimiOS.Domain.Crop.Crop;
using Currency = global::EdgIQ.MurimiOS.Domain.Currency.Currency;
using Supplier = global::EdgIQ.MurimiOS.Domain.Supplier.Supplier;

using BenchmarkCohortDefinition = global::EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition.BenchmarkCohortDefinition;
using FieldRiskRule = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.FieldRiskRule;
using LivestockFeedBatch = global::EdgIQ.MurimiOS.Domain.LivestockFeedBatch.LivestockFeedBatch;

using AnalyticsFact = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsFact;
using AnalyticsFactKind = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsFactKind;
using AnalyticsProjectionRun = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsProjectionRun;
using AnalyticsProjectionStatus = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsProjectionStatus;
using AnalyticsValueKind = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsValueKind;
using AnalyticsGovernanceStatus = global::EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition.AnalyticsGovernanceStatus;
using ComplianceAssessment = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessment;
using ComplianceAssessmentStatus = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessmentStatus;
using CostCentre = global::EdgIQ.MurimiOS.Domain.CostCentre.CostCentre;
using CostCentreType = global::EdgIQ.MurimiOS.Domain.CostCentre.CostCentreType;
using CropPlan = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlan;
using CropPlanAllocation = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanAllocation;
using CropPlanStatus = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanStatus;
using CropZone = global::EdgIQ.MurimiOS.Domain.CropZone.CropZone;
using FieldRiskAssessment = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.FieldRiskAssessment;
using CostAllocation = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.CostAllocation;
using FinanceEntryType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceEntryType;
using FinanceTransaction = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceTransaction;
using GrowingSeason = global::EdgIQ.MurimiOS.Domain.GrowingSeason.GrowingSeason;
using HarvestLot = global::EdgIQ.MurimiOS.Domain.HarvestLot.HarvestLot;
using HostedSupplierAccount = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierAccount;
using LivestockFeedAllocation = global::EdgIQ.MurimiOS.Domain.LivestockFeedBatch.LivestockFeedAllocation;
using LivestockMeasurementRecord = global::EdgIQ.MurimiOS.Domain.LivestockMeasurementRecord.LivestockMeasurementRecord;
using LivestockMilkRecord = global::EdgIQ.MurimiOS.Domain.LivestockMilkRecord.LivestockMilkRecord;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;
using PredictiveModelDefinition = global::EdgIQ.MurimiOS.Domain.PredictiveModelDefinition.PredictiveModelDefinition;
using SupplierEvidenceRequest = global::EdgIQ.MurimiOS.Domain.SupplierEvidenceRequest.SupplierEvidenceRequest;
using SupplierEvidenceRequestStatus = global::EdgIQ.MurimiOS.Domain.SupplierEvidenceRequest.SupplierEvidenceRequestStatus;
using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
using WorkOrderStatus = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderStatus;
using PlatformDbContext = global::EdgIQ.MurimiOS.Infrastructure.Data.PlatformDbContext;

public sealed class AnalyticsProjectionEngine
{
    public async Task<AnalyticsFact[]> BuildFactsAsync(
        AnalyticsProjectionRun run,
        PlatformDbContext db,
        CancellationToken cancellationToken)
    {
        var facts = new List<AnalyticsFact>();
        DateTimeOffset from = new(run.PeriodStart.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
        DateTimeOffset to = new(run.PeriodEnd.AddDays(1).ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
        var farm = await db.Farms.AsNoTracking().Where(x => x.Id == run.FarmId)
            .Select(x => new { x.Id, x.OrganisationNodeId }).SingleAsync(cancellationToken);
        GrowingSeason[] seasons = await db.GrowingSeasons.AsNoTracking()
            .Where(x => x.FarmId == run.FarmId && x.StartsOn <= run.PeriodEnd && x.EndsOn >= run.PeriodStart)
            .ToArrayAsync(cancellationToken);
        Guid[] seasonIds = seasons.Select(x => x.Id).ToArray();
        CropZone[] zones = await db.CropZones.AsNoTracking().Where(x => seasonIds.Contains(x.SeasonId)).ToArrayAsync(cancellationToken);
        Guid[] zoneIds = zones.Select(x => x.Id).ToArray();
        HarvestLot[] harvests = await db.HarvestLots.AsNoTracking()
            .Where(x => zoneIds.Contains(x.CropZoneId) && x.HarvestedAt >= from && x.HarvestedAt < to)
            .ToArrayAsync(cancellationToken);
        WorkOrder[] orders = await db.WorkOrders.AsNoTracking()
            .Where(x => x.FarmId == run.FarmId && x.ScheduledStart < to && x.ScheduledEnd >= from)
            .ToArrayAsync(cancellationToken);
        CropPlan[] plans = await db.CropPlans.AsNoTracking()
            .Where(x => x.FarmId == run.FarmId && seasonIds.Contains(x.SeasonId) && x.Status == CropPlanStatus.Approved)
            .ToArrayAsync(cancellationToken);
        Guid[] planIds = plans.Select(x => x.Id).ToArray();
        CropPlanAllocation[] planAllocations = await db.CropPlanAllocations.AsNoTracking()
            .Where(x => planIds.Contains(x.PlanId)).ToArrayAsync(cancellationToken);
        CostCentre[] centres = await db.CostCentres.AsNoTracking()
            .Where(x => x.FarmId == run.FarmId && x.Currency == run.Currency).ToArrayAsync(cancellationToken);
        FinanceTransaction[] transactions = await db.FinanceTransactions.AsNoTracking()
            .Where(x => x.FarmId == run.FarmId && x.Currency == run.Currency && x.OccurredAt >= from && x.OccurredAt < to)
            .ToArrayAsync(cancellationToken);
        Guid[] transactionIds = transactions.Select(x => x.Id).ToArray();
        CostAllocation[] allocations = await db.CostAllocations.AsNoTracking()
            .Where(x => transactionIds.Contains(x.TransactionId)).ToArrayAsync(cancellationToken);
        var transactionById = transactions.ToDictionary(x => x.Id);
        PredictiveModelDefinition? yieldModel = await db.PredictiveModelDefinitions.AsNoTracking()
            .Where(x => x.Status == AnalyticsGovernanceStatus.Active
                && x.TargetMetricCode == "crop.yield.kg_per_ha" && x.Algorithm == "risk-adjusted-weighted-baseline-v1")
            .OrderByDescending(x => x.ModelVersion).FirstOrDefaultAsync(cancellationToken);
        AnalyticsFact[] historicalYield = yieldModel is null ? [] : await db.AnalyticsFacts.AsNoTracking()
            .Where(x => x.FarmId == run.FarmId && x.MetricCode == "crop.yield.kg_per_ha"
                && x.ValueKind == AnalyticsValueKind.Actual && x.PeriodEnd < run.PeriodStart
                && x.PeriodEnd >= run.PeriodStart.AddDays(-yieldModel.TrainingWindowDays)
                && db.AnalyticsProjectionRuns.Any(r => r.Id == x.ProjectionRunId
                    && r.Status == AnalyticsProjectionStatus.Succeeded && r.SupersededAt == null))
            .ToArrayAsync(cancellationToken);
        Guid[] fieldIds = zones.Select(x => x.FieldId).Distinct().ToArray();
        FieldRiskAssessment[] fieldRisks = yieldModel is null ? [] : await db.FieldRiskAssessments.AsNoTracking()
            .Where(x => fieldIds.Contains(x.FieldId) && x.Triggered && x.AssessedAt >= from && x.AssessedAt < to)
            .ToArrayAsync(cancellationToken);

        foreach (CropZone zone in zones)
        {
            HarvestLot[] zoneHarvests = harvests.Where(x => x.CropZoneId == zone.Id).ToArray();
            decimal harvestKg = zoneHarvests.Where(x => x.Unit == "kg").Sum(x => x.Quantity);
            WorkOrder[] zoneOrders = orders.Where(x => x.CropZoneId == zone.Id).ToArray();
            decimal completion = zoneOrders.Length == 0
                ? 0
                : 100m * zoneOrders.Count(x => x.Status is WorkOrderStatus.Verified or WorkOrderStatus.Superseded) / zoneOrders.Length;
            Guid[] centreIds = centres.Where(x => x.Type == CostCentreType.CropZone && x.TargetId == zone.Id).Select(x => x.Id).ToArray();
            CostAllocation[] zoneAllocations = allocations.Where(x => centreIds.Contains(x.CostCentreId)).ToArray();
            decimal expense = Sum(zoneAllocations, transactionById, FinanceEntryType.Expense);
            decimal revenue = Sum(zoneAllocations, transactionById, FinanceEntryType.Revenue);
            decimal quality = (zone.AreaHa > 0 ? 25 : 0) + (zoneHarvests.Length > 0 ? 25 : 0)
                + (zoneAllocations.Length > 0 ? 25 : 0) + (zoneOrders.Length > 0 ? 25 : 0);
            string grain = $"crop-zone:{zone.Id}";
            if (zone.AreaHa > 0)
            {
                facts.Add(Fact(run, farm.OrganisationNodeId, AnalyticsFactKind.Crop, AnalyticsValueKind.Actual,
                    "crop.yield.kg_per_ha", harvestKg / zone.AreaHa, "kg/ha", quality, zoneHarvests.Length, grain, zone));
                facts.Add(Fact(run, farm.OrganisationNodeId, AnalyticsFactKind.Crop, AnalyticsValueKind.Actual,
                    "crop.cost.per_ha", expense / zone.AreaHa, $"{run.Currency}/ha", quality, zoneAllocations.Length, grain, zone));
                facts.Add(Fact(run, farm.OrganisationNodeId, AnalyticsFactKind.Crop, AnalyticsValueKind.Actual,
                    "crop.gross_margin.per_ha", (revenue - expense) / zone.AreaHa, $"{run.Currency}/ha", quality, zoneAllocations.Length, grain, zone));
            }
            facts.Add(Fact(run, farm.OrganisationNodeId, AnalyticsFactKind.Operations, AnalyticsValueKind.Actual,
                "operations.completion.percent", completion, "%", quality, zoneOrders.Length, grain, zone));
            CropPlanAllocation? planned = planAllocations.Where(x => x.CropZoneId == zone.Id && x.TargetYieldUnit == "kg")
                .OrderByDescending(x => plans.Single(p => p.Id == x.PlanId).Revision).FirstOrDefault();
            if (planned is not null && planned.PlannedAreaHa > 0)
            {
                decimal planYield = planned.TargetYield / planned.PlannedAreaHa;
                facts.Add(Fact(run, farm.OrganisationNodeId, AnalyticsFactKind.Crop, AnalyticsValueKind.Plan,
                    "crop.yield.kg_per_ha", planYield, "kg/ha", 100, 1, grain, zone));
                facts.Add(Fact(run, farm.OrganisationNodeId, AnalyticsFactKind.Crop, AnalyticsValueKind.Plan,
                    "crop.revenue", planned.PlannedRevenue, run.Currency, 100, 1, grain, zone));
                AnalyticsFact[] history = yieldModel is null ? [] : historicalYield.Where(x =>
                    string.Equals(x.Crop, zone.Crop, StringComparison.OrdinalIgnoreCase)
                    && (string.IsNullOrWhiteSpace(zone.Variety) || string.Equals(x.Variety, zone.Variety, StringComparison.OrdinalIgnoreCase)))
                    .ToArray();
                if (yieldModel is not null && history.Length >= yieldModel.MinimumObservations)
                {
                    decimal historicalAverage = history.Average(x => x.Value);
                    int severity = fieldRisks.Where(x => x.FieldId == zone.FieldId).Select(x => (int)x.Severity).DefaultIfEmpty(0).Max();
                    decimal riskPenaltyPercent = severity switch { 4 => 20m, 3 => 12m, 2 => 6m, 1 => 2m, _ => 0m };
                    decimal forecast = (historicalAverage * 0.7m + planYield * 0.3m) * (1m - riskPenaltyPercent / 100m);
                    decimal uncertaintyPercent = yieldModel.ValidationMapePercent is { } mape
                        ? Math.Clamp(mape, 5m, 50m) : 20m;
                    decimal confidence = Math.Clamp(100m - uncertaintyPercent, 50m, 95m);
                    DateOnly targetDate = seasons.Single(x => x.Id == zone.SeasonId).EndsOn;
                    string modelVersion = $"{yieldModel.ModelKey}:v{yieldModel.ModelVersion}";
                    facts.Add(new AnalyticsFact(Guid.NewGuid(), run.Id, run.FarmId, farm.OrganisationNodeId,
                        AnalyticsFactKind.Crop, AnalyticsValueKind.Forecast, "crop.yield.kg_per_ha",
                        decimal.Round(forecast, 8), "kg/ha", run.PeriodStart, targetDate,
                        decimal.Round(history.Average(x => x.DataQualityPercent), 4), history.Length,
                        modelVersion, grain, seasonId: zone.SeasonId, fieldId: zone.FieldId, cropZoneId: zone.Id,
                        crop: zone.Crop, variety: zone.Variety,
                        dimensionsJson: JsonSerializer.Serialize(new { historicalAverage, planYield,
                            riskPenaltyPercent, historicalObservations = history.Length, targetDate }),
                        predictiveModelDefinitionId: yieldModel.Id, confidencePercent: confidence,
                        lowerBound: decimal.Round(forecast * (1m - uncertaintyPercent / 100m), 8),
                        upperBound: decimal.Round(forecast * (1m + uncertaintyPercent / 100m), 8)));
                }
            }
        }

        LivestockSubject[] subjects = await db.LivestockSubjects.AsNoTracking().Where(x => x.FarmId == run.FarmId)
            .ToArrayAsync(cancellationToken);
        Guid[] subjectIds = subjects.Select(x => x.Id).ToArray();
        LivestockMeasurementRecord[] measurements = await db.LivestockMeasurements.AsNoTracking()
            .Where(x => subjectIds.Contains(x.SubjectId) && x.OccurredAt >= from && x.OccurredAt < to && x.SupersededById == null)
            .ToArrayAsync(cancellationToken);
        LivestockFeedAllocation[] feed = await db.LivestockFeedAllocations.AsNoTracking()
            .Where(x => subjectIds.Contains(x.SubjectId) && x.OccurredAt >= from && x.OccurredAt < to)
            .ToArrayAsync(cancellationToken);
        LivestockMilkRecord[] milk = await db.LivestockMilkRecords.AsNoTracking()
            .Where(x => subjectIds.Contains(x.SubjectId) && x.MilkedAt >= from && x.MilkedAt < to)
            .ToArrayAsync(cancellationToken);
        foreach (LivestockSubject subject in subjects)
        {
            LivestockMeasurementRecord[] weights = measurements.Where(x => x.SubjectId == subject.Id
                    && x.Metric.Equals("weight", StringComparison.OrdinalIgnoreCase) && x.Unit == "kg")
                .OrderBy(x => x.OccurredAt).ToArray();
            LivestockFeedAllocation[] subjectFeed = feed.Where(x => x.SubjectId == subject.Id && x.Unit == "kg").ToArray();
            LivestockMilkRecord[] subjectMilk = milk.Where(x => x.SubjectId == subject.Id).ToArray();
            decimal gain = weights.Length >= 2 ? weights[^1].Value - weights[0].Value : 0;
            decimal days = weights.Length >= 2 ? Math.Max(1, (decimal)(weights[^1].OccurredAt - weights[0].OccurredAt).TotalDays) : 0;
            decimal quality = (weights.Length >= 2 ? 40 : 0) + (subjectFeed.Length > 0 ? 30 : 0) + (subjectMilk.Length > 0 ? 30 : 0);
            string grain = $"livestock:{subject.Id}";
            if (days > 0)
                facts.Add(Fact(run, farm.OrganisationNodeId, "livestock.dlwg.kg_per_day", gain / days,
                    "kg/day", quality, weights.Length, grain, subject));
            if (gain > 0 && subjectFeed.Length > 0)
                facts.Add(Fact(run, farm.OrganisationNodeId, "livestock.feed_conversion",
                    subjectFeed.Sum(x => x.Quantity) / gain, "kg/kg", quality, subjectFeed.Length + weights.Length, grain, subject));
            if (subjectMilk.Length > 0)
                facts.Add(Fact(run, farm.OrganisationNodeId, "livestock.saleable_milk.litres",
                    subjectMilk.Sum(x => x.SaleableLitres), "L", quality, subjectMilk.Length, grain, subject));
        }

        decimal totalExpense = Sum(allocations, transactionById, FinanceEntryType.Expense);
        decimal totalRevenue = Sum(allocations, transactionById, FinanceEntryType.Revenue);
        decimal farmQuality = facts.Count == 0 ? 0 : facts.Average(x => x.DataQualityPercent);
        facts.Add(Fact(run, farm.OrganisationNodeId, AnalyticsFactKind.Farm, AnalyticsValueKind.Actual,
            "farm.expense", totalExpense, run.Currency, farmQuality, transactions.Length, "farm"));
        facts.Add(Fact(run, farm.OrganisationNodeId, AnalyticsFactKind.Farm, AnalyticsValueKind.Actual,
            "farm.revenue", totalRevenue, run.Currency, farmQuality, transactions.Length, "farm"));
        facts.Add(Fact(run, farm.OrganisationNodeId, AnalyticsFactKind.Farm, AnalyticsValueKind.Actual,
            "farm.gross_margin", totalRevenue - totalExpense, run.Currency, farmQuality, transactions.Length, "farm"));
        ComplianceAssessment? compliance = await db.ComplianceAssessments.AsNoTracking()
            .Where(x => x.FarmId == run.FarmId && x.Status == ComplianceAssessmentStatus.Completed
                && x.CompletedAt >= from && x.CompletedAt < to)
            .OrderByDescending(x => x.CompletedAt).FirstOrDefaultAsync(cancellationToken);
        if (compliance?.ScorePercent is { } score)
            facts.Add(Fact(run, farm.OrganisationNodeId, AnalyticsFactKind.Compliance, AnalyticsValueKind.Actual,
                "compliance.score.percent", score, "%", 100, 1, "farm"));
        HostedSupplierAccount[] suppliers = await db.HostedSupplierAccounts.AsNoTracking().ToArrayAsync(cancellationToken);
        foreach (HostedSupplierAccount supplier in suppliers)
        {
            SupplierEvidenceRequest[] requests = await db.SupplierEvidenceRequests.AsNoTracking()
                .Where(x => x.HostedSupplierId == supplier.Id && x.DueOn >= run.PeriodStart && x.DueOn <= run.PeriodEnd)
                .ToArrayAsync(cancellationToken);
            if (requests.Length == 0) continue;
            decimal completion = 100m * requests.Count(x => x.Status == SupplierEvidenceRequestStatus.Accepted) / requests.Length;
            facts.Add(new AnalyticsFact(Guid.NewGuid(), run.Id, run.FarmId, farm.OrganisationNodeId,
                AnalyticsFactKind.Supplier, AnalyticsValueKind.Actual, "supplier.evidence_completion.percent",
                completion, "%", run.PeriodStart, run.PeriodEnd, 100, requests.Length, run.ProjectionVersion,
                $"supplier:{supplier.Id}", supplierId: supplier.Id,
                dimensionsJson: JsonSerializer.Serialize(new { supplier.SupplierCode, supplier.Name })));
        }
        return facts.ToArray();
    }

    private static decimal Sum(IEnumerable<CostAllocation> allocations,
        IReadOnlyDictionary<Guid, FinanceTransaction> transactions, FinanceEntryType type) =>
        allocations.Where(x => transactions[x.TransactionId].EntryType == type)
            .Sum(x => x.Amount * (transactions[x.TransactionId].ReversalOfId is null ? 1m : -1m));

    private static AnalyticsFact Fact(AnalyticsProjectionRun run, Guid orgId, AnalyticsFactKind kind,
        AnalyticsValueKind valueKind, string metric, decimal value, string unit, decimal quality, int sourceCount,
        string grain, CropZone? zone = null) => new(Guid.NewGuid(), run.Id, run.FarmId, orgId, kind, valueKind,
        metric, decimal.Round(value, 8), unit, run.PeriodStart, run.PeriodEnd, quality, sourceCount,
        run.ProjectionVersion, grain, seasonId: zone?.SeasonId, fieldId: zone?.FieldId,
        cropZoneId: zone?.Id, crop: zone?.Crop, variety: zone?.Variety);

    private static AnalyticsFact Fact(AnalyticsProjectionRun run, Guid orgId, string metric, decimal value,
        string unit, decimal quality, int sourceCount, string grain, LivestockSubject subject) =>
        new(Guid.NewGuid(), run.Id, run.FarmId, orgId, AnalyticsFactKind.Livestock,
            AnalyticsValueKind.Actual, metric, decimal.Round(value, 8), unit, run.PeriodStart, run.PeriodEnd,
            quality, sourceCount, run.ProjectionVersion, grain, livestockSubjectId: subject.Id, species: subject.Species);
}
