# Architecture Research

**Domain:** Life OS / Personal Productivity Platform
**Researched:** 2026-02-14
**Confidence:** HIGH

## Standard Architecture

### System Overview

```
┌─────────────────────────────────────────────────────────────┐
│                     Browser (Client)                         │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐    │
│  │ Manifest │  │ Activity │  │  Goals   │  │ Balance  │    │
│  │  Page    │  │  Grid    │  │  Page    │  │  Wheel   │    │
│  └────┬─────┘  └────┬─────┘  └────┬─────┘  └────┬─────┘    │
│       └──────────────┴──────────────┴──────────────┘         │
│                    TanStack Query Cache                       │
│                    fetch('/api/proxy/...')                    │
└──────────────────────────┬───────────────────────────────────┘
                           │ httpOnly cookies (JWT)
┌──────────────────────────┴───────────────────────────────────┐
│                  Next.js 16 Server                            │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐        │
│  │ Route        │  │ Server       │  │ API Proxy    │        │
│  │ Handlers     │  │ Components   │  │ [...path]    │        │
│  │ (auth)       │  │ (prefetch)   │  │ (passthrough)│        │
│  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘        │
│         └─────────────────┴─────────────────┘                │
│                  Authorization: Bearer {JWT}                  │
└──────────────────────────┬───────────────────────────────────┘
                           │ server-to-server HTTP
┌──────────────────────────┴───────────────────────────────────┐
│                  .NET 10 API (Clean Architecture)             │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐     │
│  │ API      │  │ Application│ │ Domain   │  │ Infra    │     │
│  │ Layer    │──│ Layer     │──│ Layer    │  │ Layer    │     │
│  │(Controllers)│(Services) │ │(Entities)│  │(EF Core) │     │
│  └──────────┘  └──────────┘  └──────────┘  └────┬─────┘     │
└──────────────────────────────────────────────────┴───────────┘
                                                   │
                                          ┌────────┴────────┐
                                          │   PostgreSQL    │
                                          └─────────────────┘
```

### Component Responsibilities

| Component | Responsibility | Typical Implementation |
|-----------|----------------|------------------------|
| Browser Client Components | Interactive UI, forms, grids, charts | React 19 + TanStack Query + shadcn/ui |
| Next.js Server Components | Page-level data prefetch, auth gating | `fetchFromApi()` via DAL, HydrationBoundary |
| Next.js Route Handlers | Auth endpoints (login, register, refresh) | Set/clear httpOnly cookies |
| Next.js API Proxy | Forward authenticated client requests to .NET | Catch-all `[...path]/route.ts` |
| .NET API Layer | HTTP endpoints, request validation | Minimal APIs or Controllers + FluentValidation |
| .NET Application Layer | Business logic, CQRS commands/queries | MediatR handlers |
| .NET Domain Layer | Entities, value objects, domain rules | Pure C# classes, no framework dependencies |
| .NET Infrastructure Layer | Database access, external services | EF Core DbContext, repositories |
| PostgreSQL | Data persistence | Tables, indexes, JSON columns where appropriate |

## Recommended Project Structure

### Backend (.NET Clean Architecture)

