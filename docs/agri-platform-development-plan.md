# MurimiOS Multi-Tenant Farm & Livestock SaaS Development Plan

> 18 October 2026 checkpoint: retain this full roadmap and the original requirements. The [corrected gap audit](backend-gap-audit.md) and [deliverable index](deliverables/status.json) qualify the older broad completion annotations below. In particular, dedicated-database worker coverage, real task interchange, practice comparisons, full rotation planning, production infrastructure and deferred PWA acceptance remain open. Start with [the new-machine handoff](../NEW_MACHINE_HANDOFF.md).

> Implementation status, 2 September 2026: the backend foundation now includes JWT authentication configuration, tenant memberships, least-privilege roles and permission policies, explicit grants, optional organisation scope, RLS-protected access records, farm/field registry, tenant-partitioned offline sync, livestock individual/group subjects, namespaced identifiers, holdings, temporal memberships, lifecycle movements, measurements, health events, feed batches and allocations, veterinary medicine batches, offline treatments with immutable membership-aware withdrawal holds, dispatch controls and audited overrides, seasons and crop zones, input stock lots and movements, offline crop-zone operations/input applications/harvest lots, append-only crop transformations, deliveries, lineage, quality and recall impact, an immutable GS1 EPCIS 2.0.1 capture/query repository with CBV 2.0 mappings, producer/buyer supply relationships, versioned purpose-bound data grants, grant-aware harvest/trace projections and append-only cross-tenant access auditing, supplier invitations/hosted accounts/contracts/allocations/evidence review, versioned compliance standards/control points/evidence requirements, scored farm assessments, dashboards, findings and verified corrective actions, resumable photo/GPS evidence capture, an audited production/evidence PDF export, durable outbox delivery, notifications, observability, tenant regional settings, canonical unit conversion with entered/display-unit fidelity, money/time primitives, versioned tenant reference catalogues, Advanced Crop Planning with revisioned scenarios and planned-versus-actual variance, finance/profitability with balanced allocations, operational costing, enterprise consolidation and accounting reconciliation, Central Farm Operations with crop-plan materialization, resource reservations, assignments, schedule conflicts, approval/verification, immutable correction history, operational calendars and offline start/completion commands, shared provider ingestion, a range-partitioned connected-agriculture data plane, lineage- and quality-aware analytics projections with private benchmarking, resumable CSV master-data onboarding, and an RLS-isolated commercial SaaS control plane with tenant lifecycle, plan entitlements, trials/subscriptions, usage limits and billing records. Client feature expansion is intentionally paused while the remaining backend roadmap is delivered.

## Backend implementation annotations and remaining priority

This matrix supersedes older capability-state summaries. Priorities apply to backend work only; PWA workflow and device UX remain deliberately deferred.

