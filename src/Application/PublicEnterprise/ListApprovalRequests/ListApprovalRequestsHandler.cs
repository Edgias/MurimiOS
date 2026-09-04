using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.DelegatedApprovalRequest;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.ListApprovalRequests;
using DelegatedApprovalRequest = global::EdgIQ.MurimiOS.Domain.DelegatedApprovalRequest.DelegatedApprovalRequest;
using DelegatedApprovalStatus = global::EdgIQ.MurimiOS.Domain.DelegatedApprovalRequest.DelegatedApprovalStatus;

public sealed class ListApprovalRequestsHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListApprovalRequestsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListApprovalRequestsQuery message, CancellationToken cancellationToken) => ListApprovalRequestsAsync(message.status, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListApprovalRequestsAsync(DelegatedApprovalStatus? status, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken ct)
    {
        ResolvedOrganisationScope resolved = await scope.ResolveAsync(ct); IQueryable<DelegatedApprovalRequest> q = db.DelegatedApprovalRequests.AsNoTracking();
        if (!resolved.IsUnrestricted) q = q.Where(x => x.FarmId == null || resolved.FarmIds!.Contains(x.FarmId.Value)); if (status.HasValue) q = q.Where(x => x.Status == status);
        return UseCaseResults.Ok(await q.OrderByDescending(x => x.RequestedAt).Take(2000).ToArrayAsync(ct));
    }


}
