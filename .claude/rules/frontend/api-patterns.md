---
paths:
  - "next-js-app/**"
---

# API & Data Fetching Patterns

## Dual Client Strategy

**Browser client (`getApiClient()`):**
- For client components
- Proxies through Next.js `/api` routes (baseurl: `/`)
- No auth header — handled by proxy

**Server client (`getServerApiClient()`):**
- For server components and API routes
- Direct backend connection (baseurl: `NEXT_PUBLIC_BACKEND_BASE_URL`)
- Adds Clerk JWT in Authorization header

## Proxy Route Pattern

All API routes follow:
```
app/api/[entity]/route.ts → getServerApiClient() → backend
```

- Browser never talks to backend directly
- All requests go through `/api` proxy routes
- Server routes authenticate with Clerk JWT

## React Query

### Query Key Factory

```typescript
// entities/[entity]/model/query-keys.ts
export const entityQueryKeys = {
  all: ['entity'] as const,
  lists: () => [...entityQueryKeys.all, 'list'] as const,
  list: (filters: Filters) => [...entityQueryKeys.lists(), filters] as const,
  details: () => [...entityQueryKeys.all, 'detail'] as const,
  detail: (id: string) => [...entityQueryKeys.details(), id] as const,
};
```

### Placement

- Queries: `entities/[entity]/model/queries.ts`
- Mutations: `entities/[entity]/model/mutations.ts`
- Always invalidate related queries on mutation success
- Use query key factory for invalidation scoping

### Rules

- No global client state — use React Query for server state
- Use selective invalidation (not `queryClient.invalidateQueries()` without key)
- Always handle loading/error/empty states in UI
- Set appropriate `staleTime` for static data

## OpenAPI Types

- Never manually define API request/response types
- Always use types from `@/shared/api/v1` (auto-generated)
- Regenerate types when backend schema changes: `pnpm generate-api-types`
- Use `components['schemas']['EntityName']` for model types
