using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.LivestockMilkRecord;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockProduction.ListMilk;
using LivestockMilkRecord = global::EdgIQ.MurimiOS.Domain.LivestockMilkRecord.LivestockMilkRecord;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;

public sealed class ListMilkHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListMilkQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListMilkQuery message, CancellationToken cancellationToken) => ListMilkAsync(message.subjectId, message.from, message.to, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListMilkAsync(Guid subjectId, DateTimeOffset? from, DateTimeOffset? to,
        IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        if (await AccessibleSubject(subjectId, db, scope, cancellationToken) is null) return UseCaseResults.NotFound();
        IQueryable<LivestockMilkRecord> query = db.LivestockMilkRecords.AsNoTracking().Where(x => x.SubjectId == subjectId);
        if (from.HasValue) query = query.Where(x => x.MilkedAt >= from);
        if (to.HasValue) query = query.Where(x => x.MilkedAt <= to);
        LivestockMilkRecord[] records = await query.OrderByDescending(x => x.MilkedAt).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(records.Select(MilkResponse).ToArray());
    }


    private static async Task<LivestockSubject?> AccessibleSubject(Guid id, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        LivestockSubject? subject = await db.LivestockSubjects.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        return subject is not null && await scope.CanAccessFarmAsync(subject.FarmId, cancellationToken) ? subject : null;
    }

    private static object MilkResponse(LivestockMilkRecord x) => new { x.Id, x.FarmId, x.SubjectId, x.MilkedAt,
        x.Session, x.QuantityLitres, x.DiscardedLitres, x.SaleableLitres, x.EnteredQuantity,
        x.EnteredDiscardedQuantity, x.EnteredUnit, x.WithdrawalHoldUntil, x.FatPercent, x.ProteinPercent, x.QualityGrade, x.Notes };

}
