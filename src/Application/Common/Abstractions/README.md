# Application abstractions

Use-case-facing contracts belong here as existing use cases are extracted. Keep HTTP context,
ASP.NET Core result types, concrete persistence and provider implementations outside Application.

The first Domain move batch relocated the existing tenant/audit contexts, logging,
file-system, messaging, quotation, sales-order and sales-invoice service interfaces here.
Their names and signatures are unchanged. No speculative persistence or user-context
contracts have been introduced; further contracts will follow extracted use cases.
