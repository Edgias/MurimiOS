using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.TenantMigrationJob;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.FailMigration;
using TenantMigrationJob = global::EdgIQ.MurimiOS.Domain.TenantMigrationJob.TenantMigrationJob;

public sealed class FailMigrationHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing) : IRequestHandler<FailMigrationCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(FailMigrationCommand message, CancellationToken cancellationToken) => FailMigrationAsync(message.jobId, message.request, db, routing, cancellationToken);
    private static async Task<UseCaseResult> FailMigrationAsync(Guid jobId, FailTenantMigrationRequest request, IPlatformDbContext db, TenantDatabaseRoutingScope routing, CancellationToken ct)
    { using IDisposable control = routing.EnterControlPlane(); TenantMigrationJob? job = await db.TenantMigrationJobs.SingleOrDefaultAsync(x => x.Id == jobId, ct); if (job is null) return UseCaseResults.NotFound(); try { job.Fail(DateTimeOffset.UtcNow, request.Code, request.Message); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(job); } catch (InvalidOperationException e) { return Conflict("tenant_migration_transition_invalid", e.Message); } }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