| Product area | Verified backend state | Remaining backend annotation | Priority |
|---|---|---|---|
| Central farm operations | **Implemented.** Work-order inbox, approved-plan materialization, assignments, schedule/resource conflicts, reservations, lifecycle approvals, correction lineage, calendar, offline start/completion, audit, sync and outbox are present. | No feature build remains in this row; continue hardening through load and physical-device acceptance later. | Complete |
| Farm digital twin | **Backend core implemented.** Existing organisation/farm/site hierarchy, fields and immutable field-boundary versions are joined by temporal farm- or field-scoped land-tenure records, mapped site infrastructure with terminal decommission history, versioned non-overlapping soil-management zones tied to exact field-boundary versions, geolocated/depth-aware soil samples, and append-only laboratory analyte results. A farm overview supplies operational counts and GeoJSON asset/sample layers. Database triggers enforce tenure period exclusion, farm/field consistency and exact PostGIS containment; all six new record sets use forced RLS, least-privilege grants, audit, sync and outbox integration. | Launch deployments may extend infrastructure asset taxonomies and laboratory exchange adapters, but the plan's first-class tenure, infrastructure, soil-zone and soil-sample backend records are complete. | **Core complete** |
| Advanced crop planning | **Implemented.** Rotations, revisioned scenarios, allocations, operation/resource forecasts, budgets, approval, product-use constraints, financial entries and planned-versus-actual variance are present. | No major feature build remains; provider-derived recommendations belong to field intelligence. | Complete |
| Finance and profitability | **Implemented.** Balanced allocations, labour/machinery costing, crop-zone/herd/farm/organisation profitability and accounting reconciliation are present. | A funded accounting connector belongs to external integrations; core finance is complete. | Complete |
| Traceability and EPCIS interoperability | **Backend interoperability core implemented.** Internal crop/lot/delivery/feed trace events, transformations, lineage, mass balance and recalls are joined by an authenticated EPCIS 2.0.1 JSON/JSON-LD repository. Capture is checksum-idempotent; raw events and custom extensions are immutable; Object, Aggregation, Association, Transformation and Transaction events receive structural validation; internal events serialize with CBV 2.0 mappings and UN/CEFACT quantity codes; SimpleEventQuery filters emit retained EPCISQueryDocument exports. Exchange/event rows use audit/outbox records, forced RLS, database immutability and insert-only grants. | Run the official GS1 conformance suite and add trading-partner identifier/master-data mappings for each production interchange before claiming partner certification. | **Interoperability core complete / partner conformance later** |
| Real-time field intelligence | **Implemented backend and initial providers.** Deduplicated field weather locations, canonical observations/forecasts, boundary-versioned satellite index statistics, geolocated scouting, immutable GeoJSON map layers, versioned deterministic weather/satellite/pest/disease risk rules, reproducible assessments, alert lifecycle, follow-up work orders and feed-freshness APIs are present. Scheduled Open-Meteo weather and Copernicus Data Space Sentinel Hub statistics adapters now retain raw provenance, exact boundary version, observation/receipt time, quality and checkpoints. | Expert-approved jurisdiction/crop rule fixtures and any additional commercial providers remain launch-market work. | **Initial vertical complete** |
| External integrations | **Provider and public integration foundations implemented.** Transactional outbox/worker, protected credential versions, connection/feed registrations, database-coordinated scheduled polling, leased jobs, checkpoints, immutable raw provenance, normalized records, rate/cost/error state and retry/dead-letter controls are joined by scoped public APIs and signed webhooks. Concrete adapters exist for Open-Meteo, Copernicus Sentinel Hub and the European Commission pesticides active-substance dataset. | Add named machinery, sensor, ERP/accounting and launch-market regulatory providers through the same adapter boundary. | **Initial provider set complete** |
| Imports and onboarding | **Geospatial and master-data onboarding backend implemented.** Geospatial jobs retain source artifacts, SHA-256 and parser versions; GeoJSON, KML, zipped/raw Polygon Shapefile and ISOXML are parsed into persisted PostGIS previews with per-feature validation and overlap issues before controlled boundary application. CSV jobs now provide typed templates and matching exports for farms, fields, livestock, inventory lots, suppliers, opening balances and finance transactions; sources and immutable normalized rows are staged before apply, checksum retries are idempotent, validation and error files are durable, and committed batch checkpoints support safe resumption. Finance imports create fully balanced allocations through an audited farm-level currency clearing centre. Both paths use forced RLS, least-privilege grants, audit, sync and outbox records. | Provider-specific ISOXML task/machinery interpretation and deployment-specific legacy-data transforms remain launch integration work. | **Core complete** |
| Livestock expansion | **Backend and initial launch integrations implemented.** Registry, groups, identifiers, holdings, movements, measurements, health, feed, treatments and withdrawal enforcement are joined by pedigree/reproduction, withdrawal-aware milk, grazing/rations, unified history and performance KPIs. Immutable effective-dated regulatory profiles now drive Scotland cattle movement deadlines and downloadable movement artifacts; ScotEID delivery retains attempts and acknowledgements. ICAR ADE 1.5 JSON snapshots, idempotent delivery, pending/final acknowledgements and deadline monitoring cover breed-registry and milk-recording exchange. | Add each further jurisdiction's regulator-specific event/document profile and provider transport only after legal review and provider certification. Recording organisations may require profile-specific ICAR extensions. | **Initial launch profiles complete / market certification later** |
| Compliance | **Backend, readiness packs and controlled audit-pack export implemented.** Tenant-versioned standards/control points/evaluator versions, typed and freshness-bound evidence requirements, immutable evidence snapshots, findings and independently verified corrective actions are joined by provenance-locked readiness packs for GLOBALG.A.P. IFA v6 Smart, Codex HACCP, ISO 9001 and EUDR. Completed assessments export as immutable revisions with explicit scope statements, frozen standard/control/evaluation/findings content, optional evidence binaries revalidated against retained SHA-256 values, per-entry manifests, whole-artifact checksums, key-ID-versioned HMAC-SHA256 signatures, retained object storage, export audit/outbox records and server-side verification. | Obtain qualified-specialist approval for each launch-market mapping before making certification or legal-conformance claims. Replace deployment secrets and retain prior verification keys under the production key-management procedure. | **Controlled export complete / specialist validation required** |
| Supply-chain portal | **Backend workflow complete.** Cross-tenant relationships and grants, hash-only invitations, hosted accounts, contracts, allocations, evidence review and status projections are joined by weighted/versioned supplier-risk snapshots; immutable effective fixed, volume-tier and quality-grade price rules; server-resolved quotes; equation-checked reconciliation revisions; and short-lived hash-only public claim/detach actions. Links are bound to a predetermined supplier tenant, single-use, replay-safe and recorded in immutable history. New records use forced RLS, least-privilege grants, audit and outbox integration. | Launch deployments still need identity-provider delivery of action links, specialist approval of risk weights/thresholds, and enterprise-specific price-index feeds where indexed commodity pricing is required. | **Backend complete / deployment calibration later** |
| Analytics | **Backend warehouse and analytics-governance processing implemented.** Recurring schedules and debounced farm-domain events create revisioned, leased/retriable projections. Historical facts and content-addressed dimension snapshots remain immutable while dashboards, benchmarks and rollups select only the current successful revision. Governed, versioned cohort definitions enforce specialist approval, effective dates and privacy thresholds. Governed predictive-model definitions retain feature/training/validation provenance; the initial risk-adjusted crop-yield model materializes confidence-bounded forecasts and immutable forecast-versus-actual accuracy records. Tenant retention policies preserve sensor and machinery daily rollups before privileged raw-data removal, purge only expired inline map projections, and queue explicitly registered derived media for leased object-store deletion. Legal holds and lifecycle audit runs are retained. | Production model promotion still requires agronomist/model-risk approval and launch-market training data. Advanced model training, drift detection and additional target models should follow measured data volume and validation evidence. | **Backend maturity slice complete / model calibration ongoing** |
| Machinery, IoT and ERP | **Connected-agriculture backend core implemented.** Farm equipment, devices and typed channels, provider-backed sensor and machinery ingestion, immutable PostgreSQL range-partitioned readings/telemetry, freshness monitoring, ISOXML/ADAPT/task-file exchange and canonical accounting/ERP document lifecycles are present under forced RLS. A database-coordinated worker now creates current/future yearly sensor and telemetry partitions under advisory locking, records partial failures caused by default-partition rows, and processes retryable derived-media deletion leases. | Implement further named machinery, accounting and ERP vendor adapters only against funded provider contracts. Production operators must monitor partition-maintenance and lifecycle-retention run records and align object-store native lifecycle/versioning with the application policy. | **Core and lifecycle automation complete / funded adapters later** |
| Retail transparency | **Backend core implemented.** Versioned disclosure policies, opaque expiring/revocable public tokens, origin/crop/variety/quantity/quality/compliance/lineage projection, downstream-grant and attribute enforcement, public resolver and hashed access metadata are present. | Edge rate limiting/WAF, branded page UX and launch-market disclosure/legal review remain deployment and PWA work. | **Core complete / edge and UX later** |
| Enterprise architecture | **Backend enterprise semantics, routing and migration execution implemented.** Organisation hierarchy and scoped authorization now include effective-dated legal-company registrations. Intercompany postings require an explicit counterparty company and shared reference; approved versioned rules pair seller revenue with buyer expense. Immutable consolidation runs retain gross and eliminated totals, transaction lists, rule versions, SHA-256 source identity and tolerance-based imbalance exceptions, and enterprise profitability exposes the latest matching controlled consolidation. Reusable approval policies, quorum decisions, placement/residency records, configured per-tenant routing, write-guarded leased migration execution, authenticated encrypted artifacts, validation, transactional import and atomic cutover are also present. | Database provisioning, managed cross-region artifact transport, source-retention purge, database replication and second-region infrastructure remain deployment-control-plane responsibilities and require disaster-recovery drills. Accounting teams must approve company structures, tolerance policies and reporting-period procedures before production close. | **Backend complete / infrastructure and close-process validation later** |
| SaaS control plane | **Commercial lifecycle implemented.** Tenant provisioning/activation/suspension/closure, immutable versioned plans, feature and usage-limit entitlements, trials and recurring subscription periods, idempotent usage events and counters, billing accounts/invoices, operator-only mutations, forced RLS and request-time account/subscription enforcement are present. Existing tenants are enrolled explicitly to permit controlled rollout. | Connect the provider-neutral billing records to the selected payment processor and add automated renewal/dunning jobs when the commercial provider is chosen. | **Provider-neutral core complete** |
| API protection and identity operations | **Provider-neutral backend core implemented.** PostgreSQL-coordinated tenant/client rate windows and daily quotas, active policy management, API-key maximum-age/expiry enforcement, hash-only device and session inventory, revocation, append-only authentication/security events, attempt-limited single-use recovery cases, session invalidation, recovery outbox handoff, MFA-on-write enforcement and fail-closed production IdP configuration validation are present under forced RLS. | Select and configure the production OIDC provider, map its MFA/session claims, and implement the provider adapter that consumes verified recovery events to perform the provider-owned account-reset action. Edge WAF/DDoS controls and credential delivery remain deployment responsibilities. | **Provider-neutral core complete / IdP adapter at deployment** |
| PWA completion | **Out of scope for the current backend-only sequence.** Offline command logs, checkpoints, conflict responses, evidence uploads and module APIs are server enablers already present. | Do not schedule client workflows, maps, dashboards, storage/update UX or device testing until backend slices above are complete. Any missing server contract discovered later stays with its owning backend product area. | Deferred |

### Recommended backend execution sequence

