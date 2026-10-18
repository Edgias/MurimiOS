# Multi-Tenant Farm & Livestock Management SaaS — Development Plan

**Status:** Draft v1
**Author:** Edgias Mudzukwa
**Date:** 28 August 2026

---

## 0. Read this first: scope reality check

The feature set described (central operations, crop planning, real-time field insights, analytics, machinery/IoT/ERP connectivity, traceability and compliance) across four customer segments is the product surface of Agrivi, Syngenta Cropwise, Trimble Ag and Farmers Edge. Those platforms represent 8-12 years of work and 50-200 engineers each. Adding livestock adds a second domain of comparable size.

Two consequences shape everything below:

1. **Build order matters more than architecture.** Every phase must be independently sellable so revenue funds the next phase. A 3-year build with no revenue until month 30 will not survive self-funding.
2. **The "360-degree platform" is the destination, not the entry point.** Nobody buys a 360-degree platform. They buy a fix for one urgent pain and then expand. Pick the pain.

The full architecture below is designed so the destination is reachable without rewrites. The roadmap in §11 is designed so you can stop at any phase and still have a business.

---

## 1. Product strategy: the four segments are one platform with three sides

The four segments in the brief are not four products. They are three structural roles over the same data:

| Role | Who | What they do in the system | Who pays |
|---|---|---|---|
| **Producer** | FARMS, ENTERPRISE (farming holdings) | Enter and generate the primary data: fields, crops, herds, operations, inputs, yields | Per hectare / per head / per seat |
| **Aggregator** | FOOD & BEVERAGE, RETAIL | Contract producers, set standards, consume producer data as evidence, audit it | Per supplier in network + platform fee |
| **Verifier** | Certification bodies, auditors, competent authorities | Read-only, scoped, time-boxed access to evidence | Free (drives adoption) |

**ENTERPRISE is not a segment, it is a deployment shape.** A farming holding is a producer with multi-farm hierarchy, consolidated reporting, role-based delegation and cost-centre accounting. Build it as a tenant capability (org hierarchy + roles + consolidation), not a separate product.

**FOOD & BEVERAGE and RETAIL are the same product** with different vocabulary. F&B contracts farmers for raw material; retail contracts for finished crop. Both need: supplier onboarding, contract/allocation management, standards enforcement, evidence collection, traceability queries, and a consumer- or auditor-facing report. Ship one module, brand it two ways.

**The commercial insight:** the aggregator side has the money and the regulatory gun to its head. The producer side has the data but low willingness to pay. This is a classic supply-chain network product. The aggregator pays and pushes producers onto the platform. That inverts your go-to-market: sell to the F&B/retail buyer, onboard their producer network for free or near-free, then upsell producers on the farm-management features they now find themselves half-using.

### Livestock is missing from the brief

The brief's feature list is entirely crop-focused (season planning, fields, agronomy, satellite, GAP). Livestock management is a genuinely different domain — animal-level identity, movements, breeding cycles, health treatments with withdrawal periods, feed conversion, milk recording — and it will not fall out of the crop model. §5.2 treats it as a first-class bounded context. See §11 for why it should be Phase 3, not Phase 1.

---

## 2. Recommended wedge

Given bootstrapping, SMB focus, and US/EU target markets, the highest-leverage entry point is:

> **EUDR-driven supply chain traceability for EU-market food and beverage buyers, with a free producer app underneath it.**

Rationale:

- **A hard deadline exists.** EUDR (Regulation EU 2023/1115, as revised by 2025/2650) applies from **30 December 2026** for large and medium operators and **30 June 2027** for micro and small operators. Covered commodities include **cattle, cocoa, coffee, palm oil, rubber, soy and wood** plus derived products. That is four months from now for the large-operator cohort.
- **The core obligation is exactly your data model.** Operators must file a Due Diligence Statement proving goods were produced on land not deforested after 31 December 2020, evidenced by **geolocation of the plot of production**. Geolocated field polygons + production records + a chain-of-custody event log is precisely what a farm management platform already holds.
- **It bridges to everything else.** The producer must draw field boundaries and record operations to satisfy the buyer. That is the on-ramp to crop planning, field insights and analytics. Cattle being an EUDR commodity also gives you a non-arbitrary reason to build livestock later.
- **It is a compliance purchase, not a productivity purchase.** Compliance budgets exist and close faster than "improve your yields" budgets.

