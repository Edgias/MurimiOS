using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.MachineryTaskFile;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.ListTaskFiles;
using ExchangeStatus = global::EdgIQ.MurimiOS.Domain.MachineryTaskFile.ExchangeStatus;
using MachineryTaskFile = global::EdgIQ.MurimiOS.Domain.MachineryTaskFile.MachineryTaskFile;

public sealed class ListTaskFilesHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListTaskFilesQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListTaskFilesQuery message, CancellationToken cancellationToken) => ListTaskFilesAsync(message.farmId, message.status, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListTaskFilesAsync(Guid farmId, ExchangeStatus? status, IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct)
    { if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound(); IQueryable<MachineryTaskFile> query = db.MachineryTaskFiles.AsNoTracking().Where(x => x.FarmId == farmId); if (status.HasValue) query = query.Where(x => x.Status == status); return UseCaseResults.Ok(await query.OrderByDescending(x => x.CreatedAt).ToArrayAsync(ct)); }


}
