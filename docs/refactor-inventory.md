# Backend clean-architecture / vertical-slice inventory

Date: 2026-09-04. Historical pre-move proposal. The user approved the first Domain
batch; see the [implementation and verification report](domain-refactor-batch1.md)
for completed moves and the still-deferred ownership decisions.

The explicitly requested project skeleton is implemented. This pass fills the
missing inventory step from the supplied refactoring brief; it does not advance
to Domain or endpoint moves. The user's `EdgIQ.MurimiOS` namespace prefix takes
precedence over the brief's shorter `EdgIQ` example. PWA work is excluded.

## Inventory results

| Area | Files | Current situation |
| --- | ---: | --- |
| Domain | 145 | Legacy Entities/SharedKernel/Interfaces plus active Platform feature bundles. |
| Application | 3 | Project, assembly marker and abstractions README; no production use-case handlers yet. |
| Infrastructure | 220 | Two DbContexts, legacy configurations, platform services and 125 migration/snapshot files. |
| API | 197 | Business orchestration still lives in endpoint classes alongside HTTP handling. |
| Worker | 16 | Separate host for ingestion, outbox, webhooks, migration, MQTT and maintenance jobs. |
| Tests | 54 | Existing mixed Platform suites plus the new Application boundary test. |
| SeedData | 2 | Existing standalone tooling; preserve functionality. |
| Total | 637 | Source/project/config files; generated files and PWA excluded. |

The [file register](refactor-inventory-files.md) lists every file in this scope
with its proposed destination and tracked/untracked status. The
[boundary catalogue](refactor-inventory-boundaries.md) lists 22 interfaces,
424 API DTO/record declarations, confirmed endpoint helper coupling and HTTP
boundaries across 64 endpoint files. There are 589 literal HTTP method
registrations in source, not necessarily 589 active runtime endpoints: group
prefixes and feature flags are not expanded and health checks are separate.

Some destinations deliberately contain placeholders. Bundled aggregates,
endpoint methods, mixed tests and multi-use-case DTOs need individual ownership
decisions; this is not a compiler-resolved, final move script. No claim is made
that a lexical identifier scan proves every transitive/cross-slice dependency.

## Decisions and safety constraints

1. **Domain dependency policy.** Domain currently references MediatR 12.4.1 and
   NetTopologySuite 2.6.0. `BaseDomainEvent` implements MediatR's `INotification`;
   the legacy `MurimiOSDbContext` publishes those events after saving.
   Field boundaries and crop zones expose NetTopologySuite geometry types.
   Recommendation: retain both dependencies for the structural pass. Removing
   them is a separate event/geometry-contract change, not a file move. If
   "Domain references nothing" means no external packages, that requirement
   conflicts with preserving these existing contracts and needs explicit scope.
2. **Frozen migrations.** Leave all 125 migration/snapshot files at their current
   physical paths, with their content unchanged. Snapshots contain the existing
   fully qualified domain names. Namespace moves require relational-model
   verification; a name difference alone is not proof of a schema change.
   Relocating Infrastructure later needs linked migration compilation and a
   checked strategy for resolving the context types referenced by generated
   code. Do not update/regenerate snapshots or merge DbContexts to make it work.
3. **Shared contracts.** Move `ITenantContext`, `IAuditActorContext` and the six
   existing Domain/Interfaces contracts to Application/Common/Abstractions.
   Domain has no lexical consumers of those eight interfaces beyond their own
   declarations. Keep `TenantEntity` and `IAggregateRoot` in Domain/Common.
   Split `IRequestActorContext` from its API implementation without renaming it.
4. **Public type and wire compatibility.** Preserve simple type names, enum values,
   JSON field names, HTTP paths/statuses and sync schema versions. Legacy and
   active `LivestockTreatment` are different types; `InvoicedItem` belongs to two
   different legacy aggregates. They must remain in distinct namespaces. API
   `FieldResponse` and `SeasonResponse` have similar legacy/active collisions.
5. **HTTP-heavy flows.** Evidence/import streaming, EPCIS headers, public API
   authentication/quotas, and audit correlation cannot be moved wholesale into
   Application. Keep binding, streams/headers and HTTP responses at the API
   boundary; agree the plain input/result and context contracts before moving
   orchestration. Preserve transaction, audit, outbox and sync ordering.
