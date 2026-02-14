# Architecture

**Analysis Date:** 2026-02-14

## Pattern Overview

**Overall:** Monorepo with hybrid BFF (Backend-for-Frontend) + Clean Architecture

**Key Characteristics:**
- Frontend (Next.js 16) proxies all backend requests through `/api` routes
- Backend (.NET 10) exposes OpenAPI spec; frontend generates TypeScript types
- Clerk authentication: JWT flows through Next.js proxy to backend
- FSD (Feature-Sliced Design) on frontend; Clean Architecture on backend
- Separated concerns: browser never talks directly to backend API

## Layers

**Frontend - Browser Layer:**
- Purpose: Client-side UI rendering with React 19
- Location: `next-js-app/app/` (Next.js App Router pages)
- Contains: Page layouts, client components, auth middleware
- Depends on: Clerk providers, shared utilities, FSD layers
- Used by: End users

**Frontend - FSD Layers (bottom-up dependency):**

**Shared Layer:**
- Purpose: Cross-cutting utilities, UI components, API clients, types
- Location: `next-js-app/src/shared/`
- Contains: `api/` (openapi-fetch clients), `ui/` (shadcn/ui components), `config/`, `lib/`, `types/`, `schemas/`
- Depends on: External packages (React, TanStack Query, Zod)
- Used by: All upper layers (entities, features, widgets, pages)
- Notes: Zero business logic here; purely infrastructure

**Entities Layer:**
- Purpose: Domain business entities with minimal UI, API queries/mutations, query keys
- Location: `next-js-app/src/entities/[entity-name]/`
- Structure per entity:
  - `api/` — TanStack Query hooks (useGoals, useCreateGoal mutations)
  - `model/` — Query key factories, mutation handlers
  - `ui/` — Minimal entity display components
  - `index.ts` — Public API exports
- Examples: `src/entities/goal/`, `src/entities/activity/`, `src/entities/todo-item/`
- Depends on: Shared layer only
- Used by: Features and widgets

**Features Layer:**
- Purpose: User-facing interactions and business logic (create, update, delete, filter)
- Location: `next-js-app/src/features/[namespace]/[feature]/`
- Structure per feature:
  - `api/` — Server Actions (mutationFn executed server-side)
  - `model/` — Zod schemas, mutations using TanStack Query, business logic
  - `ui/` — Feature components (forms, dialogs)
- Examples: `src/features/goals/create-goal/`, `src/features/activities/delete-activity/`
- Depends on: Entities and Shared layers
- Used by: Widgets and pages

**Widgets Layer:**
- Purpose: Composite UI blocks assembling multiple features and entities
- Location: `next-js-app/src/widgets/[widget-name]/`
- Structure:
  - `ui/` — Complex components combining features/entities
  - `model/` — Widget-specific orchestration logic (if needed)
  - `index.ts` — Public API
- Examples: `src/widgets/goals/goal-list/`, `src/widgets/day-planner/`
- Depends on: Features, entities, shared
- Used by: Pages

**Pages Layer:**
- Purpose: Route-level components assembling widgets into complete views
- Location: `next-js-app/src/pages/[page-name]/`
- Structure:
  - `ui/` — Page-level components
  - `index.ts` — Public API
- Examples: `src/pages/goals-page/`, `src/pages/activities-page/`
- Depends on: Widgets, features, entities, shared
- Used by: Next.js App Router (`next-js-app/app/`)

**Frontend - Next.js App Router Layer:**
- Purpose: Routing, layouts, global providers, API proxy routes
- Location: `next-js-app/app/`
- Contains:
  - `layout.tsx` — Root layout wrapping DashboardLayout widget
  - `page.tsx` — Home page
  - `[route]/page.tsx` — Route pages (goals, activities, task-management)
  - `api/proxy/[...path]/route.ts` — Proxy handler
- Depends on: Pages layer (via widgets), Providers
- Notes: Minimal logic; mostly orchestration

---

## Backend - Clean Architecture Layers

**Domain Layer:**
- Purpose: Business entities and domain logic independent of frameworks
- Location: `dotnet-web-api/OrbitSpace.Domain/`
- Contains: Entity classes (Goal, Activity, TodoItem, User), Enums
- Examples: `OrbitSpace.Domain/Entities/Goal.cs`, `OrbitSpace.Domain/Enums/GoalStatus.cs`
- Depends on: Nothing (zero external dependencies)
- Used by: Application and Infrastructure layers

