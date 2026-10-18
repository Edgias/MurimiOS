# Backend status and unresolved requirements

Preserved 18 October 2026 from the planning conversation's corrected comparison
and current repository inspection. This qualifies the older broad completion
annotations in [the development plan](agri-platform-development-plan.md).
Code presence is not production acceptance; tests were not rerun for this move.

| Area | Present in source | Remaining work or explicit limit |
| --- | --- | --- |
| Central farm operations | Work orders, assignments, conflicts, reservations, approvals, correction history, calendar and offline work commands. | Fresh concurrency/load and device acceptance. No major feature omission was identified against the requested row. |
| Farm digital twin | Tenure history, infrastructure assets, boundary versions, soil zones/samples/lab results and map overview. | Launch taxonomies, lab/provider exchange and acceptance; no full client map workflow. |
| Advanced crop planning | Revisioned scenarios, allocations, operations/resources/budgets, approval and variance. | Rotation is approved/superseded plan history. A dedicated multi-season planner with crop-sequence constraints remains open. |
| Finance | Balanced allocations, labour/machinery costs, crop/herd profitability, company consolidation, eliminations and reconciliation. | Consolidation selects a currency; automatic currency translation is absent. Select and implement a real accounting provider. |
| Field intelligence | Weather/satellite/scouting, deterministic versioned risk rules, alerts/map layers/freshness; Open-Meteo and Sentinel Hub adapters. | Crop/jurisdiction specialist fixtures and additional providers. Deterministic pest/disease rules are not a comprehensive forecasting service. |
| Analytics | Scheduled/event-driven projections, crop/variety comparisons, dashboards, governed benchmarks/models, lineage facts and dimension snapshots. | Management-practice comparisons are missing. Warehouse is PostgreSQL; model training/calibration, drift monitoring and data acceptance remain. |
| Machinery, sensors and ERP | MQTT/The Things Stack, channels/telemetry, canonical REST connectors, task/document lifecycles and partition/lifecycle maintenance. | Named vendor and bidirectional integrations plus real task-file processing/delivery. Lifecycle metadata is not complete ISOXML/ADAPT interchange. |
| Time-series/cache | Native PostgreSQL range partitions, rollups/retention and database-coordinated leases/quotas. | TimescaleDB and Redis are not implemented. Original requirements remain; accept the alternatives explicitly or implement them from measured need. |
| Compliance/EUDR | Versioned standards/evaluators, assessments/findings/actions, readiness packs, controlled signed audit packs and internal DDS workflow/export. | Finite evaluator coverage, specialist/legal review and official/provider interchange validation. Internal DDS JSON/export is not proof of live regulator submission. |
| Supply network | Grants/access audits, invitations/hosted accounts/contracts/allocations/evidence, supplier risk, claim/detach and pricing/reconciliation. | Production action-link delivery, approved risk weights and enterprise index-price feeds. Portal UI remains deferred. |
| Traceability/EPCIS | Internal lineage/mass balance/recalls and EPCIS 2.0.1 capture/query/CBV serialization. | Official GS1 conformance and trading-partner identifier/master-data profiles. |
| Retail | Harvest-lot disclosure policy, tokens, public resolver, origin/quality/compliance/lineage and access logs. | Finished-product/livestock-product extension, branded consumer UX and edge/legal controls. |
| Livestock | Registry, treatments/withdrawals, reproduction, milk/grazing/rations, DLWG/FCR and initial ScotEID/ICAR ADE adapters. | Further jurisdiction/provider certification; FCR uses allocated feed rather than measured consumption. |
| Imports | GeoJSON/KML/Polygon Shapefile/ISOXML boundaries and seven CSV targets with preview/checkpoint/error handling. | General CRS transformation/attribute import and full ISOXML machinery/task interpretation. Current Shapefile handling assumes WGS84 geometry. |
| Public integrations | Scoped/rotatable public API credentials, selected read resources, signed webhooks/replay and protected credentials. | Public API is selected farm/harvest/trace/analytics reads, not every module/write workflow. |
| Enterprise | Delegated approvals, legal companies/eliminations, placement, routed API databases and executable tenant migration. | Complete cross-database worker discovery; provision databases externally; regional replication/failover and DR infrastructure are absent. |
| SaaS/identity | Provider-neutral accounts/plans/entitlements/usage/billing records, quotas, session/device/recovery/MFA controls. | Select production OIDC and recovery adapter, payment processor, renewal/dunning, credential delivery and commercial acceptance. |
| Operations | Compose development services, health/telemetry code and deterministic seeding. | Production CI/CD/IaC, supported-market deployment, WAF/security review, PITR/restore, RPO/RTO/failover drills and monitoring. |
| PWA | Angular shell, selected farm/crop/livestock workflows, offline command/media repository, sync and two shell tests. | Full module workflows, production identity, conflict inbox, maps/dashboards, storage/update UX and physical-device offline-week acceptance. Backend-first sequencing remains in effect. |
| Landing page/AI/ADRs | Original requirements are retained. No landing-page source existed. | Landing page is unimplemented; planned AI-assisted extraction/drafting/copilot and original ADR backlog remain unaccepted future work. |

## Known unchanged backend issues

- Legacy production composition does not register the MediatR dependency needed
  by `MurimiOSDbContext`; legacy endpoints remain disabled by default.
- Legacy equipment/category registration has a naming collision; five equipment
  definitions remain unregistered. Extracted route-definition counts include these.
- The historical shared database sweep observed an analytics global-maintenance
  assertion expecting one processed job but seeing three retained due jobs. This
  requires fresh isolated reproduction and suite-level isolation work.
- Some existing Infrastructure tests are in-memory or adapter tests; their project
  name does not establish real PostgreSQL coverage. Opt-in checks must be enabled.

## Delivery and decisions

Follow [the handoff](../NEW_MACHINE_HANDOFF.md) and
[status index](deliverables/status.json). The original development plan's phases
and engineering plan's M0–M4 milestones/exit budgets remain requirements, not
acceptance results. The initial mobile-framework ADR is superseded by the user's
Angular PWA instruction. Other original ADRs still need explicit records: module
shape, isolation/dedicated placement, intent-sync/conflicts, geometry/versioning,
time-series, EPCIS event model, data-driven compliance and analytics growth thresholds.
No formal ADR collection or production pipeline was found in this checkpoint.
