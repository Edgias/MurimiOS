# MurimiOS

MurimiOS is the foundation of a multi-tenant farm and livestock management SaaS. The current vertical slice provides tenant-isolated farm, field, and livestock records; PostGIS field boundaries; an offline command protocol; and an installable Angular progressive web app.

The broader product and delivery roadmap is documented in [docs/agri-platform-development-plan.md](docs/agri-platform-development-plan.md).

## Implemented foundation

- ASP.NET Core 10 API and EF Core 10 persistence
- PostgreSQL 17 with PostGIS, tenant-scoped foreign keys, and forced row-level security
- JWT bearer authentication, tenant memberships, least-privilege roles, explicit permission grants, optional organisation scope, and permission policies on every API route
- Materialised organisation hierarchy with PostgreSQL `ltree` subtree authorization enforced across farm-owned resources and synchronization
- Append-only actor-attributed audit events, scoped audit queries, and supersession-based corrections for field operations and livestock measurements
- Development-only header authentication gated by environment configuration; production rejects unauthenticated tenant headers
- Farm and field APIs, including immutable boundary versions and server-computed hectares
- Farm digital-twin APIs with farm/field tenure history, mapped and lifecycle-controlled infrastructure, exact-field-version soil-management zones, geolocated depth-aware samples, append-only laboratory results, GeoJSON overview layers, PostGIS containment and overlap guards, forced RLS, audit, sync and outbox records
- Growing seasons and crop zones, including crop/variety allocation, server-computed zone area, and containment checks
- Revisioned crop-plan scenarios with field and crop-zone allocation, yield and revenue targets, sequenced operations, canonical input/labour/machinery/contractor/irrigation budgets, jurisdictional product-use rules with rate, re-entry and pre-harvest enforcement, approval and supersession, rotation history, scenario comparison, and revision-aware planned-versus-actual variance
- Append-only finance ledger with reversible postings, fully balanced cost allocations, farm/field/crop-zone/livestock/overhead cost centres, direct labour and machinery costing, crop and herd unit economics, enterprise roll-ups, and provider-neutral accounting statement reconciliation
- Central farm operations with farm/field/crop-zone/livestock work orders, approved-plan materialization, planned/assigned/in-progress/completed/verified/rejected/corrected lifecycle, assignment and machinery conflict detection, canonical resource and stock-lot reservations, completion evidence links, approval controls, and a combined operational calendar
- Agricultural input catalogue, supplier stock lots, append-only receipts/issues, and concurrency-protected balances
- Offline input applications and harvest lots with stock-lot-to-operation-to-harvest trace projection
- Append-only crop trace events plus an authenticated GS1 EPCIS 2.0.1 JSON/JSON-LD repository: checksum-idempotent capture jobs, immutable raw event/extension retention, Object/Aggregation/Association/Transformation/Transaction event validation, CBV 2.0 business-step and disposition mappings, UN/CEFACT quantity codes, SimpleEventQuery filtering, exact EPCISQueryDocument exports, audit/outbox records, forced RLS and insert-only database grants. Internal storage-lot split/merge transformations retain enforced mass balance and declared loss, deliveries/receipts, forward/backward lineage, quality assessments, and recall impact analysis.
- Producer/buyer supply relationships, counterparty acceptance, versioned and revocable purpose-bound data grants, farm/season/commodity/attribute scoping, grant-aware harvest and trace projections, and append-only cross-tenant access auditing
- Supplier sourcing workflows with hash-only invitations and seven-day-or-shorter single-use public claim/detach actions, tenant-bound hosted-account links with immutable history, lifecycle contracts, effective fixed/volume/quality-grade price rules, server-resolved quotes, bounded commodity allocations and fulfilment, immutable reconciliation revisions with quantity/amount equations, evidence request/review cycles, versioned weighted supplier-risk snapshots, and supplier completion/status projections
- Versioned compliance schemes with immutable published control points, evaluator versions, typed evidence requirements, farm assessments, mandatory-control and evidence-freshness enforcement, weighted/critical-control scoring, dashboards, findings, corrective actions, independent verification, and database-immutable evidence snapshots. Completed assessments can be frozen into revisioned certification audit-pack ZIPs with explicit scope statements, exact standard/control/evaluation/findings content, optional re-hashed evidence binaries, per-entry and whole-artifact SHA-256 digests, retained manifests, HMAC-SHA256 signatures, immutable export auditing and server-side integrity verification.
- Offline JPEG/PNG/WebP evidence with optional GPS accuracy, IndexedDB blob queueing, resumable chunk upload, SHA-256 and file-signature verification
- Tenant-isolated crop-zone production and evidence PDF export with operations, applied input lots, harvest lots, GPS references, SHA-256 response metadata, export audit records, and a transactional integration event
- Individual-animal and livestock-group registration with namespaced identifiers, holdings, temporal group membership, lifecycle movements, health events, canonical feed batches/allocations, and a unified immutable history
- Livestock production records with biological profiles and pedigree, breeding/pregnancy/parturition/offspring/weaning lineage, withdrawal-aware milk yield and quality, grazing and per-head ration lifecycle plans, DLWG and 30-day weight projections, feed-conversion ratios, and farm reproductive/milk KPIs
- Veterinary medicine catalogue and batch stock, offline treatment capture, immutable meat/milk withdrawal dates, membership-aware group treatment propagation, sale/slaughter/milk eligibility checks, and permission-gated audited overrides
- Offline crop-zone operations and livestock measurements with device sequences, idempotent command IDs, gap detection, retained rejections, and change checkpoints
- Shared provider-ingestion infrastructure with tenant-scoped connection registrations, protected and rotatable credential references, database-coordinated scheduled polling, farm/field/device feeds, freshness and provider-health state, leased jobs, idempotency, checkpoints, raw-payload provenance and deduplication, normalized weather/satellite/regulatory records, provider-cost attempts, retry/dead-letter/replay controls, audit, sync and outbox events. Initial adapters cover Open-Meteo, Copernicus Data Space Sentinel Hub, and the European Commission pesticides active-substance dataset.
- Compliance readiness packs with immutable manifest digests and authoritative source links for GLOBALG.A.P. IFA v6 Smart, Codex HACCP CXC 1-1969 (2022), ISO 9001:2015 with Amendment 1:2024, and EUDR 2023/1115; tenant-local installation, automated farm-record evaluation, evidence snapshots, and pack-specific conformance policies are included. These operational mappings do not replace accredited certification or legal review.
- Real-time field intelligence with deduplicated field weather locations, canonical observations and forecasts, boundary-versioned NDVI/NDRE/NDMI satellite statistics, geolocated scouting, immutable GeoJSON map projections, versioned deterministic weather/satellite/pest/disease risk rules, reproducible assessments, alert acknowledgement/resolution/dismissal, feed-freshness overviews, and alert-created follow-up work orders
- Connected-agriculture backend with continuously reconnecting MQTT 5 sensor/machinery subscriptions, The Things Stack LoRaWAN MQTT 3.1.1 ingestion, canonical HTTPS machinery/ERP/accounting connectors, farm equipment and device registries, typed sensor channels, immutable range-partitioned readings and machinery telemetry, automatic current/future yearly partitions, freshness projections, ISOXML/ADAPT/task-file exchange lifecycles, canonical accounting/ERP business-document exchange, and lease-based retention of explicitly registered derived map/media products with legal holds and lifecycle audit runs. Evidence, imports, audit packs and statutory objects are excluded from derived-object deletion. See [connected-provider configuration](docs/connected-provider-configuration.md).
- Analytics warehouse core with revisioned scheduled and event-driven farm projections, leased/retriable background processing, immutable lineage- and quality-scored plan/actual/forecast facts, content-addressed historical dimension snapshots, current-revision metric rollups, forecast accuracy history, governed specialist-approved benchmark cohorts and predictive-model definitions, risk-adjusted crop-yield forecasts, raw sensor/telemetry daily rollups and retention policies, farm and organisation dashboards, crop/variety comparisons, and livestock/finance/compliance/supplier metrics
- Tenant-scoped public API clients with one-time SHA-256 credentials, versioned rotation, least-privilege farm/data scopes, public farm/harvest/trace/analytics resources, and immutable access logs
- Signed outbound webhooks fanned out transactionally from the outbox, with HTTPS-only subscriptions, HMAC-SHA256 signatures, independent leases/retries, response logs, dead-lettering and replay
- Retail transparency with versioned disclosure policies, opaque expiring/revocable tokens, public-safe origin/crop/quality/compliance/lineage presentation, downstream-grant enforcement and privacy-preserving access logs
- Enterprise controls with reusable organisation-scoped approval policies, multi-approver quorum and immutable decisions, first-class effective-dated legal companies, explicitly paired intercompany ledger references, specialist-approved versioned elimination rules, immutable consolidation runs with source digests and imbalance exceptions, elimination-aware profitability, control-plane tenant placement, configured per-tenant database routing, write-guarded leased migration execution, authenticated encrypted export artifacts, schema/digest/count validation, transactional import, and atomic route cutover. See [tenant database routing and migration](docs/tenant-database-routing-and-migration.md).
- Commercial SaaS control plane with provision/activate/suspend/close tenant lifecycle, immutable versioned plan snapshots, feature and hard/soft usage entitlements, trials and recurring subscription periods, idempotent metering with period counters, billing customer and invoice lifecycles, forced RLS, and request-time suspension/subscription enforcement. Commercial mutations additionally require the production identity's `platform_operator=true` claim.
- Identity-security operations with database-coordinated per-tenant and public-client rate windows/daily quotas, credential maximum-age and required-expiry policies, hash-only device/session inventory with revocation, append-only login/security events, single-use recovery cases with attempt locking and session invalidation, and production startup validation for HTTPS identity-provider metadata and MFA-on-write configuration.
- Tenant-scoped geospatial onboarding jobs for GeoJSON, KML, raw/zipped Polygon Shapefile and ISOXML, with retained SHA-256 source artifacts, parser versioning, persisted PostGIS previews, validation/overlap issues, and explicit transactional field creation or boundary-version replacement
- Resumable tenant-scoped CSV master-data onboarding for farms, fields, livestock, inventory lots, suppliers, opening balances and finance transactions, with retained source artifacts, checksum idempotency, typed templates, immutable staged rows, validation previews, downloadable error files, committed checkpoints, balanced finance allocations, audit/sync/outbox records and matching CSV exports
- Transactional outbox dispatcher with PostgreSQL `SKIP LOCKED` leases, exponential retry, dead-letter state, and broker-confirmed persistent RabbitMQ events
- Independently deployable .NET worker with graceful shutdown, integration-event publishing, and signed webhook delivery
- S3-compatible durable evidence storage with MinIO development infrastructure, tenant-prefixed object keys, resumable local staging, SHA-256/signature validation, and fail-closed ClamAV scanning outside Development
- Personal in-app notification inboxes, mark-read state, per-category channel preferences, and transactional `Notification.Requested` integration events
- OpenTelemetry traces, metrics, and logs with OTLP export; tenant context is carried in trace/log scopes without creating tenant-labelled metric cardinality
- Public liveness and dependency-readiness probes covering PostgreSQL, outbox lag/dead letters, RabbitMQ, media storage, and ClamAV when enabled
- Additive deterministic seed generation for realistic multi-tenant organisation, farm, PostGIS field, crop-zone, livestock, and notification data
- Tenant-scoped country, currency, locale, timezone, crop-year, and preferred-unit settings; typed money/business-time primitives; canonical measurement conversion with entered/display-unit preservation; and versioned effective-dated reference catalogues
- Angular 22 PWA with service-worker caching and IndexedDB command storage
- Unit coverage for tenant isolation, geospatial records, livestock subject types, sync outcomes, report validation, and PDF composition

