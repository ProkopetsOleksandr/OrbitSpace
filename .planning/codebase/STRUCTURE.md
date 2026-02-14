# Codebase Structure

**Analysis Date:** 2026-02-14

## Directory Layout

```
OrbitSpace/
├── .claude/                    # Project-specific Claude instructions
├── .github/                    # GitHub Actions, PR templates
├── .planning/                  # GSD planning artifacts (documents, phases)
├── docs/                       # Architecture and research documentation
├── next-js-app/                # Frontend monorepo root
│   ├── app/                    # Next.js App Router (route pages, layouts, API proxy)
│   ├── src/                    # FSD layers
│   │   ├── app/                # App-level setup (providers, query-client, styles)
│   │   ├── shared/             # Shared utilities, UI components, API clients
│   │   ├── entities/           # Domain entities with queries/mutations
│   │   ├── features/           # User interactions (create, update, delete)
│   │   ├── widgets/            # Composite UI blocks
│   │   └── pages/              # Page-level components
│   ├── public/                 # Static assets
│   ├── package.json            # pnpm workspace, dependencies
│   └── tsconfig.json           # TypeScript strict mode configuration
├── dotnet-web-api/             # Backend monorepo root
│   ├── OrbitSpace/             # WebApi project (controllers, OpenAPI config)
│   ├── OrbitSpace.Application/ # Use cases, services, DTOs
│   ├── OrbitSpace.Domain/      # Entities, enums (no external deps)
│   ├── OrbitSpace.Infrastructure/ # Repositories, EF Core, persistence
│   └── dotnet-web-api.slnx     # Solution file
└── README.md                   # Project overview
```

## Directory Purposes

**Root Project Directories:**

**`.claude/`** — Project-specific instructions
- `CLAUDE.md` — Project overview (if present)
- `rules/` — Domain model, API patterns, architecture guidelines
- Subdirectories:
  - `rules/domain.md` — Entity relationships, system levels
  - `rules/domain-mechanics.md` — Task lifecycle, onboarding flow
  - `rules/backend/api.md` — Backend API standards
  - `rules/frontend/api-patterns.md` — Dual client strategy, React Query patterns
  - `rules/frontend/architecture.md` — FSD import rules, layer responsibilities
  - `rules/frontend/components.md` — Component patterns, form patterns
  - `rules/frontend/typescript.md` — Naming conventions, import order

**`.planning/`** — GSD artifacts (auto-generated)
- `codebase/` — Documents like ARCHITECTURE.md, STRUCTURE.md, CONVENTIONS.md
- `phases/` — Implementation phases created by `/gsd:plan-phase`

**`docs/`** — Architecture and research documentation
- Research documents, architecture diagrams (hand-written notes converted)

---

## Frontend (`next-js-app/`)

**`next-js-app/app/`** — Next.js App Router (NOT FSD layer)
- Purpose: Routing, global layouts, API routes
- Key files:
  - `layout.tsx` — Root layout, wraps in Providers, includes DashboardLayout widget
  - `page.tsx` — Home page (/ route)
  - `goals/page.tsx` — Goals route page
  - `activities/page.tsx` — Activities route page
  - `task-management/page.tsx` — Task management route page
  - `api/proxy/[...path]/route.ts` — Proxy handler forwarding to backend
- Notes: Minimal JSX here; mostly imports page components from `src/pages/`

**`next-js-app/src/shared/`** — Shared layer (FSD)
- Purpose: Cross-cutting utilities, no business logic
- Subdirectories:
  - `api/` — API clients (openapi-fetch)
    - `api-client.ts` — Browser client (baseurl: `/api/proxy`)
    - `api-server-client.ts` — Server client (baseurl: `NEXT_PUBLIC_BACKEND_BASE_URL`)
    - `v1.ts` — Generated TypeScript types from .NET OpenAPI spec (auto-generated, DO NOT EDIT)
    - `models.ts` — Custom API model types
    - `index.ts` — Public exports
  - `ui/` — shadcn/ui component library
    - Components: Button, Card, Dialog, Input, Select, etc.
    - Imported from radix-ui, styled with Tailwind
  - `config/` — Configuration constants
    - `index.ts` — Backend URL, env variables
  - `lib/` — Utility functions
    - `query-client.ts` — TanStack Query client singleton
    - `hooks/` — Custom React hooks (if any)
  - `schemas/` — Shared validation schemas
    - Used by multiple features
  - `types/` — Shared type definitions
    - `UUID`, `ISODateString`, etc.

