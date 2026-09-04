using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.SupplyContract;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplierWorkflow.ChangeContract;
using ContractAllocationStatus = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractAllocationStatus;
using SupplyContract = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContract;

public sealed class ChangeContractHandler(IPlatformDbContext db) : IRequestHandler<ChangeContractCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ChangeContractCommand message, CancellationToken cancellationToken) => ChangeContractAsync(message.contractId, message.command, message.request, db, cancellationToken);

    private static async Task<UseCaseResult> ChangeContractAsync(Guid contractId, string command, SupplierCommandRequest request,
        IPlatformDbContext db, CancellationToken ct)
    {
        SupplyContract? contract = await db.SupplyContracts.SingleOrDefaultAsync(x => x.Id == contractId, ct);
        if (contract is null) return UseCaseResults.NotFound();
        try
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            if (command == "activate") contract.Activate(now);
            else if (command == "complete")
            {
                bool incomplete = await db.ContractAllocations.AnyAsync(x => x.ContractId == contractId
                    && (x.Status == ContractAllocationStatus.Planned || x.Status == ContractAllocationStatus.InProgress), ct);
                if (incomplete) return Conflict("allocations_incomplete", "Planned allocations must be fulfilled or cancelled before contract completion.");
                contract.Complete(now);
            }
            else contract.Terminate(now, request.Notes ?? "Terminated");
            await db.SaveChangesAsync(ct); return UseCaseResults.Ok(contract);
        }
        catch (InvalidOperationException e) { return Conflict("contract_transition_invalid", e.Message); }
    }

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