```
dotnet-web-api/
├── src/
│   ├── OrbitSpace.Api/              # API Layer
│   │   ├── Controllers/             # or Endpoints/ for minimal APIs
│   │   │   ├── AuthController.cs
│   │   │   ├── ManifestController.cs
│   │   │   ├── GoalsController.cs
│   │   │   ├── ActivitiesController.cs
│   │   │   └── BalanceWheelController.cs
│   │   ├── Middleware/
│   │   │   └── JwtMiddleware.cs
│   │   └── Program.cs
│   │
│   ├── OrbitSpace.Application/      # Application Layer
│   │   ├── Auth/
│   │   │   ├── Commands/            # Login, Register, RefreshToken
│   │   │   └── Queries/             # GetCurrentUser
│   │   ├── Manifest/
│   │   │   ├── Commands/            # UpdateManifest, ArchiveManifest
│   │   │   └── Queries/             # GetManifest, GetManifestSnapshot
│   │   ├── Goals/
│   │   │   ├── Commands/            # CreateGoal, UpdateGoal, CloseGoal
│   │   │   └── Queries/             # GetGoals, GetGoalDetail
│   │   ├── Activities/
│   │   │   ├── Commands/            # CheckInActivity, LogMetric, SetIntent
│   │   │   └── Queries/             # GetMonthlyGrid, GetActivityDefinitions
│   │   ├── BalanceWheel/
│   │   │   ├── Commands/            # ScoreSectors, CreateSector
│   │   │   └── Queries/             # GetCurrentWheel, GetWheelHistory
│   │   └── Common/
│   │       ├── Interfaces/          # IRepository, IUnitOfWork
│   │       └── Behaviors/           # ValidationBehavior, LoggingBehavior
│   │
│   ├── OrbitSpace.Domain/           # Domain Layer
│   │   ├── Entities/
│   │   │   ├── User.cs
│   │   │   ├── Manifest.cs
│   │   │   ├── ManifestSnapshot.cs
│   │   │   ├── Goal.cs
│   │   │   ├── GoalTask.cs
│   │   │   ├── Activity.cs
│   │   │   ├── ActivityEntry.cs     # Daily check-in record
│   │   │   ├── Metric.cs
│   │   │   ├── MetricEntry.cs       # Daily metric value
│   │   │   ├── BalanceWheelSector.cs
│   │   │   └── BalanceWheelAssessment.cs
│   │   ├── ValueObjects/
│   │   │   ├── MetricValue.cs       # Union: number, time, rating, boolean
│   │   │   └── ActivityCode.cs      # 1-5 char visual code
│   │   └── Enums/
│   │       ├── MetricType.cs        # Number, Time, Rating, Boolean
│   │       └── GoalStatus.cs        # Active, Completed, Abandoned
│   │
│   └── OrbitSpace.Infrastructure/   # Infrastructure Layer
│       ├── Persistence/
│       │   ├── OrbitSpaceDbContext.cs
│       │   ├── Configurations/      # EF Core entity configs
│       │   └── Migrations/
│       ├── Repositories/
│       └── Services/
│           └── JwtTokenService.cs
```

### Frontend (Next.js 16 + FSD)

```
next-js-app/
├── app/                             # Next.js App Router
│   ├── (auth)/                      # Auth route group
│   │   ├── login/
│   │   │   ├── page.tsx             # Server Component
│   │   │   └── login-form.tsx       # Client Component
│   │   └── register/
│   ├── (app)/                       # Authenticated route group
│   │   ├── layout.tsx               # Auth-gated layout
│   │   ├── manifest/
│   │   │   └── page.tsx
│   │   ├── grid/
│   │   │   └── page.tsx
│   │   ├── goals/
│   │   │   ├── page.tsx
│   │   │   └── [id]/
│   │   │       └── page.tsx         # Goal detail/micro-hub
│   │   └── balance-wheel/
│   │       └── page.tsx
│   ├── api/
│   │   ├── auth/
│   │   │   ├── login/route.ts
│   │   │   ├── register/route.ts
│   │   │   └── refresh/route.ts
│   │   └── proxy/
│   │       └── [...path]/route.ts   # Catch-all proxy to .NET
│   └── middleware.ts                # Auth redirect (optimistic)
│
├── src/
│   ├── entities/                    # FSD: domain entities
│   │   ├── manifest/
│   │   ├── goal/
│   │   ├── activity/
│   │   └── balance-wheel/
│   ├── features/                    # FSD: user interactions
│   │   ├── auth/
│   │   ├── create-goal/
│   │   ├── check-in-activity/
│   │   └── score-sectors/
│   ├── widgets/                     # FSD: composed UI blocks
│   │   ├── activity-grid/
│   │   ├── goal-card/
│   │   └── balance-wheel-chart/
│   └── shared/                      # FSD: shared utilities
│       ├── api/                     # openapi-fetch clients
│       ├── lib/                     # dal.ts, schemas, helpers
│       ├── ui/                      # shadcn/ui components
│       └── queries/                 # TanStack Query options
```

