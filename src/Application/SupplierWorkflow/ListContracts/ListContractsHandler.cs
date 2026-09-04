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
namespace EdgIQ.MurimiOS.Application.SupplierWorkflow.ListContracts;
using SupplyContract = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContract;
using SupplyContractStatus = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContractStatus;

public sealed class ListContractsHandler(IPlatformDbContext db) : IRequestHandler<ListContractsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListContractsQuery message, CancellationToken cancellationToken) => ListContractsAsync(message.status, message.hostedSupplierId, message.relationshipId, db, cancellationToken);

    private static async Task<UseCaseResult> ListContractsAsync(SupplyContractStatus? status, Guid? hostedSupplierId,
        Guid? relationshipId, IPlatformDbContext db, CancellationToken ct)
    {
        IQueryable<SupplyContract> query = db.SupplyContracts.AsNoTracking();
        if (status.HasValue) query = query.Where(x => x.Status == status);
        if (hostedSupplierId.HasValue) query = query.Where(x => x.HostedSupplierId == hostedSupplierId);
        if (relationshipId.HasValue) query = query.Where(x => x.RelationshipId == relationshipId);
        return UseCaseResults.Ok(await query.OrderByDescending(x => x.ValidFrom).ToArrayAsync(ct));
    }


}
