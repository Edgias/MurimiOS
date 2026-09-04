using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.LivestockDispatch;
using System.Text.Json;
using System.Security.Claims;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockHealth.GetDispatchEligibility;
using LivestockDispatch = global::EdgIQ.MurimiOS.Domain.LivestockDispatch.LivestockDispatch;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;

using LivestockDispatchPurpose = global::EdgIQ.MurimiOS.Domain.LivestockDispatch.LivestockDispatchPurpose;
using LivestockSubjectKind = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubjectKind;

public sealed class GetDispatchEligibilityHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess, TimeProvider timeProvider) : IRequestHandler<GetDispatchEligibilityQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetDispatchEligibilityQuery message, CancellationToken cancellationToken) => GetDispatchEligibilityAsync(message.subjectId, message.purpose, message.at, db, scopeAccess, timeProvider, cancellationToken);

    private static async Task<UseCaseResult> GetDispatchEligibilityAsync(
        Guid subjectId,
        LivestockDispatchPurpose purpose,
        DateTimeOffset? at,
        IPlatformDbContext db,
        IOrganisationScopeAccess scopeAccess,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(purpose)) return UseCaseResults.BadRequest(new { code = "invalid_purpose" });
        Guid? farmId = await db.LivestockSubjects.Where(x => x.Id == subjectId)
            .Select(x => (Guid?)x.FarmId).SingleOrDefaultAsync(cancellationToken);
        if (farmId is null || !await scopeAccess.CanAccessFarmAsync(farmId.Value, cancellationToken)) return UseCaseResults.NotFound();
        DateTimeOffset evaluatedAt = at ?? timeProvider.GetUtcNow();
        DispatchHold[] holds = await ActiveHoldsAsync(subjectId, purpose, evaluatedAt, db, cancellationToken);
        return UseCaseResults.Ok(ToEligibility(subjectId, purpose, evaluatedAt, holds));
    }


    private static async Task<DispatchHold[]> ActiveHoldsAsync(
        Guid subjectId,
        LivestockDispatchPurpose purpose,
        DateTimeOffset at,
        IPlatformDbContext db,
        CancellationToken cancellationToken)
    {
        if (purpose == LivestockDispatchPurpose.Movement) return [];
        LivestockSubjectKind? kind = await db.LivestockSubjects.Where(x => x.Id == subjectId)
            .Select(x => (LivestockSubjectKind?)x.Kind).SingleOrDefaultAsync(cancellationToken);
        if (kind is null) return [];

        IQueryable<PlatformLivestockTreatment> relevant = db.LivestockTreatments.Where(x => x.SubjectId == subjectId);
        if (kind == LivestockSubjectKind.Individual)
        {
            IQueryable<PlatformLivestockTreatment> inherited =
                from treatment in db.LivestockTreatments
                join membership in db.LivestockGroupMemberships on treatment.SubjectId equals membership.GroupId
                where membership.MemberSubjectId == subjectId
                    && treatment.AdministeredAt >= membership.JoinedAt
                    && (membership.LeftAt == null || treatment.AdministeredAt <= membership.LeftAt)
                select treatment;
            relevant = relevant.Concat(inherited);
        }
        else
        {
            IQueryable<PlatformLivestockTreatment> memberTreatments =
                from treatment in db.LivestockTreatments
                join membership in db.LivestockGroupMemberships on treatment.SubjectId equals membership.MemberSubjectId
                where membership.GroupId == subjectId
                    && membership.JoinedAt <= at
                    && (membership.LeftAt == null || membership.LeftAt > at)
                select treatment;
            relevant = relevant.Concat(memberTreatments);
        }

        return purpose is LivestockDispatchPurpose.Slaughter or LivestockDispatchPurpose.Sale
            ? await relevant.Where(x => x.MeatHoldUntil > at).Distinct()
                .Select(x => new DispatchHold(x.Id, x.MeatHoldUntil!.Value)).ToArrayAsync(cancellationToken)
            : await relevant.Where(x => x.MilkHoldUntil > at).Distinct()
                .Select(x => new DispatchHold(x.Id, x.MilkHoldUntil!.Value)).ToArrayAsync(cancellationToken);
    }


    private static DispatchEligibilityResponse ToEligibility(
        Guid subjectId, LivestockDispatchPurpose purpose, DateTimeOffset evaluatedAt, DispatchHold[] holds) =>
        new(subjectId, purpose, evaluatedAt, holds.Length == 0,
            holds.Select(x => x.HoldUntil).Cast<DateTimeOffset?>().Max(), holds.Select(x => x.TreatmentId).ToArray());


    private sealed record DispatchHold(Guid TreatmentId, DateTimeOffset HoldUntil);

}