### Structure Rationale

- **Backend modules by domain:** Each domain area (Auth, Manifest, Goals, Activities, BalanceWheel) is a vertical slice with Commands, Queries, and related logic. Clean Architecture layers are horizontal.
- **Frontend FSD:** Feature-Sliced Design separates entities (data models), features (user actions), widgets (composed UI), and shared code. Maps cleanly to OrbitSpace modules.
- **Route groups:** `(auth)` and `(app)` separate public and authenticated routes with different layouts.

## Architectural Patterns

### Pattern 1: BFF Proxy for Auth Security

**What:** Next.js sits between browser and .NET API. All client requests go through `/api/proxy/[...path]`, which reads JWT from httpOnly cookie and forwards as Bearer token.
**When to use:** Every client-side API call.
**Trade-offs:** +Security (tokens never in JS), +No CORS. -Extra hop (negligible for single-server deployment).

### Pattern 2: Server Component Prefetch + HydrationBoundary

**What:** Server Components call .NET API directly (server-to-server), prefetch into QueryClient, dehydrate to client via HydrationBoundary. Client components pick up data instantly.
**When to use:** Every page-level data load (Goals list, Activity Grid, Manifest view).
**Trade-offs:** +No loading spinner on first render, +SEO. -New QueryClient per request (correct — prevents data leaks between users).

### Pattern 3: CQRS with MediatR

**What:** Separate Command (write) and Query (read) handlers via MediatR. Commands return void or IDs. Queries return DTOs.
**When to use:** All .NET API endpoints. Keeps controllers thin.
**Trade-offs:** +Testable, +Single responsibility. -Boilerplate per endpoint.

### Pattern 4: Event-Driven Zero Double-Entry

**What:** When an Activity is checked in the grid, a domain event triggers updates to linked Goals (feed entry) and Habits (progress). Handlers react to `ActivityCheckedInEvent`.
**When to use:** Activity Grid check-in, metric logging.
**Trade-offs:** +Single source of truth, +Extensible. -Must handle event failures gracefully, -Ordering matters.

## Data Flow

### Request Flow (Client-Side)

```
[User clicks checkbox in Activity Grid]
    ↓
[Client Component] → fetch('/api/proxy/activities/check-in')
    ↓
[Next.js Proxy] → reads JWT from cookie → forwards to .NET
    ↓
[.NET Controller] → validates → sends CheckInActivityCommand
    ↓
[MediatR Handler] → updates ActivityEntry → raises ActivityCheckedInEvent
    ↓
[Event Handlers] → update Goal feed, Habit progress
    ↓
[Response] → 200 OK
    ↓
[TanStack Query] → invalidateQueries(['activities', 'goals'])
    ↓
[UI] → re-renders with fresh data
```

### Request Flow (Server-Side Prefetch)

```
[User navigates to /goals]
    ↓
[Server Component] → getSession() reads JWT from cookie
    ↓
[fetchFromApi('/api/goals')] → server-to-server to .NET
    ↓
[prefetchQuery] → stores in server-side QueryClient
    ↓
[dehydrate(queryClient)] → serializes cache
    ↓
[HydrationBoundary] → streams dehydrated state in HTML
    ↓
[Client Component] → useQuery picks up data instantly, no spinner
```

### Key Data Flows

1. **Activity Check-in:** Grid cell click → proxy → .NET → ActivityEntry created → domain events update linked entities → cache invalidated → UI re-renders
2. **Goal Creation:** Form submit → proxy → .NET → Goal entity created with sector link → response → cache invalidated → Goal appears in list
3. **Balance Wheel Assessment:** Score sliders → proxy → .NET → BalanceWheelAssessment created → historical comparison available → chart updates

## Database Schema Patterns

### Activity Grid (most complex)

