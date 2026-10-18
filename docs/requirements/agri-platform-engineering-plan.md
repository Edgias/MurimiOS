# Farm & Livestock Platform — Engineering Plan

**Version:** 1.0
**Scope:** Engineering only. No product strategy, segmentation, pricing or GTM.
**Baseline stack:** .NET 10, EF Core 10, PostgreSQL 17 + PostGIS + TimescaleDB, RabbitMQ, Redis, Angular 22, S3-compatible object storage.

---

## 1. Engineering constraints

These are the properties that drive design decisions. Everything downstream is a consequence of one of these.

| ID | Constraint | Consequence |
|---|---|---|
| C-1 | Field data is captured with no network for days at a time | Offline-first client with a durable command queue; server must accept out-of-order, delayed, replayed writes |
| C-2 | Geometry is a primary key concept, not an attribute | PostGIS + NetTopologySuite in the domain model; spatial indexes; area computed server-side |
| C-3 | Compliance records are legal evidence | Append-only operational records; corrections as new events; full audit trail; no destructive updates |
| C-4 | Cross-tenant reads are a required feature | Explicit grant model enforced at the query layer *and* the database layer; never an admin bypass |
| C-5 | Time-series row volume will exceed transactional volume by 2-3 orders of magnitude | Separate storage strategy, retention policy and query path from day one |
| C-6 | Load is seasonal and bursty (planting/harvest peaks) | Async-first write path; queue-absorbed spikes; no synchronous fan-out on ingest |
| C-7 | Units, locales and regulatory catalogues vary by region | Unit-typed value objects in the domain; region-scoped reference data; region-pinned tenants |
| C-8 | Single primary engineer for the first phases | Modular monolith. No distributed system until a module's scaling profile actually diverges |

---

## 2. Solution architecture

### 2.1 Shape: modular monolith, service-ready

One deployable API, one worker host, module boundaries enforced at compile time. Extraction to services later is a deployment change, not a rewrite, because modules already communicate only through contracts and integration events.

```
┌──────────────────────────────────────────────────────────────┐
│  Api.Host (ASP.NET Core, minimal APIs)                       │
│  ├─ endpoint groups per module                               │
│  ├─ auth, tenant resolution, exception handling, OTel        │
├──────────────────────────────────────────────────────────────┤
│  Modules (vertical slices, Clean Architecture per module)    │
│  FarmRegistry │ Planning │ Operations │ Inventory │          │
│  FieldIntel │ Livestock │ Traceability │ Compliance │        │
│  SupplyChain │ Finance │ Sync │ Identity                     │
├──────────────────────────────────────────────────────────────┤
│  Shared Kernel: Money, Quantity<TUnit>, Geo, TenantId,       │
│  AuditedAggregateRoot, IMustHaveTenant, DomainEvent          │
├──────────────────────────────────────────────────────────────┤
│  Infrastructure: EF Core, PostGIS, Outbox, Blob, Cache,      │
│  Integration adapters, Messaging                             │
└──────────────────────────────────────────────────────────────┘
        │                                    │
        ▼                                    ▼
┌──────────────────┐              ┌──────────────────────────┐
│ Worker.Host      │              │ PostgreSQL 17            │
│ role-flagged     │◄─ RabbitMQ ─►│ + PostGIS + TimescaleDB  │
│ [Flags] WorkerRole│             │ + Redis + S3             │
└──────────────────┘              └──────────────────────────┘
```

Reuse the KonnektCRM worker pattern directly: a single worker host, `[Flags] WorkerRole` enum, role selected by env var in compose, `PeriodicTimer` loops, `IDbContextFactory` for scoped contexts.

### 2.2 Project layout

```
src/
  BuildingBlocks/
    Domain/              AggregateRoot, DomainEvent, ValueObject, Quantity<T>, Money, GeoPolygon
    Application/         ICommand, IQuery, PagedResult<T>, QueryableExtensions, FilterCondition
    Infrastructure/      DbContextBase, TenantInterceptor, AuditInterceptor, Outbox, BlobStore
    Contracts/           integration event contracts + cross-module read contracts ONLY
  Modules/
    FarmRegistry/
      Domain/ Application/ Infrastructure/ Endpoints/
    Operations/
    Livestock/
    ...
  Hosts/
    Api/
    Worker/
tests/
  Architecture.Tests/    module boundary enforcement
  <Module>.UnitTests/
  Integration.Tests/     Testcontainers: postgis/postgis:17-3.5
  Sync.PropertyTests/
```

