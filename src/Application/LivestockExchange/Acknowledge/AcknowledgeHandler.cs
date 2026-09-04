using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockExchange.Acknowledge;
using LivestockExchangeAttempt = global::EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission.LivestockExchangeAttempt;
using LivestockExchangeAttemptOutcome = global::EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission.LivestockExchangeAttemptOutcome;
using LivestockExchangeSubmission = global::EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission.LivestockExchangeSubmission;
using LivestockExchangeSubmissionStatus = global::EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission.LivestockExchangeSubmissionStatus;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;

public sealed class AcknowledgeHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, TimeProvider clock) : IRequestHandler<AcknowledgeCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(AcknowledgeCommand message, CancellationToken cancellationToken) => AcknowledgeAsync(message.submissionId, message.request, db, scope, tenant, clock, cancellationToken);

    private static async Task<UseCaseResult> AcknowledgeAsync(Guid submissionId, AcknowledgeLivestockExchangeRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, TimeProvider clock,
        CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        await LockAsync(db, $"livestock-exchange:{submissionId:N}", cancellationToken);
        LivestockExchangeSubmission? submission = await db.LivestockExchangeSubmissions
            .SingleOrDefaultAsync(x => x.Id == submissionId, cancellationToken);
        if (submission is null || !await scope.CanAccessFarmAsync(submission.FarmId, cancellationToken)) return UseCaseResults.NotFound();
        if (submission.Status != LivestockExchangeSubmissionStatus.Submitted)
            return Conflict("livestock_exchange_not_pending", "Only a submitted exchange can receive a later acknowledgement.");
        DateTimeOffset now = clock.GetUtcNow();
        if (request.Accepted)
        {
            if (string.IsNullOrWhiteSpace(request.ExternalReference))
                return Bad("livestock_exchange_reference_required", "An accepted acknowledgement requires an external reference.");
            submission.Accept(now, request.ExternalReference);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(request.ErrorCode) || string.IsNullOrWhiteSpace(request.ErrorMessage))
                return Bad("livestock_exchange_rejection_required", "A rejected acknowledgement requires an error code and message.");
            submission.Reject(now, request.ErrorCode, request.ErrorMessage);
        }
        int number = (await db.LivestockExchangeAttempts.Where(x => x.SubmissionId == submissionId)
            .MaxAsync(x => (int?)x.AttemptNumber, cancellationToken) ?? 0) + 1;
        string responseJson = string.IsNullOrWhiteSpace(request.ResponseJson)
            ? JsonSerializer.Serialize(new { request.Accepted, request.ExternalReference, request.ErrorCode, request.ErrorMessage })
            : request.ResponseJson;
        var attempt = new LivestockExchangeAttempt(Guid.NewGuid(), submissionId, number,
            request.Accepted ? LivestockExchangeAttemptOutcome.Accepted : LivestockExchangeAttemptOutcome.Rejected,
            now, now, submission.PayloadSha256, null, request.ExternalReference, responseJson,
            request.ErrorCode, request.ErrorMessage);
        db.LivestockExchangeAttempts.Add(attempt);
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, "LivestockExchange.Acknowledged",
            JsonSerializer.Serialize(new { submissionId = submission.Id, attemptId = attempt.Id,
                submission.Status, submission.ExternalReference }), now));
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return UseCaseResults.Ok(SubmissionResponse(submission, [attempt]));
    }


    private static async Task LockAsync(IPlatformDbContext db, string key, CancellationToken cancellationToken)
    {
        if (db.Database.ProviderName?.Contains("Npgsql", StringComparison.Ordinal) == true)
            await db.Database.ExecuteSqlInterpolatedAsync($"SELECT pg_advisory_xact_lock(hashtextextended({key}, 0));", cancellationToken);
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

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
