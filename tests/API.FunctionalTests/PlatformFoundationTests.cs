using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.MediaUploadSession;
using EdgIQ.MurimiOS.Domain.EvidenceAttachment;
using EdgIQ.MurimiOS.Domain.FarmField;
using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.LandTenureRecord;
using EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset;
using EdgIQ.MurimiOS.Domain.SoilZone;
using EdgIQ.MurimiOS.Domain.SoilSample;
using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.InventoryMovement;
using EdgIQ.MurimiOS.Domain.InputStockLot;
using EdgIQ.MurimiOS.Domain.AgriculturalInput;
using EdgIQ.MurimiOS.Domain.VeterinaryMedicineStockLot;
using EdgIQ.MurimiOS.Domain.VeterinaryMedicine;
using EdgIQ.MurimiOS.Domain.LivestockTreatment;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.LivestockFeedBatch;

using EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile;
using EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission;
using EdgIQ.MurimiOS.Domain.LivestockReproductionEvent;
using EdgIQ.MurimiOS.Domain.LivestockMilkRecord;
using EdgIQ.MurimiOS.Domain.LivestockGrazingPlan;
using EdgIQ.MurimiOS.Domain.LivestockRationPlan;
using EdgIQ.MurimiOS.Domain.LivestockHolding;
using EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument;
using EdgIQ.MurimiOS.Domain.CropFeedTransfer;
using EdgIQ.MurimiOS.Domain.LivestockMeasurementRecord;
using EdgIQ.MurimiOS.Domain.LivestockDispatch;
using EdgIQ.MurimiOS.Domain.OrganisationNode;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using EdgIQ.MurimiOS.Domain.FieldOperationRecord;
using EdgIQ.MurimiOS.Domain.HarvestLot;
using EdgIQ.MurimiOS.Domain.GrowingSeason;
using EdgIQ.MurimiOS.Domain.CropZone;
using EdgIQ.MurimiOS.Domain.NotificationRecord;
using EdgIQ.MurimiOS.Domain.NotificationPreference;



using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Infrastructure.Data;
using EdgIQ.MurimiOS.Infrastructure.Data.Interceptors;

using EdgIQ.MurimiOS.Domain.SyncClient;


using EdgIQ.MurimiOS.Infrastructure.Storage;
using EdgIQ.MurimiOS.API.Endpoints.Access;
using EdgIQ.MurimiOS.API.Endpoints.Audit;
using EdgIQ.MurimiOS.API.Infrastructure.Access;
using EdgIQ.MurimiOS.API.Endpoints.Organisation;
using EdgIQ.MurimiOS.API.Endpoints.Access.Create;
using EdgIQ.MurimiOS.API.Endpoints.Access.Shared;
using EdgIQ.MurimiOS.API.Endpoints.Access.Update;
using EdgIQ.MurimiOS.API.Endpoints.Audit.List;
using EdgIQ.MurimiOS.API.Endpoints.Organisation.Create;
using EdgIQ.MurimiOS.API.Endpoints.Organisation.Shared;
using EdgIQ.MurimiOS.Infrastructure.Identity;

using EdgIQ.MurimiOS.Domain.CorrectionRecord;
using EdgIQ.MurimiOS.Domain.AuditEvent;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Security.Cryptography;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;

