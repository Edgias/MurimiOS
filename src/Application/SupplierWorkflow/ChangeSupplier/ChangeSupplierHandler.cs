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
namespace EdgIQ.MurimiOS.Application.SupplierWorkflow.ChangeSupplier;
using HostedSupplierAccount = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierAccount;

public sealed class ChangeSupplierHandler(IPlatformDbContext db) : IRequestHandler<ChangeSupplierCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ChangeSupplierCommand message, CancellationToken cancellationToken) => ChangeSupplierAsync(message.supplierId, message.command, message.request, db, cancellationToken);

    private static async Task<UseCaseResult> ChangeSupplierAsync(Guid supplierId, string command, SupplierCommandRequest request,
        IPlatformDbContext db, CancellationToken ct)
    {
        HostedSupplierAccount? supplier = await db.HostedSupplierAccounts.SingleOrDefaultAsync(x => x.Id == supplierId, ct);
        if (supplier is null) return UseCaseResults.NotFound();
        try
        {
            if (command == "suspend") supplier.Suspend();
            else supplier.Reactivate();
            await db.SaveChangesAsync(ct); return UseCaseResults.Ok(supplier);
        }
        catch (InvalidOperationException e) { return Conflict("supplier_transition_invalid", e.Message); }
    }

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
