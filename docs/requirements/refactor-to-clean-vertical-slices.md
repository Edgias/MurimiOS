## Context

- Root namespace prefix: `EdgIQ`
- Target framework: `net10.0`
- Mediator: `EdgIQ.Dispatcher`
- Host project: `API`, kept separate from `Application`
- Use-case-facing abstractions live in `Application/Common/Abstractions/`

### Mediator notes

Handlers dispatch through `EdgIQ.Dispatcher`, not MediatR. Before moving anything:

- Read the Dispatcher registration in the composition root and match its
  assembly-scanning convention. If it scans by marker type or namespace, moving
  handlers will break resolution unless the scan target moves with them.
- Confirm whether Dispatcher exposes a pipeline/behavior abstraction. If it does
  not, `Application/Common/Behaviors/` should not exist — leave cross-cutting
  validation and transaction handling where the current code puts it and note the
  gap in the Phase 7 report. Do not invent a pipeline.
- Add a smoke test that resolves one handler of each shape (command, query) from
  the container after Phase 3, so a broken scan surfaces immediately rather than
  at runtime.

## Task

Refactor this codebase to the target structure below. This is a **structural
refactor only**. No behavior changes, no logic improvements, no renames of public
types, no dependency upgrades, no reformatting of untouched code.

If you find a genuine bug while moving files, write it down in the report at the
end. Do not fix it.

## Target structure

```
src/
  Domain/
    Common/                       AggregateRoot, Entity, ValueObject, IDomainEvent, Result
    {Aggregate}/                  one folder per aggregate, NOT per technical type
      {Aggregate}.cs
      {Aggregate}Id.cs
      {ValueObjects}.cs
      Events/
      Rules/
    Abstractions/                 domain-concept interfaces only (IClock, policies)

  Application/
    Common/
      Abstractions/               IAppDbContext, ICurrentUser, IEmailSender, ...
      Behaviors/                  only if Dispatcher supports a pipeline
      Pagination/                 PagedResult, QueryableExtensions, QueryOptions
      Exceptions/
    {Feature}/                    e.g. Companies, Deals, Loans
      {UseCase}/                  e.g. CreateCompany, GetCompanyById, ListCompanies
        {UseCase}Command.cs       or Query
        {UseCase}Handler.cs
        {UseCase}Validator.cs
        {UseCase}Response.cs      or Dto — co-located, never shared globally
      Shared/                     ONLY if 2+ use cases in this feature need it

  Infrastructure/
    Data/
      MurimiOSDbContext.cs
      MurimiOSDbContextFactory.cs
      Config/             one per aggregate, mirroring Domain folders
      Interceptors/
      Migrations/                 DO NOT TOUCH
    Identity/
    Email/
    Storage/
    BackgroundJobs/
    DependencyInjection/          per-concern extension methods

  API/
    Endpoints/
      {Feature}/
        {UseCase}/
          {UseCase}Endpoint.cs    one endpoint file per slice
    Infrastructure/               EndpointGroupBase, GlobalExceptionHandler, CurrentUser
    Program.cs

tests/
  Domain.UnitTests/
  Application.UnitTests/
  Infrastructure.IntegrationTests/
  API.FunctionalTests/
  ArchitectureTests/
```

`Migrations/` stays untouched even though nothing else is off-limits: moving or
regenerating migrations is a behavior change, not a structural one.

## Rules

1. **Dependency direction.** Api → Application → Domain. Infrastructure →
   Application. Domain references nothing. Infrastructure is referenced only by
   the composition root in `API`.
2. **Application never references ASP.NET Core.** No `HttpContext`,
   no `IHttpContextAccessor`, no `IActionResult`, no `IResult` in a handler.
   Request context reaches handlers through `ICurrentUser`.
3. **No cross-slice references.** A handler in `CreateCompany` must not reference
   any type inside another use-case folder. If two slices need the same thing,
   push it to Domain (a rule) or `Common` (a utility) — or duplicate it.
   Duplication between slices is acceptable and often correct.
4. **DTOs are co-located.** Delete any global `Dtos/`, `Models/`, or `ViewModels/`
   folder in Application by moving each type into the single slice that uses it.
   If a DTO is used by two slices, copy it into both.
5. **Domain groups by aggregate.** Dissolve `Entities/`, `ValueObjects/`,
   `Enums/`, `Events/`, `Exceptions/` in Domain and redistribute into aggregate
   folders. Keep only `Common/` and `Abstractions/`.
6. **Commands go through aggregates. Queries need not.** A read handler
   projecting straight from `IQueryable` to a DTO is correct — do not route it
   through a repository just for symmetry.
7. **Namespaces follow folders.** Update every namespace to match its new path.
   Enable `IDE0130` as a warning in `.editorconfig` if it isn't already.
8. **Preserve public type names.** Renames are out of scope. EF Core TPH
   discriminators and owned-type mappings key off class names — moving a file is
   safe, renaming the class is not.

## Execution phases

Work one phase at a time. After each phase: `dotnet build` and the full test suite
must pass, then commit. Do not start the next phase on a red build.

Use `git mv` for every file move so history is preserved. Never delete-and-recreate.

**Phase 0 — Inventory (no changes).**
Map the current structure. Produce a table of every file with its current path and
proposed destination. List every cross-slice reference, every globally shared DTO,
every abstraction currently in the wrong layer, and every ASP.NET Core type
reachable from a handler. **Stop and show me this plan before touching anything.**

**Phase 1 — Project skeleton.** Create or split projects. Set project references so
the dependency rule holds and `Application` has no ASP.NET Core package
reference. Empty folders are fine.

**Phase 2 — Domain.** Regroup by aggregate. Move misplaced abstractions out to
`Application/Common/Abstractions/`.

**Phase 3 — Application slices.** Create feature and use-case folders. Move
commands, handlers, validators, and DTOs in. Dissolve shared DTO folders last,
since that's where cross-slice coupling surfaces. Add the Dispatcher resolution
smoke test before closing this phase.

**Phase 4 — Infrastructure.** Regroup by technical concern. Split monolithic DI
registration into per-concern extension methods. Leave `Migrations/` untouched.

**Phase 5 — Api.** One endpoint file per slice, grouped by feature. Endpoints stay
thin: bind, dispatch, return `TypedResults`. Anything an endpoint currently does
beyond that moves into the handler unchanged.

**Phase 6 — Architecture tests.** Add `ArchitectureTests` using NetArchTest
or ArchUnitNET, asserting at minimum:

- Domain has no dependency on Application, Infrastructure, Api, or EF Core.
- Application has no dependency on Infrastructure or Api.
- No type under `Application/{Feature}/{UseCase}/` is referenced from a different
  use-case folder.
- No handler, command, or query type references `Microsoft.AspNetCore.*`.

**Phase 7 — Report.** Summarize what moved, what you had to duplicate to break
cross-slice coupling, any bug or dead code you found and left alone, whether
Dispatcher supports a pipeline, and anything you could not place cleanly.

## Stop and ask

Do not guess on any of these — stop and ask instead:

- A type is used by three or more slices and it is unclear whether it belongs in
  Domain, `Common`, or should be duplicated.
- A move would require changing an EF Core mapping, a migration, or a
  discriminator value.
- A handler contains logic that clearly belongs in the domain model. Note it for
  the report; do not move the logic.
- An endpoint contains logic that cannot move to a handler without touching
  ASP.NET Core types from Application.
- A file does not fit any folder in the target structure.
- The build cannot be made green without a behavior change.