**Alternative wedge if you want a purely producer-side start:** offline-first field record-keeping + spray/input logging + GlobalG.A.P. and assurance-scheme reporting for UK/EU SMB farms. Lower ceiling, faster to build, more crowded (fieldmargin, Farmplan Gatekeeper, Herdwatch).

Pick one. Do not run both.

---

## 3. Non-functional requirements that drive the architecture

These are the constraints that will actually break the design if you get them wrong.

| # | Requirement | Implication |
|---|---|---|
| NFR-1 | **Offline-first mobile is mandatory** | Fields and barns have no connectivity. Every field-capture workflow must work fully offline for days and sync deterministically. This is the single biggest source of failure in agtech products. |
| NFR-2 | **Geospatial is a first-class data type** | Field boundaries, plot geolocations, machine tracks, soil zones. PostGIS from day one, not bolted on. |
| NFR-3 | **Seasonality means bursty load** | 80% of operations data arrives in planting and harvest windows. Design for burst, but do not pay for peak capacity year-round. |
| NFR-4 | **Data must be auditable and immutable-in-effect** | Compliance evidence that can be silently edited is worthless. Append-only event log + corrections-as-events, never in-place mutation of recorded operations. |
| NFR-5 | **Cross-tenant data sharing is a core feature, not a leak** | The aggregator sees a slice of the producer's tenant. This must be explicitly modelled and consented, not implemented as an admin backdoor. |
| NFR-6 | **Time-series volume is high but low-value-per-row** | Sensor and weather data will dwarf transactional data in row count. Separate storage and lifecycle from operational tables. |
| NFR-7 | **Multi-language, multi-unit, multi-region** | ha vs acres, kg/ha vs lb/acre, °C vs °F, EU vs US crop protection product registries. Units belong in the domain model as value objects, not in the UI layer. |
| NFR-8 | **Data residency** | EU customers will require EU-hosted data. Design for region-pinned tenants early even if you only run one region initially. |

---

## 4. Multi-tenancy model

Reuse the approach already proven in KonnektCRM: **shared database, shared schema, discriminator column, EF Core global query filters**, with the additions below.

### 4.1 Tenancy strategy

- **Default:** shared schema, `TenantId` on every `IMustHaveTenant` entity, named query filters (`HasQueryFilter("Tenant", …)`) so pre-auth and cross-tenant sharing paths can bypass only the tenant filter via `IgnoreQueryFilters(["Tenant"])`.
- **Enterprise escape hatch:** per-tenant database for holdings that demand isolation. Same code path, resolved by a `ITenantConnectionResolver`. Charge for it.
- **Region pinning:** `Tenant.Region` determines connection string and blob account. A tenant never spans regions.
- **Row-level security as defence in depth:** enable PostgreSQL RLS on tenant-scoped tables with a session variable set by the connection interceptor. Query filters are an application-layer convenience; RLS is what saves you when a developer writes raw SQL.

### 4.2 Organisational hierarchy inside a tenant

ENTERPRISE requirements demand structure below the tenant boundary:

```
Tenant (billing + isolation boundary)
└── Organisation (legal entity, e.g. "Midlands Estates Ltd")
    └── Farm / Holding (physical unit, has a holding number, address, region)
        ├── Field  ──► CropZone (a crop in a field for a season)
        └── Site   ──► Herd / Mob / Pen  ──► Animal
```

Permissions, cost centres, and reporting roll up this tree. Implement as a materialised path or `ltree` column so ancestor queries are cheap.

### 4.3 Cross-tenant sharing (the aggregator link)

This is the part most platforms get wrong.

- A `SupplyRelationship` aggregate links an aggregator tenant to a producer tenant.
- It carries an explicit **data-sharing grant**: which entities, which fields, which seasons, which purpose, valid from/until, revocable by the producer.
- Aggregator reads go through a dedicated query path that projects producer data through the grant. Never through the producer's own repositories.
- Every cross-tenant read is logged. Producers can see exactly what their buyer looked at. This is a trust feature and a differentiator.
- Producers may operate a **linked tenant** (they own their data, buyer sees a slice) or a **hosted sub-account** (buyer created it for them, buyer sees everything, producer can claim and detach it later). Support both; the second is essential for onboarding smallholders.

