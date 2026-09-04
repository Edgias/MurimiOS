using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplierCompletion.LinkHistory;


public sealed class LinkHistoryHandler(IPlatformDbContext db) : IRequestHandler<LinkHistoryQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(LinkHistoryQuery message, CancellationToken cancellationToken) => LinkHistoryAsync(message.supplierId, db, cancellationToken);

    private static async Task<UseCaseResult> LinkHistoryAsync(Guid supplierId, IPlatformDbContext db, CancellationToken ct)
    {
        if (!await db.HostedSupplierAccounts.AnyAsync(x => x.Id == supplierId, ct)) return UseCaseResults.NotFound();
        return UseCaseResults.Ok(await db.HostedSupplierLinkEvents.AsNoTracking().Where(x => x.HostedSupplierId == supplierId)
            .OrderByDescending(x => x.OccurredAt).ToArrayAsync(ct));
    }

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

}
