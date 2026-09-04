using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.SupplyContract;
using EdgIQ.MurimiOS.Application.Common.SupplyNetwork;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplierCompletion.Quote;
using ContractPriceRule = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractPriceRule;
using SupplyContract = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContract;

public sealed class QuoteHandler(IPlatformDbContext db, TimeProvider clock) : IRequestHandler<QuoteQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(QuoteQuery message, CancellationToken cancellationToken) => QuoteAsync(message.contractId, message.quantity, message.qualityGrade, message.effectiveOn, db, clock, cancellationToken);

    private static async Task<UseCaseResult> QuoteAsync(Guid contractId, decimal quantity, string? qualityGrade,
        DateOnly? effectiveOn, IPlatformDbContext db, TimeProvider clock, CancellationToken ct)
    {
        SupplyContract? contract = await db.SupplyContracts.AsNoTracking().SingleOrDefaultAsync(x => x.Id == contractId, ct);
        if (contract is null) return UseCaseResults.NotFound();
        ContractPriceRule[] rules = await db.ContractPriceRules.AsNoTracking().Where(x => x.ContractId == contractId).ToArrayAsync(ct);
        try { return UseCaseResults.Ok(ContractPricing.Resolve(contract, rules, quantity, qualityGrade,
            effectiveOn ?? DateOnly.FromDateTime(clock.GetUtcNow().UtcDateTime))); }
        catch (Exception e) when (e is ArgumentException or InvalidOperationException) { return Bad("contract_price_unavailable", e.Message); }
    }

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}
