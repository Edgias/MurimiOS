using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile;
using EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockExchange.ListSubmissions;
using LivestockRegulatoryProfile = global::EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile.LivestockRegulatoryProfile;

using LivestockExchangeAttempt = global::EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission.LivestockExchangeAttempt;
using LivestockExchangeSubmission = global::EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission.LivestockExchangeSubmission;
using LivestockExchangeSubmissionStatus = global::EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission.LivestockExchangeSubmissionStatus;
using LivestockRegulatoryExchangeType = global::EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile.LivestockRegulatoryExchangeType;

public sealed class ListSubmissionsHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListSubmissionsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListSubmissionsQuery message, CancellationToken cancellationToken) => ListSubmissionsAsync(message.status, message.exchangeType, message.farmId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListSubmissionsAsync(LivestockExchangeSubmissionStatus? status,
        LivestockRegulatoryExchangeType? exchangeType, Guid? farmId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        ResolvedOrganisationScope resolved = await scope.ResolveAsync(cancellationToken);
        IQueryable<LivestockExchangeSubmission> query = db.LivestockExchangeSubmissions.AsNoTracking();
        if (status is { } state) query = query.Where(x => x.Status == state);
        if (exchangeType is { } type) query = query.Where(x => x.ExchangeType == type);
        if (farmId is { } farm)
        {
            if (!resolved.AllowsFarm(farm)) return UseCaseResults.NotFound();
            query = query.Where(x => x.FarmId == farm);
        }
        else if (!resolved.IsUnrestricted) query = query.Where(x => resolved.FarmIds!.Contains(x.FarmId));
        LivestockExchangeSubmission[] rows = await query.OrderByDescending(x => x.CreatedAt).Take(500).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(rows.Select(x => SubmissionResponse(x, [])).ToArray());
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
