# Preserved planning requests

Recovered on 18 October 2026 from the MurimiOS planning conversation, in chronological order. These are requirement records, not new instructions to execute during checkpointing. The original supplied documents are retained alongside this file. Repeated requests to continue and routine status questions are omitted. Selected-deliverable text is preserved as received, including any clipped words from the original selection.

The user's mobile choice is **Angular PWA instead of MAUI**. The later delivery direction is **backend first**. Current implementation limits are recorded in [the gap audit](../backend-gap-audit.md); [status.json](../deliverables/status.json) supplies navigation.

## Request 1

Develop a plan for developing a multi-tenant SAAS platform for farm and livestock management system. For farming it should be able to cover the following clientele type: FARMS: digital agriculture products offer a 360-degree overview of farm agronomy and business processes from season planning, field operations management, resource and finance management to agronomic decision-making based on real-time insights from fields. ENTERPRISE: enable enterprise farming holdings to have complete control over all agronomic operations, gain deep insights into all processes, optimize costs and improve yields with powerful analytics. FOOD AND BEVERAGE: digital solutions for food and beverage companies enabling them to achieve traceable food sourcing processes and direct farmer contracting activities while securing quality, food safety, sustainability, and transparency. RETAIL: digital solution for retail chains, enabling them to secure complete traceability report for their own or contracted crop production, and to provide customers with transparency and reliability about the origin, quality, and safety of the purchased products. It should have the following features: ##Central Farm Operations Management## One platform for all agricultural data and process management, easily accessible anytime and anywhere. Create a digital twin of your farm. ##Advanced Crop Planning## Plan better and budget your season with precision based on a 360 overview of agronomic, financial and operational plans by crop and field. ##Real-Time Field Insights## Make fact-based decisions based on real-time insights into weather conditions, pest risk and crop health by field. Analyze satellite data and operations management data by field. ##Powerful Farm Analytics## Gain deep insights into best performing fields, varieties and practices. Identify inefficiencies, make the right decisions to optimize costs, improve crop yields and profitability. ##Connected Machinery, IoT and ERP## Automate data exchange with machinery and connect weather stations and soil sensors. Integrate ERP software to synchronize inventory and costs of production. ##Traceability and Compliance Reports## Ensure complete crop production traceability and secure compliance with food quality, food safety and sustainability standards such as ISO 9001, Global GAP and HACCP. Use the attached plans as a reference. For the mobile app replace MAUI with Angular PWA

## Request 2

it seems your implementation is mainly focusing on the mobile app.

## Request 3

let's do the whole backend first.

## Request 4

are you referencing the plans i gave you at start?

## Request 5

Selected deliverables:

- Finish organisation hierarchy and resource-scoped authorization.
- Add immutable auditing and correction records
- Implement the transactional outbox and background worker.
- Complete PostGIS field-boundary versioning
- Add units, quantities, money, time, and reference catalogues
- Add OpenTelemetry, operational health checks, and seed-data generation.
- Implement the server side of offline synchronization: command log, idempotency, gap handling, and checkpoints.
- Build the crop production and traceability core.
- Build livestock records and withdrawal enforcement
- Add supply relationships, data grants, and audited cross-tenant acces

## Request 6

do Advanced Crop Planning

## Request 7

Selected deliverables:

- Finance and profitabilityMoney primitives and inventory quantities existCost allocation, labour/machinery costs, crop-zone and herd margins, enterprise consolidation and accounting reconciliatio

## Request 8

Selected deliverables:

- Central farm operationsFarms, fields, seasons, crop zones, operations, inventory and evidence existWork orders, assignments, scheduling, resources, approval workflows and operational calenda

## Request 9

Some of the features in the table below have already been implemented. Prioritize what has not been implemented and make annotations. This is still backend only work