namespace EdgIQ.MurimiOS.API.FunctionalTests;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using CorrectionRecord = global::EdgIQ.MurimiOS.Domain.CorrectionRecord.CorrectionRecord;
using CropZone = global::EdgIQ.MurimiOS.Domain.CropZone.CropZone;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using FieldBoundaryVersion = global::EdgIQ.MurimiOS.Domain.FarmField.FieldBoundaryVersion;
using FieldOperationRecord = global::EdgIQ.MurimiOS.Domain.FieldOperationRecord.FieldOperationRecord;
using GrowingSeasonStatus = global::EdgIQ.MurimiOS.Domain.GrowingSeason.GrowingSeasonStatus;
using GrowingSeason = global::EdgIQ.MurimiOS.Domain.GrowingSeason.GrowingSeason;
using HarvestLot = global::EdgIQ.MurimiOS.Domain.HarvestLot.HarvestLot;
using InputStockLot = global::EdgIQ.MurimiOS.Domain.InputStockLot.InputStockLot;
using LivestockDispatchPurpose = global::EdgIQ.MurimiOS.Domain.LivestockDispatch.LivestockDispatchPurpose;
using LivestockDispatch = global::EdgIQ.MurimiOS.Domain.LivestockDispatch.LivestockDispatch;
using LivestockSubjectKind = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubjectKind;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;
using MediaUploadSession = global::EdgIQ.MurimiOS.Domain.MediaUploadSession.MediaUploadSession;
using NotificationPreference = global::EdgIQ.MurimiOS.Domain.NotificationPreference.NotificationPreference;
using NotificationSeverity = global::EdgIQ.MurimiOS.Domain.NotificationRecord.NotificationSeverity;
using NotificationRecord = global::EdgIQ.MurimiOS.Domain.NotificationRecord.NotificationRecord;
using OrganisationNodeType = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNodeType;
using OrganisationNode = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNode;
using SyncCommandLog = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncCommandLog;
using SyncCommandStatus = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncCommandStatus;
using MembershipPermissionGrant = global::EdgIQ.MurimiOS.Domain.TenantMembership.MembershipPermissionGrant;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using TenantRole = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantRole;
using MembershipStatus = global::EdgIQ.MurimiOS.Domain.TenantMembership.MembershipStatus;
using TenantMembership = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantMembership;
using VeterinaryMedicineStockLot = global::EdgIQ.MurimiOS.Domain.VeterinaryMedicineStockLot.VeterinaryMedicineStockLot;
using PlatformDbContext = global::EdgIQ.MurimiOS.Infrastructure.Data.PlatformDbContext;
using OrganisationScopeAccess = global::EdgIQ.MurimiOS.Infrastructure.Identity.OrganisationScopeAccess;
using LocalMediaStorage = global::EdgIQ.MurimiOS.Infrastructure.Storage.LocalMediaStorage;
using RequestActorContext = global::EdgIQ.MurimiOS.API.Infrastructure.Access.RequestActorContext;
using ITenantContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantContext;
using MediaInspection = global::EdgIQ.MurimiOS.Application.Common.Abstractions.MediaInspection;
using ResolvedOrganisationScope = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ResolvedOrganisationScope;
using AuditSaveChangesInterceptor = global::EdgIQ.MurimiOS.Infrastructure.Data.Interceptors.AuditSaveChangesInterceptor;

public sealed class PlatformFoundationTests
{
    [Fact]
    public async Task Query_filters_isolate_tenants_and_save_stamps_ownership()
    {
        Guid tenantA = Guid.NewGuid();
        Guid tenantB = Guid.NewGuid();
        string databaseName = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        await using (var tenantADb = new PlatformDbContext(options, new TestTenantContext(tenantA)))
        {
            var node = new OrganisationNode(Guid.NewGuid(), "Farm A", OrganisationNodeType.Farm, "root.farm_a");
            var farm = new Farm(Guid.NewGuid(), node.Id, "Farm A", "A", "ZW", "Africa/Harare");
            tenantADb.OrganisationNodes.Add(node);
            tenantADb.Farms.Add(farm);
            await tenantADb.SaveChangesAsync();

            Assert.Equal(tenantA, farm.TenantId);
            Assert.NotEqual(default, farm.CreatedAtUtc);
            Assert.Equal(1u, farm.Version);
        }

        await using var tenantBDb = new PlatformDbContext(options, new TestTenantContext(tenantB));
        Assert.Empty(await tenantBDb.Farms.ToListAsync());
    }