**`next-js-app/src/entities/`** — Entities layer (FSD)
- Purpose: Domain business entities with minimal UI, query definitions
- Structure per entity:
  ```
  entities/[entity-name]/
  ├── api/
  │   ├── [entity]-query-keys.ts      # TanStack Query key factory
  │   ├── [entity]-queries.ts         # useQuery hooks
  │   └── [entity]-mutations.ts       # useMutation hooks (if needed)
  ├── model/
  │   ├── types.ts                    # Entity-specific types
  │   └── index.ts                    # Public API
  ├── ui/
  │   ├── [Entity]Card.tsx            # Entity display components
  │   └── [Entity]Table.tsx           # Entity list components
  └── index.ts                        # Public API (exports queries, types)
  ```
- Examples:
  - `src/entities/goal/api/goal-query-keys.ts` — Cache key factory
  - `src/entities/goal/api/goal-queries.ts` — `useGoals()` hook
  - `src/entities/todo-item/api/todo-item-queries.ts` — `useTodoItems()` hook
- Pattern: Query keys in `api/`, queries/mutations in `api/`, minimal UI in `ui/`

**`next-js-app/src/features/`** — Features layer (FSD)
- Purpose: User interactions (create, update, delete, filter)
- Structure per feature:
  ```
  features/[namespace]/[feature]/
  ├── api/
  │   ├── [feature]-action.ts         # Server Actions
  │   └── route.ts                    # API route handlers (if needed)
  ├── model/
  │   ├── [feature]-schema.ts         # Zod validation schemas
  │   ├── [feature]-mutation.ts       # useMutation hooks
  │   ├── types.ts                    # Feature-specific types
  │   └── index.ts                    # Public API
  ├── ui/
  │   ├── [Feature]Form.tsx           # Form component
  │   ├── [Feature]Dialog.tsx         # Dialog wrapper
  │   └── [Feature]Button.tsx         # Action buttons
  └── index.ts                        # Public API
  ```
- Examples:
  - `src/features/goals/create-goal/` — Create goal feature
    - `api/create-goal-action.ts` — Server Action calling backend
    - `model/create-goal-schema.ts` — Zod schema for validation
    - `model/create-goal-mutation.ts` — `useCreateGoal()` hook
    - `ui/CreateGoalDialog.tsx` — Dialog UI
  - `src/features/goals/filter-goals/` — Filter goals feature
  - `src/features/activities/delete-activity/` — Delete activity feature
- Pattern: Zod schemas in `model/`, Server Actions in `api/`, forms in `ui/`

**`next-js-app/src/widgets/`** — Widgets layer (FSD)
- Purpose: Composite UI blocks combining features and entities
- Structure per widget:
  ```
  widgets/[widget-name]/
  ├── ui/
  │   └── [Widget].tsx                # Main widget component
  ├── model/
  │   ├── types.ts                    # Widget-specific types
  │   └── index.ts                    # Public API (if needed)
  └── index.ts                        # Public API
  ```
- Examples:
  - `src/widgets/goals/goal-list/` — List of goals with filtering, sorting
  - `src/widgets/goals/goals-toolbar/` — Toolbar with create button, filters
  - `src/widgets/goals/goals-stats/` — Stats display for goals
  - `src/widgets/dashboard-layout/` — Main dashboard layout widget
  - `src/widgets/day-planner/` — Day planner composite widget
- Pattern: Combine 2+ features/entities into reusable UI blocks

**`next-js-app/src/pages/`** — Pages layer (FSD)
- Purpose: Route-level page components
- Structure per page:
  ```
  pages/[page-name]/
  ├── ui/
  │   └── [PageName]Page.tsx          # Main page component
  └── index.ts                        # Public API
  ```
