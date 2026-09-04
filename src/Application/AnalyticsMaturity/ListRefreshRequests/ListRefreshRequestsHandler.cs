using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AnalyticsRefreshRequest;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.AnalyticsMaturity.ListRefreshRequests;
using AnalyticsRefreshRequest = global::EdgIQ.MurimiOS.Domain.AnalyticsRefreshRequest.AnalyticsRefreshRequest;
using AnalyticsRefreshRequestStatus = global::EdgIQ.MurimiOS.Domain.AnalyticsRefreshRequest.AnalyticsRefreshRequestStatus;

public sealed class ListRefreshRequestsHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListRefreshRequestsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListRefreshRequestsQuery message, CancellationToken cancellationToken) => ListRefreshRequestsAsync(message.status, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListRefreshRequestsAsync(AnalyticsRefreshRequestStatus? status,
        IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct)
    {
        ResolvedOrganisationScope resolved = await scope.ResolveAsync(ct);
        IQueryable<AnalyticsRefreshRequest> query = db.AnalyticsRefreshRequests.AsNoTracking();
        if (status is { } value) query = query.Where(x => x.Status == value);
        if (!resolved.IsUnrestricted) query = query.Where(x => resolved.FarmIds!.Contains(x.FarmId));
        return UseCaseResults.Ok(await query.OrderByDescending(x => x.LastTriggeredAt).Take(500).ToArrayAsync(ct));
    }


}
