# Next.js 16 + .NET API: production-ready JWT architecture guide

**The optimal architecture for a Next.js 16 App Router frontend with a separate .NET backend uses a hybrid BFF (Backend-for-Frontend) pattern**: Next.js Route Handlers proxy authentication, Server Components fetch data directly from the .NET API server-to-server, and client-side interactions route through Next.js to keep JWTs in httpOnly cookies — never exposed to browser JavaScript. This approach eliminates CORS complexity, prevents XSS token theft, and leverages TanStack Query's hydration system for seamless server-to-client data handoff. The sections below provide complete architectural workflows, security rationale, and production-ready code for every layer of this stack.

---

## The recommended hybrid architecture and why it wins

Three patterns exist for connecting Next.js App Router to a separate .NET API. After evaluating security, performance, and developer experience, the **hybrid BFF pattern** emerges as the clear winner for production applications.

| Factor | Direct calls | Full proxy (BFF) | Hybrid (recommended) |
|--------|-------------|-------------------|---------------------|
| Latency | Best (no proxy hop) | +5–50ms per request | Good (SSR direct, mutations proxied) |
| Token security | Tokens in browser JS | Tokens fully server-side | Mutations secured server-side |
| CORS config | Required on .NET | None needed | None (if client reads go through proxy) |
| Dev complexity | Lowest | High (mirror all endpoints) | Medium |
| File uploads | No limits | Serverless payload limits | Direct for large files |

**The hybrid pattern works like this:** Server Components call the .NET API directly (server-to-server, zero CORS issues). All authentication flows and mutations route through Next.js Route Handlers or Server Actions, which read the JWT from an httpOnly cookie and forward it to the .NET API. Client Components use TanStack Query, calling Next.js proxy endpoints that inject authentication server-side.

```
┌─────────────────────────────────────────────────────────┐
│  Browser                                                │
│  ┌──────────────┐   ┌─────────────────────────────────┐ │
│  │ Server-       │   │ Client Components               │ │
│  │ rendered HTML │   │ (TanStack Query + openapi-fetch)│ │
│  └──────────────┘   └──────────┬──────────────────────┘ │
└─────────────────────────────────┼───────────────────────┘
                                  │ same-origin requests
                                  │ (httpOnly cookie auto-attached)
┌─────────────────────────────────┼───────────────────────┐
│  Next.js Server                 ▼                       │
│  ┌──────────────┐   ┌──────────────────────┐            │
│  │ Server       │   │ Route Handlers /     │            │
│  │ Components   │   │ Server Actions       │            │
│  │ (read cookie,│   │ (read cookie,        │            │
│  │  fetch data) │   │  proxy mutations)    │            │
│  └──────┬───────┘   └──────────┬───────────┘            │
└─────────┼──────────────────────┼────────────────────────┘
          │ server-to-server     │ server-to-server
          │ (no CORS needed)     │ (no CORS needed)
          ▼                      ▼
┌──────────────────────────────────────────────────────────┐
│  .NET API Backend                                        │
│  (validates JWT, returns data)                           │
└──────────────────────────────────────────────────────────┘
```

---

## JWT storage and security: httpOnly cookies as the BFF foundation

The security consensus from OWASP, Auth0, Duende, and the IETF is unambiguous: **store JWTs in httpOnly, Secure, SameSite cookies**. Next.js Route Handlers act as the BFF that receives tokens from the .NET backend and sets them as cookies the browser cannot read via JavaScript.

**Why not localStorage or in-memory?** LocalStorage is trivially accessible to any XSS payload — a single compromised npm dependency can exfiltrate tokens. In-memory storage is more secure but loses tokens on page refresh, creating poor UX. HttpOnly cookies are **immune to JavaScript-based XSS token theft** because the browser enforces inaccessibility at the engine level.

**The cookie configuration for production:**