1. **Shared ingestion substrate — implemented:** integration connection metadata, protected and rotatable credential references, feeds, leased/idempotent ingestion jobs, checkpoints, immutable provenance, deduplication, freshness/provider health, provider cost/error attempts and retry/dead-letter/replay controls, all under forced RLS with audit/sync/outbox events.
2. **Real-time field intelligence — implemented backend core:** weather observations/forecasts, satellite field statistics, scouting, deterministic and versioned risk evaluation, alert lifecycle/follow-up work, map-layer APIs and freshness projections.
3. **Geospatial onboarding — implemented backend core:** retained GeoJSON/KML/Shapefile/ISOXML artifacts, format validation, persisted GeoJSON preview, controlled transactional application and immutable boundary lineage.
4. **Livestock expansion — backend and initial launch integrations implemented:** pedigree/reproduction lineage, milk withdrawal enforcement, grazing/ration plans, unified history, DLWG/FCR, farm production KPIs, versioned Scotland movement rules/artifacts with ScotEID delivery, and ICAR ADE breed/milk submissions with delayed acknowledgements.
5. **Compliance and supplier workflows — implemented backend:** versioned standards/control points/evidence requirements, scored assessments, dashboards/findings/verified corrective actions, invitations, hosted suppliers, public one-time claim/detach actions, immutable link history, weighted supplier risk, effective price rules and quotes, bounded allocations, equation-checked reconciliation revisions, evidence review and supplier status projections. Production link delivery and risk-model calibration remain deployment work.
6. **Analytics and connected agriculture — implemented backend core:** scheduled projection contracts and fact materialization, KPI lineage/quality, farm/enterprise comparisons, thresholded benchmarking, equipment/device/channel registries, automatically maintained partitioned sensor/telemetry ingestion, freshness, machinery task files, canonical accounting/ERP exchanges, expired-map cleanup and legal-hold-aware derived-media deletion. Concrete vendor adapters remain deployment work.
7. **Public and enterprise surfaces — implemented backend core:** scoped public API credentials and audited resources, transactional signed webhooks with delivery logs, grant-aware retail transparency tokens and presentation, delegated approval quorum, tenant database routing, and encrypted export/validate/import/cutover execution. Physical provisioning, replication and region failover drills remain deployment responsibilities.
8. **Backend verification — implemented baseline:** the default unit suite is joined by opt-in database-backed API integration, OpenAPI contract, production-authentication, direct forced-RLS, security-definer ACL, analytics end-to-end and bounded parallel load tests. Continue expanding scenario coverage and provider-specific performance budgets as launch integrations are selected.
9. **API protection and identity operations — provider-neutral core implemented:** distributed tenant/client quotas, API credential rotation constraints, device/session inventory and revocation, append-only security events, controlled recovery verification, local-session invalidation, MFA-on-write and fail-closed production OIDC configuration. Production IdP selection and its recovery adapter remain deployment integration work.
10. **Farm digital-twin depth — implemented backend core:** temporal tenure, mapped infrastructure, exact-version soil zones and samples, append-only laboratory results, GeoJSON overview, PostGIS containment/overlap guards, forced RLS and immutable-history controls.
11. **Bulk master-data onboarding — implemented backend core:** retained and checksum-idempotent CSV jobs, seven typed templates/exports, immutable staged rows, validation previews and downloadable error files, resumable transactional checkpoints, balanced finance allocation, forced RLS and audit/sync/outbox integration.
12. **GS1 EPCIS interoperability — implemented backend core:** EPCIS 2.0.1 JSON/JSON-LD capture jobs, immutable extension-preserving event storage, internal event serialization, CBV 2.0 and UN/CEFACT mappings, SimpleEventQuery document export, forced RLS and insert-only grants. Official GS1 conformance fixtures and partner-specific identifiers remain pre-launch acceptance work.
13. **Connected-data lifecycle automation — implemented:** advisory-lock-protected current/future yearly partitions, immutable-source-safe raw rollup retention, expired inline map-layer cleanup, an integrity-checked derived-media registry, explicit legal holds, leased object-store deletion with bounded retries, forced RLS and auditable maintenance runs. Evidence, imports, audit packs and statutory artifacts never enter the derived deletion queue.

**Status:** Proposed baseline  
**Prepared:** 28 August 2026  
**Scope:** Product and engineering delivery plan for farms, enterprise holdings, food and beverage buyers, and retailers  
**Client strategy:** Responsive Angular Progressive Web App (PWA); no .NET MAUI application

---

## 1. Executive decision

Build MurimiOS as one multi-tenant platform with four role-specific experiences over a shared agricultural and traceability data model:

1. **Farm workspace** for crop and livestock production, planning, daily operations, inventory, cost control, and field insight.
2. **Enterprise command centre** for multi-company and multi-farm governance, consolidated operations, approvals, benchmarking, and financial control.
3. **Food and beverage sourcing portal** for contracting, supplier onboarding, evidence collection, quality, food-safety, sustainability, and chain-of-custody.
4. **Retail traceability portal** for contracted production, product-origin reporting, recalls, assurance evidence, and controlled consumer transparency.

The delivery strategy is a **modular monolith first**, backed by PostgreSQL/PostGIS, an asynchronous worker, and an offline-first Angular PWA. The product is released in independently useful increments. Crop and livestock share tenancy, locations, inventory, finance, documents, traceability, compliance, and analytics, but retain separate production models.

The first sellable release should prove an end-to-end production record: register a farm, map a field or holding, create a crop zone or animal group, record work offline, synchronize it safely, consume inventory, create traceability events, and export an evidence report.

---

## 2. Outcomes and boundaries

### 2.1 Product outcomes

MurimiOS must enable customers to:

- Create a digital twin of organisations, farms, sites, fields, infrastructure, herds, animals, machinery, stock, and production cycles.
- Plan crop seasons and livestock cycles with agronomic, operational, resource, and financial views.
- Assign, execute, verify, and cost field and livestock work, including capture without connectivity.
- Combine weather, satellite, scouting, sensor, machinery, production, and finance data into timely decisions.
- Compare fields, varieties, practices, herds, cohorts, farms, seasons, and suppliers on yield, quality, cost, margin, and risk.
- Exchange data with machinery, IoT platforms, accounting systems, and ERPs through versioned adapters.
- Trace crop, feed, animal, harvest, batch, delivery, and transformed-product lineage.
- Produce evidence for configured schemes such as GLOBALG.A.P., HACCP-based programmes, ISO 9001 processes, and ISO 22005 traceability.
- Share only authorized data across producer, buyer, retailer, and verifier tenants.

### 2.2 Explicit boundaries

- The platform assists compliance but does not itself certify a customer.
- Agronomic and veterinary decision support must be deterministic and expert-reviewed before predictive or generative features are allowed to influence operations.
- AI may summarize, extract, explain, and draft; it may not silently create or alter legal evidence.
- Live integrations with every machinery and ERP vendor are not an MVP requirement. File import/export and one funded connector precede broad integration work.
- The PWA is the mobile client. Background execution after the user closes it cannot be assumed on every browser or operating system.

---

## 3. Clientele and product surfaces

