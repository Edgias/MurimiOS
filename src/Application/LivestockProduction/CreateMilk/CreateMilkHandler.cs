using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.LivestockMilkRecord;
using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockProduction.CreateMilk;

using LivestockMilkRecord = global::EdgIQ.MurimiOS.Domain.LivestockMilkRecord.LivestockMilkRecord;
using MilkSession = global::EdgIQ.MurimiOS.Domain.LivestockMilkRecord.MilkSession;
using LivestockSubjectKind = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubjectKind;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class CreateMilkHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant) : IRequestHandler<CreateMilkCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateMilkCommand message, CancellationToken cancellationToken) => CreateMilkAsync(message.subjectId, message.request, db, scope, tenant, cancellationToken);

    private static async Task<UseCaseResult> CreateMilkAsync(Guid subjectId, CreateMilkRecordRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, CancellationToken cancellationToken)
    {
        LivestockSubject? subject = await AccessibleSubject(subjectId, db, scope, cancellationToken);
        if (subject is null) return UseCaseResults.NotFound();
        DateTimeOffset? holdUntil = await MilkHoldUntil(subject, request.MilkedAt, db, cancellationToken);
        if (holdUntil > request.MilkedAt && request.DiscardedQuantity != request.Quantity)
            return UseCaseResults.Conflict(new { code = "milk_withdrawal_hold_active", blockedUntil = holdUntil,
                message = "All milk produced during the withdrawal period must be recorded as discarded." });
        try
        {
            var record = new LivestockMilkRecord(Guid.NewGuid(), subject.FarmId, subjectId, request.MilkedAt,
                request.Session, request.Quantity, request.DiscardedQuantity, request.Unit, holdUntil,
                request.FatPercent, request.ProteinPercent, request.QualityGrade, request.Notes);
            db.LivestockMilkRecords.Add(record);
            AddEvents(db, tenant, subject.FarmId, "LivestockMilkRecord", record.Id, "Recorded", MilkResponse(record), record.MilkedAt);
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/livestock/subjects/{subjectId}/milk-records/{record.Id}", MilkResponse(record));
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        { return Bad("milk_record_invalid", exception.Message); }
    }


    private static async Task<DateTimeOffset?> MilkHoldUntil(LivestockSubject subject, DateTimeOffset at,
        IPlatformDbContext db, CancellationToken cancellationToken)
    {
        IQueryable<PlatformLivestockTreatment> relevant = db.LivestockTreatments.Where(x => x.SubjectId == subject.Id);
        if (subject.Kind == LivestockSubjectKind.Individual)
        {
            IQueryable<PlatformLivestockTreatment> inherited = from treatment in db.LivestockTreatments
                join membership in db.LivestockGroupMemberships on treatment.SubjectId equals membership.GroupId
                where membership.MemberSubjectId == subject.Id && treatment.AdministeredAt >= membership.JoinedAt
                    && (membership.LeftAt == null || treatment.AdministeredAt <= membership.LeftAt) select treatment;
            relevant = relevant.Concat(inherited);
        }
        else
        {
            IQueryable<PlatformLivestockTreatment> members = from treatment in db.LivestockTreatments
                join membership in db.LivestockGroupMemberships on treatment.SubjectId equals membership.MemberSubjectId
                where membership.GroupId == subject.Id && membership.JoinedAt <= at
                    && (membership.LeftAt == null || membership.LeftAt > at) select treatment;
            relevant = relevant.Concat(members);
        }
        return await relevant.Where(x => x.MilkHoldUntil > at).MaxAsync(x => (DateTimeOffset?)x.MilkHoldUntil, cancellationToken);
    }


    private static async Task<LivestockSubject?> AccessibleSubject(Guid id, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        LivestockSubject? subject = await db.LivestockSubjects.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        return subject is not null && await scope.CanAccessFarmAsync(subject.FarmId, cancellationToken) ? subject : null;
    }


    private static void AddEvents(IPlatformDbContext db, ITenantContext tenant, Guid farmId, string entityType,
        Guid entityId, string operation, object value, DateTimeOffset occurredAt)
    {
        string json = JsonSerializer.Serialize(value);
        db.SyncChanges.Add(new SyncChange(tenant.TenantId, farmId, entityType, entityId, operation, json, occurredAt));
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, $"{entityType}.{operation}", json, occurredAt));
    }

    private static object MilkResponse(LivestockMilkRecord x) => new { x.Id, x.FarmId, x.SubjectId, x.MilkedAt,
        x.Session, x.QuantityLitres, x.DiscardedLitres, x.SaleableLitres, x.EnteredQuantity,
        x.EnteredDiscardedQuantity, x.EnteredUnit, x.WithdrawalHoldUntil, x.FatPercent, x.ProteinPercent, x.QualityGrade, x.Notes };

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}