## Local development

Prerequisites: .NET SDK 10, Node.js/npm, and Docker Desktop.

Start PostgreSQL/PostGIS and RabbitMQ, then apply the schema:

```powershell
docker compose -f compose.dev.yaml up -d
dotnet tool restore
dotnet ef database update --project src/Infrastructure --startup-project src/API --context PlatformDbContext
```

Run the API:

```powershell
dotnet run --project src/API --urls http://localhost:5000
```

Swagger is available at `http://localhost:5000/swagger`. Production authentication is configured through an absolute HTTPS `Authentication:Authority`, a non-empty `Authentication:Audience`, and `Authentication:RequireHttpsMetadata=true`; startup fails closed when these are missing. Write requests require an accepted MFA method in the configured `Authentication:Security:MfaClaimType` claim. Bearer identities require an active membership in the selected tenant. SaaS plan, subscription, account-lifecycle and billing mutations also require a trusted `platform_operator=true` claim. In the Development environment only, `Authentication:AllowDevelopmentTenantHeader` enables the local bootstrap identity used by the PWA with `X-Tenant-Id`; `Authentication:DevelopmentPlatformOperator` controls whether that bootstrap can exercise operator routes. These paths are disabled by default in base configuration and are not production trust boundaries.

`RequestQuotas` supplies fallback tenant and public-client window/daily limits. Tenant administrators can create narrower or wider active policies through `/api/v1/security/rate-limit-policies`; public API credentials can be governed by `/api/v1/security/credential-rotation-policies`. Counters are coordinated in PostgreSQL, so limits remain consistent across horizontally scaled API instances. Recovery verification does not mutate an external provider password: it verifies a single-use MurimiOS recovery case, revokes active local sessions, appends security evidence, and emits `Identity.RecoveryVerified` for the selected identity-provider adapter to complete the provider-owned recovery action.

