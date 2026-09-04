using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission;
using EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockExchange.ListDeadlines;
using LivestockExchangeSubmission = global::EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission.LivestockExchangeSubmission;
using LivestockExchangeSubmissionStatus = global::EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission.LivestockExchangeSubmissionStatus;
using LivestockStatutoryMovementDocument = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.LivestockStatutoryMovementDocument;
using StatutoryMovementDocumentStatus = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.StatutoryMovementDocumentStatus;

public sealed class ListDeadlinesHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, TimeProvider clock) : IRequestHandler<ListDeadlinesQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListDeadlinesQuery message, CancellationToken cancellationToken) => ListDeadlinesAsync(message.through, db, scope, clock, cancellationToken);

    private static async Task<UseCaseResult> ListDeadlinesAsync(DateTimeOffset? through, IPlatformDbContext db,
        IOrganisationScopeAccess scope, TimeProvider clock, CancellationToken cancellationToken)
    {
        DateTimeOffset now = clock.GetUtcNow();
        DateTimeOffset horizon = (through ?? now.AddDays(30)).ToUniversalTime();
        ResolvedOrganisationScope resolved = await scope.ResolveAsync(cancellationToken);
        IQueryable<LivestockStatutoryMovementDocument> documents = db.LivestockStatutoryMovementDocuments.AsNoTracking()
            .Where(x => x.Status == StatutoryMovementDocumentStatus.Ready && x.ReportingDueAt <= horizon);
        IQueryable<LivestockExchangeSubmission> submissions = db.LivestockExchangeSubmissions.AsNoTracking()
            .Where(x => x.Status != LivestockExchangeSubmissionStatus.Accepted
                && x.Status != LivestockExchangeSubmissionStatus.Rejected
                && x.ReportingDueAt != null && x.ReportingDueAt <= horizon);
        if (!resolved.IsUnrestricted)
        {
            documents = documents.Where(x => resolved.FarmIds!.Contains(x.FarmId));
            submissions = submissions.Where(x => resolved.FarmIds!.Contains(x.FarmId));
        }
        var movementRows = (await documents.ToArrayAsync(cancellationToken)).Select(x => new RegulatoryDeadlineResponse(
            "movement", x.Id, x.FarmId, x.SubjectId, x.ProviderCode, x.ReportingDueAt, x.ReportingDueAt < now,
            x.Status.ToString())).ToArray();
        var exchangeRows = (await submissions.ToArrayAsync(cancellationToken)).Select(x => new RegulatoryDeadlineResponse(
            x.ExchangeType.ToString(), x.Id, x.FarmId, x.SubjectId, x.ProviderCode, x.ReportingDueAt!.Value,
            x.ReportingDueAt < now, x.Status.ToString())).ToArray();
        return UseCaseResults.Ok(movementRows.Concat(exchangeRows).OrderBy(x => x.DueAt).ToArray());
    }

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

}