```typescript
// Recommended cookie settings for JWT storage
const COOKIE_OPTIONS = {
  httpOnly: true,                                    // JS cannot read
  secure: process.env.NODE_ENV === 'production',     // HTTPS only
  sameSite: 'lax' as const,                          // CSRF protection
  path: '/',
  maxAge: 60 * 15,                                   // 15 min (access token)
};

const REFRESH_COOKIE_OPTIONS = {
  ...COOKIE_OPTIONS,
  maxAge: 60 * 60 * 24 * 7,                          // 7 days (refresh token)
};
```

**SameSite=Lax** blocks cross-site POST requests (CSRF) while allowing normal navigation. For the highest security environments, use `SameSite=Strict`, but note it prevents cookies from being sent when following external links to your site.

**CSRF defense in depth:** Even with SameSite cookies, add a custom header check. Because browsers won't send custom headers in cross-origin simple requests without a CORS preflight, requiring `X-Requested-With: XMLHttpRequest` or a CSRF token in a custom header provides an additional layer.

### Refresh token rotation is mandatory

**Refresh token rotation (RTR)** ensures each refresh token is single-use. When a refresh token is exchanged for a new access token, the .NET backend issues a new refresh token and invalidates the old one. If a previously-used refresh token appears again, the server knows it was stolen and invalidates the entire token family.

Configure your .NET backend to:
- Assign a unique `jti` (JWT ID) claim to each refresh token
- Store a SHA-256 hash server-side (never the raw token)
- On reuse detection, invalidate ALL tokens in the family
- Enforce an absolute maximum lifetime (7–14 days) regardless of rotation

---

## Workflow A: user login from form to redirect

The login flow routes through a Next.js Route Handler so the server can receive the JWT from .NET and store it in an httpOnly cookie that the browser automatically sends on subsequent requests.

### Step 1: Shared validation schema

```typescript
// lib/schemas/auth.ts
import { z } from 'zod';

export const loginSchema = z.object({
  email: z.string().email('Invalid email address'),
  password: z.string().min(8, 'Password must be at least 8 characters'),
});
export type LoginInput = z.infer<typeof loginSchema>;
```

### Step 2: Client Component login form

```typescript
// app/(auth)/login/login-form.tsx
'use client';

import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useRouter } from 'next/navigation';
import { useState, useTransition } from 'react';
import { loginSchema, type LoginInput } from '@/lib/schemas/auth';

export function LoginForm() {
  const router = useRouter();
  const [error, setError] = useState<string | null>(null);
  const [isPending, startTransition] = useTransition();

  const { register, handleSubmit, formState: { errors } } = useForm<LoginInput>({
    resolver: zodResolver(loginSchema),
  });

  const onSubmit = handleSubmit((data) => {
    startTransition(async () => {
      setError(null);
      const res = await fetch('/api/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
      });

      if (!res.ok) {
        const body = await res.json();
        setError(body.error ?? 'Login failed');
        return;
      }

      router.push('/goals');
      router.refresh(); // Ensures Server Components re-render with new auth state
    });
  });

  return (
    <form onSubmit={onSubmit}>
      <input {...register('email')} type="email" placeholder="Email" />
      {errors.email && <p>{errors.email.message}</p>}
      <input {...register('password')} type="password" placeholder="Password" />
      {errors.password && <p>{errors.password.message}</p>}
      <button type="submit" disabled={isPending}>
        {isPending ? 'Signing in…' : 'Sign in'}
      </button>
      {error && <p className="text-red-500">{error}</p>}
    </form>
  );
}
```

### Step 3: Route Handler sets httpOnly cookies

