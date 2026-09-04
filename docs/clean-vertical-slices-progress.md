# Clean vertical slices: progress

## Completed refactor (2026-09-04)

The user has authorized continuing through every remaining phase without routine
approval pauses. This is backend-only structural work; no commits of existing
untracked implementation and no migration edits are authorized.

All refactor phases are complete with the documented compatibility exceptions.
The final solution build has zero warnings/errors; the full local suite has
226 passing tests, 28 existing opt-in skips and zero failures. See the
[final report](clean-vertical-slices-report.md) for scope and verification limits.

- Phase 2 completed: the remaining 34 bundles became 309 aggregate-owned
  declarations. A token-level comparison verified their bodies unchanged; 125
  migration/snapshot files remained byte-identical, EF reported no model changes,
  and the full existing suite passed (211 passed, 28 opt-in tests skipped).
- Phase 3 completed. Application now owns database, identity, storage and
  provider contracts plus existing pure compliance/import/reporting utilities.
  Concrete storage/network adapters stay in Infrastructure.
- The first 440 Dispatcher slices passed the full suite (219 passed, 28 skipped),
  including OpenAPI snapshot comparison, production-host command/query handler
  resolution, extracted-use-case tests and transport-boundary regressions.
  All 590 routes are now extracted, including the platform's 20 remaining
  registry/sync routes and 130 additional legacy routes. The full suite remains
  green (219 passed, 28 skipped), with the published API snapshot unchanged.
  Legacy typed-result metadata is retained explicitly at the HTTP boundary.
- Request context passes through `ICurrentUser`; output metadata uses a narrow
  host-owned port so writes preceding exceptions retain their timing. Application
  result values preserve declared payload types without ASP.NET dependencies.
- API wire DTOs remain at the boundary to preserve binding and OpenAPI identities;
  application DTO copies are owned by individual use cases. Published anonymous
  schema identifiers are stabilized against compiler ordinal changes.
- Phase 4 completed: 272 source files moved into Infrastructure concerns,
  including 189 explicit entity configuration classes. Registration is split by
  concern. The worker registers persistence ports without scanning HTTP use cases.
  The full suite passed (220 passed, 28 skipped), including a shared-model,
  two-tenant filter regression. EF reports no model changes and all 125 migration
  files remain byte-identical at their original physical location.
- Phase 5 completed: API and Worker hosts now live under `src/`. All 590 endpoint
  definitions have individual feature/use-case files; registration aggregators
  contain no route implementations. The suite passed (220 passed, 28 skipped).
- Phase 6 completed: the five requested test projects are organized and all five
  NetArchTest rules pass, including the 590-slice independence check. The final
  suite passes after the aggregate configuration placement audit; original API
  contracts and opt-in legacy response metadata remain covered.
- Phase 7 completed: the [refactor report](clean-vertical-slices-report.md)
  documents the moves, duplication, compatibility exceptions, unchanged issues
  and final verification. EF reports no pending model changes; all 125 original
  migration/snapshot files remain byte-identical and the 309-declaration Domain
  integrity check passes. No PWA changes, database migration runs or commits.

Recovery archives and machine-readable move/slice manifests are under the ignored
`output/refactoring/` directory. Syntax-aware transformations live in
`tools/Refactoring/`; their move/extraction commands are one-shot and refuse to
overwrite an existing baseline. No database migration has been applied.

## Earlier progress

The [first approved Domain move batch](domain-refactor-batch1.md) documents the
earlier 145 moves. Its remaining-work section is historical and superseded above.

The [pre-move inventory proposal](refactor-inventory.md) and its catalogues are
historical references. The skeleton notes below describe the earlier stage,
not the current Domain namespace/location state.

## Project skeleton (historical)

- Use `EdgIQ.MurimiOS` as the MSBuild root-namespace prefix across the solution.
- Add `src/Application`, targeting .NET 10 and referencing Domain only.
- Install NuGet packages `EdgIQ.Dispatcher` and
  `EdgIQ.Dispatcher.DependencyInjection`, both pinned to version `1.0.2`.
- Keep the core package in Application and the DI package/registration extension
  in Infrastructure: the DI package transitively requires `Microsoft.AspNetCore.App`.
  Its actual namespace is `Dispatcher.DependencyInjection`, unlike its README.
- Register Dispatcher from API and Worker through `AddMurimiOSApplication`,
  scanning the Application assembly via `ApplicationAssemblyMarker`.
- Route Infrastructure's project dependency through Application; API and Worker
  reference Application directly as well as Infrastructure for composition.
- Reserve `Application/Common/Abstractions` for contracts derived from extracted
  use cases. No speculative persistence or request-context interfaces were added.
- Add Platform tests for DI resolution and command/query handler scanning, and
  Application tests for absence of HTTP/Infrastructure assembly dependencies.

Dispatcher's installed package documentation exposes `IPipelineBehavior<TRequest,
TResponse>` and open-generic behavior registration. No production pipeline or
handlers have been introduced at this skeleton stage. Test handlers live only in
the test assembly and are not scanned by production registration.

## Deliberately unchanged at the skeleton stage (historical)

- Existing explicit `Edgias.MurimiOS.*` namespace declarations and public types
  remain unchanged. The new root-namespace properties do not rewrite declarations.
  Namespace updates belong with the subsequent file/slice moves.
- Existing Domain, Infrastructure, API and Worker directories have not moved.
- Both DbContexts, all migrations, snapshots and database mappings are untouched.
- Legacy MediatR domain-event/context dependencies remain; Dispatcher installation
  is not a migration of those existing event contracts.
- Existing endpoint business logic and direct Infrastructure use remain. The full
  clean-architecture dependency rules require later use-case extraction.
- No existing dependency versions were upgraded and no PWA changes were made.

This is the explicitly requested namespace/project/package foundation, not a
claim that the full vertical-slice refactor or its inventory is complete.

## Skeleton verification (historical)

- `dotnet build MurimiOS.sln`: passed with zero warnings and errors.
- `dotnet test MurimiOS.sln --no-build --no-restore`: 207 passed (206 Platform,
  1 Application); 28 opt-in integration/load tests skipped, none failed.
- Evaluated Application project references and frameworks: Domain only, core
  Dispatcher package only, and `Microsoft.NETCore.App`; no ASP.NET Core framework.
- No database migrations were applied and no commits were made; existing working
  tree changes were preserved.
