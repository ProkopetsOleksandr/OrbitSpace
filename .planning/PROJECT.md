# OrbitSpace

## What This Is

OrbitSpace is a personal "Life Operating System" (Life OS) built around the Identity-First philosophy. Instead of asking "what do I need to do?", it asks "who do I want to be?" — every goal, habit, and activity is a vote confirming the user's chosen identity. The Station (desktop/web) is the primary interface for strategic planning, deep reflection, goal architecture, and daily tracking.

## Core Value

The user can define their identity through the Manifest and see every daily action as a confirmation of who they're becoming — the connection between identity declaration and daily behavior must always be visible and meaningful.

## Requirements

### Validated

<!-- Shipped and confirmed valuable. -->

(None yet — ship to validate)

### Active

- [ ] Custom JWT authentication with Next.js BFF proxy pattern (httpOnly cookies, .NET token issuance)
- [ ] Manifest — free-form identity document with yearly snapshot archival
- [ ] Activity Grid — monthly tracking table with activities/metrics, intent/fact modes, zero double-entry
- [ ] Goals — tactical projects with tasks, activity links, feeds, and micro-hub pages
- [ ] Balance Wheel — life sectors scored 1-10 with radar/wheel visualization

### Out of Scope

- Daily Planner — deferred to v2 (core tracking works through Activity Grid + Goals)
- Habits module — deferred to v2 (identity confirmation starts with Manifest + Activities)
- Dashboard — deferred to v2 (useful once there's data to visualize)
- Knowledge Base — deferred to v2 (not core to identity-action loop)
- Reflection module — deferred to v2 (manual reflection possible without structured module)
- Metrics as standalone module — deferred to v2 (metrics tracked through Activity Grid)
- Timeline — deferred to v2
- Probe (mobile/PWA) — deferred to future platform expansion

## Context

- **Existing codebase**: Monorepo with `dotnet-web-api/` (.NET 10, Clean Architecture, C# 14) and `next-js-app/` (Next.js 16, React 19, TypeScript, FSD architecture)
- **Database**: PostgreSQL (migrated from MongoDB)
- **Auth architecture**: Custom JWT through Next.js BFF — Route Handlers proxy auth, Server Components fetch .NET API server-to-server, client interactions route through Next.js proxy. JWTs stored in httpOnly cookies, never exposed to browser JS
- **API pattern**: Frontend communicates through Next.js API proxy routes (never directly from browser). Backend exposes OpenAPI spec; frontend generates TypeScript types from it
- **Codebase map**: Available at `.planning/codebase/`
- **Domain model**: Extensively documented — 11 modules across 3 system levels (Strategic/Tactical/Operational)
- **Design principles**: Zero double-entry, no guilt accumulation, monthly active context, manual goal closure, composition over complexity

## Constraints

- **Tech stack**: .NET 10 + Next.js 16 + PostgreSQL — already established, no changes
- **Architecture**: Clean Architecture on backend, FSD on frontend — follow existing patterns
- **Auth**: Custom JWT with BFF proxy pattern — defense-in-depth (middleware + DAL + Server Actions)
- **API contract**: OpenAPI spec generated from .NET, TypeScript types auto-generated for frontend
- **Solo developer**: Single user building and using the system

## Key Decisions

| Decision | Rationale | Outcome |
|----------|-----------|---------|
| Custom JWT over Clerk | Full control over auth flow, BFF proxy pattern for security | — Pending |
| PostgreSQL over MongoDB | Relational data model fits better for cross-entity queries | — Pending |
| v1 = Manifest + Activity Grid + Goals + Balance Wheel | Core identity-action loop without overwhelming scope | — Pending |
| Daily Planner deferred | Activity Grid + Goals cover daily tracking; Planner adds convenience, not core value | — Pending |

---
*Last updated: 2026-02-14 after initialization*