### 2.3 Module boundary rules (enforced by NetArchTest in CI)

1. A module may reference `BuildingBlocks.*` and `Modules.*.Contracts`. Nothing else.
2. No module references another module's `Domain`, `Infrastructure` or DbContext.
3. Cross-module reads go through a contract interface implemented in the owning module (`IFieldLookup.GetAsync(FieldId)` returning a DTO, not an entity).
4. Cross-module writes go through integration events on the outbox. Never a direct call.
5. All modules share one physical database and one `DbContext` per module using **table-per-module schemas** (`farm.fields`, `livestock.animals`). One connection, one transaction, separate `DbContext` types with `HasDefaultSchema`.

Rule 5 is the pragmatic compromise: logical separation for the code, transactional simplicity for the database. Revisit only when a module needs its own storage engine.

---

## 3. Multi-tenancy implementation

### 3.1 Tenant resolution pipeline

```csharp
// Api.Host middleware order
UseAuthentication()
UseTenantResolution()      // claim -> ITenantContext (scoped, AsyncLocal-free)
UseAuthorization()         // resource-scoped policies need TenantContext
```

`ITenantContext` exposes `TenantId`, `Region`, `OrganisationScope` (the set of org/farm node ids the principal may see, resolved once per request from a cached permission snapshot).

### 3.2 Three enforcement layers

**Layer 1 — EF Core named query filters** (application convenience, same as KonnektCRM):

```csharp
modelBuilder.Entity<Field>()
    .HasQueryFilter("Tenant", e => e.TenantId == _tenantContext.TenantId)
    .HasQueryFilter("SoftDelete", e => !e.IsDeleted);
```

Cross-tenant grant queries use `IgnoreQueryFilters(["Tenant"])` and then apply the grant predicate explicitly. Nothing else in the codebase is permitted to call `IgnoreQueryFilters` on `"Tenant"` — enforced by an analyser rule plus a CI grep gate with an allowlist of file paths.

**Layer 2 — PostgreSQL RLS** (defence in depth against raw SQL and future mistakes):

```sql
ALTER TABLE farm.fields ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation ON farm.fields
  USING (tenant_id = current_setting('app.tenant_id')::uuid);
```

A `DbConnectionInterceptor` issues `SET LOCAL app.tenant_id` on connection open within the request scope. The grant path uses a separate role with a policy that also permits rows reachable through an active `supply.data_grants` row.

**Layer 3 — Audit of every cross-tenant read.** The grant query path writes to `supply.grant_access_log` (grant id, actor, entity type, entity ids, timestamp). This is a product feature and a forensic tool.

### 3.3 Region pinning

`Tenant.Region` resolves connection string and blob container via `ITenantConnectionResolver`. Single-region deployment initially, but the resolver indirection is present from day one so the second region is a config change. A tenant never spans regions; there is no cross-region query path.

### 3.4 Enterprise isolation escape hatch

`Tenant.IsolationMode ∈ { Shared, DedicatedDatabase }`. Dedicated tenants resolve to their own connection string; identical code path, migrations applied by a fan-out job over the tenant catalogue. Do not build this until a signed customer requires it, but keep the resolver seam.

### 3.5 Organisational scoping below the tenant

```sql
CREATE TABLE farm.org_nodes (
  id uuid PRIMARY KEY,
  tenant_id uuid NOT NULL,
  parent_id uuid REFERENCES farm.org_nodes(id),
  node_type text NOT NULL,        -- organisation | farm | site
  path ltree NOT NULL,            -- 'root.org1.farm3'
  name text NOT NULL
);
CREATE INDEX ix_org_nodes_path ON farm.org_nodes USING gist (path);
```