---

## 5. Domain model — bounded contexts

Vertical slices per context, Clean Architecture, CQRS/MediatR, consistent with your existing stack.

### 5.1 Crop side

**Farm Registry**
`Organisation`, `Farm`, `Field`, `FieldBoundary` (versioned — boundaries change), `SoilSample`, `LandTenure` (owned/rented, critical for subsidy and EUDR plot evidence).
Key rule: a field boundary is temporal. Every operation references the boundary version current at its date.

**Season & Crop Planning**
`Season`, `CropZone` (crop + variety + field + season, the central planning unit), `RotationPlan`, `PlannedOperation`, `InputPlan`, `YieldTarget`, `Budget` (planned vs actual by cost category).
Supports the "Advanced Crop Planning" feature. Budget lines must reconcile to actual operations and ERP-synced costs.

**Operations**
`FieldOperation` (sowing, spraying, fertilising, tillage, irrigation, scouting, harvest) as an append-only record with `Status` (planned → assigned → in progress → completed → verified). `WorkOrder`, `OperationInput` (product, rate, actual applied), `MachineryUsage`, `LabourEntry`.
Every completed operation is an event in the traceability log (§9).

**Inputs & Inventory**
`Product` (crop protection, fertiliser, seed), `ProductRegistration` (per country — legal max rates, pre-harvest intervals, approved crops), `StockItem`, `StockMovement`, `Warehouse`.
The registry is the compliance engine: block or warn on an application that exceeds legal rate, is unapproved for the crop, or breaches the pre-harvest interval before a planned harvest date. This is a genuine differentiator and a real liability shield for the farmer. Sources: EU Pesticides Database, national registers, USDA/EPA for US.

**Field Intelligence**
`WeatherObservation`, `WeatherForecast`, `SatelliteIndexReading` (NDVI/NDRE/LAI/NDMI per field per date), `SensorReading`, `PestRiskModelRun`, `ScoutingReport`, `Alert`.

### 5.2 Livestock side

Deliberately separate contexts. Shares Farm Registry, Inventory, Traceability, Finance; shares nothing else with crops.

**Animal Registry**
`Animal` (official ID, EID/RFID tag, management tag, species, breed, sex, date of birth, dam, sire), `AnimalGroup` (herd/mob/pen/batch), `GroupMembership` (temporal), `Holding` (the registered premises).
Support both **individual-managed** species (cattle, high-value sheep, breeding stock) and **batch-managed** species (poultry, pigs, feedlot). The data model must not force individual identity on a 40,000-bird broiler house.

**Movements & Traceability**
`AnimalMovement` (on/off holding, internal transfer, birth, death, slaughter, sale), `MovementDocument`, `HoldingRegister`.
Statutory in EU/UK: cattle identification and movement reporting (Reg. 1760/2000 and the Animal Health Law, Reg. 2016/429), holding registers, movement standstills. Integrations: **BCMS/CTS** (England/Wales), **ScotEID**, **AIMS** (Ireland), national equivalents. This is a compliance obligation with a legal deadline per movement — a strong hook.

**Health & Medication**
`HealthEvent`, `Treatment`, `MedicineProduct` (with **meat and milk withdrawal periods**), `WithdrawalHold`, `VaccinationProgramme`, `MortalityRecord`, `VetVisit`.
**Killer feature:** the platform must refuse to allow dispatch of an animal, or sale of milk, still inside a withdrawal period, and surface the clearing date. This prevents a real, expensive, career-ending mistake. Also drives statutory medicine-book compliance and antimicrobial-usage reporting.

**Breeding & Performance**
`ServiceRecord` (natural/AI), `PregnancyDiagnosis`, `BirthEvent` (calving/lambing/farrowing with ease scores and litter data), `WeaningRecord`, `WeightSession`, `DailyLiveweightGain`, `BodyConditionScore`, `MilkRecording`, `LactationCurve`.
Analytics: calving interval, conception rate, days open, mortality by age cohort, DLWG by group, feed conversion ratio.

