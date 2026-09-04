using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockRecords.GetHistory;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;

using LivestockGroupMembership = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockGroupMembership;

public sealed class GetHistoryHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<GetHistoryQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetHistoryQuery message, CancellationToken cancellationToken) => GetHistoryAsync(message.subjectId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> GetHistoryAsync(Guid subjectId, IPlatformDbContext db, IOrganisationScopeAccess scope,
        CancellationToken cancellationToken)
    {
        if (!await CanAccessSubjectAsync(subjectId, db, scope, cancellationToken)) return UseCaseResults.NotFound();
        var history = new List<LivestockHistoryItem>();
        history.AddRange((await db.LivestockMovements.Where(x => x.SubjectId == subjectId).ToArrayAsync(cancellationToken))
            .Select(x => new LivestockHistoryItem(x.Id, $"movement.{x.Type}", x.OccurredAt, x.Reference ?? x.Notes)));
        history.AddRange((await db.LivestockHealthEvents.Where(x => x.SubjectId == subjectId).ToArrayAsync(cancellationToken))
            .Select(x => new LivestockHistoryItem(x.Id, $"health.{x.Type}", x.OccurredAt, x.Title)));
        history.AddRange((await db.LivestockMeasurements.Where(x => x.SubjectId == subjectId && x.SupersededById == null).ToArrayAsync(cancellationToken))
            .Select(x => new LivestockHistoryItem(x.Id, $"measurement.{x.Metric}", x.OccurredAt, $"{x.EnteredValue} {x.EnteredUnit}")));
        var directTreatments = db.LivestockTreatments.Where(x => x.SubjectId == subjectId);
        var inheritedTreatments =
            from treatment in db.LivestockTreatments
            join membership in db.LivestockGroupMemberships on treatment.SubjectId equals membership.GroupId
            where membership.MemberSubjectId == subjectId
                && treatment.AdministeredAt >= membership.JoinedAt
                && (membership.LeftAt == null || treatment.AdministeredAt <= membership.LeftAt)
            select treatment;
        history.AddRange((await directTreatments.Concat(inheritedTreatments).Distinct().ToArrayAsync(cancellationToken))
            .Select(x => new LivestockHistoryItem(x.Id, "health.treatment", x.AdministeredAt, $"{x.EnteredDose} {x.EnteredUnit} {x.Route}")));
        history.AddRange((await db.LivestockFeedAllocations.Where(x => x.SubjectId == subjectId).ToArrayAsync(cancellationToken))
            .Select(x => new LivestockHistoryItem(x.Id, "feed.allocation", x.OccurredAt, $"{x.EnteredQuantity} {x.EnteredUnit}")));
        history.AddRange((await db.LivestockReproductionEvents.Where(x => x.SubjectId == subjectId).ToArrayAsync(cancellationToken))
            .Select(x => new LivestockHistoryItem(x.Id, $"reproduction.{x.Type}", x.OccurredAt, x.Outcome.ToString())));
        history.AddRange((await db.LivestockMilkRecords.Where(x => x.SubjectId == subjectId).ToArrayAsync(cancellationToken))
            .Select(x => new LivestockHistoryItem(x.Id, "production.milk", x.MilkedAt,
                $"{x.QuantityLitres} L total; {x.SaleableLitres} L saleable")));
        history.AddRange((await db.LivestockGrazingPlans.Where(x => x.GroupSubjectId == subjectId).ToArrayAsync(cancellationToken))
            .Select(x => new LivestockHistoryItem(x.Id, "grazing.planned", x.StartsAt, $"Paddock {x.HoldingId}; {x.Status}")));
        history.AddRange((await db.LivestockRationPlans.Where(x => x.SubjectId == subjectId).ToArrayAsync(cancellationToken))
            .Select(x => new LivestockHistoryItem(x.Id, "ration.planned",
                new DateTimeOffset(x.EffectiveFrom.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero), $"{x.Name}; {x.Status}")));
        history.AddRange((await db.LivestockDispatches.Where(x => x.SubjectId == subjectId).ToArrayAsync(cancellationToken))
            .Select(x => new LivestockHistoryItem(x.Id, $"dispatch.{x.Purpose}", x.DispatchedAt, x.Destination)));
        LivestockGroupMembership[] memberships = await db.LivestockGroupMemberships
            .Where(x => x.MemberSubjectId == subjectId || x.GroupId == subjectId).ToArrayAsync(cancellationToken);
        history.AddRange(memberships.Select(x => new LivestockHistoryItem(x.Id, "group.joined", x.JoinedAt,
            x.MemberSubjectId == subjectId ? $"Joined group {x.GroupId}" : $"Member {x.MemberSubjectId} joined")));
        history.AddRange(memberships.Where(x => x.LeftAt != null).Select(x => new LivestockHistoryItem(x.Id, "group.left",
            x.LeftAt!.Value, x.MemberSubjectId == subjectId ? $"Left group {x.GroupId}" : $"Member {x.MemberSubjectId} left")));
        return UseCaseResults.Ok(history.OrderByDescending(x => x.OccurredAt).ToArray());
    }


    private static async Task<bool> CanAccessSubjectAsync(Guid subjectId, IPlatformDbContext db, IOrganisationScopeAccess scope,
        CancellationToken cancellationToken)
    {
        Guid? farmId = await SubjectFarmAsync(subjectId, db, cancellationToken);
        return farmId is not null && await scope.CanAccessFarmAsync(farmId.Value, cancellationToken);
    }

    private static Task<Guid?> SubjectFarmAsync(Guid subjectId, IPlatformDbContext db, CancellationToken cancellationToken) =>
        db.LivestockSubjects.Where(x => x.Id == subjectId).Select(x => (Guid?)x.FarmId).SingleOrDefaultAsync(cancellationToken);

}
