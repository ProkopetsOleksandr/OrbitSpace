# Stack Research

**Domain:** Life OS / Personal Productivity Platform
**Researched:** 2026-02-14
**Confidence:** HIGH

## Recommended Stack

### Core Technologies

| Technology | Version | Purpose | Why Recommended |
|------------|---------|---------|-----------------|
| .NET 10 | 10.0 | Backend API framework | Already decided. Clean Architecture, minimal APIs, OpenAPI generation |
| Next.js | 16.1.x | Frontend framework | Already decided. App Router with Cache Components (opt-in caching), React Compiler stable, View Transitions |
| React | 19.2 | UI library | Ships with Next.js 16. Server Components, useEffectEvent, View Transitions, Activity API |
| PostgreSQL | 16+ | Primary database | Already decided. Migrated from MongoDB. Strong for relational queries across entities |
| TypeScript | 5.7+ | Type safety | End-to-end types from OpenAPI spec through frontend components |

### Backend Libraries

| Library | Version | Purpose | When to Use |
|---------|---------|---------|-------------|
| Npgsql.EntityFrameworkCore.PostgreSQL | 10.0.x | EF Core PostgreSQL provider | All database access. Supports JSON columns, array types, advanced PG features |
| Entity Framework Core | 10.0.x | ORM | Code-first migrations, LINQ queries. New: JSON complex type mapping |
| FluentValidation | 11.x | Request validation | All API endpoint input validation |
| MediatR | 12.x | CQRS mediator | Command/query separation in Clean Architecture |
| Swashbuckle / NSwag | latest | OpenAPI generation | Auto-generate swagger.json for frontend type generation |
| BCrypt.Net-Next | 4.x | Password hashing | User authentication password storage |

### Frontend Libraries

| Library | Version | Purpose | When to Use |
|---------|---------|---------|-------------|
| TanStack Query | v5 | Server state management | All data fetching. HydrationBoundary for SSR, prefetchQuery in Server Components |
| shadcn/ui | latest | UI component library | All UI components. Copy-paste model, Radix UI + Tailwind v4 + React 19 compatible |
| Tailwind CSS | v4 | Styling | All styling. shadcn/ui is built on it |
| React Hook Form | 7.x | Form management | All forms with validation |
| Zod | 4.x | Schema validation | Shared validation schemas (client + server), runtime boundary validation |
| Recharts | 2.x | Data visualization | Balance Wheel radar chart, Activity Grid heatmaps, goal progress charts |
| openapi-fetch | 0.15+ | Type-safe API client | Frontend API calls with middleware for auth. Generated from OpenAPI spec |
| openapi-typescript | latest | Type generation | Generate TypeScript types from .NET OpenAPI spec |
| date-fns | 4.x | Date utilities | Activity Grid date handling, monthly navigation, midnight boundary logic |
| nuqs | 2.x | URL state management | Sync filters/pagination with URL search params |

### Development Tools

| Tool | Purpose | Notes |
|------|---------|-------|
| React Compiler | Auto-memoization | Stable in Next.js 16, eliminates manual useMemo/useCallback |
| openapi-typescript CLI | Type generation | Run in CI: `npx openapi-typescript $API_URL/swagger/v1/swagger.json -o ./lib/api/schema.d.ts` |
| ESLint + Prettier | Code quality | Standard Next.js config |
| dotnet-ef | EF Core migrations | `dotnet ef migrations add/update` for schema changes |

## Installation

```bash
# Frontend - Core
npm install @tanstack/react-query openapi-fetch zod react-hook-form @hookform/resolvers

# Frontend - UI
npx shadcn@latest init
npm install recharts date-fns nuqs

# Frontend - Dev
npm install -D openapi-typescript

# Backend (NuGet)
# Npgsql.EntityFrameworkCore.PostgreSQL
# MediatR
# FluentValidation.AspNetCore
# Swashbuckle.AspNetCore
```

## Alternatives Considered

| Recommended | Alternative | When to Use Alternative |
|-------------|-------------|-------------------------|
| Recharts | Nivo | If you need more visual polish and built-in theming. Nivo is heavier but more beautiful out of the box |
| Recharts | Chart.js (react-chartjs-2) | If you need canvas rendering for very large datasets (10k+ points). Not needed for personal tracking |
| shadcn/ui | Mantine | If you want a batteries-included library with built-in hooks. shadcn/ui gives more control |
| TanStack Query | SWR | If you want minimal API. TanStack Query is better for complex cache management and mutations |
| date-fns | dayjs | If bundle size is critical. date-fns is tree-shakeable and more comprehensive |
| EF Core | Dapper | For complex raw SQL queries where EF generates suboptimal SQL. Can use both side by side |

## What NOT to Use

| Avoid | Why | Use Instead |
|-------|-----|-------------|
| Redux / Zustand for server state | TanStack Query handles server state. Adding Redux creates duplicate caches | TanStack Query for server state, React Context/Zustand only for client-only UI state |
| Clerk | Over-engineered for solo developer app. Custom JWT gives full control | Custom JWT with BFF proxy pattern |
| Prisma (if considered) | .NET backend uses EF Core. No need for Node.js ORM | EF Core with Npgsql |
| CSS Modules / styled-components | shadcn/ui is built on Tailwind. Mixing paradigms creates inconsistency | Tailwind CSS v4 |
| moment.js | Deprecated, massive bundle size | date-fns |
| axios | Unnecessary abstraction over fetch. openapi-fetch provides type safety | openapi-fetch for typed calls, native fetch for proxy routes |

## Version Compatibility

| Package A | Compatible With | Notes |
|-----------|-----------------|-------|
| Next.js 16.1.x | React 19.2, Tailwind v4 | React Compiler stable |
| shadcn/ui (latest) | Tailwind v4, React 19, Radix UI | Updated for v4 + React 19 |
| TanStack Query v5 | React 19, Next.js 16 | HydrationBoundary works with App Router Server Components |
| EF Core 10 | .NET 10, Npgsql 10 | JSON complex type mapping is new |
| openapi-fetch 0.15+ | TypeScript 5.7+ | Middleware system for auth injection |

## Sources

- [Next.js 16 Blog Post](https://nextjs.org/blog/next-16) — Cache Components, React Compiler, routing overhaul
- [Next.js 16 Upgrade Guide](https://nextjs.org/docs/app/guides/upgrading/version-16) — Breaking changes, new features
- [Npgsql EF Core 10.0 Release Notes](https://www.npgsql.org/efcore/release-notes/10.0.html) — JSON complex types, new features
- [TanStack Query SSR Guide](https://tanstack.com/query/latest/docs/framework/react/guides/ssr) — Server Components hydration pattern
- [shadcn/ui Tailwind v4](https://ui.shadcn.com/docs/tailwind-v4) — Updated for Tailwind v4 + React 19

---
*Stack research for: Life OS / Personal Productivity Platform*
*Researched: 2026-02-14*