    [Fact]
    public void Tenant_roles_are_least_privilege_and_explicit_grants_are_validated()
    {
        Assert.Contains(PlatformPermissions.AccessManage, PlatformPermissions.ForRole(TenantRole.Owner));
        Assert.Contains(PlatformPermissions.FarmRead, PlatformPermissions.ForRole(TenantRole.Viewer));
        Assert.DoesNotContain(PlatformPermissions.FarmWrite, PlatformPermissions.ForRole(TenantRole.Viewer));
        Assert.DoesNotContain(PlatformPermissions.WithdrawalOverride, PlatformPermissions.ForRole(TenantRole.LivestockManager));
        Assert.Contains(PlatformPermissions.AuditRead, PlatformPermissions.ForRole(TenantRole.Auditor));

        var grant = new MembershipPermissionGrant(Guid.NewGuid(), Guid.NewGuid(), PlatformPermissions.WithdrawalOverride);
        Assert.Equal(PlatformPermissions.WithdrawalOverride, grant.Permission);
        Assert.Throws<ArgumentException>(() => new MembershipPermissionGrant(Guid.NewGuid(), Guid.NewGuid(), "unknown.permission"));
    }

    [Fact]
    public void Tenant_membership_preserves_external_identity_and_organisation_scope()
    {
        Guid scope = Guid.NewGuid();
        var membership = new TenantMembership(
            Guid.NewGuid(), "oidc|user-42", "Farm Manager", "Manager@Example.Test",
            TenantRole.FarmManager, MembershipStatus.Active, scope);

        Assert.Equal("oidc|user-42", membership.ExternalSubject);
        Assert.Equal("manager@example.test", membership.Email);
        Assert.Equal(scope, membership.ScopeOrganisationNodeId);
        membership.UpdateAccess(TenantRole.Auditor, MembershipStatus.Suspended, null);
        Assert.Equal(MembershipStatus.Suspended, membership.Status);
    }

    [Fact]
    public async Task Organisation_scope_includes_descendant_farms_and_excludes_siblings()
    {
        Guid tenantId = Guid.NewGuid();
        var options = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        Guid groupId = Guid.NewGuid();
        Guid scopedFarmId = Guid.NewGuid();
        Guid siblingFarmId = Guid.NewGuid();

        await using var db = new PlatformDbContext(options, new TestTenantContext(tenantId));
        var group = new OrganisationNode(groupId, "Northern Region", OrganisationNodeType.Organisation, "group_north");
        var scopedNode = new OrganisationNode(
            Guid.NewGuid(), "Mazowe Farm", OrganisationNodeType.Farm,
            "group_north.farm_mazowe", groupId);
        var siblingNode = new OrganisationNode(
            Guid.NewGuid(), "Southern Farm", OrganisationNodeType.Farm,
            "group_south.farm_masvingo");
        db.OrganisationNodes.AddRange(group, scopedNode, siblingNode);
        db.Farms.AddRange(
            new Farm(scopedFarmId, scopedNode.Id, "Mazowe Farm", "MZW", "ZW", "Africa/Harare"),
            new Farm(siblingFarmId, siblingNode.Id, "Southern Farm", "MSV", "ZW", "Africa/Harare"));
        await db.SaveChangesAsync();

        var actor = new RequestActorContext();
        actor.Resolve(
            "oidc|manager", Guid.NewGuid(), TenantRole.FarmManager, groupId,
            PlatformPermissions.ForRole(TenantRole.FarmManager));
        var access = new OrganisationScopeAccess(db, actor);

        ResolvedOrganisationScope scope = await access.ResolveAsync(default);

        Assert.True(scope.AllowsNode(groupId));
        Assert.True(scope.AllowsNode(scopedNode.Id));
        Assert.True(scope.AllowsFarm(scopedFarmId));
        Assert.False(scope.AllowsNode(siblingNode.Id));
        Assert.False(scope.AllowsFarm(siblingFarmId));
    }

