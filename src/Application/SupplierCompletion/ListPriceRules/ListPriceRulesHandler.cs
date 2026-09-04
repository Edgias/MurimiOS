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
namespace EdgIQ.MurimiOS.Application.SupplierCompletion.ListPriceRules;


public sealed class ListPriceRulesHandler(IPlatformDbContext db) : IRequestHandler<ListPriceRulesQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListPriceRulesQuery message, CancellationToken cancellationToken) => ListPriceRulesAsync(message.contractId, db, cancellationToken);

    private static async Task<UseCaseResult> ListPriceRulesAsync(Guid contractId, IPlatformDbContext db, CancellationToken ct)
    {
        if (!await db.SupplyContracts.AnyAsync(x => x.Id == contractId, ct)) return UseCaseResults.NotFound();
        return UseCaseResults.Ok(await db.ContractPriceRules.AsNoTracking().Where(x => x.ContractId == contractId)
            .OrderBy(x => x.Priority).ToArrayAsync(ct));
    }

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

}