Permission checks become `node.path <@ ANY(@grantedPaths)`. Roll-up reporting becomes a single `path <@` predicate. Avoids recursive CTEs on every request.

---

## 4. Data model — engineering specifics

### 4.1 Temporal field boundaries

Boundaries change (merges, splits, subsidy re-declarations). Operations must reference the boundary as it was.

```sql
CREATE TABLE farm.field_boundaries (
  id uuid PRIMARY KEY,
  field_id uuid NOT NULL,
  tenant_id uuid NOT NULL,
  geom geography(MultiPolygon, 4326) NOT NULL,
  geom_projected geometry(MultiPolygon, 3857) GENERATED ALWAYS AS
      (ST_Transform(geom::geometry, 3857)) STORED,
  area_ha numeric(12,4) GENERATED ALWAYS AS
      (ST_Area(geom) / 10000.0) STORED,
  valid_from date NOT NULL,
  valid_to date,                      -- null = current
  source text NOT NULL,               -- drawn | shapefile | iacs | isoxml
  EXCLUDE USING gist (
    field_id WITH =,
    daterange(valid_from, valid_to) WITH &&
  )
);
CREATE INDEX ix_field_boundaries_geom ON farm.field_boundaries USING gist (geom);
```

The exclusion constraint makes overlapping validity periods impossible at the database level. `area_ha` is generated, never client-supplied.

### 4.2 Operations as append-only records

```sql
CREATE TABLE ops.field_operations (
  id uuid PRIMARY KEY,
  tenant_id uuid NOT NULL,
  crop_zone_id uuid NOT NULL,
  boundary_id uuid NOT NULL,          -- pins geometry version
  operation_type text NOT NULL,
  status text NOT NULL,
  occurred_at timestamptz NOT NULL,
  recorded_at timestamptz NOT NULL DEFAULT now(),
  recorded_by uuid NOT NULL,
  device_id uuid,
  capture_lat double precision,
  capture_lon double precision,
  capture_accuracy_m real,
  superseded_by uuid REFERENCES ops.field_operations(id),
  correction_reason text,
  xmin_token xid8,                     -- UseXminAsConcurrencyToken
  payload jsonb NOT NULL
);
```

Edits create a new row with `superseded_by` set on the old one. `occurred_at` vs `recorded_at` matters: offline capture means these differ by days, and audit questions are always about `occurred_at` while sync debugging is always about `recorded_at`. Capture accuracy is evidence quality; store it.

A `BEFORE UPDATE` trigger rejects any update to a completed operation other than setting `superseded_by`.

### 4.3 Time-series tables (TimescaleDB)

```sql
CREATE TABLE intel.sensor_readings (
  tenant_id uuid NOT NULL,
  sensor_id uuid NOT NULL,
  observed_at timestamptz NOT NULL,
  metric text NOT NULL,
  value double precision NOT NULL,
  quality smallint NOT NULL
);
SELECT create_hypertable('intel.sensor_readings', 'observed_at',
        chunk_time_interval => interval '7 days');

CREATE MATERIALIZED VIEW intel.sensor_hourly
WITH (timescaledb.continuous) AS
SELECT tenant_id, sensor_id, metric,
       time_bucket('1 hour', observed_at) AS bucket,
       avg(value), min(value), max(value), count(*)
FROM intel.sensor_readings GROUP BY 1,2,3,4;

SELECT add_retention_policy('intel.sensor_readings', interval '90 days');
SELECT add_retention_policy('intel.sensor_hourly',   interval '2 years');
```

Daily rollups retained indefinitely in a plain table. Satellite index readings are *not* hypertables — they are low-cardinality (one row per field per acquisition) and belong in normal transactional storage.

### 4.4 Traceability event log

One append-only table modelled on EPCIS 2.0, shared by crop and livestock modules.

