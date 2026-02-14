# Coding Conventions

**Analysis Date:** 2026-02-14

## Naming Patterns

**Files:**
- Components: PascalCase with `.tsx` extension (`CreateActivityForm.tsx`, `GoalTable.tsx`)
- Utilities and helpers: kebab-case with `.ts` extension (`query-client.ts`, `api-client.ts`)
- Hooks: prefix with `use-` and kebab-case (`use-create-goal.ts`, `use-mobile.ts`)
- Schemas: suffix with `Schema` in camelCase (`createActivitySchema`)
- Types files: `types.ts`, `form.ts`
- API handlers: `route.ts` (Next.js convention for Route Handlers)
- Constants: UPPERCASE or camelCase (`GoalNotFoundMessageTemplate` for template constants)

**Functions and exports:**
- Components: PascalCase (`CreateActivityForm`, `GoalTable`, `GoalTableRowActions`)
- Hooks: `use` prefix + camelCase (`useCreateActivity`, `useGoals`)
- Utility functions: camelCase (`getApiClient`, `getQueryClient`, `getServerApiClient`)
- Query key factories: camelCase + `QueryKeys` suffix (`goalQueryKeys`)
- Mutations: camelCase with purpose (`useCreateActivity`, `useDeleteActivity`)
- Server actions: camelCase + `Action` suffix (`createActivityAction`)
- Service methods: camelCase (`CreateAsync`, `UpdateAsync`, `DeleteAsync`, `GetAllAsync`, `GetByIdAsync`)

**Variables:**
- camelCase for local variables and props (`userId`, `formValues`, `isLoading`, `isFetching`)
- PascalCase for types and interfaces (`CreateActivityFormValues`, `GenericFormProps<T>`)
- `_` prefix for private fields in classes (`_currentUser`, `_queryClient`)

**Types:**
- PascalCase for types and interfaces (`Goal`, `Activity`, `CreateActivityFormValues`)
- Zod schema names: camelCase + `Schema` suffix (`createActivitySchema`)
- Inferred types: `z.infer<typeof schema>` pattern

## Code Style

**Formatting:**
- No explicit formatter configured (ESLint is the primary linter)
- No `.prettierrc` file — rely on IDE defaults or ESLint rules
- Use Tailwind CSS utility classes directly in JSX (no CSS files for component styles)
- Group Tailwind classes: layout → spacing → colors → typography

**Linting:**
- ESLint with flat config (`eslint.config.mjs`)
- Extends: `next/core-web-vitals`, `next/typescript`
- TanStack Query ESLint plugin enabled for query rules (`@tanstack/eslint-plugin-query`)
- Ignores: `node_modules/**`, `.next/**`, `out/**`, `build/**`, `next-env.d.ts`

**TypeScript:**
- Strict mode enabled: `"strict": true`
- Target: `ES2017`, module: `esnext`
- Module resolution: `bundler`
- No `any` types — use `unknown` if truly needed
- Always define return types for exported functions
- Use `type` over `interface` for object shapes
- Use `const` assertions for literal types

## Import Organization

**Order (strictly enforced):**

1. **External dependencies** — React, libraries
```typescript
import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
```

2. **Internal absolute imports (by FSD layer, bottom-up)** — shared first, then entities, features, widgets
```typescript
import { goalQueryKeys } from '@/entities/goal';
import { CreateGoalDialog } from '@/features/goals/create-goal';
import { Button } from '@/shared/ui/button';
```

3. **Relative imports** — same folder
```typescript
import { GoalFilters } from './types';
```

4. **Type-only imports** — at the end
```typescript
import type { Goal } from '@/shared/api/v1';
```

**Path aliases:**
- `@/*` → `./src/*` (root)
- `@/app/*` → `./src/app/*`
- `@/entities/*` → `./src/entities/*`
- `@/features/*` → `./src/features/*`
- `@/widgets/*` → `./src/widgets/*`
- `@/pages/*` → `./src/pages/*`
- `@/shared/*` → `./src/shared/*`

**Import specificity:**
- Import specific items, not entire folders: `import { Button } from '@/shared/ui/button'`
- Use barrel files (`index.ts`) only for public APIs at layer boundaries
- Never import from `node_modules/` paths — use package names

## Error Handling

**Frontend (React/Next.js):**
- Throw errors from Server Actions and mutations: `throw new Error('message')`
- Use React Query's `isError` and `error` states for async operations
- Handle form validation errors via React Hook Form + Zod: errors appear in `<FormMessage />`
- Query errors are tracked by React Query; use error boundary for unhandled render errors

**Example pattern from `createActivityAction.ts`:**
```typescript
'use server';

export async function createActivityAction(payload: CreateActivityPayload) {
  const client = await getServerApiClient();
  const { data, error, response } = await client.POST('/api/activities', { body: payload });

  if (error) {
    throw new Error(`Failed to create activity: ${response.status}`);
  }

  return data.data;
}
```

