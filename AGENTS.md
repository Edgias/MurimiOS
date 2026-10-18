# MurimiOS agent entry point

Read [NEW_MACHINE_HANDOFF.md](NEW_MACHINE_HANDOFF.md),
[docs/deliverables/status.json](docs/deliverables/status.json), and
[docs/backend-gap-audit.md](docs/backend-gap-audit.md) before resuming work.
The checkpoint is dated 18 October 2026 for the move on 19 October 2026
(Africa/Harare). It preserves work; it does not certify a release.

## Repository and requirements

- This clone is the former backend repository, with its original history and
  `master` branch at `https://github.com/Edgias/MurimiOS`. Backend projects remain
  directly under `src/`, `tests/`, and `tools/`. The PWA is now in `WebApp/`.
- `LandingPage/README.md` records that no landing-page implementation existed.
  Do not infer a completed website from that directory.
- The original supplied plans and refactor brief are retained unchanged under
  `docs/requirements/`. Their proposed architecture and schedules are requirements,
  not descriptions of the current implementation. Mobile means Angular PWA,
  superseding their MAUI references. `docs/requirements/planning-requests.md`
  preserves additional requirements formerly recorded only in the planning chat.
- The existing development plan retains older broad completion annotations.
  Use the corrected gap audit and status index to understand their limits.
  Backend-first sequencing remains the last product implementation direction;
  preserving WebApp does not authorize starting all deferred UI work.

## Architecture and preservation rules

- Namespace prefix: `EdgIQ.MurimiOS`; .NET 10; Dispatcher 1.0.2 use cases in
  `src/Application/{Feature}/{UseCase}`. Endpoints bind, dispatch and adapt results.
- Domain groups by aggregate. Application references Domain and has no ASP.NET
  dependency. Infrastructure implements Application ports. API and Worker are
  composition roots. Preserve the documented legacy MediatR and geometry exceptions.
- Keep wire contracts and the baseline in
  `tests/API.FunctionalTests/Contracts/platform-openapi.json` stable unless the
  task deliberately changes the API. Do not regenerate the baseline to hide a failure.
- Preserve tenant filters, forced RLS, organisation scope, explicit sharing grants,
  append-only auditing, outbox records and idempotent offline commands.
- All 125 frozen migration/snapshot files belong at
  `Infrastructure/Platform/Migrations/`; the Infrastructure project links them.
  Do not regenerate, rename, normalize or rewrite this migration history.
- The refactor report is historical; its "no commits authorized" restriction was
  superseded by the user's explicit 18 October checkpoint/commit/push instruction.
- Preserve unfinished source edits. Do not reset code or finish features merely
  to make a checkpoint green. Never force-push or rewrite existing history.
- Keep generated test evidence, recovery archives, build/dependency caches, local
  databases, runtime media, protection keys and private configuration out of Git.
  Keep test source, deterministic seeds and contract/design fixtures in Git.
- Do not deploy, call live providers, send real messages, mutate production data
  or perform payments without a separate task authorizing those actions.

## Verification

Use the setup and fresh-verification sequence in the handoff. A default test run
skips opt-in PostgreSQL and load checks. Historical pass counts are not current
acceptance. Integration tests must target a fresh disposable migrated database.
Refactoring tools are one-shot recovery utilities; do not rerun their move/extract
commands on this already-refactored checkout.
