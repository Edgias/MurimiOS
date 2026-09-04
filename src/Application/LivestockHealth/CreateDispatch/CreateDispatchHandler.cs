using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.LivestockDispatch;
using System.Text.Json;
using System.Security.Claims;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockHealth.CreateDispatch;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;
using TenantMembership = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantMembership;

using LivestockDispatchPurpose = global::EdgIQ.MurimiOS.Domain.LivestockDispatch.LivestockDispatchPurpose;
using LivestockDispatch = global::EdgIQ.MurimiOS.Domain.LivestockDispatch.LivestockDispatch;
using LivestockSubjectKind = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubjectKind;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;

public sealed class CreateDispatchHandler(UseCaseContext httpContext, IPlatformDbContext db, IOrganisationScopeAccess scopeAccess, ITenantContext tenantContext) : IRequestHandler<CreateDispatchCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateDispatchCommand message, CancellationToken cancellationToken) => CreateDispatchAsync(message.request, httpContext, db, scopeAccess, tenantContext, cancellationToken);

    private static async Task<UseCaseResult> CreateDispatchAsync(
        CreateLivestockDispatchRequest request,
        UseCaseContext httpContext,
        IPlatformDbContext db,
        IOrganisationScopeAccess scopeAccess,
        ITenantContext tenantContext,
        CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(request.Purpose)) return UseCaseResults.BadRequest(new { code = "invalid_purpose" });
        Guid? farmId = await db.LivestockSubjects.Where(x => x.Id == request.SubjectId)
            .Select(x => (Guid?)x.FarmId).SingleOrDefaultAsync(cancellationToken);
        if (farmId is null || !await scopeAccess.CanAccessFarmAsync(farmId.Value, cancellationToken)) return UseCaseResults.NotFound();
        DispatchHold[] holds = await ActiveHoldsAsync(request.SubjectId, request.Purpose, request.DispatchedAt, db, cancellationToken);
        DateTimeOffset? blockedUntil = holds.Select(x => x.HoldUntil).Cast<DateTimeOffset?>().Max();

        string? authorizedBy = null;
        if (blockedUntil > request.DispatchedAt)
        {
            if (string.IsNullOrWhiteSpace(request.OverrideReason))
            {
                return UseCaseResults.Conflict(new
                {
                    code = "withdrawal_hold_active",
                    message = $"Dispatch is blocked until {blockedUntil:O}.",
                    blockedUntil,
                    blockingTreatmentIds = holds.Select(x => x.TreatmentId)
                });
            }

            if (!httpContext.User.Principal.HasClaim("permission", PlatformPermissions.WithdrawalOverride))
            {
                return UseCaseResults.Json(new
                {
                    code = "override_forbidden",
                    message = "The livestock.withdrawal.override permission is required."
                }, statusCode: 403);
            }

            authorizedBy = httpContext.User.Principal.FindFirstValue("sub")
                ?? httpContext.User.Principal.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? httpContext.User.Principal.Identity?.Name;
            if (string.IsNullOrWhiteSpace(authorizedBy))
            {
                return UseCaseResults.Json(new { code = "override_identity_required" }, statusCode: 403);
            }
        }
        else if (!string.IsNullOrWhiteSpace(request.OverrideReason))
        {
            return UseCaseResults.BadRequest(new
            {
                code = "override_not_applicable",
                message = "An override reason may only be recorded when an active withdrawal hold blocks dispatch."
            });
        }

        var dispatch = new LivestockDispatch(
            Guid.NewGuid(), request.SubjectId, request.Purpose, request.DispatchedAt,
            request.Destination, blockedUntil, request.OverrideReason, authorizedBy);
        db.LivestockDispatches.Add(dispatch);
        string payload = JsonSerializer.Serialize(new
        {
            dispatch.Id, dispatch.SubjectId, dispatch.Purpose, dispatch.DispatchedAt,
            dispatch.Destination, dispatch.WithdrawalHoldUntil, dispatch.OverrideReason, dispatch.OverrideAuthorizedBy
        });
        db.OutboxMessages.Add(new OutboxMessage(
            Guid.NewGuid(), tenantContext.TenantId, "LivestockDispatch.Recorded", payload, request.DispatchedAt));
        await db.SaveChangesAsync(cancellationToken);
        return UseCaseResults.Created(
            $"/api/v1/livestock/dispatches/{dispatch.Id}",
            new LivestockDispatchResponse(
                dispatch.Id, dispatch.SubjectId, dispatch.Purpose, dispatch.DispatchedAt,
                dispatch.Destination, dispatch.WithdrawalHoldUntil, dispatch.OverrideReason, dispatch.OverrideAuthorizedBy));
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


    private sealed record DispatchHold(Guid TreatmentId, DateTimeOffset HoldUntil);

}
