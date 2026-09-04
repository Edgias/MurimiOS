using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.SupplierEvidenceRequest;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplierWorkflow.ListEvidenceRequests;
using SupplierEvidenceRequest = global::EdgIQ.MurimiOS.Domain.SupplierEvidenceRequest.SupplierEvidenceRequest;
using SupplierEvidenceRequestStatus = global::EdgIQ.MurimiOS.Domain.SupplierEvidenceRequest.SupplierEvidenceRequestStatus;

public sealed class ListEvidenceRequestsHandler(IPlatformDbContext db) : IRequestHandler<ListEvidenceRequestsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListEvidenceRequestsQuery message, CancellationToken cancellationToken) => ListEvidenceRequestsAsync(message.status, message.hostedSupplierId, message.relationshipId, db, cancellationToken);

    private static async Task<UseCaseResult> ListEvidenceRequestsAsync(SupplierEvidenceRequestStatus? status,
        Guid? hostedSupplierId, Guid? relationshipId, IPlatformDbContext db, CancellationToken ct)
    {
        IQueryable<SupplierEvidenceRequest> query = db.SupplierEvidenceRequests.AsNoTracking();
        if (status.HasValue) query = query.Where(x => x.Status == status);
        if (hostedSupplierId.HasValue) query = query.Where(x => x.HostedSupplierId == hostedSupplierId);
        if (relationshipId.HasValue) query = query.Where(x => x.RelationshipId == relationshipId);
        return UseCaseResults.Ok(await query.OrderBy(x => x.DueOn).ToArrayAsync(ct));
    }


}