- Examples:
  - `src/pages/goals-page/` — Goals page (assembles goals widget + toolbar)
  - `src/pages/activities-page/` — Activities page
  - `src/pages/task-management-page/` — Task management page
  - `src/pages/home-page/` — Home page
- Pattern: Minimal logic; mainly composition of widgets

**`next-js-app/src/app/`** — App layer (FSD, not Next.js App Router)
- Purpose: App-level setup and global state
- Subdirectories:
  - `providers/` — React providers
    - `Providers.tsx` — Wraps children in ClerkProvider, QueryClientProvider
  - `lib/` — App-level utilities
    - `query-client.ts` — TanStack Query client singleton
  - `styles/` — Global CSS
    - `globals.css` — Tailwind directives, CSS variables from shadcn/ui
- Pattern: Minimal; setup only

---

## Backend (`dotnet-web-api/`)

**`dotnet-web-api/OrbitSpace/`** — WebApi project (presentation layer)
- Purpose: HTTP endpoints, middleware, OpenAPI configuration
- Subdirectories:
  - `Controllers/`
    - `ApiControllerBase.cs` — Base class with CurrentUser, error handling
    - `[Entity]Controller.cs` — REST endpoints (GoalsController, ActivitiesController, etc.)
    - Pattern: `[Authorize]` on all endpoints, `CurrentUser.Id` for user isolation
  - `Models/`
    - `Responses/ApiResponse<T>.cs` — Standard response envelope
  - `Identity/`
    - `ApplicationUserProvider.cs` — Extracts user from JWT claims
    - `ApplicationUser.cs` — User wrapper from claims
  - `OpenApi/`
    - Custom transformers for OpenAPI spec generation
    - DocumentTransformers/, OperationTransformers/, SchemaTransformers/
  - `Startup/`
    - Service registration methods (AddPresentation, etc.)
  - `Program.cs` — App startup and configuration
  - `Constants/`
    - `CorsPolicyConstants.cs` — CORS policy names

**`dotnet-web-api/OrbitSpace.Application/`** — Application layer (business logic)
- Purpose: Use cases, service interfaces, DTOs
- Subdirectories:
  - `Services/`
    - `[Entity]Service.cs` — Business logic (GoalService, ActivityService, etc.)
      - Pattern: Validation, mapping, repository calls
    - `Interfaces/I[Entity]Service.cs` — Service contracts
  - `Services/Interfaces/` — All service contracts
  - `Dtos/` — Data transfer objects
    - `[Entity]/Create[Entity]Request.ts` — Request DTO
    - `[Entity]/Update[Entity]Request.ts` — Update request
    - `[Entity]/[Entity]Dto.ts` — Response DTO
  - `Common/`
    - `Interfaces/I[Entity]Repository.cs` — Repository contracts
    - `Interfaces/IApplicationUserProvider.cs` — User extraction interface
    - `Models/OperationResult<T>.cs` — Result type for service responses
    - `Models/OperationResultError.cs` — Error details (NotFound, Validation)
  - `Mapping/`
    - Mapster profiles (entity ↔ DTO mapping)
  - `DependencyInjection.cs` — Service registration

**`dotnet-web-api/OrbitSpace.Domain/`** — Domain layer (entities, no external deps)
- Purpose: Business entities and enums
- Subdirectories:
  - `Entities/`
    - `Goal.cs` — Goal entity (Id, Title, LifeArea, Status, etc.)
    - `Activity.cs` — Activity entity
    - `TodoItem.cs` — Todo item entity
    - `User.cs` — User entity
  - `Enums/`
    - `GoalStatus.cs` — Active, Completed, Cancelled, etc.
    - `LifeArea.cs` — Health, Career, Relationships, etc.