```sql
CREATE TABLE trace.events (
  id uuid PRIMARY KEY,
  tenant_id uuid NOT NULL,
  event_type text NOT NULL,        -- Object | Aggregation | Transformation | Transaction
  event_time timestamptz NOT NULL,
  record_time timestamptz NOT NULL DEFAULT now(),
  biz_step text NOT NULL,
  disposition text,
  read_point text,                 -- GLN or internal location id
  epc_list jsonb NOT NULL,         -- subject identifiers
  input_epcs jsonb,                -- transformation only
  output_epcs jsonb,
  parent_epc text,                 -- aggregation only
  source_module text NOT NULL,
  source_record_id uuid NOT NULL,
  ext jsonb                        -- module-specific extension fields
);
CREATE INDEX ix_trace_epc ON trace.events USING gin (epc_list jsonb_path_ops);
CREATE INDEX ix_trace_tenant_time ON trace.events (tenant_id, event_time DESC);
```

Modules never write here directly. They raise domain events; a `TraceabilityProjector` subscribes and appends. Table is `INSERT`-only — revoke `UPDATE`/`DELETE` from the application role.

Trace queries (forward: where did this batch go; backward: what went into this batch) are recursive CTEs over `epc_list`/`input_epcs`/`output_epcs`. Depth-limit them and cache results by root EPC.

### 4.5 Compliance as data, not code

```
standards → control_points → evidence_requirements → evaluations
```

`evidence_requirements.query_key` names a registered evaluator (`ISpecEvaluator` implementation) plus a `parameters jsonb`. Adding a standard is a seed data migration plus, occasionally, one new evaluator class. A nightly worker re-evaluates all active control points per tenant per season and writes `compliance.evaluations` with status, evidence record ids and computed-at timestamp.

Do not evaluate on read. Evaluations are expensive and users want a dashboard, not a 20-second wait.

### 4.6 Units in the domain

```csharp
public readonly record struct Quantity<TUnit>(decimal Value, TUnit Unit)
    where TUnit : struct, Enum;

public readonly record struct ApplicationRate(
    Quantity<MassUnit> Amount, Quantity<AreaUnit> PerArea);
```

Store canonical SI in the database (`kg`, `ha`, `L`, `°C`) with the user's display unit recorded alongside for round-trip fidelity. EF value converters handle persistence. Never convert in the UI; the API returns both canonical and display-formatted values.

---

## 5. Offline sync — protocol design

The highest-risk component. Design it explicitly, prototype it in Phase 0.

### 5.1 Principle: sync intents, not state

The device does not push rows. It pushes an ordered log of immutable commands. The server replays them idempotently. There is no row-level three-way merge, because most agricultural records are append-only observations that cannot conflict.

### 5.2 Push envelope

```json
{
  "deviceId": "uuid",
  "batch": [
    {
      "commandId": "uuid",            // client-generated, idempotency key
      "sequence": 1421,               // monotonic per device
      "type": "RecordFieldOperation",
      "capturedAt": "2026-08-14T06:11:03Z",
      "clientVersion": "2.3.1",
      "payload": { }
    }
  ]
}
```

Server behaviour:

- `sync.command_log (device_id, command_id PK, sequence, status, result_id, error)`. A replayed `commandId` returns the original result, never re-executes.
- Commands apply strictly in `sequence` order per device. A gap parks the batch and returns `409` with the expected sequence; the client resends from there.
- Each command returns one of `Applied`, `Duplicate`, `Rejected(reason)`, `Conflict(serverState)`.
- Rejections are terminal and surfaced in a device-side error inbox. They are never silently dropped, and never retried indefinitely.

### 5.3 Conflict policy — decided per command type, documented in code

```csharp
[SyncCommand(Conflict = ConflictPolicy.AppendAlways)]      // observations
[SyncCommand(Conflict = ConflictPolicy.ServerWins)]        // plan edits
[SyncCommand(Conflict = ConflictPolicy.LastWriteWins)]     // draft notes
[SyncCommand(Conflict = ConflictPolicy.ManualResolution)]  // rare, inbox
```

An architecture test fails the build if a sync command type has no explicit policy attribute. Defaults are how sync systems rot.

### 5.4 Pull (delta)

Checkpoint-based, not timestamp-based. Timestamps break on clock skew and on rows written inside a long transaction.