Compliance audit-pack signing is configured under `ComplianceAuditPacks`. `SigningKeyBase64` must decode to at least 32 bytes, `SigningKeyId` identifies the active key in every detached signature, and `VerificationKeys` retains prior key IDs during rotation so historical exports remain verifiable. Production startup fails if the active signing configuration is absent or invalid. Signing keys belong in the deployment secret store, not committed configuration; the Development value is local-only.

Run the integration-event worker in another terminal:

```powershell
dotnet run --project src/Worker
```

The development RabbitMQ broker listens on AMQP port `5673` and its management UI is at `http://localhost:15672` (`murimios` / `murimios-dev`). MinIO exposes its S3-compatible API at `http://localhost:59300` (`murimios` / `murimios-dev-secret`); its administration console remains container-local by default. The worker publishes persistent JSON messages to the durable `murimios.events` topic exchange and marks an outbox row complete only after broker confirmation. Consumers must be idempotent on the AMQP `message_id`, because the outbox provides at-least-once delivery. For a database-only diagnostic run, override the publisher with `--Messaging:Publisher=Logging`.

Retry and leasing behavior is configured under `Outbox`: workers claim rows with expiring leases, use exponential backoff after failures, and set `dead_lettered_at` after `MaximumAttempts`. Dead-letter rows remain in PostgreSQL for operator review and controlled replay; they are never deleted automatically.