**Feed & Nutrition**
`Ration`, `FeedBatch`, `FeedAllocation`, `GrazingPlan`, `Paddock` (links back to Field — this is where crop and livestock actually meet on mixed farms), `GrassGrowthMeasurement`.
Home-grown feed transfers from crop inventory to feed inventory, carrying its own traceability lineage. That transfer is the single most important crop/livestock integration point.

### 5.3 Shared contexts

**Finance & Costing:** `CostCentre`, `CostAllocation`, `Transaction`, `GrossMargin` per crop zone / per head / per enterprise. This is what makes "Powerful Farm Analytics" more than charts.

**Supply Chain:** `SupplyRelationship`, `Contract` (contracted area or headcount, quality specs, price mechanism), `Delivery`, `QualityAssessment`, `Standard`, `ComplianceRequirement`, `Evidence`.

**Traceability:** see §9.

**Identity & Access:** two-phase login and `WorkspaceRole`-style model from KonnektCRM, extended with resource-scoped permissions (a farm manager sees only their farms; an agronomist sees only assigned clients).

---

## 6. Technical architecture

### 6.1 Stack

Stay with what you are fast in. Novelty here buys nothing.

| Layer | Choice | Note |
|---|---|---|
| API | .NET 10, minimal APIs, vertical slices, CQRS/MediatR | Same shape as KonnektCRM |
| Primary DB | PostgreSQL 17 + **PostGIS** | Non-negotiable for geospatial |
| Time-series | **TimescaleDB** extension on the same Postgres, or partitioned hypertables | Avoid a second database until volume forces it |
| Cache/queue | Redis + RabbitMQ (already in your stack) | |
| Object storage | S3-compatible (DO Spaces / Azure Blob) | Photos, raster tiles, documents, exports |
| Web app | Angular 22 (consistency with Konnekt) or React | Whichever you will maintain |
| Mobile | **.NET MAUI** (shares domain + validation with backend) or React Native | See §6.4 — this decision is load-bearing |
| Maps | MapLibre GL + OpenStreetMap base + your own vector tiles | Avoid Google Maps pricing |
| Hosting | Start on a small DO/Hetzner footprint; move to Azure when enterprise customers demand it | |

### 6.2 Geospatial design

- Boundaries as `geography(MultiPolygon, 4326)`. Compute area server-side with `ST_Area(geography)`; never trust client-computed hectares.
- Store a generated `geometry` projection column in an appropriate local CRS for fast distance/intersection ops.
- GiST indexes on all geometry columns.
- Import/export: Shapefile, GeoJSON, KML, ISOXML. Farmers arrive with existing boundary files from subsidy schemes (RPA in England, IACS/LPIS in the EU). **Importing their existing boundaries in one click is the highest-value onboarding feature in the product.**
- Boundary versioning: `FieldBoundary(FieldId, Geometry, ValidFrom, ValidTo)`. Never destructively update.

### 6.3 Satellite and weather

**Do not build a raster pipeline.** In phase 1, consume statistics, not imagery.

- **Sentinel-2 L2A** via Copernicus Data Space Ecosystem (free) or Sentinel Hub Statistical API (paid, dramatically less work). Request per-field-polygon aggregate index values (NDVI, NDRE, NDMI, LAI) at each cloud-free acquisition. Store scalars, not pixels.
- Cache a rendered PNG/COG tile per field per date in object storage for the visual layer. Storage is cheap; recomputation is not.
- Cloud masking via the scene classification band. Reject readings below a valid-pixel threshold, and store the threshold with the reading so results are reproducible.
- **Weather:** Open-Meteo for free forecast and historical reanalysis at field centroid; a paid provider (Meteomatics, Visual Crossing, DTN) when customers demand SLA-backed forecasts. Store observations and forecasts separately — forecast accuracy analysis is a later analytics feature.
- **Pest and disease risk:** start with deterministic, published agronomic models, not ML. Growing degree-day accumulation, Mills periods for apple scab, TOM-CAST for tomato blight, septoria and fusarium risk models for cereals. These are transparent, defensible, cheap to compute, and agronomists trust them. ML models come after you have three seasons of your own outcome data.

