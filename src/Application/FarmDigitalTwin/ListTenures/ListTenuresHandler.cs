using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.LandTenureRecord;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FarmDigitalTwin.ListTenures;
using LandTenureRecord = global::EdgIQ.MurimiOS.Domain.LandTenureRecord.LandTenureRecord;

public sealed class ListTenuresHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListTenuresQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListTenuresQuery message, CancellationToken cancellationToken) => ListTenuresAsync(message.farmId, message.fieldId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListTenuresAsync(Guid farmId, Guid? fieldId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound();
        IQueryable<LandTenureRecord> query = db.LandTenureRecords.AsNoTracking().Where(x => x.FarmId == farmId);
        if (fieldId.HasValue) query = query.Where(x => x.FieldId == fieldId);
        return UseCaseResults.Ok(await query.OrderByDescending(x => x.ValidFrom).ToArrayAsync(ct));
    }


}
