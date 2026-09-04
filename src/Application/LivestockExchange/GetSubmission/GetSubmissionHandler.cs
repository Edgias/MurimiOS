using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockExchange.GetSubmission;
using LivestockExchangeAttempt = global::EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission.LivestockExchangeAttempt;
using LivestockExchangeSubmission = global::EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission.LivestockExchangeSubmission;

public sealed class GetSubmissionHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<GetSubmissionQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetSubmissionQuery message, CancellationToken cancellationToken) => GetSubmissionAsync(message.submissionId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> GetSubmissionAsync(Guid submissionId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        LivestockExchangeSubmission? submission = await db.LivestockExchangeSubmissions.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == submissionId, cancellationToken);
        if (submission is null || !await scope.CanAccessFarmAsync(submission.FarmId, cancellationToken)) return UseCaseResults.NotFound();
        LivestockExchangeAttempt[] attempts = await db.LivestockExchangeAttempts.AsNoTracking()
            .Where(x => x.SubmissionId == submissionId).OrderBy(x => x.AttemptNumber).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(SubmissionResponse(submission, attempts));
    }

    private static object SubmissionResponse(LivestockExchangeSubmission x, IReadOnlyCollection<LivestockExchangeAttempt> attempts) => new
    {
        x.Id, x.ProfileId, x.FarmId, x.SubjectId, x.SourceRecordId, x.ExchangeType, x.ProviderCode,
        x.SchemaVersion, x.MediaType, payload = JsonSerializer.Deserialize<JsonElement>(x.PayloadJson),
        x.PayloadSha256, x.CreatedAt, x.CreatedBy, x.ReportingDueAt, x.Status, x.FirstSubmittedAt,
        x.DecidedAt, x.ExternalReference, x.RejectionCode, x.RejectionMessage,
        attempts = attempts.Select(a => new { a.Id, a.AttemptNumber, a.Outcome, a.StartedAt, a.CompletedAt,
            a.HttpStatus, a.ExternalReference, a.ErrorCode, a.ErrorMessage }).ToArray()
    };

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

}
