using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockRecords.ListHealthEvents;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;

using LivestockHealthEventType = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockHealthEventType;

public sealed class ListHealthEventsHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListHealthEventsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListHealthEventsQuery message, CancellationToken cancellationToken) => ListHealthEventsAsync(message.subjectId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListHealthEventsAsync(Guid subjectId, IPlatformDbContext db, IOrganisationScopeAccess scope,
        CancellationToken cancellationToken)
    {
        if (!await CanAccessSubjectAsync(subjectId, db, scope, cancellationToken)) return UseCaseResults.NotFound();
        LivestockHealthEventResponse[] rows = await db.LivestockHealthEvents.Where(x => x.SubjectId == subjectId)
            .OrderByDescending(x => x.OccurredAt).Select(x => new LivestockHealthEventResponse(x.Id, x.SubjectId, x.Type,
                x.Title, x.OccurredAt, x.Diagnosis, x.Practitioner, x.Notes)).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(rows);
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
