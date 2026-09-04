using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.SupplyContract;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplierWorkflow.ChangeAllocation;
using SupplyContract = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContract;

using ContractAllocation = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractAllocation;

public sealed class ChangeAllocationHandler(IPlatformDbContext db) : IRequestHandler<ChangeAllocationCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ChangeAllocationCommand message, CancellationToken cancellationToken) => ChangeAllocationAsync(message.contractId, message.allocationId, message.command, message.request, db, cancellationToken);

    private static async Task<UseCaseResult> ChangeAllocationAsync(Guid contractId, Guid allocationId, string command,
        AllocationCommandRequest request, IPlatformDbContext db, CancellationToken ct)
    {
        ContractAllocation? allocation = await db.ContractAllocations.SingleOrDefaultAsync(x => x.Id == allocationId && x.ContractId == contractId, ct);
        if (allocation is null) return UseCaseResults.NotFound();
        try
        {
            if (command == "fulfil") allocation.RecordFulfilment(request.Quantity ?? 0); else allocation.Cancel();
            await db.SaveChangesAsync(ct); return UseCaseResults.Ok(allocation);
        }
        catch (InvalidOperationException e) { return Conflict("allocation_transition_invalid", e.Message); }
    }

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