**`dotnet-web-api/OrbitSpace.Infrastructure/`** — Infrastructure layer (data access)
- Purpose: Database, repositories, external services
- Subdirectories:
  - `Persistence/`
    - `AppDbContext.cs` — EF Core DbContext (PostgreSQL)
    - `Repositories/`
      - `[Entity]Repository.cs` — Data access (GoalRepository, etc.)
        - Pattern: CRUD operations (GetAllAsync, GetByIdAsync, CreateAsync, UpdateAsync, DeleteAsync)
    - `Configurations/`
      - EF Core entity configurations (fluent API for table/column mapping)
  - `Services/`
    - Infrastructure service implementations
  - `Settings/`
    - Configuration classes (connection strings, etc.)
  - `DependencyInjection.cs` — Service registration (DbContext, repositories)

---

## Key File Locations

**Entry Points:**

- `next-js-app/app/layout.tsx` — Frontend root layout
- `next-js-app/app/api/proxy/[...path]/route.ts` — API proxy handler
- `dotnet-web-api/OrbitSpace/Program.cs` — Backend startup

**Configuration:**

- `next-js-app/package.json` — Frontend dependencies, scripts
- `next-js-app/tsconfig.json` — TypeScript strict mode
- `dotnet-web-api/OrbitSpace/appsettings.json` — Backend configuration
- `next-js-app/src/shared/config/index.ts` — Frontend environment variables
- `.claude/rules/` — Project guidelines and conventions

**Core Logic:**

- `next-js-app/src/entities/*/api/` — Query hooks and query keys
- `next-js-app/src/features/*/api/` — Server Actions for mutations
- `next-js-app/src/features/*/model/` — Business logic and schemas
- `dotnet-web-api/OrbitSpace.Application/Services/` — Business logic
- `dotnet-web-api/OrbitSpace.Infrastructure/Persistence/Repositories/` — Data access

**Types & Validation:**

- `next-js-app/src/shared/api/v1.ts` — Generated OpenAPI types (auto-generated)
- `next-js-app/src/features/*/model/*-schema.ts` — Zod validation schemas
- `dotnet-web-api/OrbitSpace.Domain/Entities/` — Domain entities

**Styling & UI:**

- `next-js-app/src/shared/ui/` — shadcn/ui components
- `next-js-app/src/app/styles/globals.css` — Global CSS and Tailwind setup
- `next-js-app/src/features/*/ui/` — Feature-specific components
- `next-js-app/src/widgets/*/ui/` — Widget components

**Authentication & API:**

- `next-js-app/src/shared/api/api-client.ts` — Browser API client
- `next-js-app/src/shared/api/api-server-client.ts` — Server API client
- `next-js-app/src/app/providers/Providers.tsx` — Clerk + TanStack Query providers
- `dotnet-web-api/OrbitSpace/Identity/ApplicationUserProvider.cs` — JWT claim extraction
- `dotnet-web-api/OrbitSpace/Controllers/ApiControllerBase.cs` — Base controller with CurrentUser

---

## Naming Conventions

**Files:**

- Components: `PascalCase.tsx` (e.g., `GoalCard.tsx`, `CreateGoalDialog.tsx`)
- Utilities: `kebab-case.ts` (e.g., `query-client.ts`, `api-client.ts`)
- Hooks: `use-*.ts` (e.g., `use-create-goal.ts`, `use-goals-list.ts`)
- Schemas: `*-schema.ts` (e.g., `create-goal-schema.ts`)
- Types: `types.ts`
- Constants: `constants.ts`
- API routes: `route.ts` (Next.js convention)
- Query keys: `*-query-keys.ts`
- Queries/mutations: `*-queries.ts`, `*-mutations.ts`
- Server Actions: `*-action.ts`
- Indices: `index.ts` (public API exports)

**Backend C# Files:**

- Entities: `[Entity].cs` (e.g., `Goal.cs`)
- DTOs: `[Entity]Dto.cs`, `Create[Entity]Request.cs`, `Update[Entity]Request.cs`
- Services: `[Entity]Service.cs`
- Repositories: `[Entity]Repository.cs`
- Controllers: `[Entity]Controller.cs`
- Interfaces: `I[Entity]Service.cs`, `I[Entity]Repository.cs`
- Enums: `[EnumName].cs` (e.g., `GoalStatus.cs`, `LifeArea.cs`)

**Directories:**