| Clientele | Primary jobs | Product surface | Key value measures |
|---|---|---|---|
| Farms | Run daily crop/livestock work, remain compliant, control cost | Farm workspace and installed field PWA | Work completed on time, fewer input errors, yield/head performance, gross margin |
| Enterprise holdings | Govern many farms and legal entities | Command centre with hierarchy, approvals, consolidation, and benchmarking | Cost/ha, cost/head, yield variance, compliance status, operational variance |
| Food & beverage | Contract suppliers and secure safe, sustainable, traceable raw materials | Sourcing and supplier portal | Supplier completion, trace coverage, quality acceptance, risk and exception rate |
| Retail | Assure own/contracted production and communicate origin | Retail assurance portal, recall tools, consumer resolver | Trace time, covered products, recall precision, evidence completeness |
| Verifiers/auditors | Review evidence without broad system access | Time-boxed read-only workspace | Audit preparation time, evidence gaps, findings closure time |

Enterprise is a capability tier inside the producer model rather than a separate codebase. Food and beverage and retail reuse the same supply-network, contract, evidence, and traceability modules with different terminology and dashboards.

---

## 4. Capability map

### 4.1 Central farm operations management

- Organisation → legal entity → farm/holding → site → field/paddock/pen hierarchy.
- GIS map, farm register, assets, equipment, workers, suppliers, customers, warehouses, and calendars.
- Unified operations inbox with planned, assigned, in-progress, completed, verified, rejected, and corrected work.
- Role dashboards for owner, farm manager, agronomist, livestock manager, operator, buyer, and auditor.
- Documents, photos, signatures, comments, alerts, and audit history attached to business records.

### 4.2 Advanced crop planning

- Seasons, crop zones, varieties, rotations, yield targets, and field allocation.
- Planned operations, input requirements, labour, machinery, contractor, irrigation, and timing.
- Budget by crop zone and field, with planned-versus-actual quantity, cost, yield, revenue, and margin.
- Constraints for product authorization, maximum application rate, re-entry interval, and pre-harvest interval by jurisdiction.
- Scenario comparison and approval workflow before the season plan becomes active.

### 4.3 Livestock management

- Individual animals and batch-managed groups, with species-specific identifiers and lifecycle states.
- Temporal group membership, births, purchases, transfers, sales, deaths, and holding movements.
- Health events, vaccinations, medicine inventory, treatments, withdrawal periods, and dispatch/milk-sale holds.
- Breeding, pregnancy, birth, weaning, weights, body condition, milk, mortality, and performance records.
- Feed batches, rations, allocations, grazing plans, paddocks, feed conversion, and crop-to-feed lineage.
- Regulatory movement/medicine-book adapters introduced one country at a time.

### 4.4 Real-time field and herd insight

- Weather observations and forecasts by deduplicated farm grid/field centroid.
- Satellite acquisition statistics by field: NDVI/NDRE/NDMI or other configured indices, cloud/valid-pixel metadata, and anomaly alerts.
- Scouting observations, pest/disease risk, crop health, soil samples, and sensor readings.
- Livestock threshold alerts for weight gain, health, mortality, fertility, feed, water, and environmental conditions.
- Offline-capable alert acknowledgement and follow-up work creation.

“Real time” means source-appropriate freshness shown to the user. Every insight displays observation time, provider, quality, and last successful refresh rather than implying all data is instantaneous.

### 4.5 Analytics

- Field, crop, variety, practice, season, herd, cohort, animal, farm, and supplier comparisons.
- Yield, cost/ha, cost/tonne, gross margin/ha, input efficiency, daily liveweight gain, feed conversion, mortality, and quality metrics.
- Planned versus actual operations and spend.
- Enterprise roll-ups with controlled benchmarking and drill-down.
- Data-quality score beside every analytical result so missing records are not presented as fact.

### 4.6 Connected machinery, IoT, accounting, and ERP

- Boundary and task data import/export: CSV, GeoJSON, KML, Shapefile packages, and later ISOXML/ADAPT.
- Provider-neutral machinery job and telemetry contracts.
- MQTT/LoRaWAN/provider-webhook sensor ingestion with validation, buffering, and device identity.
- Accounting connector first; ERP connectors only against an agreed customer workflow.
- Webhooks and public API for operation completion, animal movement, quality results, trace events, threshold alerts, and compliance exceptions.

### 4.7 Traceability and compliance

- Append-only visibility event log aligned with GS1 EPCIS concepts.
- Crop-to-harvest-to-lot-to-delivery and animal/feed-to-product chain-of-custody.
- Forward trace, backward trace, mock recall, mass-balance checks, and trace completeness.
- Standards represented as versioned control points, evidence requirements, evaluations, findings, and corrective actions.
- Audit packs and controlled public/consumer reports generated from the same evidence graph.
- Corrections create new records and preserve original evidence; completed compliance records are never silently overwritten.

---

## 5. Architecture baseline

### 5.1 Logical shape

```text
Angular PWA
  ├─ Producer/field workspace
  ├─ Enterprise command centre
  ├─ Sourcing/retail portal
  └─ IndexedDB data cache + command outbox
             │ HTTPS/JSON, resumable media upload
             ▼
ASP.NET Core API modular monolith
  ├─ Identity & Tenancy        ├─ Farm Registry
  ├─ Crop Planning             ├─ Operations
  ├─ Livestock                 ├─ Inventory & Finance
  ├─ Field Intelligence        ├─ Supply Network
  ├─ Traceability              ├─ Compliance
  ├─ Analytics                 └─ Sync & Files
             │ transactional outbox
             ▼
Worker host ── RabbitMQ ── provider adapters
             │
             ├─ PostgreSQL + PostGIS
             ├─ time-series partitions/TimescaleDB
             ├─ Redis
             └─ S3-compatible object storage
```

### 5.2 Recommended stack

| Layer | Baseline |
|---|---|
| API | .NET 10, ASP.NET Core minimal APIs, OpenAPI, vertical feature slices |
| Persistence | EF Core 10, PostgreSQL 17, Npgsql, PostGIS/NetTopologySuite |
| Time series | PostgreSQL partitioning initially; TimescaleDB when deployment support and measured volume justify it |
| Async work | Transactional outbox, RabbitMQ, independently scalable worker roles |
| Cache | Redis for ephemeral cache, locks, rate limits, and permission snapshots—not source-of-truth data |
| Files | S3-compatible object storage, pre-signed resumable uploads, malware scanning |
| Client | Supported Angular release, standalone components, signals, lazy feature routes, Angular PWA shell |
| Offline data | IndexedDB through a thin repository layer; evaluate Dexie during the Phase 0 spike |
| Maps | MapLibre GL, vector tiles from PostGIS, offline working-set cache |
| Observability | OpenTelemetry traces/metrics/logs, structured logging, health checks, cost meters |
| Deployment | Containers, managed PostgreSQL where PostGIS is supported, CI/CD with expand/contract migrations |

Pin exact package versions in an ADR and update them as one tested compatibility set. Do not repeat the current state where `net10.0` projects reference EF Core 8 packages.

### 5.3 Modular-monolith rules

- One deployable API and one worker host at first.
- Each module owns its domain model, application features, tables/schema, and public contracts.
- A module may use shared building blocks and another module’s contract, never its internal entities or DbContext.
- Cross-module state changes publish integration events through the transactional outbox.
- Cross-module reads use stable projections/contracts, not entity navigation.
- Architecture tests enforce boundaries and tenant-scoping conventions.
- Extract a service only when scaling, isolation, release cadence, or team ownership is demonstrably different.

