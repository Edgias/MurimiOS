# MurimiOS Angular PWA

This is the tracked copy of the original sibling WebApp source, added to the existing MurimiOS repository for the machine move. Read [the repository handoff](../NEW_MACHINE_HANDOFF.md) and [gap audit](../docs/backend-gap-audit.md). Selected workflows and IndexedDB sync/evidence queues exist; full PWA workflows, production identity and physical-device acceptance remain deferred under the backend-first plan.

From this directory run `npm ci`, `npm run build`, `npm test -- --watch=false` and `npm start`. The API defaults to `http://localhost:5000/api/v1`; local development uses tenant-header authentication. See the root handoff for supported Node/npm versions and backend setup.