### 6.4 Offline-first mobile — the make-or-break component

Treat this as a distinct engineering project, not a thin client.

- **Sync intents, not state.** The device queues immutable commands (`RecordSpray`, `WeighAnimal`, `MoveGroup`) with client-generated UUIDs and a monotonic device sequence. The server replays them idempotently. Do not attempt bidirectional row-level state merge; it will fail at exactly the wrong moment.
- **Conflict policy per command type**, decided explicitly and documented. Most agricultural records are append-only observations and simply do not conflict. The few that do (editing a plan, closing a work order) resolve server-authoritative with a visible conflict inbox.
- Local store: SQLite, with a scoped subset (this farm, this season, active herds, product catalogue).
- Media: photos captured offline, queued, uploaded on reconnect with progressive backoff, referenced by client UUID so the record is valid before the photo lands.
- GPS: capture position and accuracy on every field record. Accuracy is evidence quality; store it.
- Test on a device in aeroplane mode for a full simulated working week before shipping. Then do it again after every release.

### 6.5 Integrations

Sequence by effort-to-value, not by prestige.

| Tier | Integration | Effort | When |
|---|---|---|---|
| 1 | Boundary file import (Shapefile/GeoJSON/KML/ISOXML) | Low | Phase 1 |
| 1 | CSV import/export for everything | Low | Phase 1 |
| 1 | Weather API | Low | Phase 1 |
| 2 | Satellite index API | Medium | Phase 2 |
| 2 | Accounting: **Xero, QuickBooks, Sage** (SMB reality) | Medium | Phase 2 |
| 2 | LoRaWAN / MQTT sensor ingest (The Things Network webhooks, generic MQTT broker) | Medium | Phase 2 |
| 3 | ISOXML task data import/export via **AgGateway ADAPT** | High | Phase 3 |
| 3 | Statutory livestock movement APIs (BCMS/CTS, ScotEID) | Medium, high value | Phase 3 |
| 4 | **John Deere Operations Center**, Climate FieldView, CNH, Claas Telematics | Very high, per-vendor certification | Phase 4+, customer-funded only |
| 4 | ERP: Dynamics 365 Business Central, SAP, NetSuite | Very high | Phase 4+, customer-funded only |

On machinery: telematics integrations are a swamp. Each vendor has its own auth, data model, certification programme and rate limits. Build the ADAPT-based file exchange first — it covers a large share of real usage because contractors already move task data on USB sticks. Only build a live telematics link when a named customer has signed for it.

On ERP: your BDO background is an asset here. Dynamics 365 Business Central is the credible first ERP because it is dominant in the SMB/mid-market accountancy channel, and that channel is a distribution route you personally understand.

### 6.6 Analytics

Resist the lakehouse instinct.

- **Phase 1-2:** materialised views + well-indexed aggregate tables refreshed on a schedule. Postgres will handle several hundred farms comfortably.
- **Phase 3:** a per-tenant star schema in the same Postgres (`analytics` schema), populated by a nightly job from the event log. Facts: operation, cost, yield, weight, movement. Dimensions: field, crop zone, variety, animal, group, product, season, date.
- **Phase 4:** ClickHouse or DuckDB-backed extract only if query latency actually degrades. Measure before you migrate.

The analytics that sell are comparative and monetary, not descriptive: gross margin per hectare by variety, cost per tonne by field, DLWG per pound of feed by group, which fields lost money last season and why. Charts of NDVI over time impress nobody who farms for a living.

### 6.7 AI features

Align with the AI-powered positioning of the holding company, but as features on a solid data foundation, not as the foundation.

Ranked by value-to-effort:

