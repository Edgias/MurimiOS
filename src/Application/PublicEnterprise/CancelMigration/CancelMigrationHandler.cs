using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.TenantMigrationJob;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.CancelMigration;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using TenantMigrationJob = global::EdgIQ.MurimiOS.Domain.TenantMigrationJob.TenantMigrationJob;

public sealed class CancelMigrationHandler(IPlatformDbContext db, ITenantContext tenant, TenantDatabaseRoutingScope routing) : IRequestHandler<CancelMigrationCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CancelMigrationCommand message, CancellationToken cancellationToken) => CancelMigrationAsync(message.jobId, db, tenant, routing, cancellationToken);
    private static async Task<UseCaseResult> CancelMigrationAsync(Guid jobId, IPlatformDbContext db, ITenantContext tenant, TenantDatabaseRoutingScope routing, CancellationToken ct)
    { using IDisposable control = routing.EnterControlPlane(); TenantMigrationJob? job = await db.TenantMigrationJobs.SingleOrDefaultAsync(x => x.Id == jobId, ct); if (job is null) return UseCaseResults.NotFound(); try { DateTimeOffset now = DateTimeOffset.UtcNow; job.Cancel(now); db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, "Enterprise.TenantMigrationCancelled", JsonSerializer.Serialize(new { job.Id, job.Status }), now)); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(job); } catch (InvalidOperationException e) { return Conflict("tenant_migration_transition_invalid", e.Message); } }

    private static void Add(Dictionary<string, object?> target, string[] attributes, string key, object? value) { if (attributes.Contains(key, StringComparer.Ordinal)) target[key] = value; }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