```typescript
// app/api/auth/login/route.ts
import { NextRequest, NextResponse } from 'next/server';
import { cookies } from 'next/headers';
import { loginSchema } from '@/lib/schemas/auth';

export async function POST(request: NextRequest) {
  const body = await request.json();
  const parsed = loginSchema.safeParse(body);
  if (!parsed.success) {
    return NextResponse.json({ error: 'Invalid input' }, { status: 400 });
  }

  // Forward credentials to .NET backend
  const backendRes = await fetch(`${process.env.DOTNET_API_URL}/api/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(parsed.data),
  });

  if (!backendRes.ok) {
    const err = await backendRes.json().catch(() => ({}));
    return NextResponse.json(
      { error: err.message ?? 'Authentication failed' },
      { status: 401 }
    );
  }

  const { accessToken, refreshToken, user } = await backendRes.json();

  // Store tokens in httpOnly cookies — browser JS cannot access these
  const cookieStore = await cookies();

  cookieStore.set('access-token', accessToken, {
    httpOnly: true,
    secure: process.env.NODE_ENV === 'production',
    sameSite: 'lax',
    maxAge: 60 * 15,  // 15 minutes
    path: '/',
  });

  cookieStore.set('refresh-token', refreshToken, {
    httpOnly: true,
    secure: process.env.NODE_ENV === 'production',
    sameSite: 'lax',
    maxAge: 60 * 60 * 24 * 7,  // 7 days
    path: '/',
  });

  // Return only safe user data — never return raw tokens to the client
  return NextResponse.json({ user: { id: user.id, name: user.name, role: user.role } });
}
```

**The flow:** Browser submits credentials → Next.js Route Handler forwards to .NET → .NET returns JWT → Route Handler stores JWT in httpOnly cookie → returns only user profile to browser → `router.push('/goals')` triggers navigation → `router.refresh()` forces Server Components to re-render with the new cookie.

---

## Workflow B: authenticated page load with Server Component data fetching

When a user navigates to `/goals`, the Server Component reads the JWT from the httpOnly cookie, fetches data from the .NET API server-to-server, prefetches into TanStack Query, and hydrates the client cache — all before the browser receives any HTML.

### The Data Access Layer (defense-in-depth)

**Critical security note:** After CVE-2025-29927 demonstrated that Next.js middleware could be bypassed, the Next.js team now recommends a **Data Access Layer (DAL)** pattern. Never rely solely on middleware for authentication. Verify auth at every data access point.

```typescript
// lib/dal.ts — Data Access Layer
import { cache } from 'react';
import { cookies } from 'next/headers';
import { redirect } from 'next/navigation';

export const getSession = cache(async () => {
  const cookieStore = await cookies();
  const token = cookieStore.get('access-token')?.value;

  if (!token) {
    redirect('/login');
  }

  return { token };
});

// Authenticated fetch utility for Server Components
export async function fetchFromApi<T>(endpoint: string): Promise<T> {
  const { token } = await getSession();

  const res = await fetch(`${process.env.DOTNET_API_URL}${endpoint}`, {
    headers: {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
  });

  if (res.status === 401) {
    redirect('/login');
  }

  if (!res.ok) {
    throw new Error(`API error: ${res.status} ${res.statusText}`);
  }

  return res.json();
}
```

`React.cache()` memoizes `getSession()` so multiple components in a single render pass share one auth check rather than making redundant calls.

### TanStack Query setup for App Router

```typescript
// lib/get-query-client.ts
import { isServer, QueryClient } from '@tanstack/react-query';

function makeQueryClient() {
  return new QueryClient({
    defaultOptions: {
      queries: {
        staleTime: 60 * 1000,  // 60 seconds — prevents immediate refetch after hydration
      },
    },
  });
}

let browserQueryClient: QueryClient | undefined;

export function getQueryClient() {
  if (isServer) {
    return makeQueryClient();  // Server: always new (per-request isolation)
  }
  if (!browserQueryClient) {
    browserQueryClient = makeQueryClient();  // Browser: singleton
  }
  return browserQueryClient;
}
```

```typescript
// app/providers.tsx
'use client';

import { QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import { getQueryClient } from '@/lib/get-query-client';

export function Providers({ children }: { children: React.ReactNode }) {
  const queryClient = getQueryClient();
  return (
    <QueryClientProvider client={queryClient}>
      {children}
      <ReactQueryDevtools initialIsOpen={false} />
    </QueryClientProvider>
  );
}
```

```typescript
// app/layout.tsx
import { Providers } from './providers';

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en">
      <body>
        <Providers>{children}</Providers>
      </body>
    </html>
  );
}
```

### Reusable query options for type safety

```typescript
// lib/queries/goals.ts
import { queryOptions } from '@tanstack/react-query';

