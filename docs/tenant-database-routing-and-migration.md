# Tenant database routing and migration

MurimiOS keeps tenant placement and migration state in a control database. Before EF opens a tenant connection, the API and worker resolve the active primary placement through the control database and replace the unopened connection with the configured destination connection. Database credentials never appear in placement rows: `database_key` is only an opaque key into deployment configuration.

## Configuration

Every API and worker host must define the same database-key catalogue. A value may name an entry under `ConnectionStrings` or be a directly injected PostgreSQL connection string.

```json
{
  "ConnectionStrings": {
    "MurimiOS": "...",
    "TenantAcme": "..."
  },
  "TenantDatabases": {
    "ControlDatabaseKey": "primary",
    "RouteCacheSeconds": 15,
    "Connections": {
      "primary": "MurimiOS",
      "acme-dedicated": "TenantAcme"
    }
  }
}
```

Unknown database keys fail closed. The migration API rejects an unconfigured destination. A non-control primary placement cannot be activated directly; only a completed migration can perform that cutover.

Each destination must already have the complete MurimiOS schema, extensions, least-privilege application role, row-level-security policies, and the same EF migration version as the source. Provisioning credentials should remain outside the application process; infrastructure automation provisions the database and injects only its runtime connection string.

## Execution lifecycle

Creating `POST /api/v1/enterprise/migration-jobs` leaves a draft job for the worker. The worker then:

1. Atomically leases the job in the control database and moves it to `Exporting`.
2. Activates the API write guard and waits for the configured drain interval.
3. Enumerates top-level tenant tables in foreign-key order under the source tenant's RLS context.
4. Writes an authenticated encrypted record artifact using the shared ASP.NET Core Data Protection key ring.
5. Stores its SHA-256 digest, byte and row counts, table manifest, and source schema version.
6. Reopens and authenticates every record, verifies the tenant ID and digest, and requires the destination schema version to match.
7. Imports under the destination tenant's RLS context in a single transaction using conflict-safe inserts.
8. Verifies every destination table count against the export manifest.
9. Atomically closes the source primary placement, creates the destination placement, completes the job, and invalidates the route cache.

Migration management tables remain in the control database and are deliberately excluded from data-plane artifacts. Reads remain available during execution; non-control writes receive HTTP `423` with code `tenant_migration_write_locked`. Only cancellation and explicit failure remain manual transitions—operators cannot manually mark an unvalidated export or import complete.

## Artifact storage and key management

The worker stores artifacts below `TenantMigrationExecution:ArtifactRoot`. The directory must be durable, private to the migration worker, backed up appropriately, and shared by the worker instance that may resume a lease. Every record is independently authenticated and encrypted by the `MurimiOS.TenantMigrationArtifact.v1` Data Protector. API and worker instances must share the protected Data Protection key ring; loss of that ring makes existing artifacts intentionally unreadable.

For cross-region production deployments, mount the artifact root on an encrypted replicated object/filesystem gateway and apply retention deletion after the rollback window. The source data is retained after cutover for rollback and must be isolated from normal application traffic, then purged under an approved retention policy.

## Operational controls

- Migration claims use database leases, bounded retries, exponential backoff, and `SKIP LOCKED` coordination.
- Re-import is idempotent, but any pre-existing or extra destination tenant rows cause the table-count validation to fail.
- Route results are cached briefly and explicitly invalidated at placement changes and cutover.
- Placement and migration endpoints always use the control database even after the tenant has moved.
- Run a live migration and rollback drill for every database/region topology before production use.