1. **Compliance narrative and DDS drafting.** Generate the due-diligence statement, the risk assessment narrative, and audit-pack summaries from structured records. High value, low risk, directly monetisable.
2. **Agronomic copilot over the tenant's own data.** Natural-language querying with tool calls into the query layer, never free-form SQL. "Which fields am I behind on nitrogen for?" Strictly scoped to the tenant.
3. **Scouting photo triage.** Pest/disease/deficiency identification from a field photo, always as a suggestion requiring agronomist confirmation, never an automated recommendation.
4. **Yield and forecast modelling.** Needs three seasons of your own data. Do not promise it in year one.
5. **Document extraction.** Parse invoices, delivery notes, lab reports, vet records into structured records. Unglamorous, genuinely loved by users.

Hard rule: no AI output ever writes directly to a compliance record. It drafts; a human commits.

---

## 7. API design

- REST, versioned (`/api/v1`), resource-oriented, `PagedResult<T>` and the filter/sort infrastructure already built for KonnektCRM.
- **Public API from Phase 2.** Aggregators and enterprise customers will demand programmatic access, and it is a defence against being seen as a closed silo. Per-tenant API keys with scoped permissions.
- **Webhooks** for operation completed, animal moved, threshold breached, compliance failed. Signed payloads, retries with backoff, replay endpoint.
- **Bulk endpoints** for import — hundreds of fields or thousands of animals at onboarding will time out a per-record API.
- Idempotency keys on all write endpoints (required by the mobile sync design anyway).

---

## 8. Data lifecycle and cost control

Sensor and satellite data will dominate storage. Plan retention on day one:

- Raw sensor readings: full resolution 90 days, hourly rollup 2 years, daily rollup forever.
- Satellite scalars: keep forever (tiny). Rendered tiles: 2 seasons, regenerate on demand after that.
- Operations, movements, treatments, traceability events: **keep forever, immutable.** These are legal records; retention obligations run 3-10 years depending on scheme and jurisdiction, and disputes arrive late.
- Photos: full resolution 1 year, then compressed derivative.

---

## 9. Traceability and compliance architecture

This is the segment-spanning core and deserves a deliberate design rather than per-report special cases.

### 9.1 Use GS1 EPCIS 2.0 as the event backbone

Model every traceability-relevant fact as an EPCIS-style event:

- **ObjectEvent** — something observed or acted upon (crop treated, animal weighed, batch inspected)
- **AggregationEvent** — things combined or split (harvest into bins, animals into a consignment)
- **TransformationEvent** — inputs become outputs (grain milled, milk processed, animal slaughtered)
- **TransactionEvent** — custody or ownership changes (delivery accepted)

Each event carries what/when/where/why (`bizStep`, `disposition`), which is exactly the vocabulary retail and F&B supply chains already speak. Identifiers: GLN for locations, GTIN for products, SSCC for logistic units, plus your internal IDs.

Why this matters: retailers and processors will eventually ask you to interoperate. If your traceability model is an ad-hoc set of joins, that request becomes a rewrite. If it is EPCIS, it is a serialiser. Adopting the model costs little now and saves a great deal later.

### 9.2 Standards to support, in order

| Standard | Segment | Priority |
|---|---|---|
| **EUDR** (EU 2023/1115 / 2025/2650) — geolocation, DDS, deforestation-free proof | F&B, Retail, cattle producers | **First** — deadline 30 Dec 2026 (large/medium), 30 Jun 2027 (micro/small) |
| **GlobalG.A.P.** (IFA crops and livestock) | Farms, Enterprise | Second |
| Farm assurance schemes: Red Tractor (UK), LEAF, QMS | UK farms | Second |
| **HACCP** | F&B | Third |
| **ISO 22005** (feed and food traceability) | F&B, Retail | Third |
| **ISO 9001**, ISO 22000 | Enterprise, F&B | Fourth |
| Organic certification (EU 2018/848, USDA NOP) | Niche, high margin | Opportunistic |
| CSRD / GHG Protocol Scope 3 agricultural emissions | Enterprise, F&B | Emerging, watch it |

### 9.3 Compliance engine design

Do not hard-code standards. Model them:

```
Standard → ControlPoint → EvidenceRequirement → EvidenceBinding
```

- `ControlPoint`: a single auditable requirement ("spray records retained for 3 years", "no application within PHI", "plot geolocation recorded to polygon accuracy")
- `EvidenceRequirement`: what satisfies it, expressed as a query over the event log
- `EvidenceBinding`: the actual records that satisfy it, evaluated continuously
- `ComplianceStatus` per control point per season: satisfied / at risk / failed / not applicable

