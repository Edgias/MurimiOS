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
namespace EdgIQ.MurimiOS.Application.ProviderIntegration.GetRecord;
using IngestionJob = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJob;

using IngestionQuality = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionQuality;
using IngestionRecord = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionRecord;
using IngestionRecordStatus = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionRecordStatus;

public sealed class GetRecordHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<GetRecordQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetRecordQuery message, CancellationToken cancellationToken) => GetRecordAsync(message.recordId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> GetRecordAsync(Guid recordId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        IngestionRecord? record = await db.IngestionRecords.AsNoTracking().SingleOrDefaultAsync(x => x.Id == recordId, cancellationToken);
        if (record is null || !await scope.CanAccessFarmAsync(record.FarmId, cancellationToken)) return UseCaseResults.NotFound();
        return UseCaseResults.Ok(ToRecord(record, includePayload: true));
    }

    private static IngestionRecordResponse ToRecord(IngestionRecord x, bool includePayload) => new(x.Id, x.JobId, x.FeedId,
        x.FarmId, x.ExternalRecordId, x.SchemaVersion, x.ObservedAt, x.ReceivedAt, x.SourceModifiedAt, x.ContentSha256,
        x.Quality, x.Status, x.ProcessedAt, x.RejectionCode, x.RejectionReason, includePayload ? ParseJson(x.PayloadJson) : null);

    private static JsonElement ParseJson(string json) => JsonSerializer.Deserialize<JsonElement>(json);

}
