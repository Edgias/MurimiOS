using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.IngestionJob;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ProviderIntegration.GetJob;
using IngestionAttempt = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionAttempt;
using IngestionAttemptOutcome = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionAttemptOutcome;
using IngestionJob = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJob;
using IngestionJobKind = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJobKind;
using IngestionJobStatus = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJobStatus;
using IngestionRecordStatus = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionRecordStatus;

public sealed class GetJobHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<GetJobQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetJobQuery message, CancellationToken cancellationToken) => GetJobAsync(message.jobId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> GetJobAsync(Guid jobId, IPlatformDbContext db, IOrganisationScopeAccess scope,
        CancellationToken cancellationToken)
    {
        IngestionJob? job = await FindJobAsync(jobId, db, scope, cancellationToken, noTracking: true); if (job is null) return UseCaseResults.NotFound();
        IngestionAttempt[] attempts = await db.IngestionAttempts.AsNoTracking().Where(x => x.JobId == jobId).OrderBy(x => x.AttemptNumber).ToArrayAsync(cancellationToken);
        var counts = await db.IngestionRecords.AsNoTracking().Where(x => x.JobId == jobId).GroupBy(x => x.Status)
            .Select(x => new { x.Key, Count = x.Count() }).ToDictionaryAsync(x => x.Key, x => x.Count, cancellationToken);
        return UseCaseResults.Ok(new IngestionJobDetailResponse(ToJob(job), attempts.Select(ToAttempt).ToArray(),
            counts.GetValueOrDefault(IngestionRecordStatus.Received), counts.GetValueOrDefault(IngestionRecordStatus.Processed),
            counts.GetValueOrDefault(IngestionRecordStatus.Rejected), attempts.Sum(x => x.Cost)));
    }

    private static async Task<IngestionJob?> FindJobAsync(Guid id, IPlatformDbContext db, IOrganisationScopeAccess scope,
        CancellationToken cancellationToken, bool noTracking = false)
    {
        IQueryable<IngestionJob> query = noTracking ? db.IngestionJobs.AsNoTracking() : db.IngestionJobs;
        IngestionJob? job = await query.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        return job is not null && await scope.CanAccessFarmAsync(job.FarmId, cancellationToken) ? job : null;
    }

    private static IngestionJobResponse ToJob(IngestionJob x) => new(x.Id, x.FeedId, x.FarmId, x.Kind,
        x.IdempotencyKey, x.Status, x.RequestedAt, x.RequestedBy, x.AvailableAt, x.Attempts, x.MaximumAttempts,
        x.LeasedUntil, x.LastClaimedAt, x.CompletedAt, x.WindowStart, x.WindowEnd, x.PreviousJobId,
        x.ResultCheckpointJson is null ? null : ParseJson(x.ResultCheckpointJson), x.LastErrorCode, x.LastErrorMessage, x.Version);

    private static IngestionAttemptResponse ToAttempt(IngestionAttempt x) => new(x.Id, x.AttemptNumber, x.StartedAt,
        x.CompletedAt, x.Outcome, x.HttpStatusCode, x.RequestUnits, x.Cost, x.Currency, x.ErrorCode, x.ErrorMessage);

    private static JsonElement ParseJson(string json) => JsonSerializer.Deserialize<JsonElement>(json);

}