Suggested solution layout:

```text
src/
  BuildingBlocks/{Domain,Application,Infrastructure,Contracts}
  Modules/
    IdentityTenancy/
    FarmRegistry/
    CropPlanning/
    Operations/
    Livestock/
    InventoryFinance/
    FieldIntelligence/
    SupplyNetwork/
    Traceability/
    Compliance/
    Analytics/
    SyncFiles/
  Hosts/{Api,Worker}
apps/
  web/                 # Angular PWA
tests/
  Architecture.Tests/
  Domain.UnitTests/
  Integration.Tests/
  Sync.PropertyTests/
  Web.E2E/
```

---

## 6. Multi-tenancy, hierarchy, and sharing

### 6.1 Tenancy model

- `Tenant` is the billing, configuration, primary isolation, and regional-residency boundary.
- `Membership` links a user to one or more tenants with role and organisation-node scope.
- All tenant-owned rows carry a non-null `TenantId`; the server derives it from authenticated context, never request payload.
- Default deployment is shared database with tenant discriminator.
- An `ITenantConnectionResolver` preserves a future dedicated-database option for contracted enterprise customers.
- Tenants are pinned to a region and encryption/storage configuration.

### 6.2 Defence in depth

1. EF Core global query filters for normal application access.
2. PostgreSQL row-level security using a request/transaction-scoped tenant setting.
3. Resource authorization against organisation/farm paths and explicit permissions.
4. Automated reflection-driven isolation tests for every tenant-owned entity.
5. Immutable audit records for privileged and cross-tenant access.

Background jobs must carry an explicit tenant context. “System” access is not an unscoped bypass; it is a separately authorized path with an allow-listed purpose and audit record.

### 6.3 Organisation hierarchy

Use a materialized path/`ltree` model:

```text
Tenant
└─ Organisation/legal entity
   └─ Farm/holding
      ├─ Field/paddock
      ├─ Site/barn/pen
      └─ Warehouse/cost centre
```

Permissions and reports are scoped to one or more paths. This supports enterprise consolidation without duplicating every feature for “enterprise.”

### 6.4 Cross-tenant supply relationships

- A `SupplyRelationship` connects buyer/retailer and producer tenants.
- A versioned, revocable `DataGrant` specifies purpose, entities, attributes, farms, commodities, seasons, validity, and downstream disclosure.
- Aggregator queries use a dedicated grant-aware read model; they never impersonate the producer.
- Every grant read records actor, purpose, grant, entity category, time, and result count.
- Hosted supplier accounts can later be claimed and detached by the producer under an explicit workflow.

---

## 7. Domain and data design

### 7.1 Core invariants

- Every business record has tenant, creator, creation time, and concurrency token.
- Operational observations distinguish `OccurredAt` from `RecordedAt` to support delayed offline capture.
- Completed field operations, movements, treatments, inspections, and trace events are immutable-in-effect. Corrections supersede; they do not erase.
- Quantities use canonical units in storage and preserve entered/display units for round-trip fidelity.
- Currency, timezone, locale, country, regulatory catalogue, and crop year are tenant or organisation settings—not global assumptions.
- External identifiers include source and namespace; a tag or product code is not globally unique by itself.

### 7.2 Farm and crop model

- `Organisation`, `Farm`, `Site`, `Field`, `FieldBoundaryVersion`, `LandTenure`, `SoilSample`.
- `Season`, `CropZone`, `Crop`, `Variety`, `RotationPlan`, `YieldTarget`.
- `PlannedOperation`, `WorkOrder`, `FieldOperation`, `OperationInput`, `LabourEntry`, `MachineryUsage`.
- `Product`, `ProductRegistration`, `StockItem`, `StockLot`, `StockMovement`, `Warehouse`.
- `Harvest`, `HarvestLot`, `StorageLot`, `Delivery`, `QualityAssessment`.

Field geometry is versioned. Operations point to the boundary version valid when work occurred. Area is computed server-side from validated PostGIS geography rather than accepted from the PWA.

### 7.3 Livestock model

- `Animal`, `AnimalIdentifier`, `AnimalGroup`, `GroupMembership`, `Holding`, `Species`, `Breed`.
- `Movement`, `Birth`, `Purchase`, `Sale`, `Death`, `Dispatch`, `MovementDocument`.
- `HealthEvent`, `Treatment`, `MedicineProduct`, `WithdrawalHold`, `VaccinationProgramme`, `VetVisit`.
- `BreedingEvent`, `PregnancyDiagnosis`, `Parturition`, `Weaning`, `WeightMeasurement`, `MilkRecord`, `BodyConditionScore`.
- `FeedBatch`, `Ration`, `FeedAllocation`, `GrazingSession`, `WaterObservation`.

Use individual identity where required and batch identity where appropriate. A poultry flock or feedlot cohort must not be forced through an individual-animal model.

### 7.4 Traceability model

Use an append-only event projection aligned to EPCIS 2.0 concepts. Domain modules emit events; the traceability projector creates standardized visibility events with:

- what: animals, crop lots, inputs, batches, logistic units, or products;
- when: event and recording times;
- where: farm, field, holding, facility, or read point;
- why: business step and disposition;
- lineage: inputs, outputs, parent/child aggregations, transaction references;
- source: originating module and immutable record identifier.

The backend now exposes authenticated EPCIS 2.0.1 JSON/JSON-LD capture and SimpleEventQuery export over immutable exchange/event records. Internal events map to CBV 2.0 business steps/dispositions and UN/CEFACT quantity codes; incoming custom JSON-LD extensions are retained exactly. Official GS1 conformance fixtures plus trading-partner identifier and master-data profiles remain release-gate work before any certification claim.

### 7.5 Compliance model

```text
StandardVersion
  → ControlPoint
    → EvidenceRequirement
      → Evaluation
        → EvidenceBinding
          → Finding/CorrectiveAction
```

- Standard versions are effective-dated and region/commodity scoped.
- Evidence evaluators are registered code with versioned parameters, not arbitrary stored SQL.
- Evaluations are precomputed after relevant events and on a scheduled reconciliation run.
- Manual evidence records issuer, validity, reviewer, and attached document hash.
- Reports identify standard/version, scope, evaluation time, exclusions, and unresolved gaps.

Initial control libraries should be chosen with a domain specialist. A practical order is production record integrity and traceability first, then scope-appropriate GLOBALG.A.P. controls, HACCP/ISO 22005 supply-chain evidence, and organisation-specific ISO 9001 procedures. Do not assume that one GLOBALG.A.P. library covers both crops and livestock: current IFA scopes and editions must be checked for the launch commodity, and the former IFA livestock scope has been discontinued.

### 7.6 Time series and lifecycle

- Transactional agriculture and legal-evidence data stays in ordinary PostgreSQL tables.
- Sensor readings use partitioned/hypertable storage, batch ingestion, quality flags, and retention policies.
- Store satellite field statistics as normal records; store large rendered/raster assets in object storage.
- Retention is policy-driven by tenant region, standard, and contract. Legal evidence defaults to preservation, while raw sensor/media tiers can expire after durable roll-ups and required derivatives exist.