**Backend (.NET/C#):**
- Use `OperationResult<T>` and `OperationResult` for operation outcomes
- Return `OperationResultError.NotFound()` or `OperationResultError.Validation()` on failure
- Implicit conversion handles wrapping: `return OperationResultError.NotFound()` automatically becomes `OperationResult`
- Controllers check `result.IsSuccess` before returning; use `HandleError()` helper
- Global exception handler catches unhandled exceptions and returns RFC 7807 `ProblemDetails`

**Example pattern from `ActivityService.cs`:**
```csharp
public async Task<OperationResult<ActivityDto>> CreateAsync(CreateActivityRequest request, Guid userId)
{
    if (string.IsNullOrWhiteSpace(request.Name))
    {
        return OperationResultError.Validation("Name is required");
    }

    // ... creation logic ...
    return mapper.Map<ActivityDto>(createdItem);
}
```

**Controllers check and convert:**
```csharp
var result = await goalService.CreateAsync(request, CurrentUser.Id);
if (!result.IsSuccess)
{
    return HandleError(result.Error);
}
```

## Logging

**Frontend:**
- No explicit logging framework configured
- Use `console.log`, `console.error` for debugging
- React Query DevTools available in dev for query inspection

**Backend:**
- Use `ILogger<T>` injected via DI
- Global exception handler logs unhandled exceptions: `logger.LogError(exception, "Unhandled exception")`

## Comments

**When to comment:**
- Explain business logic that isn't obvious from code
- Document template string constants: `private const string GoalNotFoundMessageTemplate = "Goal with id {0} not found"`
- Add JSDoc/TSDoc for exported public APIs

**JSDoc/TSDoc:**
- No consistent pattern observed in codebase
- Type annotations are preferred over comments

## Function Design

**Size guidelines:**
- Keep functions small and focused (single responsibility)
- Query functions typically 8-15 lines: setup client, call API, throw or return
- Component render functions stay under 50 lines; break into sub-components if larger

**Parameters:**
- React components: props destructuring, type via `GenericFormProps<T>`
- Hooks: minimal parameters, state via local `useState`
- Server actions: payload type (Zod schema inferred type)
- Service methods: dependency injection via constructor, parameters are domain objects or requests

**Return values:**
- React components: JSX (no explicit void return)
- Async functions: `Promise<T>` where T is the expected result type
- Service methods: `OperationResult<T>` or `Task<T>` (with error thrown on failure)
- Mutations: data from response, throw on error

## Module Design

**Exports:**
- Each feature/entity exports public API from `index.ts`
- Feature structure: `index.ts` re-exports UI and hooks
- Avoid exporting implementation details (private services, internal hooks)

**Example from `entities/goal/index.ts`:**
```typescript
export { goalQueryKeys } from './api/goal-query-keys';
export { useGoals } from './api/goal-queries';
export { GoalTable } from './ui/goal-table/GoalTable';
export type { Goal } from '@/shared/api/v1';
```

**Barrel files:**
- Use barrel files at layer boundaries: `entities/*/index.ts`, `features/*/index.ts`
- No deep re-exports — import directly from module within same FSD layer

## FSD Architecture Rules

**Import direction (strictly enforced):**
- Lower layers CANNOT import from upper layers
- Valid flow: `shared` ← `entities` ← `features` ← `widgets` ← `pages` ← `app`

**Layer responsibilities:**
- `shared/`: UI components, utilities, types, API client config (no business logic)
- `entities/`: domain models, query keys, queries/mutations, minimal UI
- `features/`: user actions (create, delete, filter), forms, specific business logic
- `widgets/`: compose features/entities into complex UI blocks
- `pages/`: assemble widgets into complete page views
- `app/`: Next.js App Router, layouts, providers, global styles

**Entity structure:**
```
entities/[entity-name]/
├── api/              # API route handlers, query keys, queries
├── ui/               # Entity components (table, card, row actions)
└── index.ts          # Public API exports
```

**Feature structure:**
```
features/[namespace]/[feature-name]/
├── api/              # Server actions
├── model/            # Zod schemas, mutations, business logic
├── ui/               # Feature UI components
└── index.ts          # Public exports
```

## Data Fetching Patterns

**Query key factory pattern (always use this):**
```typescript
export const goalQueryKeys = {
  all: ['goals'] as const,
  lists: () => [...goalQueryKeys.all, 'list'] as const,
  list: (filters: string) => [...goalQueryKeys.lists(), filters] as const,
  details: () => [...goalQueryKeys.all, 'detail'] as const,
  detail: (id: string) => [...goalQueryKeys.details(), id] as const
};
```

**Mutation cache invalidation (always use selective invalidation):**
```typescript
onSuccess: async () => {
  await queryClient.invalidateQueries({ queryKey: activityQueryKeys.lists() });
}
```

**Never use:** `queryClient.invalidateQueries()` without a key — always scope to specific query keys.

## Form Pattern

**Schema in `model/schemas.ts`:**
```typescript
import * as z from 'zod';

export const createActivitySchema = z.object({
  name: z.string().min(1, 'Name is required'),
  code: z.string().min(1).max(5)
});

export type CreateActivityFormValues = z.infer<typeof createActivitySchema>;
```

**Form component in `ui/[feature]-form.tsx`:**
- Use React Hook Form + `zodResolver`
- Accept `form` prop from parent (caller provides via `useForm()`)
- Accept `onSubmit` callback
- Use `GenericFormProps<FormValues>` type

**Dialog wrapper in `ui/[feature]-dialog.tsx`:**
- Create `useForm()` instance
- Pass to form component
- Handle mutation in `onSubmit`

---

*Convention analysis: 2026-02-14*
