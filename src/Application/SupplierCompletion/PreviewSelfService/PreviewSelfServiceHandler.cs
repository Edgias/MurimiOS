using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.SupplierSelfServiceToken;
using EdgIQ.MurimiOS.Domain.HostedSupplierAccount;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplierCompletion.PreviewSelfService;
using HostedSupplierAccount = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierAccount;
using SupplierSelfServiceToken = global::EdgIQ.MurimiOS.Domain.SupplierSelfServiceToken.SupplierSelfServiceToken;
using SupplierSelfServiceTokenStatus = global::EdgIQ.MurimiOS.Domain.SupplierSelfServiceToken.SupplierSelfServiceTokenStatus;

public sealed class PreviewSelfServiceHandler(IPlatformDbContext db, IMutableTenantContext tenant, TimeProvider clock) : IRequestHandler<PreviewSelfServiceQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(PreviewSelfServiceQuery message, CancellationToken cancellationToken) => PreviewSelfServiceAsync(message.token, db, tenant, clock, cancellationToken);

    private static async Task<UseCaseResult> PreviewSelfServiceAsync(string token, IPlatformDbContext db,
        IMutableTenantContext tenant, TimeProvider clock, CancellationToken ct)
    {
        SupplierSelfServiceIdentity? identity = await ResolveSelfServiceAsync(token, db, ct);
        if (identity is null) return UseCaseResults.NotFound();
        tenant.Resolve(identity.TenantId);
        SupplierSelfServiceToken action = await db.SupplierSelfServiceTokens.AsNoTracking().SingleAsync(x => x.Id == identity.TokenId, ct);
        HostedSupplierAccount supplier = await db.HostedSupplierAccounts.AsNoTracking().SingleAsync(x => x.Id == identity.SupplierId, ct);
        if (action.Status != SupplierSelfServiceTokenStatus.Pending) return UseCaseResults.Problem(statusCode: 410, title: "Supplier action unavailable", detail: "This action has already been used or revoked.");
        if (action.ExpiresAt <= clock.GetUtcNow()) return UseCaseResults.Problem(statusCode: 410, title: "Supplier action expired", detail: "Request a new supplier action link.");
        return UseCaseResults.Ok(new { supplier = new { supplier.Name, supplier.SupplierCode }, action.Action,
            action.ClaimedTenantId, action.ExpiresAt });
    }


    private static async Task<SupplierSelfServiceIdentity?> ResolveSelfServiceAsync(string raw, IPlatformDbContext db, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(raw) || raw.Length > 200) return null;
        SupplierSelfServiceIdentity? result = null;
        await db.Database.OpenConnectionAsync(ct);
        try
        {
            await using var command = db.Database.GetDbConnection().CreateCommand();
            command.CommandText = "select tenant_id,token_id,supplier_id from supply.resolve_supplier_self_service_token(@hash)";
            var parameter = command.CreateParameter(); parameter.ParameterName = "hash"; parameter.Value = Hash(raw); command.Parameters.Add(parameter);
            await using var reader = await command.ExecuteReaderAsync(ct);
            if (await reader.ReadAsync(ct)) result = new(reader.GetGuid(0), reader.GetGuid(1), reader.GetGuid(2));
        }
        finally { await db.Database.CloseConnectionAsync(); }
        return result;
    }


    private static string Hash(string value) => Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(value.Trim())));

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

    private sealed record SupplierSelfServiceIdentity(Guid TenantId, Guid TokenId, Guid SupplierId);

}
