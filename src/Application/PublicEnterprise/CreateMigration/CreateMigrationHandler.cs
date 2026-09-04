using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.TenantPlacement;
using EdgIQ.MurimiOS.Domain.TenantMigrationJob;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.CreateMigration;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using TenantMigrationJob = global::EdgIQ.MurimiOS.Domain.TenantMigrationJob.TenantMigrationJob;
using TenantMigrationKind = global::EdgIQ.MurimiOS.Domain.TenantMigrationJob.TenantMigrationKind;
using TenantMigrationStatus = global::EdgIQ.MurimiOS.Domain.TenantMigrationJob.TenantMigrationStatus;
using TenantDatabaseMode = global::EdgIQ.MurimiOS.Domain.TenantPlacement.TenantDatabaseMode;
using TenantPlacement = global::EdgIQ.MurimiOS.Domain.TenantPlacement.TenantPlacement;

public sealed class CreateMigrationHandler(IPlatformDbContext db, IRequestActorContext actor, ITenantContext tenant, ITenantDatabaseRegistry registry, TenantDatabaseRoutingScope routing) : IRequestHandler<CreateMigrationCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateMigrationCommand message, CancellationToken cancellationToken) => CreateMigrationAsync(message.request, db, actor, tenant, registry, routing, cancellationToken);
    private static async Task<UseCaseResult> CreateMigrationAsync(CreateTenantMigrationRequest request, IPlatformDbContext db, IRequestActorContext actor,
        ITenantContext tenant, ITenantDatabaseRegistry registry, TenantDatabaseRoutingScope routing, CancellationToken ct)
    { using IDisposable control = routing.EnterControlPlane(); if (!registry.Contains(request.DestinationDatabaseKey)) return Bad("destination_database_unconfigured", "The destination database key is not configured on this host."); TenantPlacement? source = await db.TenantPlacements.SingleOrDefaultAsync(x => x.Id == request.SourcePlacementId && x.EffectiveTo == null, ct); if (source is null) return Bad("source_placement_invalid", "An active source placement is required."); if (source.DatabaseKey.Equals(request.DestinationDatabaseKey, StringComparison.OrdinalIgnoreCase)) return Bad("destination_database_unchanged", "Source and destination database keys must differ."); if (await db.TenantMigrationJobs.AnyAsync(x => x.Status != TenantMigrationStatus.Completed && x.Status != TenantMigrationStatus.Failed && x.Status != TenantMigrationStatus.Cancelled, ct)) return Conflict("migration_in_progress", "Only one non-terminal tenant migration is allowed."); try { DateTimeOffset now = DateTimeOffset.UtcNow; var job = new TenantMigrationJob(Id(request.Id), request.Kind, request.SourcePlacementId, request.DestinationRegion, request.DestinationDatabaseKey, request.DestinationMode, now, actor.Subject); db.TenantMigrationJobs.Add(job); db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, "Enterprise.TenantMigrationRequested", JsonSerializer.Serialize(new { job.Id, job.Kind, job.SourcePlacementId, job.DestinationRegion, job.DestinationDatabaseKey }), now)); await db.SaveChangesAsync(ct); return UseCaseResults.Created($"/api/v1/enterprise/migration-jobs/{job.Id}", job); } catch (ArgumentException e) { return Bad("tenant_migration_invalid", e.Message); } }

    private static void Add(Dictionary<string, object?> target, string[] attributes, string key, object? value) { if (attributes.Contains(key, StringComparer.Ordinal)) target[key] = value; }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message }); 
private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