| Product areaCurrent stateStill required |                                                                                        |                                                                                                                               |
| --------------------------------------- | -------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------- |
| Central farm operations                 | Farms, fields, seasons, crop zones, operations, inventory and evidence exist           | Work orders, assignments, scheduling, resources, approval workflows and operational calendar                                  |
| Advanced crop planning                  | Basic seasons and crop allocation exist                                                | Rotations, production plans, input/resource forecasts, budgets and planned-versus-actual comparisons                          |
| Finance and profitability               | Money primitives and inventory quantities exist                                        | Cost allocation, labour/machinery costs, crop-zone and herd margins, enterprise consolidation and accounting reconciliation   |
| Real-time field intelligence            | Not implemented                                                                        | Weather, satellite indices, scouting, pest/disease risk, alerts, map layers and data-freshness monitoring                     |
| Analytics                               | Basic operational reports only                                                         | KPI projections, yield/variety/practice comparisons, enterprise dashboards, benchmarking and analytics warehouse              |
| Machinery, IoT and ERP                  | Infrastructure seams only                                                              | Sensor gateway, TimescaleDB readings, weather stations, machinery/task files, telematics, accounting and ERP connectors       |
| Compliance                              | Traceability, recall, evidence and audit foundations exist                             | Versioned standards/control points, evaluators, evidence requirements, compliance dashboards, findings and corrective actions |
| Supply-chain portal                     | Relationships and data grants exist                                                    | Invitations, hosted supplier accounts, contracts, allocations, evidence requests and supplier-status portal                   |
| Retail transparency                     | Cross-tenant trace reads exist                                                         | Consumer trace pages, disclosure policies, origin/quality presentation and public transparency resolver                       |
| Livestock expansion                     | Registry, groups, movements, health, feed, treatments and withdrawal enforcement exist | Breeding, reproduction, milk records, grazing plans, DLWG/performance analytics, feed conversion and statutory adapters       |
| Imports and onboarding                  | Manual/API creation exists                                                             | GeoJSON, KML, Shapefile and ISOXML import jobs with validation and preview                                                    |
| External integrations                   | Outbox foundation exists                                                               | Public API, signed webhooks, delivery logs and integration credential management                                              |
| Enterprise architecture                 | Organisation hierarchy exists                                                          | Delegated approvals, multi-company consolidation, dedicated databases, second-region support and tenant migration tooling     |
| PWA completion                          | Installable shell, offline commands and evidence queue exist                           | Full workflows for all backend modules, conflict inbox, maps, dashboards, update/storage UX and physical-device testing       |

## Request 10

Selected deliverables:

- Shared provider-ingestion and credential infrastructure
- Real-time field intelligence
- GeoJSON/KML/Shapefile onboarding
- Livestock reproduction, milk, grazing and performance
- Compliance engine and supplier workflows
- Analytics warehouse and connected machinery/ERP
- Public APIs, retail transparency and enterprise deployment tooling

## Request 11

check again what has been implemented so far against this plan

## Request 12

Selected deliverables:

- Implement the actual EUDR workflow: plot screening, evidence snapshots, risk decisions, DDS assembly and export.
- Add concrete weather, satellite and regulatory-data adapters with scheduled ingestion.
- Supply version-controlled GlobalGAP/HACCP/ISO/EUDR standard packs and automated compliance evaluators.
- Complete livestock statutory adapters, movement documents and crop-to-feed traceability.
- Implement real MQTT/LoRa, machinery and ERP/accounting connectors.
- Turn enterprise placement and migration records into actual database routing and migration execution.
- Add scheduled analytics projections, retention jobs and warehouse refresh processing.
- Add API integration, database/RLS, contract, load, security and end-to-end tests.

## Request 13

what's left for the backend as a whole to be complete?

## Request 14

Selected deliverables:

- SaaS control planeAdd tenant provisioning/lifecycle, plans, entitlements, trials, subscriptions, billing, suspension, usage metering and per-tier limits. Current tenancy is an isolation boundary, not a complete commercial SaaS lifecycl

## Request 15

Selected deliverables:

- API protection and identity operationsAdd per-tenant/client rate limits and quotas, API-key rotation policies, device/session inventory, login/security events, account recovery controls and production IdP/MFA configuratio

## Request 16

Selected deliverables:

- Compliance audit packsProduction/evidence reports exist, but the controlled certification audit-pack export described in the plan is still missing. It needs versioned contents, scope statements, signatures/checksums and export auditing.
- Farm digital-twin depthThe registry currently centres on farms, fields and boundary versions. The original plan’s first-class SoilSample and LandTenure records are not present. Site infrastructure, soil zones and fuller land/lease history also need completing if “360-degree digital twin” is interpreted literally.
- Bulk master-data onboardingGeoJSON, KML, Shapefile and ISOXML onboarding exists. The plan’s “CSV import/export for everything” does not. Bulk farms, fields, animals, inventory, suppliers, opening balances and finance imports need validation, preview, error files and resumability.
- EPCIS interoperabilityInternal traceability, transformations, lineage, mass balance and recalls exist, but an explicit GS1 EPCIS 2.0 capture/query serializer and Core Business Vocabulary mapping are not evident. That is required for standards-based retailer and processor interchange.
- Supply-chain completionAdd supplier risk scoring and public self-service claiming/detachment of hosted supplier accounts. Contract pricing mechanisms and enterprise-specific contract reconciliation may also need greater depth.
- Launch-market livestock integrationsCore livestock production is complete. What remains is jurisdiction-specific movement submission and acknowledgement, statutory document formats, breed-registry interfaces, milk-recording submissions and regulator-specific deadlines. ScotEID is only an initial adapter, not complete international coverage
- Analytics maturityScheduled projections, retention, dashboards and benchmarking exist. Full roadmap completion still includes event-driven refresh, specialist-approved benchmark cohorts, historical star-schema depth, forecast accuracy, predictive yield/risk models and enterprise legal-company elimination rule
- Connected-data lifecycle automationAutomatically create future sensor/telemetry partitions and enforce retention for derived media and map products before production volume grows.

## Request 17

compare current backend against what's listed in the table below except the PWA stuff.&#x20;

| Product areaCurrent stateStill required |                                                                                        |                                                                                                                               |
| --------------------------------------- | -------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------- |
| Central farm operations                 | Farms, fields, seasons, crop zones, operations, inventory and evidence exist           | Work orders, assignments, scheduling, resources, approval workflows and operational calendar                                  |
| Advanced crop planning                  | Basic seasons and crop allocation exist                                                | Rotations, production plans, input/resource forecasts, budgets and planned-versus-actual comparisons                          |
| Finance and profitability               | Money primitives and inventory quantities exist                                        | Cost allocation, labour/machinery costs, crop-zone and herd margins, enterprise consolidation and accounting reconciliation   |
| Real-time field intelligence            | Not implemented                                                                        | Weather, satellite indices, scouting, pest/disease risk, alerts, map layers and data-freshness monitoring                     |
| Analytics                               | Basic operational reports only                                                         | KPI projections, yield/variety/practice comparisons, enterprise dashboards, benchmarking and analytics warehouse              |
| Machinery, IoT and ERP                  | Infrastructure seams only                                                              | Sensor gateway, TimescaleDB readings, weather stations, machinery/task files, telematics, accounting and ERP connectors       |
| Compliance                              | Traceability, recall, evidence and audit foundations exist                             | Versioned standards/control points, evaluators, evidence requirements, compliance dashboards, findings and corrective actions |
| Supply-chain portal                     | Relationships and data grants exist                                                    | Invitations, hosted supplier accounts, contracts, allocations, evidence requests and supplier-status portal                   |
| Retail transparency                     | Cross-tenant trace reads exist                                                         | Consumer trace pages, disclosure policies, origin/quality presentation and public transparency resolver                       |
| Livestock expansion                     | Registry, groups, movements, health, feed, treatments and withdrawal enforcement exist | Breeding, reproduction, milk records, grazing plans, DLWG/performance analytics, feed conversion and statutory adapters       |
| Imports and onboarding                  | Manual/API creation exists                                                             | GeoJSON, KML, Shapefile and ISOXML import jobs with validation and preview                                                    |
| External integrations                   | Outbox foundation exists                                                               | Public API, signed webhooks, delivery logs and integration credential management                                              |
| Enterprise architecture                 | Organisation hierarchy exists                                                          | Delegated approvals, multi-company consolidation, dedicated databases, second-region support and tenant migration tooling     |
| PWA completion                          | Installable shell, offline commands and evidence queue exist                           | Full workflows for all backend modules, conflict inbox, maps, dashboards, update/storage UX and physical-device testing       |

## Request 18

root namespace should be EdgIQ.MurimiOS. install `EdgIQ.Dispatcher`  from Nugget. Add the Application project

## Request 19

continue until you're done with all the tasks listed in the refactor plan i gave you. No longer wait to ask