The EF design-time connection uses the `murimios` migration owner. The running API and worker use the non-superuser `murimios_app` role so forced PostgreSQL row-level-security policies remain effective. The worker can cross tenant boundaries only through lock-token-protected, security-definer outbox functions. Supply-network reads use dedicated security-definer projections that independently validate the recipient, active relationship, grant status, validity window, category, farm, season, and commodity scope; the application never switches into the producer tenant. The compose initialization script creates this role and its schema grants for a new development volume.

Development defaults to local media below `src/API/App_Data/media`, which is excluded from Git. Set `Storage:Provider=S3` and configure `Storage:S3` to use MinIO, AWS S3, or another compatible endpoint. S3 uploads use local resumable staging before validated objects are committed under tenant-prefixed keys; horizontally scaled deployments should place staging on shared durable storage or add direct multipart upload coordination. `MediaScanning:Provider=Development` is accepted only in the Development environment and logs an explicit bypass warning. Other environments use the ClamAV adapter and reject completion when the scanner is unavailable. Production still needs provider-specific retention, versioning, encryption, and lifecycle policies.

The worker's `ConnectedDataLifecycle` settings control the future partition horizon, partition check interval, deletion batch/lease sizes and retry ceiling. Tenant analytics retention policies independently set raw sensor, raw telemetry, failed projection, derived-media and expired-map-layer windows. Physical deletion is restricted to object keys created in that tenant's `derived` namespace; legal holds cancel unclaimed deletion and block future queueing.

Generate realistic deterministic data after migrations have been applied:

```powershell
dotnet run --project tools/SeedData
```

The default dataset contains two tenants, regional settings, crop and livestock-species catalogues, two farms per tenant, mapped PostGIS fields and active crop zones, individual cattle and livestock groups, owner memberships, and welcome notifications. The command is additive and idempotent; rerunning it does not duplicate or delete data. Use `--help` for scale and connection options, or set `MURIMIOS_SEED_DB` for a non-default database.

Operational quantities are stored in canonical units (`kg`, `ha`, `L`, `m`, `C`, or the applicable count/time base) while preserving the submitted value/unit and preferred display unit. Conversion is server-owned and uses the unit catalogue exposed by the API. Money values require an explicit three-letter currency; tenant settings supply presentation defaults but never silently convert currencies. Business-time primitives retain separate UTC occurrence and recording instants and convert local input through the configured IANA timezone.

Operational probes are public so orchestrators can use them without application credentials:

- `GET /health/live` checks the API process only.
- `GET /health/ready` and `GET /health` check PostgreSQL, outbox lag/dead letters, RabbitMQ, media storage, and the configured malware scanner.

Readiness becomes degraded when undelivered outbox work is older than five minutes or dead letters exist. Set `OpenTelemetry:OtlpEndpoint` to export application/worker traces, runtime and HTTP metrics, custom outbox/media metrics, and structured logs to an OTLP collector.

Run the PWA in another terminal:

```powershell
cd apps/web
npm install
npm start
```

Open `http://localhost:4200`. The PWA uses the API at `http://localhost:5000` by default and stores its generated device ID, monotonically increasing command sequence, pending commands, rejected commands, and pull checkpoint in IndexedDB.

## Verification

```powershell
dotnet build MurimiOS.sln
dotnet test MurimiOS.sln --no-build
cd apps/web
npm run build
```

Database-backed API, OpenAPI contract, PostgreSQL RLS, security and analytics end-to-end tests are opt-in so the default unit-test command does not require infrastructure. After applying migrations to the local development database, run:

```powershell
$env:MURIMIOS_RUN_INTEGRATION_TESTS = "1"
dotnet test tests/API.FunctionalTests --filter "FullyQualifiedName!~ApiLoadTests"

$env:MURIMIOS_RUN_LOAD_TESTS = "1"
dotnet test tests/API.FunctionalTests --filter "FullyQualifiedName~ApiLoadTests"
```

Set `MURIMIOS_TEST_CONNECTION` to use a dedicated migrated test database. The bounded load smoke issues 200 authenticated requests at 20-way concurrency and enforces a 2-second p95 ceiling.

## API surface

All current application routes are under `/api/v1`:

- `GET|POST /farms`
- `GET|POST /organisation/nodes`
- `GET|POST /farms/{farmId}/fields`
- `GET|POST /farms/{farmId}/seasons`
- `GET|POST /seasons/{seasonId}/crop-zones`
- `GET /crop-zones/{cropZoneId}/operations`
- `GET|POST /crop-planning/farms/{farmId}/plans`
- `GET /crop-planning/plans/{planId}`
- `PUT /crop-planning/plans/{planId}/draft`
- `POST /crop-planning/plans/{planId}/submit|approve|reject|revisions`
- `GET /crop-planning/plans/{planId}/variance`
- `POST /crop-planning/compare`
- `GET /crop-planning/fields/{fieldId}/rotation`
- `GET|POST /crop-planning/inputs/{inputId}/application-rules`
- `GET|POST /crop-planning/plans/{planId}/financial-entries`
- `GET|POST /finance/farms/{farmId}/cost-centres`
- `POST /finance/cost-centres/{costCentreId}/close`
- `GET|POST /finance/farms/{farmId}/transactions`
- `POST /finance/transactions/{transactionId}/reverse`
- `POST /finance/farms/{farmId}/labour-entries`
- `POST /finance/farms/{farmId}/machinery-entries`
- `GET /finance/farms/{farmId}/profitability`
- `GET /finance/organisation/{organisationNodeId}/profitability`
- `POST /finance/farms/{farmId}/accounting-imports`
- `GET /finance/accounting-imports/{batchId}`
- `POST /finance/accounting-imports/{batchId}/reconcile`
- `GET|POST /operations/work-orders` (creation is farm-scoped at `/operations/farms/{farmId}/work-orders`)
- `GET|PUT /operations/work-orders/{workOrderId}` (draft update uses `/draft`)
- `POST /operations/work-orders/{workOrderId}/plan|start|complete|verify|reject|reopen|cancel|corrections`
- `POST|DELETE /operations/work-orders/{workOrderId}/assignments`
- `POST /operations/work-orders/{workOrderId}/resources`
- `POST /operations/plans/{planId}/materialize`
- `GET /operations/calendar`
- `GET /operations/conflicts`
- `GET|POST /farms/{farmId}/inputs`
- `GET|POST /inputs/{inputId}/stock-lots`
- `GET /stock-lots/{stockLotId}/trace`
- `GET /crop-zones/{cropZoneId}/harvest-lots`
- `GET /traceability/farms/{farmId}/events`
- `POST /traceability/transformations`
- `GET /traceability/objects/{objectType}/{objectId}/lineage`
- `GET /traceability/objects/{objectType}/{objectId}/mass-balance`
- `GET|POST /traceability/.../quality-assessments`
- `POST /epcis/capture?farmId={farmId}`
- `GET /epcis/capture?farmId={farmId}` and `GET /epcis/capture/{captureId}`
- `GET|OPTIONS /epcis/events?farmId={farmId}` (EPCIS SimpleEventQuery subset)
- `GET /epcis/cbv/mappings`
- `GET|POST /deliveries`
- `POST /deliveries/{deliveryId}/receive`
- `GET|POST /recalls`
- `GET /recalls/{recallId}/impact`
- `POST /recalls/{recallId}/close`
- `GET|POST /supply-network/relationships`
- `POST /supply-network/relationships/{relationshipId}/accept|reject|revoke`
- `GET|POST /supply-network/grants`
- `POST /supply-network/grants/{grantId}/revoke`
- `GET /supply-network/shared/harvest-lots?grantId={grantId}&purpose={purpose}`
- `GET /supply-network/shared/trace/{grantId}/{harvestLotId}?purpose={purpose}`
- `GET /supply-network/access-audit`
- `GET|POST /supplier-portal/invitations`
- `POST /supplier-portal/invitations/accept`
- `GET|POST /supplier-portal/suppliers`
- `GET /supplier-portal/suppliers/{supplierId}/status`
- `GET /supplier-portal/suppliers/{supplierId}/link-history`
- `POST /supplier-portal/suppliers/{supplierId}/self-service-actions`
- `POST /supplier-portal/self-service-actions/{tokenId}/revoke`
- `GET|POST /supplier-portal/risk-assessments`
- `GET|POST /supplier-portal/contracts`
- `POST /supplier-portal/contracts/{contractId}/activate|complete|terminate`
- `POST /supplier-portal/contracts/{contractId}/allocations`
- `GET|POST /supplier-portal/contracts/{contractId}/price-rules`
- `GET /supplier-portal/contracts/{contractId}/price-quote`
- `GET|POST /supplier-portal/contracts/{contractId}/reconciliations`
- `GET|POST /supplier-portal/evidence-requests`
- `POST /supplier-portal/evidence-requests/{requestId}/submit|accept|reject|cancel`
- `GET|POST /compliance/standards`
- `GET /compliance/standards/{standardId}`
- `POST /compliance/standards/{standardId}/controls`
- `POST /compliance/standards/{standardId}/publish|retire`
- `GET|POST /farms/{farmId}/compliance/assessments`
- `PUT /farms/{farmId}/compliance/assessments/{assessmentId}/controls/{controlId}`
- `POST /farms/{farmId}/compliance/assessments/{assessmentId}/start|submit|complete|cancel`
- `GET|POST /farms/{farmId}/compliance/assessments/{assessmentId}/audit-packs`
- `GET /farms/{farmId}/compliance/audit-packs/{exportId}`
- `GET /farms/{farmId}/compliance/audit-packs/{exportId}/manifest|verify|download`
- `GET /farms/{farmId}/compliance/dashboard`
- `GET /farms/{farmId}/compliance/findings`
- `POST /compliance/findings/{findingId}/actions`
- `POST /compliance/findings/{findingId}/actions/{actionId}/start|submit|verify|reject|cancel`
- `POST /media/uploads`
- `PUT /media/uploads/{uploadId}` with `Upload-Offset`
- `POST /media/uploads/{uploadId}/complete`
- `GET /crop-zones/{cropZoneId}/evidence`
- `GET /evidence/{evidenceId}/content`
- `GET /reports/crop-zones/{cropZoneId}/production.pdf`
- `GET|POST /livestock/subjects`
- `GET|POST /farms/{farmId}/livestock/holdings`
- `GET|POST /livestock/subjects/{subjectId}/identifiers`
- `GET|POST /livestock/groups/{groupId}/memberships`
- `GET|POST /livestock/subjects/{subjectId}/movements`
- `GET|POST /livestock/subjects/{subjectId}/health-events`
- `GET|POST /farms/{farmId}/livestock/feed-batches`
- `GET|POST /livestock/subjects/{subjectId}/feed-allocations`
- `GET /livestock/subjects/{subjectId}/history`
- `GET|POST /livestock/subjects/{subjectId}/biological-profile`
- `GET|POST /livestock/subjects/{subjectId}/reproduction-events`
- `GET|POST /livestock/subjects/{subjectId}/milk-records`
- `GET|POST /farms/{farmId}/livestock/grazing-plans`
- `POST /farms/{farmId}/livestock/grazing-plans/{planId}/activate|complete|cancel`
- `GET|POST /livestock/subjects/{subjectId}/ration-plans`
- `POST /livestock/ration-plans/{planId}/activate|retire`
- `GET /livestock/subjects/{subjectId}/performance`
- `GET /farms/{farmId}/livestock/performance`
- `GET /access/me`
- `GET|POST /access/memberships`
- `PUT /access/memberships/{membershipId}`
- `GET /audit/events`
- `GET /corrections`
- `POST /field-operations/{operationId}/corrections`
- `POST /livestock/measurements/{measurementId}/corrections`
- `GET|POST /farms/{farmId}/medicines`
- `GET|POST /medicines/{medicineId}/stock-lots`
- `GET /livestock/subjects/{subjectId}/treatments`
- `GET /livestock/subjects/{subjectId}/dispatch-eligibility`
- `POST /livestock/dispatches`
- `GET|PUT /settings/regional`
- `GET /reference-data/units`
- `POST /reference-data/units/convert`
- `GET|POST /reference-catalogues`
- `GET|POST /reference-catalogues/{catalogueId}/items`
- `GET|POST /notifications`
- `POST /notifications/{notificationId}/read`
- `GET /notification-preferences`
- `PUT /notification-preferences/{category}`
- `POST /sync/push`
- `GET /sync/pull?after={checkpoint}`
- `GET|POST /integrations/connections`
- `GET|PUT /integrations/connections/{connectionId}`
- `POST /integrations/connections/{connectionId}/credentials`
- `POST /integrations/connections/{connectionId}/credentials/{credentialId}/revoke`
- `POST /integrations/connections/{connectionId}/activate|pause|disable`
- `GET /integrations/feeds`
- `POST /integrations/farms/{farmId}/feeds`
- `POST /integrations/feeds/{feedId}/pause|resume|disable`
- `GET|POST /integrations/feeds/{feedId}/jobs`
- `GET /integrations/jobs/{jobId}`
- `POST /integrations/jobs/{jobId}/claim|records|complete|fail|replay|cancel`
- `GET /integrations/records/{recordId}`
- `POST /integrations/records/{recordId}/processed|reject`
- `GET /field-intelligence/farms/{farmId}/overview`
- `GET /field-intelligence/fields/{fieldId}/weather|satellite|scouting|map-layers`
- `POST /field-intelligence/fields/{fieldId}/scouting`
- `POST /field-intelligence/ingestion/records/{recordId}/weather-observations|weather-forecasts|satellite-statistics`
- `GET|POST /field-intelligence/risk-rules`
- `POST /field-intelligence/risk-rules/{ruleId}/activate|retire`
- `POST /field-intelligence/fields/{fieldId}/evaluate-risks`
- `GET /field-intelligence/fields/{fieldId}/risk-assessments`
- `GET /field-intelligence/alerts`
- `POST /field-intelligence/alerts/{alertId}/acknowledge|resolve|dismiss|work-orders`
- `GET|POST /imports/geospatial/farms/{farmId}/jobs`
- `GET /imports/geospatial/jobs/{jobId}`
- `GET /imports/geospatial/jobs/{jobId}/preview`
- `POST /imports/geospatial/jobs/{jobId}/apply`
- `GET|POST /connected/farms/{farmId}/equipment`
- `POST /connected/equipment/{equipmentId}/maintenance|activate|retire`
- `GET|POST /connected/farms/{farmId}/devices`
- `POST /connected/devices/{deviceId}/channels`
- `POST /connected/ingestion/records/{recordId}/sensor-readings|machinery-telemetry`
- `GET /connected/farms/{farmId}/sensor-readings|telemetry|freshness`
- `GET|POST /connected/farms/{farmId}/task-files|business-documents`
- `POST /connected/task-files/{taskFileId}/validate|complete|reject`
- `POST /connected/business-documents/{documentId}/validate|complete|reject`
- `GET|POST /connected/farms/{farmId}/derived-media`
- `POST /connected/derived-media/{assetId}/hold|release`
- `GET|POST /analytics/kpis`
- `POST /analytics/kpis/{kpiId}/activate|retire`
- `GET|POST /analytics/farms/{farmId}/projections`
- `POST /analytics/projections/{runId}/cancel`
- `GET /analytics/schedules`
- `POST /analytics/farms/{farmId}/schedules`
- `POST /analytics/schedules/{scheduleId}/pause|resume`
- `GET|POST /analytics/retention-policies`
- `POST /analytics/retention-policies/{policyId}/disable`
- `GET /analytics/farms/{farmId}/facts|dashboard`
- `GET /analytics/organisation/{organisationNodeId}/dashboard`
- `GET /analytics/comparisons/crops`
- `GET|POST /analytics/benchmarks/enrollments`
- `GET /analytics/benchmarks/{metricCode}`
- `GET|POST /analytics/benchmark-cohorts`
- `POST /analytics/benchmark-cohorts/{id}/approve|retire`
- `GET|POST /analytics/predictive-models`
- `POST /analytics/predictive-models/{id}/approve|retire`
- `GET /analytics/refresh-requests`
- `GET /analytics/farms/{farmId}/dimensions|forecasts|forecast-accuracy`
- `GET|POST /public-api/clients`
- `POST /public-api/clients/{clientId}/activate|suspend|revoke|credentials`
- `GET|POST /public-api/webhooks`
- `POST /public-api/webhooks/{subscriptionId}/activate|suspend|disable|rotate-secret`
- `GET /public-api/webhooks/{subscriptionId}/deliveries`
- `POST /public-api/webhook-deliveries/{deliveryId}/replay`
- `GET|POST /retail-transparency/policies`
- `POST /retail-transparency/policies/{policyId}/publish|retire|tokens`
- `POST /retail-transparency/tokens/{tokenId}/revoke`
- `GET|POST /enterprise/approval-policies`
- `GET|POST /enterprise/approval-requests`
- `POST /enterprise/approval-requests/{requestId}/decisions|cancel|expire`
- `GET|POST /enterprise/legal-companies`
- `POST /enterprise/legal-companies/{id}/close`
- `GET|POST /enterprise/elimination-rules`
- `POST /enterprise/elimination-rules/{id}/approve|retire`
- `POST /enterprise/organisations/{organisationNodeId}/consolidations`
- `GET /enterprise/consolidations/{id}`
- `GET|POST /enterprise/placements`
- `GET|POST /enterprise/migration-jobs`
- `POST /enterprise/migration-jobs/{jobId}/cancel|fail` (export, validation, import and completion are worker-owned transitions)
- `GET|POST /saas/account`
- `POST /saas/account/activate|resume|suspend|close`
- `GET|POST /saas/plans`
- `POST /saas/plans/{planId}/entitlements|publish|retire`
- `GET|POST /saas/subscriptions`
- `POST /saas/subscriptions/{subscriptionId}/activate|past-due|suspend|renew|schedule-cancellation|cancel`
- `GET /saas/entitlements`
- `GET|POST /saas/usage`
- `GET|POST /saas/billing/accounts|invoices`
- `POST /saas/billing/accounts/{accountId}/activate|suspend|close`
- `POST /saas/billing/invoices/{invoiceId}/paid|void|uncollectible`

