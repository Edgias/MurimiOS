# Clean vertical-slice refactor report

## Scope

Backend-only structural refactor against the supplied
`refactor-to-clean-vertical-slices.md`, using the requested root namespace
`EdgIQ.MurimiOS`. Business rules, public type simple names, database semantics and
existing dependency versions are preserved. The Angular PWA is untouched.

## Result by phase

| Phase | Result |
| --- | --- |
| 0 — Inventory | Recorded original paths, proposed destinations and coupling in `refactor-inventory.md` and its linked catalogues; retained as historical baselines. |
| 1 — Skeleton | Added Application, project dependency boundaries and Dispatcher registration. |
| 2 — Domain | Regrouped by aggregate under `src/Domain`; after the earlier 145 moves, split the remaining 34 bundles into 309 aggregate-owned declarations. |
| 3 — Application | Extracted 590 Dispatcher use cases, including 135 legacy endpoint definitions, into feature/use-case folders. Added persistence, actor/context, storage and provider ports. |
| 4 — Infrastructure | Regrouped 272 source files by technical concern, including 189 explicit entity configurations; split registrations by concern. |
| 5 — API | Created 590 individual endpoint files under `src/API/Endpoints`, retaining registration aggregators, binding, policies and transport contracts. Moved the worker host to `src/Worker`. |
| 6 — Tests | Organized the five requested test projects and added executable NetArchTest architecture rules. |
| 7 — Report | This document records moves, compatibility decisions, verification and intentionally unchanged issues. |

The solution now contains `src/Domain`, `src/Application`, `src/Infrastructure`,
`src/API`, `src/Worker` and the five test projects: `Domain.UnitTests`,
`Application.UnitTests`, `Infrastructure.IntegrationTests`, `API.FunctionalTests`
and `ArchitectureTests`. Existing seed-data tooling remains outside the hosts.

## Boundaries and duplication

- Each endpoint binds input, dispatches through `EdgIQ.Dispatcher`, and converts
  the application result to ASP.NET typed results. Application has no ASP.NET
  dependency. `ICurrentUser`, response metadata and lazy incoming-content ports
  preserve request access, header-write timing and stream behavior.
- Wire DTOs stay at the HTTP boundary to preserve binding and published schema
  identities. Application DTOs and endpoint-local helpers are copied into their
  owning use cases where sharing would create cross-slice dependencies. There is
  no global Application DTO/model folder.
- Existing reusable compliance, import, finance, supply-network and traceability
  utilities live in Application Common. Analytics fact queries and livestock
  regulatory defaults were extracted from endpoint helpers into shared utilities.
  Workflow logic was not redesigned or moved into aggregates during extraction.
- Concrete persistence, cryptography, media, regulatory adapters and external IO
  live in Infrastructure behind Application ports. API types outside its
  composition root do not depend on Infrastructure. Worker remains a separate
  composition root, with its existing host-specific processing and scheduling.
- Query projections remain direct queries. Persistence ports expose the existing
  EF query/transaction surface rather than introducing repositories or changing
  transaction boundaries, tenant filters, sync processing or outbox behavior.

## Dispatcher and retained dependencies

`EdgIQ.Dispatcher` and `EdgIQ.Dispatcher.DependencyInjection` are pinned to 1.0.2.
The core package lives in Application; the DI package lives in Infrastructure
because it transitively requires ASP.NET Core. Its registration namespace is
`Dispatcher.DependencyInjection`. The API scans `ApplicationAssemblyMarker`;
the worker registers persistence contracts without scanning HTTP use cases.

The installed Dispatcher supports `IPipelineBehavior<TRequest, TResponse>` and
open-generic behavior registration. No production pipeline was invented: existing
validation, authorization and transaction behavior stays where it operated before.
Production-host tests resolve both a command and query handler and dispatch a query.

As previously approved, Domain retains its existing MediatR 12.4.1 domain-event
contracts and NetTopologySuite 2.6.0 geometry dependency. Application retains the
existing EF Core Relational 10.0.11 version for its persistence ports. Thus Domain
has no project/persistence/HTTP dependency, but it is not a zero-package assembly.
NetArchTest.Rules 1.3.2 is a new test-only dependency. No existing dependency was
upgraded.