- Every syncable table has `change_seq bigint` from a per-tenant sequence, set by trigger on insert/update.
- Client stores `lastSeq` per collection. Pull is `WHERE change_seq > @lastSeq ORDER BY change_seq LIMIT n`.
- Deletes are tombstones with their own `change_seq`, retained 90 days. A client offline longer than 90 days is forced into a full resync.
- Scope: current season, assigned farms, active herds, reference catalogues. Cap the working set explicitly; never sync an entire tenant to a phone.

### 5.5 Media

Photos are queued separately from commands. A command references `mediaId`; the record is valid before the bytes arrive. Upload is direct-to-object-storage via a pre-signed URL, chunked, resumable, with exponential backoff. A reconciliation worker flags media referenced but never uploaded after 14 days.

### 5.6 Testing the sync layer

- **Property tests:** generate random command sequences with duplicates, reordering, gaps and replays; assert final server state equals the state from an ideal in-order application. This is where the bugs are.
- **Airplane-mode acceptance test:** a scripted week of offline capture on a physical device, then reconnect, run before every release.
- **Clock-skew tests:** device clock ±48 hours.
- **Storage-exhaustion test:** device fills local disk mid-capture.

---

## 6. Geospatial engineering

- **NetTopologySuite** with `UseNetTopologySuite()` on Npgsql. Domain uses NTS `Geometry`; do not invent a wrapper type.
- `geography(4326)` for storage and true-area/distance; a generated `geometry(3857)` column for fast intersection and tiling.
- Validate on ingest: `ST_IsValid`, `ST_MakeValid`, ring orientation, self-intersection rejection, area sanity bounds (reject a "field" of 4 million ha), and a maximum vertex count with `ST_SimplifyPreserveTopology` above it.
- **Import pipeline** (highest-value onboarding feature): Shapefile (`.shp`/`.dbf`/`.prj` zip), GeoJSON, KML, ISOXML. Run in a worker, not in the request. Detect source CRS from `.prj`, transform to 4326, present a preview map with attribute-to-field mapping before commit. Idempotent by content hash so a re-upload does not duplicate.
- **Tile serving:** `ST_AsMVT` straight from Postgres for field layers, cached in Redis by `z/x/y + tenant + layer version`. No separate tile server until profiling says otherwise.
- **Raster:** do not store or process rasters in Phase 1-2. Store scalar index values per field per acquisition; cache pre-rendered PNG overlays in object storage keyed by `field_id/date/index`.

---

## 7. Async processing and integration

### 7.1 Outbox everywhere

Every domain event that crosses a module boundary or leaves the process goes through a transactional outbox in the same commit as the state change. A dispatcher worker publishes to RabbitMQ with at-least-once delivery. Consumers are idempotent by `messageId` in a processed-message table with a 7-day TTL.

### 7.2 Worker roles

```csharp
[Flags]
public enum WorkerRole
{
    None = 0,
    OutboxDispatch   = 1 << 0,
    SatelliteIngest  = 1 << 1,
    WeatherIngest    = 1 << 2,
    SensorIngest     = 1 << 3,
    ComplianceEval   = 1 << 4,
    AnalyticsRefresh = 1 << 5,
    SpatialImport    = 1 << 6,
    Notifications    = 1 << 7,
    MediaProcessing  = 1 << 8,
}
```

One host image, role set by env var, scaled per role. Same pattern already proven in KonnektCRM.

### 7.3 External provider abstraction

Every third party sits behind a module-owned port with an anti-corruption adapter:

```
IWeatherProvider        → OpenMeteoAdapter, MeteomaticsAdapter
ISatelliteIndexProvider → SentinelHubAdapter, CopernicusAdapter
IAccountingConnector    → XeroAdapter, QuickBooksAdapter
IMovementRegistry       → BcmsAdapter, ScotEidAdapter
ITaskDataCodec          → IsoxmlCodec (via ADAPT)
```

Every adapter gets: Polly retry with jitter, a circuit breaker, a per-provider rate limiter, a response cache with an explicit TTL, and a **call meter tagged by tenant** (satellite and weather APIs are metered costs; you need per-tenant attribution before you need it).

Credentials: envelope encryption per tenant, same pattern as the KonnektCRM OAuth refresh-token storage.