export type Goal = {
  id: string;
  title: string;
  status: 'active' | 'completed';
  progress: number;
};

type GoalsParams = { page: number; status?: string };

export const goalsQueryOptions = (params: GoalsParams) =>
  queryOptions({
    queryKey: ['goals', params],
    queryFn: () =>
      fetch(`/api/proxy/goals?page=${params.page}&status=${params.status ?? ''}`)
        .then(res => {
          if (!res.ok) throw new Error('Failed to fetch goals');
          return res.json() as Promise<{ items: Goal[]; totalPages: number; hasMore: boolean }>;
        }),
    staleTime: 60 * 1000,
  });
```

### Server Component with prefetching and HydrationBoundary

```typescript
// app/goals/page.tsx (Server Component)
import { dehydrate, HydrationBoundary, QueryClient } from '@tanstack/react-query';
import { fetchFromApi } from '@/lib/dal';
import { GoalsList } from './goals-list';

export default async function GoalsPage() {
  const queryClient = new QueryClient();

  // Prefetch on the server — this calls .NET directly (server-to-server)
  await queryClient.prefetchQuery({
    queryKey: ['goals', { page: 1, status: '' }],
    queryFn: () => fetchFromApi('/api/goals?page=1'),
  });

  return (
    <HydrationBoundary state={dehydrate(queryClient)}>
      <h1>Your Goals</h1>
      <GoalsList />
    </HydrationBoundary>
  );
}
```

**The hydration flow in detail:**

1. Server Component creates a fresh `QueryClient` (per-request — no data leaks between users)
2. `prefetchQuery` fetches from .NET, stores the result in the server-side QueryClient cache
3. `dehydrate(queryClient)` serializes the cache into a plain JSON object
4. `HydrationBoundary` streams the dehydrated state to the browser as part of the HTML
5. On the client, `HydrationBoundary` hydrates the data into the browser's singleton `QueryClient`
6. Any `useQuery` with a matching `queryKey` picks up the data **instantly** — no loading spinner on first render
7. After `staleTime` expires, TanStack Query refetches in the background

---

## Workflow C: client-side pagination and filtering without page reloads

When a user clicks a pagination button or applies a filter, TanStack Query handles the request entirely client-side. The queryKey includes pagination/filter params — changing them triggers an automatic refetch.

```typescript
// app/goals/goals-list.tsx
'use client';

import { useQuery, useQueryClient, keepPreviousData } from '@tanstack/react-query';
import { useState, useEffect } from 'react';
import { goalsQueryOptions } from '@/lib/queries/goals';

export function GoalsList() {
  const queryClient = useQueryClient();
  const [page, setPage] = useState(1);
  const [statusFilter, setStatusFilter] = useState('');

  const { data, isFetching, isPlaceholderData } = useQuery({
    ...goalsQueryOptions({ page, status: statusFilter }),
    placeholderData: keepPreviousData,  // Show previous data while fetching next page
  });

  // Prefetch the next page for instant navigation
  useEffect(() => {
    if (!isPlaceholderData && data?.hasMore) {
      queryClient.prefetchQuery(goalsQueryOptions({ page: page + 1, status: statusFilter }));
    }
  }, [data, isPlaceholderData, page, statusFilter, queryClient]);

  return (
    <div>
      <select value={statusFilter} onChange={(e) => { setStatusFilter(e.target.value); setPage(1); }}>
        <option value="">All</option>
        <option value="active">Active</option>
        <option value="completed">Completed</option>
      </select>

      <ul style={{ opacity: isFetching ? 0.7 : 1, transition: 'opacity 200ms' }}>
        {data?.items.map((goal) => (
          <li key={goal.id}>{goal.title} — {goal.progress}%</li>
        ))}
      </ul>

      <button onClick={() => setPage(p => p - 1)} disabled={page === 1}>
        Previous
      </button>
      <span>Page {page}</span>
      <button onClick={() => setPage(p => p + 1)} disabled={isPlaceholderData || !data?.hasMore}>
        Next
      </button>
      {isFetching && <span>Updating…</span>}
    </div>
  );
}
```

**How the client gets the JWT for these requests:** The client calls `/api/proxy/goals?page=2` — a same-origin request. The browser automatically attaches the httpOnly cookie. The Next.js proxy Route Handler reads it and forwards to .NET:

```typescript
// app/api/proxy/[...path]/route.ts — Catch-all proxy
import { NextRequest, NextResponse } from 'next/server';
import { cookies } from 'next/headers';

