using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.StorageLot;
using EdgIQ.MurimiOS.Domain.TraceEvent;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Traceability.GetMassBalance;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;

using StorageLot = global::EdgIQ.MurimiOS.Domain.StorageLot.StorageLot;
using TraceabilityMassBalance = global::EdgIQ.MurimiOS.Domain.StorageLot.TraceabilityMassBalance;
using TraceObjectRole = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectRole;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;

public sealed class GetMassBalanceHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<GetMassBalanceQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetMassBalanceQuery message, CancellationToken cancellationToken) => GetMassBalanceAsync(message.objectType, message.objectId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> GetMassBalanceAsync(string objectType, Guid objectId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        if (!TryObjectType(objectType, out TraceObjectType parsed) || parsed is not (TraceObjectType.HarvestLot or TraceObjectType.StorageLot))
            return Bad("unsupported_object_type", "Mass balance is available for harvest and storage lots.");
        SourceLot? source = await LoadCropSourceAsync(parsed, objectId, db, cancellationToken);
        if (source is null || !await scope.CanAccessFarmAsync(source.FarmId, cancellationToken)) return UseCaseResults.NotFound();
        return UseCaseResults.Ok(new MassBalanceResponse(parsed, objectId, source.Original, source.Consumed,
            source.Available, source.Unit, Math.Abs(source.Original - source.Consumed - source.Available) <= 0.0001m));
    }


    private static async Task<SourceLot?> LoadCropSourceAsync(TraceObjectType type, Guid id, IPlatformDbContext db,
        CancellationToken cancellationToken)
    {
        SourceBase? item = type switch
        {
            TraceObjectType.HarvestLot => await (from lot in db.HarvestLots
                join zone in db.CropZones on lot.CropZoneId equals zone.Id
                join season in db.GrowingSeasons on zone.SeasonId equals season.Id
                where lot.Id == id select new SourceBase(season.FarmId, zone.Crop, zone.Variety, lot.Quantity, lot.Unit))
                .SingleOrDefaultAsync(cancellationToken),
            TraceObjectType.StorageLot => await db.StorageLots.Where(x => x.Id == id)
                .Select(x => new SourceBase(x.FarmId, x.Crop, x.Variety, x.Quantity, x.Unit)).SingleOrDefaultAsync(cancellationToken),
            _ => null
        };
        if (item is null) return null;
        decimal consumed = await db.TraceEventObjects
            .Where(x => x.ObjectType == type && x.ObjectId == id && x.Role == TraceObjectRole.Input)
            .SumAsync(x => x.Quantity ?? 0, cancellationToken);
        return new SourceLot(item.FarmId, item.Crop, item.Variety, item.Original, item.Unit, consumed,
            TraceabilityMassBalance.Available(item.Original, consumed));
    }


    private static bool TryObjectType(string value, out TraceObjectType type) =>
        Enum.TryParse(value.Replace("-", string.Empty), true, out type) && Enum.IsDefined(type);

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });


    private sealed record SourceBase(Guid FarmId, string Crop, string? Variety, decimal Original, string Unit);

    private sealed record SourceLot(Guid FarmId, string Crop, string? Variety, decimal Original, string Unit, decimal Consumed, decimal Available);

}