### 7.4 Ingest paths

- **Weather:** scheduled pull per distinct field centroid, deduplicated to a ~5 km grid so 200 fields on one farm cost one call, not 200.
- **Satellite:** scheduled scan of acquisition availability per tenant AOI, then per-field statistical requests for new cloud-free scenes. Store the cloud/valid-pixel threshold with each reading so results are reproducible.
- **Sensors:** MQTT bridge or provider webhook → validation → hypertable insert via `COPY` in batches. Never one INSERT per reading.

---

## 8. API design

- REST, `/api/v1`, minimal APIs, endpoint groups mirroring Application feature folders, `TypedResults`, single `IExceptionHandler` emitting ProblemDetails. Same as KonnektCRM.
- Reuse `PagedResult<T>`, `QueryOptions<T>`, `ApplySearch/ApplyFilters/ApplySort`, `FilterCondition`, cached-reflection multi-field sort with mandatory PK tiebreaker.
- **Idempotency keys required on all POST/PUT** — the sync layer depends on it, and it costs nothing to make it universal.
- **Bulk endpoints** for import (`POST /fields:bulk`, `POST /animals:bulk`) returning a job id; onboarding a 3,000-head herd through a per-record API will time out.
- **Webhooks:** signed (HMAC-SHA256 over body + timestamp), retried with backoff, replayable from a delivery log, with per-subscription failure suspension.
- **Public API:** per-tenant keys with scoped permissions, separate rate-limit bucket, separate OpenAPI document from the internal surface.
- **Cursor pagination** for the sync and event-log endpoints (offset pagination breaks under concurrent inserts).

---

## 9. Frontend and mobile

### 9.1 Web (Angular 22)

- Standalone components, signals for local state, one facade service per module.
- Map layer: **MapLibre GL** with MVT from the API. Isolate all map interaction behind a `MapFacade` so the mapping library can be replaced.
- Server-driven query state: the existing colon-delimited sort/filter query-string encoding from KonnektCRM carries over directly.
- Known issue to design around up front (it bit KonnektCRM): stale UI state after writes. Use an explicit invalidation registry — each command declares which query keys it invalidates — rather than ad-hoc refetches.

### 9.2 Mobile

**Recommendation: .NET MAUI.** The decisive factor is that domain validation, unit conversions, the sync command contracts and the offline business rules are shared source with the backend. In a product where a wrong rate calculation is a legal problem, one implementation of that logic is worth more than React Native's larger ecosystem.

The counterargument is real: MAUI's map and camera stories are weaker and the talent pool is smaller. If you expect to hire mobile contractors, choose React Native and accept duplicated validation with a shared JSON-schema contract as the reconciliation mechanism.

Decide this in Phase 0. It is expensive to reverse.

Architecture either way: SQLite local store, command queue table, background sync service, pre-signed media upload, map tiles cached for the working set before leaving connectivity.

---

## 10. Security engineering

- **Auth:** two-phase login from KonnektCRM (authenticate → workspace list → exchange purpose-scoped token + tenant choice for access token). Extend claims with `OrganisationScope` node paths.
- **Authorisation:** resource-scoped policies. `[RequirePermission("operations.record", Resource = "farm")]` with an `IAuthorizationHandler` checking the org path against the principal's granted paths. Permission snapshot cached in Redis, invalidated on role change.
- **Cross-tenant grant enforcement** is the highest-severity control in the system. It gets: a dedicated integration test suite, a fuzz test that attempts grant-scope escape with mutated ids, and RLS as a backstop.
- **Secrets:** no credentials in appsettings; env-injected, envelope-encrypted per-tenant integration credentials with a rotatable KEK.
- **Audit:** `ForensicsInterceptor`-style field-change capture (already built for KonnektCRM deals) applied to compliance-relevant entities.
- **Media:** virus scanning on upload. Note the KonnektCRM constraint — ClamAV was dropped for memory reasons on a 2 GB droplet. Here, run scanning as a separate worker role on its own instance, or use a managed scanning service.
- **PII:** farm worker records and vet details are personal data. Field-level encryption for contact details, documented retention, export/erasure endpoints.