async function proxyToBackend(request: NextRequest) {
  const cookieStore = await cookies();
  const token = cookieStore.get('access-token')?.value;

  if (!token) {
    return NextResponse.json({ error: 'Unauthorized' }, { status: 401 });
  }

  const path = request.nextUrl.pathname.replace('/api/proxy/', '');
  const url = `${process.env.DOTNET_API_URL}/api/${path}${request.nextUrl.search}`;

  const headers = new Headers();
  headers.set('Authorization', `Bearer ${token}`);
  headers.set('Content-Type', request.headers.get('Content-Type') ?? 'application/json');

  const res = await fetch(url, {
    method: request.method,
    headers,
    body: ['GET', 'HEAD'].includes(request.method) ? undefined : await request.text(),
  });

  return new NextResponse(res.body, {
    status: res.status,
    headers: res.headers,
  });
}

export const GET = proxyToBackend;
export const POST = proxyToBackend;
export const PUT = proxyToBackend;
export const DELETE = proxyToBackend;
export const PATCH = proxyToBackend;
```

`keepPreviousData` is the key UX optimization — it keeps the current list visible while the next page loads, avoiding a blank/loading state between pages. Combined with next-page prefetching, pagination feels instant.

---

## Workflow D: mutations with optimistic updates and cache invalidation

Mutations (create, update, delete) can use either TanStack Query's `useMutation` for rich client-side UX or Server Actions for progressive enhancement. The recommended approach for this stack combines both: **useMutation calls a Server Action** (or proxy Route Handler), then invalidates the TanStack Query cache.

### TanStack Query mutation with optimistic updates

```typescript
// app/goals/create-goal-form.tsx
'use client';

import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { z } from 'zod';

const createGoalSchema = z.object({
  title: z.string().min(1, 'Title is required'),
  description: z.string().optional(),
});
type CreateGoalInput = z.infer<typeof createGoalSchema>;

export function CreateGoalForm() {
  const queryClient = useQueryClient();
  const { register, handleSubmit, reset, formState: { errors } } = useForm<CreateGoalInput>({
    resolver: zodResolver(createGoalSchema),
  });

  const mutation = useMutation({
    mutationFn: async (newGoal: CreateGoalInput) => {
      const res = await fetch('/api/proxy/goals', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(newGoal),
      });
      if (!res.ok) throw new Error('Failed to create goal');
      return res.json();
    },

    // Optimistic update via cache manipulation
    onMutate: async (newGoal) => {
      await queryClient.cancelQueries({ queryKey: ['goals'] });
      const previousGoals = queryClient.getQueryData(['goals', { page: 1, status: '' }]);

      queryClient.setQueryData(['goals', { page: 1, status: '' }], (old: any) =>
        old ? {
          ...old,
          items: [{ id: `temp-${Date.now()}`, ...newGoal, status: 'active', progress: 0 }, ...old.items],
        } : old
      );

      return { previousGoals };
    },

    onError: (_err, _newGoal, context) => {
      // Rollback on error
      if (context?.previousGoals) {
        queryClient.setQueryData(['goals', { page: 1, status: '' }], context.previousGoals);
      }
    },

    onSettled: () => {
      // Always refetch to reconcile optimistic state with server truth
      queryClient.invalidateQueries({ queryKey: ['goals'] });
    },
  });

  return (
    <form onSubmit={handleSubmit((data) => { mutation.mutate(data); reset(); })}>
      <input {...register('title')} placeholder="Goal title" />
      {errors.title && <p>{errors.title.message}</p>}
      <button type="submit" disabled={mutation.isPending}>
        {mutation.isPending ? 'Creating…' : 'Add Goal'}
      </button>
      {mutation.isError && <p className="text-red-500">{mutation.error.message}</p>}
    </form>
  );
}
```

**The optimistic update sequence:** `onMutate` fires before the network request — it snapshots current data, inserts the new item into the cache with a temporary ID, and returns the snapshot for rollback. If the mutation fails, `onError` restores the snapshot. `onSettled` always runs and invalidates all `['goals']` queries to reconcile with the server.

### Server Action alternative for mutations

When you want progressive enhancement (forms work without JavaScript) or need to revalidate Next.js server-side caches:

```typescript
// app/actions/goals.ts
'use server';

