using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.EquipmentAsset;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.ListEquipment;
using EquipmentAsset = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentAsset;
using EquipmentStatus = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentStatus;

public sealed class ListEquipmentHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListEquipmentQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListEquipmentQuery message, CancellationToken cancellationToken) => ListEquipmentAsync(message.farmId, message.status, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListEquipmentAsync(Guid farmId, EquipmentStatus? status, IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound();
        IQueryable<EquipmentAsset> query = db.EquipmentAssets.AsNoTracking().Where(x => x.FarmId == farmId);
        if (status.HasValue) query = query.Where(x => x.Status == status);
        return UseCaseResults.Ok(await query.OrderBy(x => x.Code).ToArrayAsync(ct));
    }


}
