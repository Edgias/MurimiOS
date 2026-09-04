using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.CorrectionRecord;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Correction.List;
using CorrectionRecord = global::EdgIQ.MurimiOS.Domain.CorrectionRecord.CorrectionRecord;

public sealed class ListHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess) : IRequestHandler<ListQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListQuery message, CancellationToken cancellationToken) => ListAsync(message.entityType, message.entityId, message.limit, db, scopeAccess, cancellationToken);

    private static async Task<UseCaseResult> ListAsync(
        string? entityType,
        Guid? entityId,
        int? limit,
        IPlatformDbContext db,
        IOrganisationScopeAccess scopeAccess,
        CancellationToken cancellationToken)
    {
        int pageSize = Math.Clamp(limit ?? 100, 1, 250);
        ResolvedOrganisationScope scope = await scopeAccess.ResolveAsync(cancellationToken);
        IQueryable<CorrectionRecord> query = db.CorrectionRecords.AsNoTracking();
        if (!scope.IsUnrestricted)
            query = query.Where(item => item.FarmId != null && scope.FarmIds!.Contains(item.FarmId.Value));
        if (!string.IsNullOrWhiteSpace(entityType)) query = query.Where(item => item.EntityType == entityType.Trim());
        if (entityId is { } id && id != Guid.Empty)
            query = query.Where(item => item.OriginalEntityId == id || item.ReplacementEntityId == id);
        CorrectionRecord[] corrections = await query
            .OrderByDescending(item => item.CorrectedAt)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);
        CorrectionResponse[] results = corrections.Select(ToResponse).ToArray();
        return UseCaseResults.Ok(results);
    }


    private static CorrectionResponse ToResponse(CorrectionRecord item) => new(
        item.Id,
        item.EntityType,
        item.OriginalEntityId,
        item.ReplacementEntityId,
        item.Reason,
        item.CorrectedAt,
        item.CorrectedBy,
        item.FarmId);

}