    [Fact]
    public async Task Missing_organisation_scope_fails_closed()
    {
        Guid tenantId = Guid.NewGuid();
        var options = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var db = new PlatformDbContext(options, new TestTenantContext(tenantId));
        var actor = new RequestActorContext();
        actor.Resolve(
            "oidc|stale", Guid.NewGuid(), TenantRole.Viewer, Guid.NewGuid(),
            PlatformPermissions.ForRole(TenantRole.Viewer));

        ResolvedOrganisationScope scope = await new OrganisationScopeAccess(db, actor).ResolveAsync(default);

        Assert.False(scope.IsUnrestricted);
        Assert.Empty(scope.FarmIds!);
        Assert.Empty(scope.OrganisationNodeIds!);
    }

    [Fact]
    public async Task Save_interceptor_writes_actor_attributed_before_and_after_audit_events()
    {
        Guid tenantId = Guid.NewGuid();
        var actor = new RequestActorContext();
        Guid membershipId = Guid.NewGuid();
        actor.Resolve(
            "oidc|audited-user", membershipId, TenantRole.Owner, null,
            PlatformPermissions.ForRole(TenantRole.Owner));
        var interceptor = new AuditSaveChangesInterceptor(
            new TestTenantContext(tenantId), actor, TimeProvider.System);
        var options = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .AddInterceptors(interceptor)
            .Options;

        await using var db = new PlatformDbContext(options, new TestTenantContext(tenantId));
        var node = new OrganisationNode(Guid.NewGuid(), "Audited Farm", OrganisationNodeType.Farm, "farm_audited");
        var farm = new Farm(Guid.NewGuid(), node.Id, "Audited Farm", "AUD", "ZW", "Africa/Harare");
        db.OrganisationNodes.Add(node);
        db.Farms.Add(farm);
        var membership = new TenantMembership(
            Guid.NewGuid(), "oidc|member-being-audited", "Audited Member", null,
            TenantRole.FarmManager, MembershipStatus.Active);
        db.TenantMemberships.Add(membership);
        await db.SaveChangesAsync();

        membership.UpdateAccess(TenantRole.Auditor, MembershipStatus.Active, null);
        await db.SaveChangesAsync();

        AuditEvent[] events = await db.AuditEvents.OrderBy(item => item.EntityType).ToArrayAsync();
        Assert.Equal(4, events.Length);
        Assert.All(events, item => Assert.Equal("oidc|audited-user", item.ActorSubject));
        Assert.All(events, item => Assert.Equal(membershipId, item.MembershipId));
        Assert.Contains(events, item => item.EntityId == farm.Id
            && item.FarmId == farm.Id
            && item.Changes.Contains("Audited Farm", StringComparison.Ordinal));
        Assert.Contains(events, item => item.EntityId == membership.Id
            && item.Action == "Updated"
            && item.Changes.Contains("\"before\"", StringComparison.Ordinal)
            && item.Changes.Contains("\"after\"", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Completed_records_are_superseded_and_corrections_are_append_only()
    {
        Guid tenantId = Guid.NewGuid();
        var options = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var db = new PlatformDbContext(options, new TestTenantContext(tenantId));
        Guid fieldId = Guid.NewGuid();
        Guid boundaryId = Guid.NewGuid();
        var original = new FieldOperationRecord(
            Guid.NewGuid(), fieldId, boundaryId, "Scouting",
            DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow, Guid.NewGuid(), "Incorrect result");
        db.FieldOperations.Add(original);
        await db.SaveChangesAsync();

        var replacement = new FieldOperationRecord(
            Guid.NewGuid(), fieldId, boundaryId, "Scouting",
            original.OccurredAt, DateTimeOffset.UtcNow, Guid.NewGuid(), "Correct result");
        original.Supersede(replacement.Id, "The original observation was entered against the wrong plot.");
        var correction = new CorrectionRecord(
            Guid.NewGuid(), typeof(FieldOperationRecord).FullName!, original.Id, replacement.Id,
            original.CorrectionReason!, DateTimeOffset.UtcNow, "oidc|corrector");
        db.FieldOperations.Add(replacement);
        db.CorrectionRecords.Add(correction);
        await db.SaveChangesAsync();

        Assert.Equal(replacement.Id, original.SupersededById);
        Assert.Throws<InvalidOperationException>(() => original.Supersede(Guid.NewGuid(), "Second correction"));
        db.Entry(correction).State = EntityState.Modified;
        await Assert.ThrowsAsync<InvalidOperationException>(() => db.SaveChangesAsync());
    }

    [Fact]
    public void Field_boundary_requires_valid_geometry_and_uses_wgs84()
    {
        GeometryFactory factory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        Polygon polygon = factory.CreatePolygon([
            new Coordinate(31.0, -17.8),
            new Coordinate(31.01, -17.8),
            new Coordinate(31.01, -17.81),
            new Coordinate(31.0, -17.8)
        ]);

        var boundary = new FieldBoundaryVersion(
            Guid.NewGuid(),
            Guid.NewGuid(),
            factory.CreateMultiPolygon([polygon]),
            new DateOnly(2026, 8, 28),
            "drawn");

        Assert.Equal(4326, boundary.Boundary.SRID);
        Assert.Equal("drawn", boundary.Source);
    }

    [Fact]
    public void Livestock_supports_individual_and_batch_subjects()
    {
        Guid farmId = Guid.NewGuid();
        var individual = new LivestockSubject(Guid.NewGuid(), farmId, LivestockSubjectKind.Individual, "Cow 12", "Cattle", "ZW-12");
        var group = new LivestockSubject(Guid.NewGuid(), farmId, LivestockSubjectKind.Group, "Broiler house 4", "Poultry", null);

        Assert.Equal(LivestockSubjectKind.Individual, individual.Kind);
        Assert.Equal(LivestockSubjectKind.Group, group.Kind);
    }

    [Fact]
    public void Sync_command_log_preserves_terminal_rejection_details()
    {
        var command = new SyncCommandLog(
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            "RecordFieldOperation",
            1,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow);

        command.MarkRejected("field_not_found", "Field not found.");

        Assert.Equal(SyncCommandStatus.Rejected, command.Status);
        Assert.Equal("field_not_found", command.ErrorCode);
    }

    [Fact]
    public void Season_requires_an_ordered_date_range()
    {
        Assert.Throws<ArgumentException>(() => new GrowingSeason(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Summer 2027",
            new DateOnly(2027, 10, 1),
            new DateOnly(2027, 3, 31),
            GrowingSeasonStatus.Planned));
    }

    [Fact]
    public void Crop_zone_and_operation_preserve_the_versioned_production_context()
    {
        GeometryFactory factory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        Polygon polygon = factory.CreatePolygon([
            new Coordinate(31.0, -17.8),
            new Coordinate(31.01, -17.8),
            new Coordinate(31.01, -17.81),
            new Coordinate(31.0, -17.8)
        ]);
        Guid fieldId = Guid.NewGuid();
        Guid boundaryVersionId = Guid.NewGuid();
        var zone = new CropZone(
            Guid.NewGuid(),
            Guid.NewGuid(),
            fieldId,
            boundaryVersionId,
            "North maize",
            "Maize",
            "SC 719",
            factory.CreateMultiPolygon([polygon]));
        var operation = new FieldOperationRecord(
            Guid.NewGuid(),
            fieldId,
            boundaryVersionId,
            "Sowing",
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow,
            Guid.NewGuid(),
            null,
            zone.Id);

        Assert.Equal("SC 719", zone.Variety);
        Assert.Equal(zone.Id, operation.CropZoneId);
    }

    [Fact]
    public void Stock_lot_prevents_over_issue_and_preserves_on_hand_quantity()
    {
        var lot = new InputStockLot(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "SUPPLIER-LOT-01",
            new DateOnly(2026, 8, 1),
            new DateOnly(2027, 8, 1),
            100m,
            "kg");

        lot.Issue(35.5m);

        Assert.Equal(64.5m, lot.QuantityOnHand);
        Assert.Throws<InvalidOperationException>(() => lot.Issue(65m));
    }

    [Fact]
    public void Harvest_lot_requires_positive_quantity_and_keeps_source_command()
    {
        Guid commandId = Guid.NewGuid();
        var harvest = new HarvestLot(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "HARVEST-2027-001",
            2.5m,
            "t",
            DateTimeOffset.UtcNow,
            commandId,
            null);

        Assert.Equal(commandId, harvest.SourceCommandId);
        Assert.Equal(2500m, harvest.Quantity);
        Assert.Equal("kg", harvest.Unit);
        Assert.Equal(2.5m, harvest.EnteredQuantity);
        Assert.Equal("t", harvest.EnteredUnit);
    }

    [Fact]
    public void Treatment_snapshots_withdrawal_dates_and_issues_medicine_stock()
    {
        DateTimeOffset administeredAt = new(2026, 8, 28, 10, 0, 0, TimeSpan.Zero);
        var stock = new VeterinaryMedicineStockLot(
            Guid.NewGuid(), Guid.NewGuid(), "VET-BATCH-01", new DateOnly(2026, 8, 1), new DateOnly(2027, 8, 1), 100m, "mL");
        var treatment = new PlatformLivestockTreatment(
            Guid.NewGuid(), Guid.NewGuid(), stock.Id, 12.5m, "ml", "Intramuscular",
            administeredAt, administeredAt, 28, 72, Guid.NewGuid(), null);
        stock.Issue(treatment.Dose);

        Assert.Equal(0.0875m, stock.QuantityOnHand);
        Assert.Equal(0.0125m, treatment.Dose);
        Assert.Equal(12.5m, treatment.EnteredDose);
        Assert.Equal(administeredAt.AddDays(28), treatment.MeatHoldUntil);
        Assert.Equal(administeredAt.AddHours(72), treatment.MilkHoldUntil);
    }

    [Fact]
    public void Dispatch_is_blocked_by_withdrawal_hold_unless_an_authorized_override_is_recorded()
    {
        Guid subjectId = Guid.NewGuid();
        DateTimeOffset dispatchAt = new(2026, 8, 29, 10, 0, 0, TimeSpan.Zero);
        DateTimeOffset holdUntil = dispatchAt.AddDays(7);

        Assert.Throws<InvalidOperationException>(() => new LivestockDispatch(
            Guid.NewGuid(), subjectId, LivestockDispatchPurpose.Slaughter, dispatchAt,
            "Licensed abattoir", holdUntil, null, null));

        var overridden = new LivestockDispatch(
            Guid.NewGuid(), subjectId, LivestockDispatchPurpose.Slaughter, dispatchAt,
            "Licensed abattoir", holdUntil, "Emergency welfare slaughter", "vet@example.test");
        Assert.Equal("vet@example.test", overridden.OverrideAuthorizedBy);
        Assert.Equal(holdUntil, overridden.WithdrawalHoldUntil);
    }

    [Fact]
    public void Media_upload_session_enforces_sequential_offsets()
    {
        var upload = new MediaUploadSession(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "field.jpg",
            "image/jpeg",
            10,
            new string('a', 64),
            DateTimeOffset.UtcNow,
            -17.8,
            31.0,
            4.5,
            DateTimeOffset.UtcNow.AddDays(7));

        upload.Advance(0, 4);

        Assert.Equal(4, upload.ReceivedBytes);
        Assert.Throws<InvalidOperationException>(() => upload.Advance(3, 2));
    }

    [Fact]
    public async Task Local_media_storage_resumes_and_verifies_bytes()
    {
        string root = Path.Combine(Path.GetTempPath(), "murimios-media-tests", Guid.NewGuid().ToString("N"));
        try
        {
            var storage = new LocalMediaStorage(root);
            Guid tenantId = Guid.NewGuid();
            Guid uploadId = Guid.NewGuid();
            Guid evidenceId = Guid.NewGuid();
            byte[] first = [1, 2, 3];
            byte[] second = [4, 5];

            await storage.AppendChunkAsync(tenantId, uploadId, 0, new MemoryStream(first), 10, default);
            await storage.AppendChunkAsync(tenantId, uploadId, 3, new MemoryStream(second), 10, default);
            await using (Stream pending = await storage.OpenPendingReadAsync(tenantId, uploadId, default))
            {
                Assert.Equal(5, pending.Length);
            }
            MediaInspection inspection = await storage.InspectPendingAsync(tenantId, uploadId, default);
            string objectKey = await storage.CommitAsync(tenantId, uploadId, evidenceId, "image/jpeg", default);
            await using Stream saved = await storage.OpenReadAsync(objectKey, default);
            await storage.CheckHealthAsync(default);

            Assert.Equal(5, inspection.Length);
            Assert.Equal(Convert.ToHexStringLower(SHA256.HashData([1, 2, 3, 4, 5])), inspection.Sha256);
            Assert.Equal(5, saved.Length);
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }

    [Fact]
    public async Task Local_media_storage_only_deletes_tenant_scoped_derived_objects()
    {
        string root = Path.Combine(Path.GetTempPath(), "murimios-derived-media-tests", Guid.NewGuid().ToString("N"));
        try
        {
            var storage = new LocalMediaStorage(root);
            Guid tenantId = Guid.NewGuid();
            byte[] content = [1, 3, 5, 7];
            string key = await storage.PutDerivedAsync(tenantId, Guid.NewGuid(), "tif", "image/tiff",
                new MemoryStream(content), content.Length, default);

            await storage.DeleteDerivedAsync(tenantId, key, default);
            await Assert.ThrowsAsync<FileNotFoundException>(async () =>
            {
                await using Stream _ = await storage.OpenReadAsync(key, default);
            });
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                storage.DeleteDerivedAsync(tenantId, $"{tenantId:N}/imports/source", default));
        }
        finally
        {
            if (Directory.Exists(root)) Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void Notification_requires_a_safe_application_relative_action_url_and_marks_read_once()
    {
        Assert.Throws<ArgumentException>(() => new NotificationRecord(
            Guid.NewGuid(), Guid.NewGuid(), "weather", "Storm warning", "Heavy rainfall is expected.",
            NotificationSeverity.Warning, actionUrl: "https://example.test/farms"));

        var notification = new NotificationRecord(
            Guid.NewGuid(), Guid.NewGuid(), "weather", "Storm warning", "Heavy rainfall is expected.",
            NotificationSeverity.Warning, actionUrl: "/farms/field-01");
        DateTimeOffset firstRead = new(2026, 8, 29, 8, 0, 0, TimeSpan.Zero);
        notification.MarkRead(firstRead);
        notification.MarkRead(firstRead.AddHours(1));

        Assert.Equal(firstRead, notification.ReadAt);
        Assert.Equal("weather", notification.Category);
    }

    [Fact]
    public void Notification_preference_normalizes_category_and_keeps_one_channel_enabled()
    {
        var preference = new NotificationPreference(
            Guid.NewGuid(), Guid.NewGuid(), "  Weather Alerts ", true, false, false);

        Assert.Equal("weather alerts", preference.Category);
        Assert.Throws<ArgumentException>(() => preference.SetChannels(false, false, false));
        preference.SetChannels(false, true, true);
        Assert.True(preference.EmailEnabled);
        Assert.True(preference.SmsEnabled);
    }

    private sealed class TestTenantContext(Guid tenantId) : ITenantContext
    {
        public Guid TenantId { get; } = tenantId;

        public bool IsResolved => true;
    }
}
