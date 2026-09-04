using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.EpcisExchange;
using EdgIQ.MurimiOS.Domain.RecallCase;
using EdgIQ.MurimiOS.Domain.QualityAssessment;
using EdgIQ.MurimiOS.Domain.StorageLot;
using EdgIQ.MurimiOS.Domain.TraceEvent;

using EdgIQ.MurimiOS.Domain.Delivery;
using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;

namespace EdgIQ.MurimiOS.Domain.TenantMigrationJob;
using TenantDatabaseMode = global::EdgIQ.MurimiOS.Domain.TenantPlacement.TenantDatabaseMode;
using PublicApiClient = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiClient;

public sealed class TenantMigrationJob : TenantEntity
{
    private TenantMigrationJob() { }
    public TenantMigrationJob(Guid id, TenantMigrationKind kind, Guid sourcePlacementId, string destinationRegion, string destinationDatabaseKey, TenantDatabaseMode destinationMode, DateTimeOffset requestedAt, string requestedBy) : base(id)
    { if (sourcePlacementId == Guid.Empty) throw new ArgumentException("Source placement is required."); Kind = kind; SourcePlacementId = sourcePlacementId; DestinationRegion = PublicApiClient.Text(destinationRegion, 80); DestinationDatabaseKey = PublicApiClient.Text(destinationDatabaseKey, 160); DestinationMode = destinationMode; RequestedAt = requestedAt.ToUniversalTime(); RequestedBy = PublicApiClient.Text(requestedBy, 240); Status = TenantMigrationStatus.Draft; }
    public TenantMigrationKind Kind { get; private set; } public Guid SourcePlacementId { get; private set; } public string DestinationRegion { get; private set; } = ""; public string DestinationDatabaseKey { get; private set; } = ""; public TenantDatabaseMode DestinationMode { get; private set; } public TenantMigrationStatus Status { get; private set; } public DateTimeOffset RequestedAt { get; private set; } public string RequestedBy { get; private set; } = ""; public string? ArtifactObjectKey { get; private set; } public string? ArtifactSha256 { get; private set; } public string? ManifestJson { get; private set; } public DateTimeOffset? CompletedAt { get; private set; } public string? FailureCode { get; private set; } public string? FailureMessage { get; private set; }
    public void StartExport() { if (Status != TenantMigrationStatus.Draft) throw new InvalidOperationException("Only a draft migration can start export."); Status = TenantMigrationStatus.Exporting; }
    public void CompleteExport(string objectKey, string sha256, string manifestJson) { if (Status != TenantMigrationStatus.Exporting) throw new InvalidOperationException("Migration is not exporting."); ArtifactObjectKey = PublicApiClient.Text(objectKey, 1000); ArtifactSha256 = PublicApiClient.Hash(sha256); ManifestJson = PublicApiClient.Text(manifestJson, 1_000_000); Status = TenantMigrationStatus.Exported; }
    public void Validate() { if (Status != TenantMigrationStatus.Exported) throw new InvalidOperationException("Only an exported artifact can be validated."); Status = TenantMigrationStatus.Validated; }
    public void StartImport() { if (Status != TenantMigrationStatus.Validated) throw new InvalidOperationException("Only a validated migration can import."); Status = TenantMigrationStatus.Importing; }
    public void Complete(DateTimeOffset at) { if (Status != TenantMigrationStatus.Importing) throw new InvalidOperationException("Only an importing migration can complete."); Status = TenantMigrationStatus.Completed; CompletedAt = at.ToUniversalTime(); }
    public void Fail(DateTimeOffset at, string code, string message) { if (Status is TenantMigrationStatus.Completed or TenantMigrationStatus.Cancelled or TenantMigrationStatus.Failed) throw new InvalidOperationException("A terminal migration cannot fail."); Status = TenantMigrationStatus.Failed; CompletedAt = at.ToUniversalTime(); FailureCode = PublicApiClient.Text(code, 80); FailureMessage = PublicApiClient.Text(message, 2000); }
    public void Cancel(DateTimeOffset at) { if (Status is TenantMigrationStatus.Completed or TenantMigrationStatus.Cancelled or TenantMigrationStatus.Failed) throw new InvalidOperationException("A terminal migration cannot be cancelled."); Status = TenantMigrationStatus.Cancelled; CompletedAt = at.ToUniversalTime(); }
}