- Frontend FSD layers: `lowercase` (shared, entities, features, widgets, pages, app)
- Feature namespaces: `lowercase` (goals, activities, todo-items)
- Feature names: `kebab-case` (create-goal, delete-activity, update-todo-item)
- Backend projects: `PascalCase` (OrbitSpace.Domain, OrbitSpace.Application, etc.)
- Backend folders: `PascalCase` (Controllers, Services, Repositories, Models, Entities, Dtos)

---

## Where to Add New Code

**New Actionable Feature (e.g., "Update Goal"):**

1. Create feature directory:
   ```
   src/features/goals/update-goal/
   ├── api/
   │   └── update-goal-action.ts
   ├── model/
   │   ├── update-goal-schema.ts
   │   ├── update-goal-mutation.ts
   │   └── index.ts
   ├── ui/
   │   ├── UpdateGoalForm.tsx
   │   └── UpdateGoalDialog.tsx
   └── index.ts
   ```

2. Define validation schema in `model/update-goal-schema.ts`
3. Create Server Action in `api/update-goal-action.ts`
4. Create mutation hook in `model/update-goal-mutation.ts`
5. Create form/dialog UI in `ui/`
6. Import and use in widget or page

**New Entity (e.g., "Habit"):**

1. Create entity directory:
   ```
   src/entities/habit/
   ├── api/
   │   ├── habit-query-keys.ts
   │   ├── habit-queries.ts
   │   └── habit-mutations.ts
   ├── model/
   │   ├── types.ts
   │   └── index.ts
   ├── ui/
   │   ├── HabitCard.tsx
   │   └── HabitTable.tsx
   └── index.ts
   ```

2. Define query key factory in `api/habit-query-keys.ts`
3. Create `useHabits()` and `useHabit(id)` hooks in `api/habit-queries.ts`
4. Create minimal UI components in `ui/`
5. Export public API from `index.ts`

**New Page (e.g., "Habits"):**

1. Create page directory:
   ```
   src/pages/habits-page/
   ├── ui/
   │   └── HabitsPage.tsx
   └── index.ts
   ```

2. Create page component assembling widgets
3. Add route page in `app/habits/page.tsx`
4. Import and render page component

**New Shared Component:**

1. Create in `src/shared/ui/`:
   ```
   src/shared/ui/my-component/
   ├── MyComponent.tsx
   ├── my-component.module.css (or inline Tailwind)
   └── index.ts
   ```

2. Export from `index.ts`
3. Import where needed (any FSD layer)

**New Utility/Hook:**

1. Shared utility: `src/shared/lib/utility-name.ts`
2. Shared hook: `src/shared/lib/hooks/use-hook-name.ts`
3. Export from `src/shared/lib/index.ts` if meant to be public

**Backend - New Endpoint:**

1. Create DTO in `OrbitSpace.Application/Dtos/[Entity]/`
2. Create/extend Service in `OrbitSpace.Application/Services/[Entity]Service.cs`
3. Create/extend Repository in `OrbitSpace.Infrastructure/Persistence/Repositories/[Entity]Repository.cs`
4. Create/extend Controller in `OrbitSpace/Controllers/[Entity]Controller.cs`
5. Endpoint automatically appears in OpenAPI spec

---

## Special Directories

**`next-js-app/.next/`**
- Purpose: Next.js build output (generated, ignored by git)
- Generated: Yes
- Committed: No

**`next-js-app/node_modules/`**
- Purpose: npm dependencies
- Generated: Yes (by pnpm install)
- Committed: No

**`next-js-app/.clerk/`**
- Purpose: Clerk SDK cache
- Generated: Yes
- Committed: No

**`dotnet-web-api/bin/` and `obj/`**
- Purpose: .NET build output
- Generated: Yes
- Committed: No

**`.planning/codebase/`**
- Purpose: GSD mapping documents (ARCHITECTURE.md, STRUCTURE.md, etc.)
- Generated: Yes (by `/gsd:map-codebase`)
- Committed: Yes (helpful for team context)

**`.planning/phases/`**
- Purpose: GSD implementation phases
- Generated: Yes (by `/gsd:plan-phase`)
- Committed: Optional (for transparency)

---

*Structure analysis: 2026-02-14*
