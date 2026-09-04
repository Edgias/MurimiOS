using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Compliance.ListStandards;
using ComplianceStandardStatus = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardStatus;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;

public sealed class ListStandardsHandler(IPlatformDbContext db) : IRequestHandler<ListStandardsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListStandardsQuery message, CancellationToken cancellationToken) => ListStandardsAsync(message.status, db, cancellationToken);

    private static async Task<UseCaseResult> ListStandardsAsync(ComplianceStandardStatus? status, IPlatformDbContext db, CancellationToken ct)
    {
        IQueryable<ComplianceStandardVersion> query = db.ComplianceStandardVersions.AsNoTracking();
        if (status.HasValue) query = query.Where(x => x.Status == status);
        return UseCaseResults.Ok(await query.OrderBy(x => x.Code).ThenByDescending(x => x.Revision).ToArrayAsync(ct));
    }


}