---

## 8. Angular PWA plan

### 8.1 PWA architecture

The PWA is one Angular workspace with lazy role-oriented features and shared domain contracts generated from the API schema where useful.

```text
apps/web/src/app/
  core/          auth, tenant context, API, telemetry, updates
  offline/       IndexedDB, outbox, sync, conflicts, storage health
  shell/         responsive navigation and install/update UX
  features/
    farm-map/ crop-planning/ operations/ livestock/
    inventory/ insights/ analytics/ sourcing/ compliance/
  shared/        controls, units, dates, maps, media
```

Use standalone components, typed reactive forms, signals for local view state, accessible component primitives, and route-level authorization. Large maps, reports, and enterprise modules are lazy loaded.

### 8.2 Offline contract

The following workflows must work for at least seven days without connectivity after a working set is downloaded:

- View assigned farms, fields, crop zones, active groups/animals, tasks, and permitted reference catalogues.
- Create field operations, scouting reports, livestock measurements, treatments, movements, notes, photos, and GPS evidence.
- Update drafts and complete allowed assignments.
- See pending, synchronized, rejected, and conflicted commands.
- Retry or correct rejected commands without data loss.

The PWA uses IndexedDB stores for scoped read models, immutable commands, attachments, sync checkpoints, conflicts, and diagnostic state. It must request persistent browser storage when supported, monitor quota, warn before exhaustion, and provide an encrypted export/recovery package for unsynchronized commands where policy permits.

### 8.3 Service-worker boundary

- Use Angular PWA support for manifest, installability, versioned application shell, static assets, and conservative GET caching.
- Do not depend on service-worker response caching as the business database.
- The application sync coordinator reads/writes IndexedDB and runs on app launch, foreground, authenticated reconnect, manual request, and a guarded periodic timer while open.
- Background Sync is a progressive enhancement only; unsupported browsers still retain commands and show a clear “open MurimiOS to synchronize” status.
- A custom service-worker extension may process short queued requests after the spike proves browser behavior, but it must share the same idempotent server protocol.
- Deployments show an update prompt and never activate a new client bundle while local database migrations or unsynchronized commands are unsafe.

### 8.4 Command synchronization

```json
{
  "deviceId": "uuid",
  "commands": [
    {
      "commandId": "uuid",
      "sequence": 1421,
      "type": "RecordFieldOperation",
      "schemaVersion": 1,
      "capturedAt": "2026-08-14T06:11:03Z",
      "payload": {}
    }
  ]
}
```

- `commandId` is the idempotency key; replay returns the stored result.
- `sequence` is monotonic per installed client identity. Gaps are reported, not guessed around.
- Each command type declares append, server-wins, last-write-wins, or manual conflict behavior. There is no global default.
- Tenant and actor come from fresh server authentication at upload time, not cached command payload.
- Pull synchronization uses ordered server change checkpoints, not device timestamps.
- Tombstones are retained for a documented maximum offline interval; older clients perform a full scoped refresh.
- Media is queued separately, uploaded resumably to pre-signed URLs, checksummed, and reconciled against referenced records.

### 8.5 PWA security and device behavior

- Use HTTPS, strict CSP, Trusted Types where practical, dependency auditing, and no third-party scripts on operational routes without review.
- Keep access tokens in memory; use secure, HttpOnly, same-site refresh/session cookies where the chosen identity flow supports them.
- Provide local screen lock for an installed/offline session and re-authenticate before sync or sensitive disclosure.
- Cache only the user’s assigned working set and avoid unnecessary personal data.
- Remotely revoke the server-side device registration and grants. Be clear that a browser cache cannot be remotely erased while permanently offline.
- Test install, camera, GPS, file import, IndexedDB, quota, updates, and offline recovery on the supported Android, iOS/iPadOS, Windows, and desktop browser matrix.

---

## 9. API, integration, and processing

### 9.1 API conventions

- REST under `/api/v1` with endpoint groups aligned to modules.
- RFC Problem Details with stable error codes and field errors.
- Idempotency keys on all externally retriable writes.
- Cursor pagination for event streams and changing collections; deterministic sort tiebreakers elsewhere.
- Optimistic concurrency for editable plans and configuration.
- Bulk import endpoints return job identifiers and downloadable validation results.
- Separate public/integration OpenAPI surface from the internal application API.
- Per-tenant and per-client rate limits, quotas, and cost meters.

### 9.2 Asynchronous processing

- Domain state and outbox messages commit in the same database transaction.
- Outbox publishing and all consumers are idempotent.
- Worker roles cover outbox, imports, files, insights, sensors, trace projection, compliance evaluation, analytics, notifications, and integration delivery.
- Provider failures degrade the relevant insight/integration; they do not make farm record-keeping unavailable.
- Dead-letter items have an operator workflow with payload redaction, retry, and audit.

### 9.3 Integration sequence

1. CSV, GeoJSON, KML, Shapefile package, and standardized report export.
2. Weather provider and one satellite statistics provider.
3. One accounting connector selected by initial customers.
4. Generic MQTT/webhook sensor gateway.
5. ISOXML/ADAPT task-file exchange.
6. One statutory livestock movement adapter for the chosen launch country.
7. Customer-funded machinery telematics and ERP adapters.

Every adapter maps external data into an internal canonical contract and stores provider, external key, raw receipt hash, mapping version, and processing result.

---

## 10. Analytics and decision support

Start with operational projections and materialized aggregate tables. Add a star schema only when cross-season and enterprise reporting requires it.

Recommended facts:

- field operation, input application, harvest/yield, inventory movement, cost, animal measurement, treatment, movement, feed allocation, delivery, quality result, and compliance evaluation.

Recommended dimensions:

- tenant, organisation, farm, field, crop zone, crop, variety, season, animal/group, species, product, supplier, contract, standard, and date.

Rules:

- Analytics reads never weaken tenant/grant authorization.
- Expensive roll-ups are scheduled or event-refreshed, not recomputed on every dashboard request.
- Every KPI has an owner, formula, unit, time grain, exclusions, and data-quality rule.
- Benchmarking is opt-in and returns anonymized cohorts only after minimum cohort thresholds are met.

---

## 11. Security, privacy, and operational controls

- Standards-based identity provider, MFA for privileged roles, short-lived sessions, and device/session inventory.
- Role plus resource-scope authorization; no broad `Admin` shortcut across tenants.
- Encryption in transit and at rest; per-tenant integration secrets encrypted with rotatable keys.
- Append-only security/audit stream for logins, role changes, exports, grants, corrections, and cross-tenant reads.
- Signed webhooks with replay protection and delivery logs.
- Pre-signed file upload, content-type/size limits, checksums, and isolated malware scanning.
- Backup/restore drills, point-in-time recovery, regional recovery objectives, and tested tenant export.
- Avoid tenant IDs and other high-cardinality fields as metric labels; keep them in controlled logs/traces for investigation.
- Threat-model tenancy, grant projection, offline storage, file import, public trace pages, webhooks, and integration credentials before release.

Initial service objectives after production hardening:

| Objective | Target |
|---|---|
| API availability, excluding scheduled maintenance | 99.9% monthly |
| Farm list/operation query | < 300 ms p95 |
| Sync push of 100 commands | < 2 seconds p95 excluding media |
| Cached map tile | < 100 ms p95 |
| Compliance dashboard | < 750 ms p95 from precomputed projection |
| RPO/RTO | 15 minutes / 4 hours initially; tighten for enterprise contracts |

---

## 12. Quality strategy

| Quality area | Required tests/gates |
|---|---|
| Domain | Unit tests around planning, quantities, withdrawal, trace, and correction invariants |
| Architecture | Module boundaries, no forbidden references, tenant marker/filter conventions, no local-time APIs |
| Persistence | Real PostgreSQL/PostGIS integration tests and real migrations |
| Multi-tenancy | Generated isolation suite plus adversarial grant-scope tests |
| Offline sync | Property-based duplicates, gaps, reordering, retries, clock skew, and schema-upgrade tests |
| PWA | Playwright offline/update tests plus physical-device airplane-mode acceptance |
| Geospatial | Golden imports, invalid geometry, CRS conversion, area, split/merge, and vertex limits |
| Compliance | Passing/failing fixture for every control-point evaluator and standard version |
| Traceability | Forward/backward trace, split/merge, transformation, recall, and mass-balance fixtures |
| Security | SAST/dependency/secret scans, authorization tests, and periodic penetration testing |
| Performance | Seasonal burst, map, sync, cross-tenant portal, and high-volume sensor scenarios |

No release is accepted until an installed PWA completes a simulated offline week and synchronizes without losing, duplicating, or silently rejecting records.

---

## 13. Current MurimiOS assessment and migration approach

The repository already contains useful concepts for crops, fields, seasons, warehouses, inventory items, sales, purchasing, work items, assets, and individual livestock. Preserve business terminology and tested behavior, but do not carry forward the current persistence shape unchanged.

Observed baseline on 28 August 2026:

- Three projects: `API`, `Domain`, and `Infrastructure`, with one `MurimiOSDbContext`.
- .NET 10 target frameworks with EF Core/Identity/SQL Server 8.0.8 packages; align the framework/provider set before feature work.
- SQL Server provider and no PostGIS geometry model.
- No test projects or migrations detected.
- No tenant ownership, organisation hierarchy, audit metadata, or concurrency contract in `BaseEntity`.
- Domain events are published after `SaveChangesAsync`; use a transactional outbox for durable cross-module/integration work.
- `Field.UsableArea` is a mutable scalar; introduce temporal boundary geometry and computed area.
- The current livestock aggregate is very large and individual-animal oriented; split lifecycle capabilities and add batch/group management.
- No Angular application currently exists in the solution.

Migration principles:

1. Freeze broad feature additions while the platform spine is introduced.
2. Add characterization tests around existing calculations and critical API behavior.
3. Align .NET/EF packages and make the current solution build/test reproducibly.
4. Introduce tenant/audit/concurrency contracts and backfill a default tenant in an explicit migration.
5. Move to PostgreSQL/PostGIS before production customer data exists. If production data already exists, rehearse schema/data conversion and reconciliation in staging.
6. Introduce module boundaries incrementally around existing features; avoid a big-bang rewrite.
7. Replace post-commit event publication with persisted outbox dispatch.
8. Add versioned API compatibility while the Angular PWA replaces direct/manual API use.

---

## 14. Delivery roadmap

Estimates assume a small cross-functional team: two backend/full-stack engineers, one Angular engineer, part-time UX/QA, and retained agronomy/livestock/compliance specialists. A solo build should expect roughly 1.8–2.5 times these elapsed durations.

### Phase 0 — Stabilize and prove the risky paths (4–6 weeks)

Deliver:

- Reproducible build, package alignment, test projects, CI, coding/architecture rules.
- ADRs for modules, tenancy, PostgreSQL/PostGIS, Angular PWA, sync, units, and event/outbox design.
- Angular PWA shell installed on target devices.
- Throwaway vertical sync spike using IndexedDB: one field operation and one livestock measurement captured offline, replayed with duplicates/gaps, and pulled by checkpoint.
- PostgreSQL/PostGIS development environment and one versioned field boundary.

Exit criteria:

- Two tenants cannot access each other through API or direct database role tests.
- A seven-day simulated offline queue survives app restart and client upgrade.
- The chosen device/browser matrix is documented from physical testing.
- Current MurimiOS behavior covered by initial characterization tests.

### Phase 1 — Platform foundations (6–8 weeks)

Deliver:

- Identity, tenant membership, organisation hierarchy, resource-scoped permissions, region settings.
- RLS/query filters, audit, transactional outbox, worker, files, notifications, observability.
- Unit/quantity, money, time, location, and reference-catalogue building blocks.
- PWA offline repository, command outbox, conflict inbox, update UX, telemetry, and storage-health screen.
- Farm/holding/site registry and map-based field drawing/import preview.

Exit criteria:

- Automated isolation and authorization gates pass.
- A farm and versioned field boundary can be created online/offline with measured server area.
- Outbox recovery is proven across process termination.

### Phase 2 — Sellable production record (12–16 weeks)

Deliver crop core:

- Seasons, crop zones, operations, inputs, harvest lots, inventory movements, photos, GPS, and basic reports.

Deliver livestock core:

- Individual/batch registry, groups, movements, health/treatments, withdrawal holds, weights, feed allocations, and photos.

Deliver shared value:

- Offline work assignment/capture, traceability projection, basic evidence rules, audit-pack export, supplier/customer records.

Exit criteria:

- Crop input can be traced from stock lot to operation to harvest lot and delivery.
- Animal/feed/treatment/movement histories are complete and corrected without erasure.
- Dispatch is blocked or explicitly overridden under controlled permission when a withdrawal hold applies.
- Physical-device offline-week acceptance and initial production-shaped load tests pass.

### Phase 3 — Planning, finance, and field intelligence (12–16 weeks)

Deliver:

- Crop rotations, season plans, work orders, resources, budgets, planned-versus-actual, and approval workflows.
- Cost allocation and gross margin by crop zone, field, herd/group, farm, and enterprise.
- Weather, satellite field statistics, scouting, deterministic risk rules, alerts, and map layers.
- Deeper livestock breeding, reproduction, milk/performance, grazing, and feed-conversion workflows.
- One accounting connector and scheduled reconciliation.

Exit criteria:

- Budget-to-actual figures reconcile to operations/inventory/accounting within agreed tolerances.
- Insight freshness, quality, provider cost, and failure status are visible per tenant.
- Domain specialists sign off rule fixtures and user workflows.

### Phase 4 — Supply network, compliance, and retail (12–16 weeks)

Deliver:

- Supply relationships, supplier invitations, hosted/claimed accounts, contracts, allocations, deliveries, and quality assessments.
- Data grants, cross-tenant portal, audited access, evidence requests, findings, and corrective actions.
- Configured standard-version libraries selected for the launch market.
- Trace queries, mock recall, mass balance, controlled audit packs, and consumer/public disclosure policy.
- Public integration API and signed webhooks.

Exit criteria:

- Producer revocation immediately removes buyer access without deleting producer data.
- A lot can be traced backward and forward across tenant boundaries only through valid grants.
- Compliance outputs pass review by a qualified specialist; reports state scope and limitations.

### Phase 5 — Enterprise scale and connected agriculture (16–24 weeks)

