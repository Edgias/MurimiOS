# New-machine handoff — 19 October 2026

Prepared 18 October 2026, Africa/Harare. This is a source/planning checkpoint for
the machine move. Fresh verification is deliberately left for the new machine.

## Start here

1. Read [AGENTS.md](AGENTS.md) and [the corrected backend gap audit](docs/backend-gap-audit.md).
2. Use [the deliverable index](docs/deliverables/status.json) to select the next task.
3. Retain [the full development plan](docs/agri-platform-development-plan.md) and
   [original requirements](docs/requirements/README.md). Older "complete" annotations
   mean a backend core exists; they do not close the limitations in the gap audit.
4. Restore dependencies and run fresh build, tests and model checks before editing.
   Reproduce database checks in a disposable database, separately from day-to-day data.

## One clone and its layout

```powershell
git clone --branch master https://github.com/Edgias/MurimiOS.git MurimiOS
cd MurimiOS
```

The existing backend repository/history and `origin/master` remain intact. Its
clone root contains:

```text
AGENTS.md, NEW_MACHINE_HANDOFF.md
MurimiOS.sln, global.json, compose.dev.yaml, .config/dotnet-tools.json
src/Domain/              aggregate-owned entities and invariants
src/Application/         Dispatcher use cases and Application ports
src/Infrastructure/      persistence, adapters, storage and background services
src/API/                 HTTP host and thin feature/use-case endpoints
src/Worker/              independently hosted background processing
Infrastructure/Platform/Migrations/   frozen linked migration history
tests/                   five test projects and contract fixtures
tools/SeedData/           deterministic, additive seed generator
tools/Refactoring/        historical one-shot source transformation utilities
docker/postgres/init/    development least-privilege runtime-role bootstrap
WebApp/                  Angular PWA source/configuration and package lock
LandingPage/README.md    records the previously empty directory
docs/                    plans, refactor records, setup guidance and status index
```

On the old machine the Git checkout is the workspace's `Backend/` directory.
The former sibling `WebApp/` source was copied into the checkout's `WebApp/`,
with all 30 source/configuration files hash-checked before handoff documentation
updates. The original sibling was left intact. There was no independent frontend
Git history. No parent repository, submodule or second clone is required. There
was no landing-page implementation to preserve.

## What was preserved

- Existing backend code, five test projects, OpenAPI fixture, migrations,
  deterministic seed/refactoring tools, development compose and config defaults.
- Eight pre-existing modified paths: Asset/DeleteGroupId and Farms/GetFarms
  endpoints; API `GlobalUsings.cs`, `Program.cs` and `RequestQuotaMiddleware.cs`;
  and Application Eudr/AddPlot request, command and handler. The substantive edits
  include import/format cleanup and PascalCase `CaseId`/`Request` command properties.
  These changes were preserved without completing or redesigning their workflows.
- All original product/engineering requirements and the refactor prompt, copied
  unchanged from the supplied documents to `docs/requirements/`. Additional
  planning requests and deliverable selections are retained in
  `docs/requirements/planning-requests.md`.
- The planning chat's later corrected gap analysis, including dedicated-database
  worker discovery, practice-based analytics and full machinery-task interchange.

The current checkout was missing all 125 migration/snapshot source files despite
the project linking `Infrastructure/Platform/Migrations/`. They were recovered
from the local refactor source backup, with every byte verified against the
original source-baseline SHA-256. The 62 migration/designer pairs and final
snapshot are now source-controlled at their expected path. `.gitattributes`
prevents line-ending conversion of these frozen files. Latest migration:
`20260903041246_FixDerivedMediaClaimResult`. No migration was regenerated or applied.
The recovery archive and hash-baseline evidence were left on the old machine.

## Actual implementation state

The structural backend refactor is recorded as complete in
[the report](docs/clean-vertical-slices-report.md): five backend projects, 590
extracted route definitions/use cases, aggregate-owned Domain and five test
projects. This is a structural count, including disabled/unregistered legacy
definitions; it is not a count of enabled production routes.

Backend cores exist for tenancy/access/RLS, audit/corrections/outbox, farms and
digital twin, crop production/planning/finance/work orders, inventory, livestock
and medicine withdrawal, traceability/EPCIS, supply relationships/portal workflows,
compliance/audit packs/EUDR, ingestion/field intelligence, geospatial/CSV onboarding,
connected data, analytics, public APIs/webhooks, retail disclosure, enterprise
routing/migration/consolidation, and provider-neutral SaaS/identity operations.
Their limits and remaining launch work are in the gap audit and status index.

The PWA is a working-source foundation with farm/field/season/crop-zone/input and
livestock workflows, evidence capture, IndexedDB command/media queues and sync.
It has two shell tests. It is not the complete client for the backend. API base
URL is currently hard-coded to `http://localhost:5000/api/v1`; authentication uses
the development tenant header. Production OIDC, full module workflows, conflict
inbox, maps/dashboards and storage/update/device acceptance remain deferred.

## Known failures, limits and decisions

- No fresh build/test/model validation was performed for this checkpoint. The
  September refactor record's 226 passes and 28 skips are historical only.
- Historical database sweep failure: analytics maintenance expected one processed
  job but found three retained due jobs. Unrelated digital-twin/master-data cases
  passed independently. Reproduce with isolated fresh databases and review global
  worker assertions; a focused pass does not settle shared-suite isolation.
- Legacy endpoints are disabled by default. `MurimiOSDbContext` needs MediatR that
  the production root does not register. Five legacy equipment definitions remain
  unregistered because equipment/category registrations share a method name.
- Several worker stores remain tied to one configured worker connection. Tenant
  API routing/migration is implemented, but complete dedicated-database job
  discovery and processing remains a priority.