The user-facing payoff is a live readiness dashboard rather than a panic before the audit, and an audit pack export that assembles evidence automatically. Adding a new standard becomes configuration plus a handful of new evidence queries.

### 9.4 Consumer transparency (RETAIL)

A public, unauthenticated resolver: scan a code on the pack, get a curated view of origin, practices and certifications. Built from the same event log, filtered through a `PublicDisclosurePolicy` per product so commercially sensitive data never leaks. Low engineering cost, disproportionate sales impact in retail demos.

---

## 10. Team, cost and honest timeline

Assuming you are the primary engineer with occasional contract help:

| Phase | Solo, part-time | Solo, full-time | Small team (you + 2) |
|---|---|---|---|
| Phase 1 | 8-10 months | 4-5 months | 3 months |
| Phase 2 | 8 months | 4 months | 2.5 months |
| Phase 3 | 10 months | 5 months | 3 months |
| Phase 4 | 12 months+ | 6 months | 4 months |

Roles you will eventually need and cannot substitute for: an **agronomist or livestock advisor** on retainer (domain correctness is not googleable, and wrong agronomic advice is a liability), and a **compliance specialist** for the certification mappings. Budget for both from Phase 1 revenue, not equity.

Infrastructure cost should stay under $200/month until roughly 100 producer tenants. The costs that scale non-linearly are satellite API calls and paid weather data. Meter them per tenant from day one so pricing can reflect them.

---

## 11. Roadmap

### Phase 0 — Foundations (4-6 weeks)

Platform skeleton: tenancy (query filters + RLS), org hierarchy, auth with two-phase login and resource-scoped roles, audit interceptor, PostGIS setup, units/measures value objects, i18n scaffolding, CI/CD, observability.

*Not sellable. Keep it short. Reuse aggressively from KonnektCRM.*

### Phase 1 — Traceable production record (3-5 months)

**Target buyer:** an EU-market F&B or retail buyer with an EUDR obligation.

- Farm/field registry with boundary import and drawing
- Season and crop zone setup
- Field operation recording (sowing, spraying, fertilising, harvest) with input products
- **Offline-first mobile capture** with photo and GPS
- EPCIS event log
- Supply relationship + data-sharing grants
- Aggregator portal: supplier list, onboarding invites, evidence status per supplier
- **EUDR module:** plot geolocation collection, deforestation-risk screening against public forest-cover baselines, DDS data assembly, export
- Basic operations reports and audit pack export

**Sellable as:** supply chain traceability and EUDR compliance.

### Phase 2 — Farm management proper (3-4 months)

**Target buyer:** producers already on the platform via their buyer, plus direct SMB farms.

- Crop planning and budgeting, planned vs actual
- Input inventory and stock movements
- Product registry with legal rate and PHI enforcement
- Weather integration and field-level forecast
- Satellite index tracking per field
- Deterministic pest and disease risk models
- Cost allocation and gross margin per crop zone
- Accounting integration (Xero/QuickBooks/Sage)
- GlobalG.A.P. and assurance scheme control-point mapping

**Sellable as:** a farm management platform. Producer-side subscription begins here.

### Phase 3 — Livestock (4-5 months)

**Target buyer:** mixed and livestock farms; beef and dairy supply chains.

- Animal registry, individual and batch
- Groups, movements, holding register
- Statutory movement reporting integrations (BCMS/CTS, ScotEID)
- Health, treatments, **withdrawal-period enforcement**, medicine book
- Breeding and performance records
- Weights and DLWG analytics
- Feed rations, grazing plans, feed conversion
- Crop-to-feed inventory transfer with traceability lineage
- Livestock traceability events feeding the same EPCIS log (unlocks EUDR cattle)

**Sellable as:** livestock management and beef/dairy chain traceability.

### Phase 4 — Enterprise and intelligence (6+ months)

- Multi-farm consolidation, cost centres, delegated approval workflows
- Star schema analytics and benchmarking
- Machinery integrations (ADAPT first, telematics customer-funded)
- IoT sensor and weather-station ingest
- ERP integration (Business Central first)
- AI copilot and document extraction
- Public API and webhooks (bring forward if a customer requires it)
- Consumer transparency resolver