---

## 11. Testing strategy

| Layer | Approach | Gate |
|---|---|---|
| Domain | Pure unit tests, no mocks, no DB | 90% line coverage on domain projects |
| Architecture | NetArchTest: module boundaries, `IgnoreQueryFilters` allowlist, sync policy attributes, no `DateTime.Now` | Build fails |
| Application | Handler tests against in-memory fakes for ports | Every command and query has one |
| Integration | Testcontainers `postgis/postgis:17-3.5` + Timescale; real migrations; real EF | All repositories, all RLS policies |
| Multi-tenancy | Dedicated suite that asserts every tenant-scoped entity is unreachable from another tenant, generated by reflection over `IMustHaveTenant` | Build fails on a new entity without coverage |
| Sync | Property-based command-sequence tests (§5.6) | Build fails |
| Spatial | Golden-file tests for import of real Shapefile/ISOXML samples; area computation against known parcels | |
| Compliance | Each control point has a fixture pair (passing farm, failing farm) | Required per new standard |
| Load | k6 against a seeded tenant: 500 farms, 20k fields, 200k animals, 50M sensor rows | Before each phase release |

Seed a realistic dataset generator early. Every performance question you will have for the next three years needs it, and building it retroactively is miserable.

---

## 12. Observability and operations

- **OpenTelemetry** traces, metrics, logs. Every span tagged `tenant.id`, `module`, `org.node`.
- **Structured logging** (Serilog) with a `TenantId` enricher. Never log geometry payloads or full jsonb bodies.
- **Domain metrics that matter:** sync command backlog per device, sync rejection rate by command type, satellite/weather API calls and cost per tenant, compliance evaluation duration, outbox lag, hypertable chunk growth rate.
- **Alerts:** outbox lag > 5 min, sync rejection rate > 1%, any tenant's external API spend above threshold, RLS policy violation attempts, media reconciliation backlog.
- **Health checks:** DB, RabbitMQ, Redis, each external provider (degraded not unhealthy — the platform must work when Sentinel Hub is down).

---

## 13. Migrations and release engineering

- **EF Core migrations** with snake_case naming convention. Note the KonnektCRM failure mode: the `IDesignTimeDbContextFactory` must call the naming convention or migrations generate with the wrong casing.
- **Expand/contract** for every schema change. Add nullable → backfill in a worker → switch reads → make non-null → drop old. No downtime windows, because harvest season has no maintenance window.
- Raw SQL migrations for PostGIS/Timescale objects (hypertables, continuous aggregates, RLS policies, generated columns, exclusion constraints) that EF cannot express. Keep them in ordered `.sql` files applied by the migration runner.
- **Backfill jobs are first-class code**, tested, resumable, tenant-batched, not one-off scripts run from a laptop.
- Environments: local (`compose.dev.yaml`, infrastructure only, API from IDE), staging with anonymised production-shaped data, production.
- CI: build → architecture tests → unit → integration (Testcontainers) → migration dry-run against a staging snapshot → deploy.

---

## 14. Capacity and performance budgets

Target at 500 tenants / 100k fields / 500k animals:

| Operation | Budget |
|---|---|
| Field list page (50 rows, filtered) | < 200 ms p95 |
| Map tile (MVT, cached) | < 50 ms p95 |
| Map tile (cold) | < 400 ms p95 |
| Sync push (100 commands) | < 2 s p95 |
| Sync pull delta (500 rows) | < 1 s p95 |
| Traceability trace (depth 10) | < 3 s p95 |
| Compliance dashboard | < 500 ms (precomputed) |
| Season analytics | < 2 s (materialised) |

Anything requiring aggregation across seasons is precomputed by a worker. No analytics query touches the operational tables synchronously.

---

## 15. Engineering milestones

Exit criteria are engineering-verifiable, not commercial.

### M0 — Foundations (4-6 weeks)

Solution skeleton, module boundaries + architecture tests, tenancy (query filters + RLS + interceptor), org hierarchy with ltree, auth and resource-scoped policies, audit interceptor, outbox + worker host, PostGIS setup with NTS, units value objects, OTel, CI/CD, seed data generator.

