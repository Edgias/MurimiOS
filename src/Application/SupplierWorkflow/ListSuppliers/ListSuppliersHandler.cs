using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.HostedSupplierAccount;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplierWorkflow.ListSuppliers;
using HostedSupplierAccount = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierAccount;
using HostedSupplierStatus = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierStatus;

public sealed class ListSuppliersHandler(IPlatformDbContext db) : IRequestHandler<ListSuppliersQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListSuppliersQuery message, CancellationToken cancellationToken) => ListSuppliersAsync(message.status, db, cancellationToken);

    private static async Task<UseCaseResult> ListSuppliersAsync(HostedSupplierStatus? status, IPlatformDbContext db, CancellationToken ct)
    {
        IQueryable<HostedSupplierAccount> query = db.HostedSupplierAccounts.AsNoTracking();
        if (status.HasValue) query = query.Where(x => x.Status == status);
        return UseCaseResults.Ok(await query.OrderBy(x => x.Name).ToArrayAsync(ct));
    }


}
