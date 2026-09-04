using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.OrganisationNode;



using EdgIQ.MurimiOS.Domain.SyncClient;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Interceptors;
using TenantEntity = global::EdgIQ.MurimiOS.Domain.Common.TenantEntity;

using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using OrganisationNode = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNode;
using SyncCommandLog = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncCommandLog;
using TenantMembership = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantMembership;
using IAuditActorContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IAuditActorContext;
using ITenantContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantContext;

public sealed class AuditSaveChangesInterceptor(
    ITenantContext tenantContext,
    IAuditActorContext actorContext,
    TimeProvider timeProvider) : SaveChangesInterceptor
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        AddAuditEvents(eventData.Context);
        return result;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        AddAuditEvents(eventData.Context);
        return ValueTask.FromResult(result);
    }

    private void AddAuditEvents(DbContext? context)
    {
        if (context is null || !tenantContext.IsResolved || tenantContext.TenantId == Guid.Empty)
            return;

        EntityEntry[] entries = context.ChangeTracker.Entries()
            .Where(entry => entry.Entity is TenantEntity
                && entry.Entity is not AuditEvent
                && entry.Entity is not SyncCommandLog
                && entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToArray();

        foreach (EntityEntry entry in entries)
        {
            Guid entityId = ReadGuid(entry, "Id") ?? Guid.Empty;
            if (entityId == Guid.Empty) continue;

            string action = entry.State switch
            {
                EntityState.Added => "Created",
                EntityState.Modified => "Updated",
                EntityState.Deleted => "Deleted",
                _ => throw new UnreachableException()
            };
            var before = new SortedDictionary<string, object?>(StringComparer.Ordinal);
            var after = new SortedDictionary<string, object?>(StringComparer.Ordinal);
            foreach (PropertyEntry property in entry.Properties)
            {
                if (property.Metadata.IsShadowProperty()) continue;
                if (entry.State == EntityState.Modified && !property.IsModified) continue;
                if (entry.State is EntityState.Modified or EntityState.Deleted)
                    before[property.Metadata.Name] = AuditValue(property.Metadata.Name, property.OriginalValue);
                if (entry.State is EntityState.Modified or EntityState.Added)
                    after[property.Metadata.Name] = AuditValue(property.Metadata.Name, property.CurrentValue);
            }

            Guid? farmId = entry.Entity is Farm ? entityId : ReadGuid(entry, "FarmId");
            Guid? organisationNodeId = entry.Entity switch
            {
                OrganisationNode => entityId,
                Farm farm => farm.OrganisationNodeId,
                TenantMembership membership => membership.ScopeOrganisationNodeId,
                _ => ReadGuid(entry, "OrganisationNodeId") ?? ReadGuid(entry, "ScopeOrganisationNodeId")
            };
            string changes = JsonSerializer.Serialize(new { before, after }, JsonOptions);
            context.Set<AuditEvent>().Add(new AuditEvent(
                Guid.NewGuid(),
                tenantContext.TenantId,
                timeProvider.GetUtcNow(),
                actorContext.IsResolved ? actorContext.Subject : "system",
                actorContext.IsResolved ? actorContext.MembershipId : null,
                action,
                entry.Metadata.ClrType.FullName ?? entry.Metadata.ClrType.Name,
                entityId,
                farmId,
                organisationNodeId,
                changes,
                Activity.Current?.TraceId.ToString()));
        }
    }

    private static Guid? ReadGuid(EntityEntry entry, string propertyName)
    {
        PropertyEntry? property = entry.Properties.FirstOrDefault(item => item.Metadata.Name == propertyName);
        object? value = entry.State == EntityState.Deleted ? property?.OriginalValue : property?.CurrentValue;
        return value switch
        {
            Guid guid when guid != Guid.Empty => guid,
            _ => null
        };
    }

    private static object? AuditValue(string propertyName, object? value)
    {
        if (propertyName.Contains("password", StringComparison.OrdinalIgnoreCase)
            || propertyName.Contains("secret", StringComparison.OrdinalIgnoreCase)
            || propertyName.Contains("token", StringComparison.OrdinalIgnoreCase))
            return "[REDACTED]";

        return value switch
    {
        null => null,
        Geometry geometry => new { geometry.GeometryType, Wkt = geometry.AsText(), geometry.SRID },
        byte[] bytes => new { ByteLength = bytes.Length },
        JsonDocument document => document.RootElement.Clone(),
        _ => value
    };
    }
}