## EF, namespaces and compatibility exceptions

- All 125 migration/snapshot files remain physically under the original
  `Infrastructure/Platform/Migrations` directory, linked into the relocated
  project. Their bytes, namespaces and generated history were not rewritten.
  A project-level context alias lets frozen migration references compile.
- Entity configurations preserve the original invocation order. Tenant filter
  expressions are still built on the DbContext, preserving per-context tenant
  values when EF reuses a model. Explicit configuration methods avoid accidentally
  adding active-platform mappings to the legacy context's assembly scan.
- Folder namespaces use `EdgIQ.MurimiOS`; IDE0130 is enabled for backend sources.
  Frozen migration namespaces and the top-level host `Program` are deliberate
  exceptions. `Bins` avoids the SDK's case-insensitive `bin` source exclusion.
  The two original livestock treatment concepts retain their distinct namespaces
  and unchanged simple type names.
- Published anonymous OpenAPI schema identifiers are stabilized at the API
  boundary. Original routes, names, tags, authorization and typed-result metadata
  are retained; the pre-refactor platform OpenAPI snapshot is not regenerated.
- Existing API `App_Data` moved with the content root to `src/API/App_Data`.
  Existing media and protection keys were not regenerated. Worker relative storage
  paths continue to resolve to the same relocated API data directory.

## Existing issues deliberately left unchanged

- Opt-in legacy endpoints require `MurimiOSDbContext`, whose constructor depends
  on MediatR, but the existing production composition root does not register that
  mediator. Legacy endpoints remain disabled by default. The legacy metadata test
  supplies MediatR only in its test host; production activation is not repaired by
  this structural refactor.
- Existing commented-out legacy dependency registrations and other dead/commented
  code were retained. Business logic found in endpoints now resides unchanged in
  handlers; domain-model redesign and cleanup remain separate work.
- The original equipment and equipment-category registration methods both use the
  name `MapMachineryCategories`; the legacy registration imports only the category
  namespace. The five equipment endpoint definitions therefore remain unregistered,
  as before. The 590 count describes extracted definitions, not a promise that
  every definition is exposed by the default host.
- The requested test project names express ownership, not a claim that every
  Infrastructure test runs a real database. Existing in-memory and adapter tests
  retain their original execution style.

## Verification

Final verification on 2026-09-04:

- The complete solution builds with zero warnings and zero errors.
- The complete local suite passes: **226 passed, 28 skipped, zero failed**.
  Passing counts are Domain 40, Application 23, Infrastructure 129, API 29 and
  Architecture 5. API coverage includes the unchanged platform OpenAPI snapshot,
  opt-in legacy typed-response metadata, production Dispatcher resolution and
  transport-boundary regressions. The legacy test sets its feature flag before
  top-level startup reads it; no production configuration behavior was changed.
- Five NetArchTest rules pass, including all 590 use-case namespaces, dependency
  direction and absence of ASP.NET dependencies throughout Application.
- Domain integrity verification passes for all 309 extracted declarations; all
  125 original migration/snapshot files are byte-identical.
- EF reports no pending model changes.
- Tracked working-tree and staged diffs pass `git diff --check`; the final
  aggregate-configuration placement audit reports no mismatched owners.

Reproduce from the repository root after restoring packages:

```powershell
dotnet build MurimiOS.sln --no-restore -m:1 -nr:false -p:UseSharedCompilation=false
dotnet test MurimiOS.sln --no-build --no-restore -m:1 -nr:false
dotnet ef migrations has-pending-model-changes --project src/Infrastructure --startup-project src/API --context PlatformDbContext --no-build
```

The 28 existing opt-in database, RLS, integration and load checks require their
configured external test environment and remain skipped locally. Passing local
checks does not certify those scenarios or production readiness.

No database migrations were applied and no commits were created. Tracked moves
use Git history-preserving moves; previously untracked work was moved without
broad staging. Ignored recovery archives and move manifests are retained under
`output/refactoring`; the one-shot transformation utility is under
`tools/Refactoring`. Old ignored build-output directories were not destructively
cleaned.