**Application Layer:**
- Purpose: Use cases, service interfaces, DTOs
- Location: `dotnet-web-api/OrbitSpace.Application/`
- Contains:
  - `Services/` — Business logic (GoalService, ActivityService)
  - `Services/Interfaces/` — Service contracts (IGoalService, IActivityService)
  - `Dtos/` — Transfer objects for API requests/responses
  - `Common/Interfaces/` — Repository and generic interfaces
  - `Common/Models/` — OperationResult, error handling models
  - `Mapping/` — Mapster profiles for entity ↔ DTO mapping
- Examples: `OrbitSpace.Application/Services/GoalService.cs`
- Depends on: Domain layer
- Used by: Infrastructure and WebApi layers

**Infrastructure Layer:**
- Purpose: Data access, external services, framework implementations
- Location: `dotnet-web-api/OrbitSpace.Infrastructure/`
- Contains:
  - `Persistence/` — AppDbContext (EF Core), Repository implementations
  - `Persistence/Repositories/` — Data access (GoalRepository, ActivityRepository)
  - `Persistence/Configurations/` — EF Core entity configurations
  - `Services/` — External service implementations
  - `Settings/` — Configuration classes
- Examples: `OrbitSpace.Infrastructure/Persistence/Repositories/GoalRepository.cs`
- Depends on: Domain and Application layers
- Used by: WebApi layer

**WebApi Layer (Presentation):**
- Purpose: HTTP endpoints, middleware, OpenAPI configuration
- Location: `dotnet-web-api/OrbitSpace/` (OrbitSpace.WebApi.csproj)
- Contains:
  - `Controllers/` — API endpoints (GoalsController, ActivitiesController)
  - `Controllers/ApiControllerBase.cs` — Base class handling errors, auth
  - `Models/` — API response wrappers
  - `Models/Responses/ApiResponse<T>` — Standardized response envelope
  - `Identity/ApplicationUserProvider.cs` — JWT claim extraction
  - `OpenApi/` — OpenAPI customizations and transformers
  - `Startup/` — Dependency injection configuration
  - `Program.cs` — App initialization
- Examples: `OrbitSpace/Controllers/GoalsController.cs`
- Depends on: Application, Infrastructure, Domain
- Used by: Next.js frontend (via proxy)

## Data Flow

**Request Flow (Browser → Next.js → Backend → Database):**

1. **User Action** (e.g., "Create Goal")
   - Client Component triggers form submission
   - Location: `next-js-app/src/features/goals/create-goal/ui/`

2. **Mutation Hook** (TanStack Query)
   - Hook: `useCreateGoal()` at `next-js-app/src/features/goals/create-goal/model/create-goal-mutation.ts`
   - Calls Server Action with form values

3. **Server Action** (Server-side execution)
   - Function: `createGoalAction()` at `next-js-app/src/features/goals/create-goal/api/create-goal-action.ts`
   - Gets Clerk JWT via `getServerApiClient()`
   - Calls backend via direct connection (server-to-server, not proxy)

4. **Next.js Proxy** (only for client-initiated requests)
   - Route: `/api/proxy/[...path]` at `next-js-app/app/api/proxy/[...path]/route.ts`
   - Used by: Client components calling TanStack Query
   - Attaches Clerk JWT from httpOnly cookie
   - Forwards to backend

5. **Backend Controller**
   - Endpoint: `POST /api/goals` in `dotnet-web-api/OrbitSpace/Controllers/GoalsController.cs`
   - Extracts `userId` from JWT claims via `CurrentUser` property
   - Validates input (request.Id == id match)

6. **Application Service**
   - Logic: `GoalService.CreateAsync()` at `dotnet-web-api/OrbitSpace.Application/Services/GoalService.cs`
   - Validates business rules (e.g., active goals require due date)
   - Maps DTO to Domain Entity
   - Calls repository

7. **Repository (Data Access)**
   - Method: `GoalRepository.CreateAsync()` at `dotnet-web-api/OrbitSpace.Infrastructure/Persistence/Repositories/GoalRepository.cs`
   - Adds entity to DbContext, calls SaveChangesAsync()
   - Uses EF Core to generate SQL for PostgreSQL

