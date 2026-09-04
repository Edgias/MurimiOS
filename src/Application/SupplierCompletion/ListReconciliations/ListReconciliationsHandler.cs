using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplierCompletion.ListReconciliations;


public sealed class ListReconciliationsHandler(IPlatformDbContext db) : IRequestHandler<ListReconciliationsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListReconciliationsQuery message, CancellationToken cancellationToken) => ListReconciliationsAsync(message.contractId, db, cancellationToken);

    private static async Task<UseCaseResult> ListReconciliationsAsync(Guid contractId, IPlatformDbContext db, CancellationToken ct)
    {
        if (!await db.SupplyContracts.AnyAsync(x => x.Id == contractId, ct)) return UseCaseResults.NotFound();
        return UseCaseResults.Ok(await db.ContractReconciliations.AsNoTracking().Where(x => x.ContractId == contractId)
            .OrderByDescending(x => x.Revision).ToArrayAsync(ct));
    }

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

}