import { cookies } from 'next/headers';
import { revalidatePath } from 'next/cache';
import { redirect } from 'next/navigation';
import { z } from 'zod';

const createGoalSchema = z.object({
  title: z.string().min(1),
  description: z.string().optional(),
});

export async function createGoalAction(prevState: any, formData: FormData) {
  const cookieStore = await cookies();
  const token = cookieStore.get('access-token')?.value;
  if (!token) redirect('/login');

  const parsed = createGoalSchema.safeParse(Object.fromEntries(formData.entries()));
  if (!parsed.success) {
    return { error: 'Invalid input', fieldErrors: parsed.error.flatten().fieldErrors };
  }

  const res = await fetch(`${process.env.DOTNET_API_URL}/api/goals`, {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(parsed.data),
  });

  if (!res.ok) {
    return { error: 'Failed to create goal' };
  }

  revalidatePath('/goals');
  return { success: true };
}
```

---

## openapi-fetch configuration with type-safe auth middleware

openapi-fetch v0.15+ provides a middleware system that intercepts every request. Create separate clients for server and client environments:

```typescript
// lib/api/schema.d.ts — Generated from .NET API's OpenAPI spec
// Run: npx openapi-typescript https://your-dotnet-api.com/swagger/v1/swagger.json -o ./lib/api/schema.d.ts

// lib/api/server-client.ts — For Server Components and Server Actions
import createClient, { type Middleware } from 'openapi-fetch';
import type { paths } from './schema';
import { cookies } from 'next/headers';

export async function createServerApiClient() {
  const cookieStore = await cookies();
  const token = cookieStore.get('access-token')?.value;

  const client = createClient<paths>({
    baseUrl: process.env.DOTNET_API_URL!,
  });

  const authMiddleware: Middleware = {
    async onRequest({ request }) {
      if (token) {
        request.headers.set('Authorization', `Bearer ${token}`);
      }
      return request;
    },
  };

  client.use(authMiddleware);
  return client;
}

// lib/api/browser-client.ts — For Client Components (routes through proxy)
import createClient, { type Middleware } from 'openapi-fetch';
import type { paths } from './schema';

const errorMiddleware: Middleware = {
  async onResponse({ response }) {
    if (response.status === 401) {
      // Attempt token refresh, then retry or redirect
      window.location.href = '/login';
    }
    if (!response.ok) {
      throw new Error(`API error: ${response.status} ${response.statusText}`);
    }
  },
};

export const browserApiClient = createClient<paths>({
  baseUrl: '/api/proxy',  // Routes through Next.js proxy — cookie auto-attached
});

browserApiClient.use(errorMiddleware);
```

### Type-safe TanStack Query integration with openapi-react-query

The `openapi-react-query` package (a **1kb wrapper**) generates fully typed query hooks from your OpenAPI schema:

```typescript
// lib/api/query-client.ts
import createFetchClient from 'openapi-fetch';
import createClient from 'openapi-react-query';
import type { paths } from './schema';

const fetchClient = createFetchClient<paths>({ baseUrl: '/api/proxy' });
export const $api = createClient(fetchClient);

// Usage in any Client Component:
const { data, isLoading } = $api.useQuery('get', '/goals/{id}', {
  params: { path: { id: goalId } },
});