8. **Database**
   - Storage: PostgreSQL (migrated from MongoDB on 2026-02-14)
   - Table: `Goals` with schema from EF Core configurations

9. **Response Cascade (Database → Backend → Frontend)**
   - Service returns `OperationResult<GoalDto>` wrapping entity
   - Controller returns `ApiResponse<GoalDto>` (envelope)
   - Next.js receives JSON response
   - Server Action returns data to mutation handler
   - Mutation handler invalidates query cache (e.g., `goalQueryKeys.lists()`)
   - React re-renders with new data

**Query Flow (Fetch existing data):**

1. **Page Render** — Server Component or Client Component needs data
2. **Query Hook** — `useGoals()` from `next-js-app/src/entities/goal/api/goal-queries.ts`
3. **Client → Proxy** — Browser calls `/api/proxy/api/goals` (with cookie attached)
4. **Proxy → Backend** — Adds JWT, forwards to backend
5. **Backend → Database** — Controller → Service → Repository
6. **Response** — JSON returned, cached by TanStack Query
7. **UI Update** — Component re-renders with data

**State Management:**
- Server state: TanStack React Query (cached responses, mutations, refetching)
- No Redux/Zustand — server state is source of truth
- Invalidation pattern: mutation success → `invalidateQueries()` → automatic refetch
- Cache keys from factory: `goalQueryKeys.list()`, `goalQueryKeys.detail(id)`

## Key Abstractions

**Query Key Factory:**
- Purpose: Centralized cache key structure for TanStack Query
- Examples: `src/entities/goal/api/goal-query-keys.ts`
- Pattern: Hierarchical keys enable scoped invalidation
  ```typescript
  goalQueryKeys = {
    all: ['goals'],
    lists: () => [...all, 'list'],
    list: (filters) => [...lists(), filters],
    details: () => [...all, 'detail'],
    detail: (id) => [...details(), id]
  }
  // Invalidate all goal queries: invalidateQueries({ queryKey: goalQueryKeys.all })
  // Invalidate only lists: invalidateQueries({ queryKey: goalQueryKeys.lists() })
  ```
- Location: `src/entities/[entity]/api/[entity]-query-keys.ts`

**API Client Dual Strategy:**
- Browser Client (`getApiClient()`): Baseurl `/api/proxy`, routes through Next.js
- Server Client (`getServerApiClient()`): Baseurl `NEXT_PUBLIC_BACKEND_BASE_URL`, direct connection with JWT
- Reason: httpOnly cookies only work for same-origin requests; server needs direct auth
- Location: `src/shared/api/api-client.ts`, `src/shared/api/api-server-client.ts`

**OpenAPI Code Generation:**
- Typescript types auto-generated from .NET OpenAPI spec
- Command: `pnpm generate-api-types`
- Output: `src/shared/api/v1.ts` — typed `paths` object used by openapi-fetch
- Benefit: API contract changes propagate as TypeScript errors at compile time
- Never manually define API types — always use generated `v1.ts`

**OperationResult Pattern (Backend):**
- Purpose: Encapsulate success/failure with type safety
- Type: `OperationResult<T>` containing `Data`, `Error`, `IsSuccess`
- Error types: `OperationResultError.NotFound()`, `OperationResultError.Validation(msg)`
- Controller maps to HTTP status: NotFound → 404, Validation → 400
- Location: `dotnet-web-api/OrbitSpace.Application/Common/Models/`

**ApiResponse Envelope (Backend):**
- Purpose: Wrap domain objects in consistent response format
- Type: `ApiResponse<T>` containing `Data` and metadata
- Used by: All controller responses for consistency
- Location: `dotnet-web-api/OrbitSpace/Models/Responses/ApiResponse.cs`

**CurrentUser Abstraction (Backend):**
- Purpose: Extract authenticated user from JWT claims without repetition
- Property: `ApiControllerBase.CurrentUser` — lazy-loaded from `IApplicationUserProvider`
- Provider: Extracts `sub` (user ID) and `email` from Clerk JWT
- Used by: All controllers without explicit claim parsing
- Location: `dotnet-web-api/OrbitSpace/Identity/ApplicationUserProvider.cs`