- TimescaleDB, Redis and cloud CI/IaC are not implemented merely because original
  plans name them. Native PostgreSQL partitions are the implemented time-series
  approach; record an explicit acceptance or implementation decision.
- Existing user endpoint edits contain two trailing-space warnings from
  `git diff --check`; source was not reformatted to hide them.
- Named production providers, IdP recovery, payments/dunning, qualified compliance
  approval, official partner conformance, PITR/restore and regional failover drills
  remain open. No live services or deployment were used in checkpointing.

## Dependencies and fresh verification

Install Git, .NET SDK `10.0.400` with `latestPatch` roll-forward (the old machine
has `10.0.401`), Docker Desktop/Compose and a supported Node/npm combination.
The frontend CLI declares Node `^22.22.3 || ^24.15.0 || >=26.0.0`; the package
declares npm `11.17.0`. The old machine has Node `26.5.0`. Restore the lockfile
rather than changing dependency versions during the move.

From the clone root:

```powershell
dotnet tool restore
dotnet restore MurimiOS.sln
dotnet build MurimiOS.sln --no-restore -m:1 -nr:false -p:UseSharedCompilation=false
dotnet test MurimiOS.sln --no-build --no-restore -m:1 -nr:false
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet ef migrations has-pending-model-changes --project src/Infrastructure --startup-project src/API --context PlatformDbContext --no-build
Push-Location WebApp
npm ci
npm run build
npm test -- --watch=false
Pop-Location
```

Default tests deliberately skip database/load cases. The names of test projects
do not imply all their tests use a real database.

For local API development, start only local infrastructure and apply the restored
migrations as the migration owner. The factory reads `MURIMIOS_DB`; API and Worker
use their `ConnectionStrings` configuration with the restricted `murimios_app` role.

```powershell
docker compose -f compose.dev.yaml up -d
$env:ASPNETCORE_ENVIRONMENT = 'Development'
$env:DOTNET_ENVIRONMENT = 'Development'
dotnet ef database update --project src/Infrastructure --startup-project src/API --context PlatformDbContext
dotnet run --project tools/SeedData -- --help
dotnet run --project tools/SeedData
dotnet run --project src/API --urls http://localhost:5000
```

In separate terminals run `npm start` from `WebApp/`, and, when background
processing is needed, `dotnet run --project src/Worker` from the clone root with
`DOTNET_ENVIRONMENT=Development`. The worker's development publisher is Logging.
Do not reuse production connections or register live provider feeds for verification.
See [README.md](README.md) for local credentials/ports and
[provider configuration](docs/connected-provider-configuration.md) for adapter setup.

For database checks, create a new disposable database on the local container,
bootstrap its schema privileges with the retained init script, and migrate it:

```powershell
docker compose -f compose.dev.yaml exec postgres psql -U murimios -d postgres -c "CREATE DATABASE murimios_verify OWNER murimios;"
Get-Content docker/postgres/init/10-app-role.sql | docker compose -f compose.dev.yaml exec -T postgres psql -U murimios -d murimios_verify -v ON_ERROR_STOP=1
$env:MURIMIOS_DB = 'Host=localhost;Port=55432;Database=murimios_verify;Username=murimios;Password=murimios-dev'
dotnet ef database update --project src/Infrastructure --startup-project src/API --context PlatformDbContext
$env:MURIMIOS_TEST_CONNECTION = 'Host=localhost;Port=55432;Database=murimios_verify;Username=murimios_app;Password=murimios-app-dev;Include Error Detail=true'
$env:MURIMIOS_RUN_INTEGRATION_TESTS = '1'
dotnet test tests/API.FunctionalTests --no-build --filter 'FullyQualifiedName!~ApiLoadTests'
$env:MURIMIOS_RUN_LOAD_TESTS = '1'
dotnet test tests/API.FunctionalTests --no-build --filter 'FullyQualifiedName~ApiLoadTests'
Remove-Item Env:MURIMIOS_RUN_INTEGRATION_TESTS,Env:MURIMIOS_RUN_LOAD_TESTS,Env:MURIMIOS_TEST_CONNECTION,Env:MURIMIOS_DB
```

Use a new database name if `murimios_verify` already exists; do not erase a
database to rerun this recipe. Generate fresh results locally. No historical
evidence pack is needed to run the preserved tests.

## Resume order

1. Verify the restored source and preserved edits on the new machine. Triage
   reproducible failures without resetting user code; check API contract and model parity.
2. Close dedicated-database worker discovery/coverage before claiming enterprise
   placement works for every background service. Add focused cross-database checks.
3. Decide the time-series/Redis/deployment ADRs; implement the selected funded
   machinery task interchange and accounting/ERP providers through existing ports.
4. Add practice-based analytics, then a full multi-season crop-rotation planner and
   broader CRS/attribute/task-data imports where required by the launch scope.
5. Complete launch IdP/recovery and commercial billing adapters, specialist/provider
   certification, isolated security/load acceptance, CI/IaC and backup/restore drills.
6. Resume full PWA workflows and device/offline-week acceptance after the backend
   priorities are settled. Landing-page design/implementation remains separate work.

Status-index dependencies describe this order. Full original milestones, acceptance
budgets, AI-assistance ideas and launch decisions remain in the supplied plans;
they have not been dropped or marked complete by this checkpoint.

## Excluded local state

Generated logs/TRX/results/evidence/PDFs, refactor recovery archives, build outputs,
frontend dependencies/cache, local databases/media and Data Protection keys are not
part of this checkpoint. The already-tracked generated production-evidence PDF was
removed from the new Git tree while retained on disk; older history was not rewritten.
Versioned development defaults are examples only. Transfer real credentials or
encrypted-data key rings separately through the appropriate secret-management route
only if that data is intentionally carried over. Fresh development data needs neither.
