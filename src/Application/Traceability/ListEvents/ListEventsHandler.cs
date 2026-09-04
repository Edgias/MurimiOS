using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Traceability.ListEvents;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;

using TraceEventType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEventType;

public sealed class ListEventsHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListEventsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListEventsQuery message, CancellationToken cancellationToken) => ListEventsAsync(message.farmId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListEventsAsync(Guid farmId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        if (!await scope.CanAccessFarmAsync(farmId, cancellationToken)) return UseCaseResults.NotFound();
        TraceEventResponse[] events = await db.TraceEvents.Where(x => x.FarmId == farmId)
            .OrderByDescending(x => x.OccurredAt).Take(500)
            .Select(x => new TraceEventResponse(x.Id, x.EventType, x.BusinessStep, x.Disposition,
                x.OccurredAt, x.RecordedAt, x.LocationType, x.LocationId, x.SourceType, x.SourceId,
                x.TransactionReference)).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(events);
    }


}