**Zod Schemas (Frontend):**
- Purpose: Runtime validation for form inputs and API responses
- Location: `src/features/[feature]/model/[feature]-schema.ts`
- Usage: Validation in forms, Server Actions, API boundaries
- Example: `src/features/goals/create-goal/model/create-goal-schema.ts`
- Types inferred: `z.infer<typeof schema>` for TypeScript

## Entry Points

**Frontend - Browser Entry:**
- Location: `next-js-app/app/layout.tsx` (Root Layout)
- Triggers: User navigates to any route
- Responsibilities:
  - Wraps app in `Providers` (Clerk, TanStack Query, React devtools)
  - Renders `DashboardLayout` widget
  - Loads global CSS and fonts

**Frontend - API Proxy Entry:**
- Location: `next-js-app/app/api/proxy/[...path]/route.ts`
- Triggers: Browser fetch to `/api/proxy/*`
- Responsibilities:
  - Extracts Clerk JWT from httpOnly cookie
  - Constructs backend URL from path params
  - Forwards request to backend with Authorization header
  - Returns backend response to browser
- Note: No logic here — pure pass-through with auth injection

**Backend - Program Entry:**
- Location: `dotnet-web-api/OrbitSpace/Program.cs`
- Triggers: `dotnet run`
- Responsibilities:
  - Registers DI services (AddPresentation, AddApplication, AddInfrastructure)
  - Configures CORS, auth (JWT Bearer), OpenAPI
  - Maps controllers and Swagger/OpenAPI endpoints
  - Runs middleware pipeline

**Backend - Controller Entry Points:**
- Location: `dotnet-web-api/OrbitSpace/Controllers/[Entity]Controller.cs`
- Examples: `GoalsController.cs`, `ActivitiesController.cs`, `TodoItemsController.cs`
- Endpoints per controller:
  - `GET /api/goals` — Get all
  - `GET /api/goals/{id}` — Get details
  - `POST /api/goals` — Create
  - `PUT /api/goals/{id}` — Update
  - `DELETE /api/goals/{id}` — Delete
- Authentication: `[Authorize]` attribute on all endpoints
- Current user: Extracted from JWT via `CurrentUser` property

## Error Handling

**Strategy:** Layered error handling from Frontend to Backend

**Frontend (Client Components):**
- TanStack Query `useQuery` hook catches errors
- UI displays error state: `isError` boolean, `error` object
- Pattern: Check `error` state and render error message
- Location: Component error boundaries at page level

**Frontend (Server Actions):**
- try/catch in `createGoalAction()` throws Error if API fails
- Mutation handler catches via `onError` callback
- Rollback optimistic updates on error
- Example: `src/features/goals/create-goal/api/create-goal-action.ts`

**Backend (Controllers):**
- `ApiControllerBase.HandleError()` maps OperationResult errors to HTTP status
- NotFound → 404 with ProblemDetails
- Validation errors → 400 with ProblemDetails
- Unhandled exceptions caught by global ExceptionHandler middleware
- Uses RFC 7807 (Problem Details) format
- Location: `dotnet-web-api/OrbitSpace/Controllers/ApiControllerBase.cs`

**Backend (Application Layer):**
- Services return `OperationResult<T>` with error details
- Validation happens before database writes
- Example: `GoalService.CreateAsync()` validates "active goals need due date"
- Location: `dotnet-web-api/OrbitSpace.Application/Services/`

## Cross-Cutting Concerns

**Logging:** Console logging in development; structured logging in production (not yet configured)

**Validation:**
- Frontend: Zod schemas in feature layer + React Hook Form
- Backend: Service business logic before repository calls
- API: DTO validation attributes (if using)

**Authentication:**
- Frontend: Clerk SDK manages login/logout, provides JWT
- Backend: JWT Bearer scheme in Startup (Microsoft.AspNetCore.Authentication.JwtBearer)
- Claim extraction: `IApplicationUserProvider` reads `sub` and `email` from JWT
- All endpoints protected by `[Authorize]` attribute

**Authorization:**
- User isolation: Controllers filter by `CurrentUser.Id` (e.g., get only user's goals)
- No role-based authorization currently implemented
- Future: Add role claims if multi-tenant features needed

**CORS:**
- Backend allows frontend origin in `CorsPolicyConstants.PolicyName.AllowSpecificOrigins`
- Not strictly needed when using proxy pattern, but enabled for flexibility

---

*Architecture analysis: 2026-02-14*