Deliver:

- Multi-company consolidation, cost centres, delegated approvals, benchmarking, and enterprise dashboards.
- Analytics star schema and scheduled projections where measured need exists.
- Sensor gateway, equipment/task-data exchange, one funded machinery connector, and one funded ERP connector.
- Dedicated-database/second-region capability when contracted.
- AI-assisted document extraction, evidence drafting, and tenant-data copilot behind human approval and evaluation gates.

Exit criteria:

- Seasonal scale test meets budgets at forecast tenant/field/animal/sensor volume.
- Largest-tenant analytics refresh meets agreed SLA.
- Dedicated tenant migration and regional restore drills succeed without application-code forks.

---

## 15. First 90-day implementation backlog

### Weeks 1–2: decisions and stabilization

- Record ADRs and choose initial geography, commodities, supported devices, and first compliance scheme.
- Align .NET/EF dependencies; add build, unit, architecture, and integration pipelines.
- Create PostgreSQL/PostGIS development composition and migration runner.
- Add initial Angular workspace, installable manifest, shell, authentication placeholder, and UI foundations.

### Weeks 3–6: tenancy and offline proof

- Add tenant/user membership and organisation path model.
- Add RLS/query filters and generated isolation tests.
- Implement IndexedDB repositories, device registration, command outbox, server command log, idempotency, gaps, and checkpoints.
- Prove one crop and one livestock offline command on target Android and iOS devices.

### Weeks 7–10: farm registry vertical slice

- Add farms/sites/fields, versioned PostGIS boundaries, drawing, GeoJSON import, and computed areas.
- Add map facade, working-set download, offline map/entity view, and sync status.
- Add audit and transactional outbox/worker.

### Weeks 11–13: production record vertical slice

- Add season/crop zone and field-operation capture.
- Add livestock group/measurement/treatment capture and withdrawal calculation fixture.
- Add photos/GPS, resumable media queue, correction flow, traceability projection, and first evidence export.
- Run airplane-mode, tenant-isolation, recovery, and production-shaped acceptance tests.

---

## 16. Team and governance

Minimum effective team:

- Product owner/domain lead.
- Backend/platform engineer.
- Full-stack or second backend engineer.
- Angular/PWA engineer with offline and browser-storage experience.
- UX designer/researcher, initially part time.
- QA/automation engineer, initially part time but active from Phase 0.
- Agronomist and livestock adviser on retainer.
- Food-safety/compliance specialist during standard mapping.
- DevOps/security support at production hardening and enterprise onboarding.

Operating cadence:

- Two-week increments with a demonstrable field workflow, not horizontal technical layers alone.
- Monthly physical-device/offline field test.
- Architecture and threat-model review at every phase gate.
- Quarterly regulatory/catalogue review for supported markets.
- Each integration and compliance library has a named owner and version-support policy.

---

## 17. Major risks and mitigations

| Risk | Impact | Mitigation |
|---|---|---|
| Scope across four clienteles and two production domains | Product never reaches usable depth | Shared platform, phased releases, explicit exit criteria, named launch geography/commodities |
| PWA background/storage limitations | Unsynchronized or evicted field data | IndexedDB outbox, persistent-storage request, quota monitor, visible sync, recovery export, physical-device tests, background sync only as enhancement |
| Tenant or grant leak | Catastrophic trust/legal failure | RLS + query filters + resource authorization + generated isolation/adversarial tests + access audit |
| SQL Server to PostGIS migration grows late | Rework and geospatial compromise | Complete before production data; rehearse if any live data exists |
| Compliance rules are wrong or stale | False assurance and liability | Versioned schemes, qualified specialist approval, fixture tests, limitations in reports |
| Crop and livestock semantics become one generic model | Poor workflows and invalid rules | Separate bounded contexts sharing only genuine concepts/events |
| Event loss after database commit | Missing trace/compliance evidence | Transactional outbox and idempotent projectors |
| Provider and telemetry costs expand | Negative unit economics | Per-tenant metering, deduplication, budgets/caps, provider abstraction |
| Machinery/ERP integrations consume roadmap | Core product stalls | File exchange first; named, funded customer before live connector |
| Analytics contradict operational/finance records | Loss of trust | KPI contracts, reconciliation tests, lineage and data-quality scores |
| Large livestock aggregate becomes unmaintainable | Slow changes and invalid invariants | Split registry, health, movement, reproduction, and feed behaviors with temporal records |

---

## 18. Decisions required before Phase 1

1. Launch geography and data-residency region.
2. Initial crops/commodities and livestock species.
3. First compliance scheme and who will validate its mapping.
4. Supported PWA device/browser matrix and minimum OS versions.
5. Whether any existing MurimiOS database contains production data requiring migration.
6. Identity provider and enterprise federation requirements.
7. Shared-database isolation baseline and commercial trigger for dedicated databases.
8. First weather, satellite, accounting, and object-storage providers.
9. Required retention, deletion, export, and disaster-recovery policies by launch market.
10. Phase 2 pilot customers and the exact crop and livestock workflows used for acceptance.

---

## 19. Definition of product success

Track adoption and data trust rather than feature count:

- Active farms/holdings, mapped fields, active crop zones, animals/groups, and contracted suppliers.
- Weekly field users and completion rate of assigned work.
- Percentage of field/livestock records captured offline and successfully synchronized.
- Sync failure/rejection age, duplicate rate, and unrecovered-command count.
- Trace completeness and median forward/backward trace time.
- Evidence completeness by standard and time to close findings.
- Planned-versus-actual operation, input, cost, yield, and livestock-performance variance.
- Provider cost per active field/animal/tenant.
- Tenant-isolation incidents: target zero.
- Time to onboard a farm from boundary/animal import to first completed operation.

---

## 20. Authoritative references to maintain

- [Angular service worker overview](https://angular.dev/ecosystem/service-workers) — Angular describes the built-in worker as a basic caching utility and recommends native browser APIs for advanced offline behavior.
- [Angular PWA setup](https://angular.dev/ecosystem/service-workers/getting-started) — manifest, install assets, service-worker registration, and cache configuration.
- [MDN Background Synchronization API](https://developer.mozilla.org/en-US/docs/Web/API/Background_Synchronization_API) — limited browser availability; use as progressive enhancement.
- [MDN IndexedDB](https://developer.mozilla.org/en-US/docs/Web/API/IndexedDB_API) — durable browser-side structured storage and its lifecycle limitations.
- [GS1 EPCIS 2.0.1](https://ref.gs1.org/standards/epcis/2.0.1/) — interoperable visibility event standard.
- [ISO 22005:2007](https://www.iso.org/standard/36297.html) — traceability-system principles and requirements for feed and food chains.
- [EUR-Lex EUDR materials](https://eur-lex.europa.eu/) — validate the current legal text, application dates, scope, guidance, and information-system rules before implementing an EUDR module.
- [GLOBALG.A.P. solutions](https://globalgap.org/what-we-offer/solutions/) and [legacy solutions](https://globalgap.org/what-we-offer/legacy-solutions/) — use current scope-specific documents and effective dates; do not treat discontinued livestock scope as active.

Regulations, schemes, catalogues, APIs, and browser capabilities are versioned dependencies. Assign owners and revalidate them before each supported-market release.