Anonymous integration and consumer routes are under `/api/public/v1` and authenticate internally rather than through a tenant user session:

- `GET /farms` with `farms.read`
- `GET /harvest-lots` with `harvest.read`
- `GET /harvest-lots/{harvestLotId}/trace` with `trace.read`
- `GET /analytics/facts` with `analytics.read`
- `GET /transparency/{opaqueToken}`
- `GET /supplier-accounts/actions/{token}` and `POST /supplier-accounts/actions/{token}/consume`

The original prototype endpoints and schema are retained in the repository but disabled by default through `Features:LegacyEndpointsEnabled`.

## Architecture boundaries

See the [clean vertical-slice refactor report](docs/clean-vertical-slices-report.md)
for project ownership, compatibility decisions and verification. The five test
projects cover Domain, Application, Infrastructure, API and architecture rules.

`src/Domain` groups the domain model by aggregate. `src/Application` owns Dispatcher use cases and their contracts. `src/Infrastructure` groups persistence, identity, storage, integrations and background processing by concern. `src/API/Endpoints` contains one thin HTTP endpoint per use case; `src/Worker` hosts background jobs. Original migration files remain untouched under `Infrastructure/Platform/Migrations` and are linked into the Infrastructure project. `apps/web` is the offline-first Angular client and is outside this backend refactor. Every tenant-owned table carries `tenant_id`; application filters, composite relationships, and PostgreSQL row-level-security policies provide independent layers of tenant isolation.