**Also in M0, non-negotiable:** a throwaway end-to-end spike of the sync protocol — device queue, push, replay, gap handling — proving the design under property tests. Do not defer this to M1.

*Exit:* two tenants, isolation suite green, a field with a versioned boundary, one operation recorded offline and synced.

### M1 — Crop record and traceability core

Farm registry, boundary import pipeline (Shapefile/GeoJSON/KML), season and crop zone, field operations with inputs, mobile capture with photos and GPS, trace event log with projector, supply relationships with data grants + access log, compliance engine with the first standard's control points, audit pack export.

*Exit:* full offline week test passes; a trace query returns a complete chain; a grant-scope fuzz test finds no escape; load test at 20k fields within budget.

### M2 — Planning, inputs, intelligence

Crop planning and budgets, inventory and stock movements, product registry with rate/PHI rule enforcement, weather ingest with grid deduplication, satellite index ingest, deterministic degree-day and disease risk models, cost allocation and gross margin, accounting connector (one).

*Exit:* satellite and weather ingest running under metered cost per tenant; rate-violation rules blocking correctly; margin figures reconciling to accounting export.

### M3 — Livestock

Animal registry (individual and batch), groups with temporal membership, movements and holding register, statutory movement registry adapter, health and treatments with **withdrawal-period enforcement blocking dispatch**, breeding and performance, weights and DLWG, feed and rations, crop-to-feed inventory transfer carrying trace lineage.

*Exit:* withdrawal enforcement suite green including edge cases (retrospective treatment entry, treatment recorded offline after dispatch attempt); movement adapter passing the registry's certification tests; livestock events in the shared trace log.

### M4 — Scale and integration

Analytics star schema and nightly ETL, ISOXML/ADAPT task data exchange, MQTT/LoRaWAN sensor ingest with hypertable batching, ERP connector, public API and webhooks, dedicated-database tenancy mode, second region.

*Exit:* load test at 50M sensor rows; analytics refresh under an hour for the largest tenant; a tenant migrated to a dedicated database with no code change.

---

## 16. Engineering risks

| Risk | Impact | Mitigation |
|---|---|---|
| Sync layer underestimated | Ships late, corrupts data | M0 spike with property tests before any feature work depends on it |
| `IgnoreQueryFilters` misuse leaks tenant data | Critical | Analyser + CI allowlist + RLS backstop + reflection-driven isolation suite |
| Mobile framework choice reversed mid-build | Months lost | Decide in M0; prototype both for one screen if genuinely uncertain |
| Time-series volume outgrows Postgres | Query latency collapse | Hypertables + continuous aggregates + retention from day one; ClickHouse only on measured evidence |
| Boundary geometry corruption on import | Silent bad data, wrong areas | Validate/`ST_MakeValid`/sanity bounds on ingest; golden-file tests; preview-before-commit UX |
| Machinery telematics integrations consume a quarter | Roadmap derailment | File-based ADAPT exchange only until a signed customer funds a live link |
| External API cost runs away | Margin destruction | Per-tenant call metering and hard caps from first integration |
| Timezone and `occurred_at`/`recorded_at` confusion | Wrong compliance evidence | `timestamptz` everywhere, ban `DateTime.Now` in architecture tests, always store both timestamps |
| Compliance rules hard-coded | Every new standard is a release | Standards modelled as data with registered evaluators from M1 |

---

## 17. ADRs to write before M1

1. Modular monolith with schema-per-module vs separate databases
2. Tenancy: query filters + RLS, and the conditions that trigger dedicated-database mode
3. Sync protocol: intent queue vs state replication, and the conflict policy taxonomy
4. Mobile framework: MAUI vs React Native
5. Geometry storage: `geography` + generated `geometry` projection, and the boundary versioning model
6. Time-series storage: TimescaleDB in-database vs a separate store, with the migration trigger defined
7. Traceability: EPCIS 2.0 event model as the shared log
8. Compliance: data-modelled standards with registered evaluators
9. Analytics: materialised views → star schema → columnar, with the measured thresholds for each transition
