using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.LivestockReproductionEvent;
using EdgIQ.MurimiOS.Domain.LivestockMeasurementRecord;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockProduction.FarmPerformance;
using LivestockReproductionEvent = global::EdgIQ.MurimiOS.Domain.LivestockReproductionEvent.LivestockReproductionEvent;


using LivestockMeasurementRecord = global::EdgIQ.MurimiOS.Domain.LivestockMeasurementRecord.LivestockMeasurementRecord;
using ReproductionEventType = global::EdgIQ.MurimiOS.Domain.LivestockReproductionEvent.ReproductionEventType;
using ReproductionOutcome = global::EdgIQ.MurimiOS.Domain.LivestockReproductionEvent.ReproductionOutcome;
using LivestockSubjectKind = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubjectKind;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;

public sealed class FarmPerformanceHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, TimeProvider clock) : IRequestHandler<FarmPerformanceQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(FarmPerformanceQuery message, CancellationToken cancellationToken) => FarmPerformanceAsync(message.farmId, message.from, message.to, db, scope, clock, cancellationToken);

    private static async Task<UseCaseResult> FarmPerformanceAsync(Guid farmId, DateTimeOffset? from, DateTimeOffset? to,
        IPlatformDbContext db, IOrganisationScopeAccess scope, TimeProvider clock, CancellationToken cancellationToken)
    {
        if (!await scope.CanAccessFarmAsync(farmId, cancellationToken)) return UseCaseResults.NotFound();
        DateTimeOffset end = to ?? clock.GetUtcNow(); DateTimeOffset start = from ?? end.AddDays(-90);
        if (end <= start) return Bad("performance_window_invalid", "The performance end must follow its start.");
        LivestockSubject[] subjects = await db.LivestockSubjects.AsNoTracking().Where(x => x.FarmId == farmId).OrderBy(x => x.Name).ToArrayAsync(cancellationToken);
        var rows = new List<LivestockPerformanceResponse>();
        foreach (LivestockSubject subject in subjects) rows.Add(await CalculatePerformance(subject, start, end, db, cancellationToken));
        var reproductive = await db.LivestockReproductionEvents.AsNoTracking().Where(x => x.FarmId == farmId
            && x.OccurredAt >= start && x.OccurredAt <= end).GroupBy(_ => 1).Select(group => new
            {
                Breedings = group.Count(x => x.Type == ReproductionEventType.Breeding),
                Pregnant = group.Count(x => x.Type == ReproductionEventType.PregnancyDiagnosis && x.Outcome == ReproductionOutcome.Pregnant),
                Open = group.Count(x => x.Type == ReproductionEventType.PregnancyDiagnosis && x.Outcome == ReproductionOutcome.Open),
                Parturitions = group.Count(x => x.Type == ReproductionEventType.Parturition),
                Weanings = group.Count(x => x.Type == ReproductionEventType.Weaning)
            }).SingleOrDefaultAsync(cancellationToken);
        int diagnosed = (reproductive?.Pregnant ?? 0) + (reproductive?.Open ?? 0);
        return UseCaseResults.Ok(new { farmId, from = start, to = end, subjects = rows,
            totalMilkLitres = rows.Sum(x => x.TotalMilkLitres), saleableMilkLitres = rows.Sum(x => x.SaleableMilkLitres),
            feedAllocatedKg = rows.Sum(x => x.FeedAllocatedKg), reproduction = new
            {
                breedings = reproductive?.Breedings ?? 0, pregnantDiagnoses = reproductive?.Pregnant ?? 0,
                openDiagnoses = reproductive?.Open ?? 0, parturitions = reproductive?.Parturitions ?? 0,
                weanings = reproductive?.Weanings ?? 0,
                pregnancyRatePercent = diagnosed == 0 ? (decimal?)null : decimal.Round((reproductive!.Pregnant * 100m) / diagnosed, 2)
            }});
    }


    private static async Task<LivestockPerformanceResponse> CalculatePerformance(LivestockSubject subject,
        DateTimeOffset start, DateTimeOffset end, IPlatformDbContext db, CancellationToken cancellationToken)
    {
        LivestockMeasurementRecord[] weights = await db.LivestockMeasurements.AsNoTracking()
            .Where(x => x.SubjectId == subject.Id && x.SupersededById == null && x.Unit == "kg"
                && (x.Metric.ToLower() == "weight" || x.Metric.ToLower() == "liveweight")
                && x.OccurredAt >= start && x.OccurredAt <= end).OrderBy(x => x.OccurredAt).ToArrayAsync(cancellationToken);
        LivestockMeasurementRecord? first = weights.FirstOrDefault(); LivestockMeasurementRecord? last = weights.LastOrDefault();
        decimal? gain = first is not null && last is not null && last.OccurredAt > first.OccurredAt ? last.Value - first.Value : null;
        decimal? days = first is not null && last is not null ? (decimal)(last.OccurredAt - first.OccurredAt).TotalDays : null;
        decimal? dlwg = gain.HasValue && days > 0 ? decimal.Round(gain.Value / days.Value, 4) : null;
        decimal feed = await db.LivestockFeedAllocations.AsNoTracking().Where(x => x.SubjectId == subject.Id && x.Unit == "kg"
            && x.OccurredAt >= start && x.OccurredAt <= end).SumAsync(x => (decimal?)x.Quantity, cancellationToken) ?? 0;
        decimal? fcr = gain > 0 ? decimal.Round(feed / gain.Value, 4) : null;
        var milk = await db.LivestockMilkRecords.AsNoTracking().Where(x => x.SubjectId == subject.Id
            && x.MilkedAt >= start && x.MilkedAt <= end).GroupBy(_ => 1)
            .Select(group => new { Total = group.Sum(x => x.QuantityLitres), Saleable = group.Sum(x => x.SaleableLitres) })
            .SingleOrDefaultAsync(cancellationToken);
        return new LivestockPerformanceResponse(subject.Id, subject.Name, subject.Kind, start, end, first?.Value,
            first?.OccurredAt, last?.Value, last?.OccurredAt, gain, dlwg, dlwg.HasValue && last is not null
                ? decimal.Round(last.Value + dlwg.Value * 30, 4) : null, feed, fcr, milk?.Total ?? 0, milk?.Saleable ?? 0);
    }

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}
