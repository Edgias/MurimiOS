using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.ScoutingObservation;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FieldIntelligence.Scouting;
using InsightSeverity = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.InsightSeverity;
using ScoutingCategory = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.ScoutingCategory;
using ScoutingObservation = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.ScoutingObservation;

public sealed class ScoutingHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ScoutingQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ScoutingQuery message, CancellationToken cancellationToken) => ScoutingAsync(message.fieldId, message.category, message.from, message.to, message.limit, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ScoutingAsync(Guid fieldId, ScoutingCategory? category, DateTimeOffset? from,
        DateTimeOffset? to, int? limit, IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        if (!await FieldAllowedAsync(fieldId, db, scope, cancellationToken)) return UseCaseResults.NotFound();
        IQueryable<ScoutingObservation> query = db.ScoutingObservations.AsNoTracking().Where(x => x.FieldId == fieldId);
        if (category is { } selected) query = query.Where(x => x.Category == selected);
        if (from is { } start) query = query.Where(x => x.ObservedAt >= start.ToUniversalTime());
        if (to is { } end) query = query.Where(x => x.ObservedAt <= end.ToUniversalTime());
        ScoutingObservation[] rows = await query.OrderByDescending(x => x.ObservedAt).Take(Math.Clamp(limit ?? 250, 1, 2_000)).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(rows.Select(ToScouting).ToArray());
    }


    private static async Task<bool> FieldAllowedAsync(Guid fieldId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        Guid? farmId = await db.FarmFields.AsNoTracking().Where(x => x.Id == fieldId).Select(x => (Guid?)x.FarmId).SingleOrDefaultAsync(cancellationToken);
        return farmId is { } farm && await scope.CanAccessFarmAsync(farm, cancellationToken);
    }

    private static ScoutingObservationResponse ToScouting(ScoutingObservation x) => new(x.Id, x.FarmId, x.FieldId,
        x.BoundaryVersionId, x.CropZoneId, x.Category, x.Severity, x.IssueCode, x.Title, x.Notes,
        x.ObservedAt, x.RecordedAt, x.RecordedBy, x.Location?.Y, x.Location?.X, x.EvidenceId, x.SourceCommandId);

}
