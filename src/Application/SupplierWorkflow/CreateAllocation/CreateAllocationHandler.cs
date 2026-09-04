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
namespace EdgIQ.MurimiOS.Application.SupplierWorkflow.CreateAllocation;
using ContractAllocation = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractAllocation;
using ContractAllocationStatus = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractAllocationStatus;
using SupplyContract = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContract;
using SupplyContractStatus = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContractStatus;

public sealed class CreateAllocationHandler(IPlatformDbContext db) : IRequestHandler<CreateAllocationCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateAllocationCommand message, CancellationToken cancellationToken) => CreateAllocationAsync(message.contractId, message.request, db, cancellationToken);

    private static async Task<UseCaseResult> CreateAllocationAsync(Guid contractId, CreateContractAllocationRequest request,
        IPlatformDbContext db, CancellationToken ct)
    {
        SupplyContract? contract = await db.SupplyContracts.SingleOrDefaultAsync(x => x.Id == contractId, ct);
        if (contract is null) return UseCaseResults.NotFound();
        if (contract.Status is SupplyContractStatus.Completed or SupplyContractStatus.Terminated) return Conflict("contract_closed", "Closed contracts cannot receive allocations.");
        decimal allocated = await db.ContractAllocations.Where(x => x.ContractId == contractId && x.Status != ContractAllocationStatus.Cancelled).SumAsync(x => x.AllocatedQuantity, ct);
        if (allocated + request.AllocatedQuantity > contract.ContractedQuantity) return Bad("contract_overallocated", "Allocations cannot exceed the contracted quantity.");
        if (!string.Equals(contract.QuantityUnit, request.QuantityUnit, StringComparison.OrdinalIgnoreCase)) return Bad("allocation_unit_mismatch", "Allocation and contract units must match.");
        try
        {
            var allocation = new ContractAllocation(Id(request.Id), contractId, request.Reference, request.DeliveryFrom,
                request.DeliveryTo, request.AllocatedQuantity, request.QuantityUnit);
            db.ContractAllocations.Add(allocation); await db.SaveChangesAsync(ct);
            return UseCaseResults.Created($"/api/v1/supplier-portal/contracts/{contractId}/allocations/{allocation.Id}", allocation);
        }
        catch (ArgumentException e) { return Bad("allocation_invalid", e.Message); }
    }

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