```sql
-- Global activity/metric definitions (per user)
activities (id, user_id, name, code, type, is_active, created_at)
metrics (id, user_id, name, type, unit, is_active, created_at)

-- Monthly active context (which activities/metrics tracked this month)
monthly_contexts (id, user_id, year, month, created_at)
monthly_context_activities (context_id, activity_id, sort_order)
monthly_context_metrics (context_id, metric_id, sort_order)

-- Daily entries (the actual grid data)
activity_entries (id, user_id, activity_id, date, is_intent, is_fact, created_at)
metric_entries (id, user_id, metric_id, date, value_number, value_text, created_at)
```

### Goals with Tasks

```sql
goals (id, user_id, title, description, deadline, sector_id, status,
       predecessor_id, created_at, closed_at, close_summary)
goal_tasks (id, goal_id, title, description, is_completed, completed_at, created_at)
goal_activity_links (goal_id, activity_id)
goal_feed_entries (id, goal_id, entry_type, content, created_at)
```

### Balance Wheel

```sql
balance_wheel_sectors (id, user_id, name, sort_order, created_at)
balance_wheel_assessments (id, user_id, assessed_at)
balance_wheel_scores (assessment_id, sector_id, score)
```

## Scaling Considerations

| Scale | Architecture Adjustments |
|-------|--------------------------|
| 1 user (solo) | Monolith is perfect. Single PostgreSQL instance. No caching needed |
| 1-100 users | Same architecture. Add database indexes on (user_id, date) for grid queries |
| 100-10k users | Add Redis for TanStack Query server-side caching. Connection pooling for PostgreSQL |
| 10k+ users | Unlikely for personal Life OS. If needed: read replicas, CDN for static assets |

### Scaling Priorities

1. **First bottleneck:** Activity Grid queries — monthly grid loads 30 days x N activities. Index on (user_id, activity_id, date) is critical.
2. **Second bottleneck:** Goal feed queries with joins. Denormalize feed entry counts on goal entity.

## Anti-Patterns

### Anti-Pattern 1: Exposing JWT to Browser JavaScript

**What people do:** Store JWT in localStorage or regular cookies
**Why it's wrong:** XSS can steal tokens. Single XSS vulnerability = full account compromise
**Do this instead:** httpOnly cookies via BFF proxy. Browser JS never sees the token

### Anti-Pattern 2: God Entity for Activity Grid

**What people do:** Single wide table or JSON blob for entire monthly grid
**Why it's wrong:** Can't query individual days, activities, or metrics efficiently. Schema changes require data migration
**Do this instead:** Normalized tables: activity_entries and metric_entries as individual rows. Query with (user_id, date range)

### Anti-Pattern 3: Bidirectional Sync for Zero Double-Entry

**What people do:** When activity checked → update goal AND when goal updated → update activity
**Why it's wrong:** Creates circular updates, infinite loops, conflicting state
**Do this instead:** Unidirectional: Activity Grid is source of truth → domain events flow outward to Goals/Habits. Never reverse direction.

## Integration Points

### Internal Boundaries

| Boundary | Communication | Notes |
|----------|---------------|-------|
| Activity Grid ↔ Goals | Domain events (ActivityCheckedInEvent) | One-way: Grid → Goals. Never reverse |
| Goals ↔ Balance Wheel | Foreign key (sector_id) | Goal links to sector. Sector deletion blocked if goals exist |
| Manifest ↔ Everything | No DB link | Connection is philosophical, not technical. UI can show context |
| Auth ↔ All modules | JWT middleware | Every request validated. User ID extracted from token claims |

## Sources

- [Next.js 16 App Router docs](https://nextjs.org/docs/app) — Server Components, Route Handlers, Cache Components
- [TanStack Query SSR/Hydration](https://tanstack.com/query/latest/docs/framework/react/guides/ssr) — HydrationBoundary pattern
- [EF Core PostgreSQL provider](https://www.npgsql.org/efcore/) — Npgsql capabilities
- OrbitSpace domain documentation (domain.md, domain-mechanics.md)

---
*Architecture research for: Life OS / Personal Productivity Platform*
*Researched: 2026-02-14*