6. **Working-tree baseline.** Of the scoped files, 274 are tracked and 363 are
   untracked. Do not stage or commit that existing implementation incidentally.
   `git mv` applies only to tracked paths. Before the move phases, agree whether
   to baseline the existing work in Git or allow recoverable filesystem moves
   for untracked files. Do not overwrite or discard either group.
7. **Worker extension.** The target tree omits Worker and SeedData. Keep their
   projects in place for now; update their references as dependencies move.
   Moving worker orchestration into Application is a separately reviewed part
   of the extraction, not removal of the host.

## Confirmed extraction hazards

- `AnalyticsEndpoints` and `PublicEnterpriseEndpoints` call
  `AnalyticsMaturityEndpoints.CurrentFacts`. Extract the shared selection
  contract, preserving its current-version filtering; one handler must not call
  another endpoint/handler just to reuse a query.
- `LivestockStatutoryEndpoints` calls default-profile installation in
  `LivestockExchangeEndpoints`. The shared statutory-profile behavior needs a
  non-endpoint owner.
- `PlatformContracts.cs` holds 51 public declarations spanning farms, inventory,
  evidence, livestock and offline sync. Split by consumers last, after use cases
  have boundaries; do not simply move it into Application/Common/DTOs.
- `OrganisationScopeAccess` is an API service performing database-specific scope
  resolution. Its fail-closed behavior and request-local cache must survive an
  abstraction split.
- `SyncCommandProcessor` is an 870-line API orchestrator spanning commands,
  transactions, permissions, idempotency, gaps, auditing and outbox writes.
  Preserve its complete transaction/replay behavior before considering smaller
  slices. Do not translate incidental properties named `Results` into a claim
  that this class depends on ASP.NET result types.
- `PlatformDbContext` is 3,736 lines. Splitting configuration and interceptors is
  a later phase; do not rewrite mappings as part of Domain regrouping.

## Existing discrepancies recorded, not fixed

- Legacy endpoint registration is feature-flagged. The legacy context requires
  `IMediator`, but no `AddMediatR` registration was found in the inspected hosts.
  This is a potential legacy startup/resolution gap, not a verified runtime bug
  and not authorization to replace the event pipeline during a structural move.
- `Infrastructure/Dependencies.cs` contains a commented registration stub.
  Keep it pending legacy-code review; do not silently delete it.
- New Dispatcher use cases use `EdgIQ.Dispatcher`; that installation has not
  migrated existing MediatR event contracts. Its DI package belongs outside
  Application because it carries an ASP.NET Core framework reference.

## Recommended first implementation batch after approval

1. Resolve the tracked/untracked baseline policy; take an explicit pre-move
   source/migration hash inventory without committing unrelated work.
2. Move the eight application-facing Domain interfaces into Application while
   preserving names and signatures; update their consumers and test doubles.
3. Relocate the Domain project and regroup the shared foundation, legacy
   aggregates and unambiguous single-aggregate platform files. Use
   `EdgIQ.MurimiOS.Domain.*` namespaces; retain the existing event/geometry
   packages and preserve public class names.
4. Leave ownership-ambiguous bundled models for explicit per-type review; do not
   claim Phase 2 complete while any remain. No endpoint business logic moves in
   this batch.
5. Verify project references, build, unit tests and EF model consistency. Verify
   the frozen migration hashes. Run DB/RLS/API tests only against an explicitly
   designated disposable test database; preserve their existing opt-in controls.

Later batches extract Application use cases, regroup Infrastructure, make API
endpoints thin, add architecture tests and report any remaining exceptions.
Enable IDE0130 when source namespaces move, with frozen generated migration
namespaces handled explicitly rather than rewritten.

## Verification in this inventory pass

- `dotnet ef migrations has-pending-model-changes --project Infrastructure/Infrastructure.csproj --startup-project API/API.csproj --context PlatformDbContext --no-build`
  completed successfully: no model changes since the last migration. This is a
  pre-refactor model baseline, not a claim that all future namespace moves are
  safe or that the database itself has been verified.
- This pass changes documentation only. No source moves, package changes,
  migrations, database writes or commits were performed.
- The previous skeleton verification remains: build with zero warnings/errors;
  207 tests passed and 28 opt-in integration/load tests skipped. Those test
  results are from the prior pass, not a new full-suite run here.

Approval is required before the first move batch, as requested in the supplied
brief. In particular, retain the existing Domain packages and settle the
untracked-file move policy before proceeding.
