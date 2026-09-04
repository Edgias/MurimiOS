using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.SupplierRiskAssessment;
using EdgIQ.MurimiOS.Domain.SupplyContract;
using EdgIQ.MurimiOS.Domain.HostedSupplierAccount;
using EdgIQ.MurimiOS.Domain.SupplierEvidenceRequest;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplierWorkflow.SupplierStatus;
using HostedSupplierAccount = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierAccount;
using SupplierEvidenceRequest = global::EdgIQ.MurimiOS.Domain.SupplierEvidenceRequest.SupplierEvidenceRequest;
using SupplierEvidenceRequestStatus = global::EdgIQ.MurimiOS.Domain.SupplierEvidenceRequest.SupplierEvidenceRequestStatus;
using SupplierRiskAssessment = global::EdgIQ.MurimiOS.Domain.SupplierRiskAssessment.SupplierRiskAssessment;
using ContractAllocation = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractAllocation;
using SupplyContract = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContract;
using SupplyContractStatus = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContractStatus;

public sealed class SupplierStatusHandler(IPlatformDbContext db) : IRequestHandler<SupplierStatusQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(SupplierStatusQuery message, CancellationToken cancellationToken) => SupplierStatusAsync(message.supplierId, db, cancellationToken);

    private static async Task<UseCaseResult> SupplierStatusAsync(Guid supplierId, IPlatformDbContext db, CancellationToken ct)
    {
        HostedSupplierAccount? supplier = await db.HostedSupplierAccounts.AsNoTracking().SingleOrDefaultAsync(x => x.Id == supplierId, ct);
        if (supplier is null) return UseCaseResults.NotFound();
        SupplyContract[] contracts = await db.SupplyContracts.AsNoTracking().Where(x => x.HostedSupplierId == supplierId).ToArrayAsync(ct);
        Guid[] contractIds = contracts.Select(x => x.Id).ToArray();
        ContractAllocation[] allocations = await db.ContractAllocations.AsNoTracking().Where(x => contractIds.Contains(x.ContractId)).ToArrayAsync(ct);
        SupplierEvidenceRequest[] requests = await db.SupplierEvidenceRequests.AsNoTracking().Where(x => x.HostedSupplierId == supplierId).ToArrayAsync(ct);
        SupplierRiskAssessment? latestRisk = await db.SupplierRiskAssessments.AsNoTracking()
            .Where(x => x.HostedSupplierId == supplierId).OrderByDescending(x => x.AssessedAt).FirstOrDefaultAsync(ct);
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        return UseCaseResults.Ok(new
        {
            supplier,
            activeContracts = contracts.Count(x => x.Status == SupplyContractStatus.Active),
            contractedQuantity = contracts.Where(x => x.Status == SupplyContractStatus.Active).GroupBy(x => x.QuantityUnit).ToDictionary(x => x.Key, x => x.Sum(c => c.ContractedQuantity)),
            allocationProgress = allocations.GroupBy(x => x.QuantityUnit).ToDictionary(x => x.Key, x => new { allocated = x.Sum(a => a.AllocatedQuantity), fulfilled = x.Sum(a => a.FulfilledQuantity) }),
            evidence = new { requested = requests.Count(x => x.Status == SupplierEvidenceRequestStatus.Requested), submitted = requests.Count(x => x.Status == SupplierEvidenceRequestStatus.Submitted), accepted = requests.Count(x => x.Status == SupplierEvidenceRequestStatus.Accepted), overdue = requests.Count(x => x.DueOn < today && x.Status is SupplierEvidenceRequestStatus.Requested or SupplierEvidenceRequestStatus.Rejected) },
            risk = latestRisk is null ? null : new { latestRisk.Score, latestRisk.Band, latestRisk.ModelVersion, latestRisk.FactsAsOf, latestRisk.AssessedAt }
        });
    }

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

}