const { mutate } = $api.useMutation('post', '/goals');
mutate({ body: { title: 'New Goal', description: 'Details' } });
```

---

## Middleware and route protection with defense-in-depth

After **CVE-2025-29927** (CVSS 9.1, March 2025) showed that attackers could bypass Next.js middleware by spoofing internal headers, the framework team changed their guidance: **middleware is an optimistic first layer, not the security boundary**. Always verify auth at the data access layer too.

```typescript
// middleware.ts (or proxy.ts in Next.js 16)
import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';

export function middleware(request: NextRequest) {
  const token = request.cookies.get('access-token')?.value;
  const refreshToken = request.cookies.get('refresh-token')?.value;
  const { pathname } = request.nextUrl;

  const publicRoutes = ['/login', '/register', '/forgot-password'];
  if (publicRoutes.some(route => pathname.startsWith(route))) {
    return NextResponse.next();
  }

  // No access token — try refresh
  if (!token && refreshToken) {
    // Middleware can attempt a lightweight token refresh
    // For complex refresh logic, redirect to a refresh endpoint
    return NextResponse.redirect(new URL('/api/auth/refresh', request.url));
  }

  // No tokens at all — redirect to login
  if (!token) {
    const loginUrl = new URL('/login', request.url);
    loginUrl.searchParams.set('callbackUrl', pathname);
    return NextResponse.redirect(loginUrl);
  }

  return NextResponse.next();
}

export const config = {
  matcher: ['/((?!api|_next/static|_next/image|favicon.ico).*)'],
};
```

**Three-layer auth verification:**

1. **Middleware** — fast optimistic redirect (catches most unauthenticated requests)
2. **Data Access Layer** — `getSession()` with `React.cache()` in every Server Component that fetches data
3. **Server Actions** — re-verify auth before every mutation

---

## Token refresh mechanism for long-running sessions

The token refresh flow handles expired access tokens transparently. Implement refresh at two levels: **proactively in middleware** (before Server Components render) and **reactively in the proxy** (when a 401 occurs during client-side requests).

```typescript
// app/api/auth/refresh/route.ts
import { NextRequest, NextResponse } from 'next/server';
import { cookies } from 'next/headers';

export async function POST(request: NextRequest) {
  const cookieStore = await cookies();
  const refreshToken = cookieStore.get('refresh-token')?.value;

  if (!refreshToken) {
    return NextResponse.json({ error: 'No refresh token' }, { status: 401 });
  }

  const backendRes = await fetch(`${process.env.DOTNET_API_URL}/api/auth/refresh`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ refreshToken }),
  });

  if (!backendRes.ok) {
    // Refresh failed — clear cookies, force re-login
    cookieStore.delete('access-token');
    cookieStore.delete('refresh-token');
    return NextResponse.json({ error: 'Session expired' }, { status: 401 });
  }

  const { accessToken: newAccess, refreshToken: newRefresh } = await backendRes.json();

  cookieStore.set('access-token', newAccess, {
    httpOnly: true, secure: process.env.NODE_ENV === 'production',
    sameSite: 'lax', maxAge: 60 * 15, path: '/',
  });
  cookieStore.set('refresh-token', newRefresh, {
    httpOnly: true, secure: process.env.NODE_ENV === 'production',
    sameSite: 'lax', maxAge: 60 * 60 * 24 * 7, path: '/',
  });

  return NextResponse.json({ success: true });
}
```

### Preventing race conditions during concurrent refresh

When multiple client-side requests fail simultaneously with 401, a **request queue pattern** ensures only one refresh occurs:

```typescript
// lib/api/refresh-queue.ts
let isRefreshing = false;
let failedQueue: Array<{
  resolve: (value: unknown) => void;
  reject: (reason?: any) => void;
}> = [];

function processQueue(error: Error | null) {
  failedQueue.forEach(({ resolve, reject }) => {
    if (error) reject(error);
    else resolve(undefined);
  });
  failedQueue = [];
}

