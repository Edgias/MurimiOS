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
namespace EdgIQ.MurimiOS.Application.SupplierWorkflow.CreateSupplier;
using HostedSupplierAccount = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierAccount;

public sealed class CreateSupplierHandler(IPlatformDbContext db) : IRequestHandler<CreateSupplierCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateSupplierCommand message, CancellationToken cancellationToken) => CreateSupplierAsync(message.request, db, cancellationToken);

    private static async Task<UseCaseResult> CreateSupplierAsync(CreateHostedSupplierRequest request, IPlatformDbContext db, CancellationToken ct)
    {
        try
        {
            var supplier = new HostedSupplierAccount(Id(request.Id), request.SupplierCode, request.Name, request.ContactName, request.Email, request.Phone);
            db.HostedSupplierAccounts.Add(supplier); await db.SaveChangesAsync(ct);
            return UseCaseResults.Created($"/api/v1/supplier-portal/suppliers/{supplier.Id}", supplier);
        }
        catch (ArgumentException e) { return Bad("supplier_invalid", e.Message); }
    }

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}