---

## 12. Commercial model

| Tier | Who | Pricing basis | Indicative |
|---|---|---|---|
| Producer Free | Smallholders onboarded by a buyer | Free, capped area/headcount | £0 |
| Producer Pro | SMB farms | Per hectare + per head, banded, annual | £X/ha/yr with a floor |
| Enterprise | Holdings | Per hectare + per seat + consolidation module | Negotiated |
| Supply Chain | F&B, Retail | Platform fee + per active supplier per year | The revenue engine |
| Compliance modules | Any | Per standard, per year | Add-on |
| Verifier | Auditors, certifiers | Free | Adoption driver |

Per-hectare and per-head pricing is the sector norm; do not fight it. Put a floor under it so small farms are not free, and cap it so a 10,000 ha holding does not price itself into a custom deal you cannot service.

---

## 13. Principal risks

| Risk | Severity | Mitigation |
|---|---|---|
| **Scope defeats a bootstrapped build** | Critical | Phase gates. Do not start Phase 2 without Phase 1 revenue. |
| **Offline sync proves harder than estimated** | High | Intent-queue design, not state merge. Prototype it in Phase 0, not Phase 1. |
| **Crop + livestock dilutes focus** | High | Sequential, not parallel. Livestock only after crop side has paying customers. |
| **Agronomic or veterinary correctness errors** | High | Domain expert on retainer. Deterministic published models only. Never auto-commit AI output. Explicit disclaimers and liability limits in terms. |
| **Crowded market** (Agrivi, Cropwise, Trimble, fieldmargin, Gatekeeper, Herdwatch, TrustTrace, Agriplace) | High | Compete on the supply-chain link, not on farm-management features. The network is the moat, not the forms. |
| **EUDR timeline slips again** (it has twice) | Medium | Do not make the product EUDR-only. Build generic traceability with EUDR as the first configured standard. |
| **Long agricultural sales cycles** (one season minimum to prove value) | Medium | Compliance framing shortens this. Deadlines close deals faster than yield promises. |
| **Seasonal churn** | Medium | Annual contracts only. Monthly billing invites off-season cancellation. |
| **Data-ownership objections from producers** | Medium | Explicit, revocable, auditable grants. Make it a marketed feature. |

---

## 14. Decisions to make before writing code

1. **Which wedge** — supply-chain traceability, or producer-side record-keeping? (§2)
2. **Which geography first** — UK/EU or US? Determines compliance scheme priority, product registries, units, movement APIs. Do not attempt both.
3. **Which commodity first** — the EUDR list (cattle, coffee, cocoa, soy, palm, rubber, wood) or general arable? Narrow commodity focus makes the agronomic and compliance model tractable.
4. **Mobile framework** — MAUI (domain reuse, smaller ecosystem) vs React Native (larger ecosystem, duplicated validation logic). Affects everything downstream.
5. **Hosting region and residency posture** — EU-first changes provider selection.
6. **Whether to build livestock at all** in v1 scope, or position as crop-first with livestock on the published roadmap.

---

## Appendix A — Reference sources to track

- **EUDR:** Regulation (EU) 2023/1115; Regulation (EU) 2025/2650; Commission simplification package of 4 May 2026 (updated guidance, FAQs, draft delegated act on product scope, EUDR Information System implementing act)
- **Livestock ID:** Regulation (EC) 1760/2000; Animal Health Law (EU) 2016/429; national implementations (BCMS/CTS, ScotEID, AIMS)
- **Traceability:** GS1 EPCIS 2.0 and Core Business Vocabulary; ISO 22005
- **Farm assurance:** GlobalG.A.P. IFA v6; Red Tractor standards
- **Satellite:** Copernicus Data Space Ecosystem; Sentinel Hub Statistical API; STAC specification
- **Machinery:** AgGateway ADAPT; ISO 11783 (ISOBUS) part 10 task data
- **Emissions:** GHG Protocol Land Sector and Removals Guidance; CSRD ESRS E1/E4
