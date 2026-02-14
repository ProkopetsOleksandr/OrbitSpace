# Research Summary: OrbitSpace

**Domain:** Life OS / Personal Productivity Platform
**Researched:** 2026-02-14
**Overall confidence:** MEDIUM-HIGH

## Executive Summary

OrbitSpace's Identity-First philosophy is genuinely novel in the productivity space — no major competitor builds around "who before what." The .NET 10 + Next.js 16 + PostgreSQL stack is well-suited for this application, with strong library support for every module. The key technical challenge is the Activity Grid: a 2D matrix data model that requires careful schema design and query optimization to avoid the performance death spiral common in grid-based tracking tools.

The biggest risk isn't technical — it's onboarding. Productivity apps have 17% day-one retention. OrbitSpace compounds this with an unfamiliar concept (identity-first) that requires multiple steps before value becomes clear. Every module needs an onboarding state that demonstrates value immediately rather than showing an empty screen.

The zero double-entry system is the most architecturally complex feature. Checking an activity must atomically update the Activity Grid AND any linked Goal feeds. This demands transactional consistency with domain events — not fire-and-forget async patterns. Get this wrong and the core value proposition breaks.

The auth architecture (custom JWT with BFF proxy, httpOnly cookies) is well-designed and follows post-CVE-2025-29927 best practices with defense-in-depth. This should be built first as foundation for everything else.

## Key Findings

**Stack:** .NET 10 + EF Core 10 (Npgsql) + Next.js 16.1 (React 19.2) + TanStack Query v5 + shadcn/ui (Tailwind v4) + Recharts for visualizations. All current, well-supported, compatible.

**Table Stakes:** Auth, Manifest CRUD with auto-save, Activity Grid with fact/intent modes, Goals with tasks and sector linking, Balance Wheel with radar chart. Missing any of these makes v1 feel incomplete.

**Architecture:** BFF proxy pattern for security, Server Component prefetch + HydrationBoundary for performance, CQRS with MediatR for backend organization, domain events for zero double-entry cascading.

**Critical Pitfall:** Zero double-entry partial failures — if a check-in updates the grid but fails to update the goal feed, data becomes inconsistent. Must be transactional.

## Implications for Roadmap

Based on research, suggested phase structure:

1. **Foundation & Auth** — JWT auth system, user entity, project scaffolding
   - Addresses: AUTH features, timezone handling
   - Avoids: Security pitfalls, middleware-only auth

2. **Balance Wheel** — Sectors, scoring, radar chart
   - Addresses: WHEEL features
   - Avoids: Building dependent modules first (Goals need sectors)

3. **Manifest** — Identity document CRUD with auto-save
   - Addresses: MANIFEST features
   - Avoids: User abandonment (identity foundation creates early value)

4. **Activity Grid** — Monthly tracking with fact/intent modes
   - Addresses: GRID features, monthly context
   - Avoids: Grid performance pitfall (correct schema from start)

5. **Goals** — Goal CRUD, tasks, sector linking, activity linking
   - Addresses: GOAL features, zero double-entry
   - Avoids: Circular sync (unidirectional events from Grid → Goals)

6. **Integration & Polish** — Zero double-entry wiring, onboarding states, edge cases
   - Addresses: Cross-module integration, onboarding
   - Avoids: User abandonment, incomplete feel

**Phase ordering rationale:**
- Auth must be first (everything depends on it)
- Balance Wheel before Goals (Goals link to sectors)
- Manifest early (low complexity, high identity value, validates core concept)
- Activity Grid before Goals (Grid is source of truth for zero double-entry)
- Integration last (all modules must exist before wiring them together)

**Research flags for phases:**
- Phase 4 (Activity Grid): Likely needs deeper research — complex data model, grid rendering performance
- Phase 5 (Goals + zero double-entry): Needs careful domain event design
- Phase 1 (Auth): Standard patterns, unlikely to need additional research

## Confidence Assessment

| Area | Confidence | Notes |
|------|------------|-------|
| Stack | HIGH | All versions verified, compatibility confirmed |
| Features | MEDIUM | Table stakes based on competitor analysis + domain docs. Prioritization based on project context |
| Architecture | HIGH | BFF proxy, CQRS, HydrationBoundary are established patterns with official documentation |
| Pitfalls | MEDIUM | Domain-specific pitfalls derived from mechanics analysis. Performance claims need validation with actual data |

## Gaps to Address

- Email service for password reset — not yet decided (SendGrid? Resend? AWS SES?)
- Rich text editor for Manifest — TipTap vs Lexical vs plain markdown?
- Activity Grid rendering approach — HTML table vs CSS Grid vs virtualized grid library?
- Deployment strategy — not discussed yet (Docker? Vercel + separate .NET host?)

---
*Research summary for: OrbitSpace*
*Researched: 2026-02-14*