export async function handleTokenRefresh(): Promise<void> {
  if (isRefreshing) {
    return new Promise((resolve, reject) => {
      failedQueue.push({ resolve, reject });
    });
  }

  isRefreshing = true;
  try {
    const res = await fetch('/api/auth/refresh', { method: 'POST' });
    if (!res.ok) throw new Error('Refresh failed');
    processQueue(null);
  } catch (error) {
    processQueue(error as Error);
    window.location.href = '/login';
    throw error;
  } finally {
    isRefreshing = false;
  }
}
```

Integrate this into your TanStack Query configuration for automatic retry on 401:

```typescript
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 60 * 1000,
      retry: (failureCount, error) => {
        // Don't retry auth errors — the refresh handler will take care of it
        if (error.message.includes('401')) return false;
        return failureCount < 3;
      },
    },
  },
});
```

---

## When to use Server Components versus Client Components

The decision framework is straightforward: **Server Components for initial data loading, Client Components for interactivity**.

| Use Case | Component Type | Why |
|----------|---------------|-----|
| Page-level data fetch + layout | Server Component | Fetches with auth cookie, no JS shipped to browser |
| List with pagination/filtering | Client Component | Needs `useState` for page/filter, `useQuery` for data |
| Forms with validation | Client Component | React Hook Form requires hooks, real-time validation |
| Static content with auth gate | Server Component | Just needs `getSession()` check |
| Optimistic updates | Client Component | `useMutation` + cache manipulation |
| Displaying user info in header | Client Component (hydrated by server) | Needs to react to auth state changes |

**The composition pattern:** A Server Component at the route level prefetches data and wraps a Client Component with `HydrationBoundary`. The Client Component takes over interactivity after hydration. This gives you the best of both worlds — fast initial render with zero JS for data fetching, then full interactivity once React hydrates.

---

## Production concerns and error handling

### TanStack Query error boundaries

```typescript
// app/goals/error.tsx — Next.js Error Boundary (auto-catches render errors)
'use client';

export default function GoalsError({ error, reset }: { error: Error; reset: () => void }) {
  return (
    <div>
      <h2>Something went wrong loading your goals</h2>
      <p>{error.message}</p>
      <button onClick={reset}>Try again</button>
    </div>
  );
}
```

### Request retry with exponential backoff

```typescript
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 3,
      retryDelay: (attemptIndex) => Math.min(1000 * 2 ** attemptIndex, 30000),
    },
  },
});
```

### Type safety end-to-end

The type chain flows from .NET OpenAPI spec → `openapi-typescript` generated types → `openapi-fetch` typed client → `openapi-react-query` typed hooks → TypeScript components. A schema change in .NET propagates type errors through the entire frontend at compile time. Regenerate types in CI with:

```bash
npx openapi-typescript $DOTNET_API_URL/swagger/v1/swagger.json -o ./lib/api/schema.d.ts
```

### Zod v4 runtime validation

Even with TypeScript types, **always validate at runtime boundaries** — API responses, form inputs, Server Action parameters. Zod v4.1 provides this validation layer. Share schemas between client-side form validation and server-side Server Action validation to guarantee consistency.

---

## Conclusion: the architecture that ships

The hybrid BFF pattern resolves the fundamental tension in this stack. Server Components get the performance benefit of direct server-to-server calls to the .NET API. Client Components get secure, seamless authentication through httpOnly cookies and the Next.js proxy layer. TanStack Query bridges the two worlds through its dehydration/hydration system — data fetched on the server appears instantly in client components without a loading state, then TanStack Query takes over for pagination, mutations, and cache management.

**Three architectural decisions matter most.** First, route all authentication through Next.js Route Handlers so JWTs live exclusively in httpOnly cookies — this eliminates the largest class of token theft attacks. Second, implement defense-in-depth auth verification (middleware → DAL → Server Actions) rather than trusting any single layer. Third, use the `queryOptions` helper from TanStack Query to share query definitions between server prefetching and client hooks — this ensures cache keys always match and hydration works correctly.

The openapi-fetch middleware system and openapi-react-query integration provide the type-safety backbone. When the .NET API's OpenAPI spec changes, generated types cascade through every query, mutation, and component in the frontend. Combined with Zod validation at runtime boundaries, this creates a system where API contract violations surface as build errors rather than production bugs.