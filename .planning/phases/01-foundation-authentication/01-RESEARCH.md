# Phase 1: Foundation & Authentication - Research

**Date:** 2026-02-15
**Researcher:** Claude (gsd-phase-researcher)
**Status:** Complete

---

## Executive Summary

This research provides everything needed to plan Phase 1: implementing custom JWT authentication with the BFF proxy pattern in OrbitSpace. The phase involves completely removing Clerk and implementing a secure, production-ready authentication system that follows defense-in-depth principles.

**Key Findings:**
1. The codebase already has 80% of backend auth infrastructure in place (JWT service, password hashing, user repository)
2. The frontend has Clerk deeply integrated in middleware and API routes - complete removal and replacement required
3. The architecture guide provides a proven pattern for httpOnly cookie-based JWT storage with token rotation
4. Multi-device support requires a refresh token database table (not yet implemented)

**Complexity Assessment:** Medium-High
- Backend: Low (mostly extension of existing patterns)
- Frontend: Medium-High (complete Clerk removal, new auth flows, middleware rewrite)
- Security: High (critical to implement defense-in-depth correctly)

---

## Part 1: Current State Analysis

### Backend (dotnet-web-api)

**What Already Exists:**

1. **User Entity & Repository** ✅
   - Location: `OrbitSpace.Domain/Entities/User.cs`
   - Fields: `Id`, `Email`, `FirstName`, `LastName`, `PasswordHash`
   - Repository: `OrbitSpace.Infrastructure/Persistence/Repositories/UserRepository.cs`
   - Methods: `CreateAsync()`, `GetByEmailAsync()`

2. **Authentication Service** ✅
   - Location: `OrbitSpace.Application/Services/AuthenticationService.cs`
   - Implements: `RegisterAsync()`, `LoginAsync()`
   - Uses BCrypt for password hashing (via `IPasswordHasherService`)
   - Returns `LoginResponseDto` with `AccessToken` and `UserDto`

3. **JWT Token Service** ✅
   - Location: `OrbitSpace.Infrastructure/Services/JwtTokenService.cs`
   - Generates access tokens with claims: `Jti`, `Sub` (userId), `Email`
   - Currently hardcoded to 7-day expiration (line 29)
   - Uses HS512 signature algorithm
   - Settings from `appsettings.json` JwtSettings section

4. **Password Hashing** ✅
   - Implementation: BCrypt via `BCrypt.Net-Next` (v4.0.3)
   - Service: `OrbitSpace.Infrastructure/Services/BCryptPasswordHasherService.cs`
   - Secure by default (BCrypt auto-salts and uses adaptive cost factor)

5. **Authentication Controller** ⚠️ NEEDS MAJOR CHANGES
   - Location: `OrbitSpace/Controllers/AuthenticationController.cs`
   - Has `Register` and `Login` endpoints
   - **Problem:** Login accepts separate string parameters instead of DTO
   - **Problem:** Marked `[ApiExplorerSettings(IgnoreApi = true)]` - hidden from OpenAPI
   - **Problem:** No refresh endpoint implemented

6. **JWT Validation Middleware** ✅
   - Location: `OrbitSpace/Startup/AuthenticationConfig.cs`
   - Configured for JWT Bearer authentication
   - Custom 401 response with RFC 7807 Problem Details
   - Token validation parameters set up correctly

7. **Database Migration** ✅
   - Users table created in `InitialCreate` migration (20260214195509)
   - Schema: `id`, `email`, `first_name`, `last_name`, `password_hash`
   - Constraints: email max 256, names max 100, password_hash max 256

**What's Missing:**

1. **Refresh Token System** ❌
   - No `RefreshToken` entity
   - No refresh token repository
   - No `ITokenService.GenerateRefreshToken()` method
   - No token rotation logic
   - No device/session tracking

2. **Date of Birth Field** ❌
   - User entity doesn't have `DateOfBirth` property (required per phase context)
   - Need migration to add this field

3. **Token Expiration Strategy** ❌
   - Access token expiration hardcoded to 7 days (too long for security)
   - No configuration for "remember me" functionality
   - Refresh token expiration not implemented

4. **Email Verification Stub** ❌
   - User entity needs `EmailVerified` boolean and `EmailVerificationToken`
   - Not implementing verification flow now, but need database fields for future

5. **Login Request DTO** ⚠️
   - `LoginRequestDto` doesn't exist - controller accepts raw strings
   - Need to create DTO for proper validation and OpenAPI spec

### Frontend (next-js-app)

**What Already Exists:**

1. **Zod Schemas** ✅ (but wrong location)
   - `src/shared/schemas/loginSchema.ts` - validates email + password (min 6 chars)
   - `src/shared/schemas/registerSchema.ts` - validates email, firstName, lastName, password, repeatPassword
   - **Problem:** Password minimum is 6, but phase context says 4 for development

2. **Clerk Integration** 🔴 MUST REMOVE COMPLETELY
   - `proxy.ts` - Clerk middleware protecting all routes
   - `src/shared/api/api-server-client.ts` - uses Clerk's `auth()` and `getToken()`
   - `app/api/(proxy)/[...path]/route.ts` - proxy route uses Clerk token
   - `.env.local` - Clerk publishable and secret keys
   - `.clerk/` directory exists
   - Dependencies: `@clerk/nextjs` in package.json

3. **API Proxy Pattern** ✅ (but coupled to Clerk)
   - Browser client: `src/shared/api/api-client.ts` - baseUrl: `/` (routes through Next.js)
   - Server client: `src/shared/api/api-server-client.ts` - direct backend connection
   - Proxy route: `app/api/(proxy)/[...path]/route.ts` - forwards to .NET API
   - **Good:** Pattern is correct, just need to replace Clerk auth with cookie-based auth

4. **FSD Architecture** ✅
   - `src/entities/` - domain entities (goal, todo-item, activity)
   - `src/features/` - user interactions
   - `src/widgets/` - composite UI blocks
   - `src/pages/` - page-level components
   - `src/shared/` - UI components, utilities, API client

5. **TanStack Query Setup** ✅
   - Already in use for data fetching (see `src/entities/goal/api/goal-queries.ts`)
   - Query key factories implemented
   - React Query provider in `app/layout.tsx` (via Providers)

**What's Missing:**

1. **Auth Entity Layer** ❌
   - No `src/entities/auth/` directory
   - Need: `model/queries.ts`, `model/schemas.ts`, `ui/` components

2. **Auth Pages** ❌
   - No `/login` page
   - No `/register` page
   - No auth layout (centered card pattern)

3. **Auth API Routes** ❌
   - No `/api/auth/login` route handler
   - No `/api/auth/register` route handler
   - No `/api/auth/refresh` route handler
   - No `/api/auth/logout` route handler

4. **Data Access Layer (DAL)** ❌
   - No `lib/dal.ts` for defense-in-depth auth verification
   - Critical security pattern - cannot rely on middleware alone

5. **Custom Middleware** ❌
   - Need to replace `proxy.ts` (Clerk middleware) with custom auth middleware
   - Must check for `access-token` cookie
   - Must redirect to login if missing
   - Must handle token refresh flow

6. **Auth Context/State** ❌
   - No client-side auth context for user info
   - No `useAuth()` hook
   - No way to access current user in components

---

## Part 2: Authentication Architecture Deep Dive

### The BFF (Backend-for-Frontend) Proxy Pattern

**Why This Architecture?**

1. **Security:** JWTs never exposed to browser JavaScript (XSS protection)
2. **Simplicity:** No CORS configuration needed (same-origin requests)
3. **Flexibility:** Next.js Route Handlers can add request validation, rate limiting, logging
4. **SSR-Friendly:** Server Components can read cookies and fetch data before HTML is sent

**The Flow:**

```
┌─────────────────────────────────────────────────────────────┐
│                         BROWSER                              │
│  - No JWT access (httpOnly cookies)                         │
│  - Makes requests to same origin (http://localhost:3000)    │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                  NEXT.JS SERVER (BFF)                        │
│  ┌─────────────────────────────────────────────────────┐   │
│  │ Route Handler: /api/auth/login                      │   │
│  │  1. Validate credentials with Zod                   │   │
│  │  2. Forward to .NET API                             │   │
│  │  3. Store JWT in httpOnly cookie                    │   │
│  │  4. Return user data (no token)                     │   │
│  └─────────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────────┐   │
│  │ Proxy Route: /api/proxy/[...path]                   │   │
│  │  1. Read JWT from cookie                            │   │
│  │  2. Add Authorization header                        │   │
│  │  3. Forward request to .NET API                     │   │
│  │  4. Return response                                 │   │
│  └─────────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────────┐   │
│  │ Server Component: app/goals/page.tsx                │   │
│  │  1. Read JWT from cookie (via DAL)                  │   │
│  │  2. Fetch from .NET API directly                    │   │
│  │  3. Prefetch into TanStack Query                    │   │
│  │  4. Hydrate client cache                            │   │
│  └─────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    .NET API (Backend)                        │
│  - Validates JWT signature                                   │
│  - Extracts claims (userId, email)                           │
│  - Processes request                                         │
│  - Returns JSON response                                     │
└─────────────────────────────────────────────────────────────┘
```

### Token Strategy: Access + Refresh with Rotation

**Token Types:**

1. **Access Token**
   - **Purpose:** Short-lived token for API authorization
   - **Lifetime:** 15 minutes (configurable)
   - **Storage:** httpOnly cookie named `access-token`
   - **Claims:** `jti` (unique token ID), `sub` (userId), `email`, `iat`, `exp`
   - **Cookie Settings:**
     - `httpOnly: true` (JavaScript cannot access)
     - `secure: true` (HTTPS only in production)
     - `sameSite: 'lax'` (CSRF protection)
     - `path: '/'` (available to all routes)

2. **Refresh Token**
   - **Purpose:** Long-lived token to obtain new access tokens
   - **Lifetime:** 7 days (session) or 30 days ("remember me")
   - **Storage:** httpOnly cookie named `refresh-token`
   - **Database Record:** Stored in `refresh_tokens` table
   - **Rotation:** Each refresh generates a new refresh token and invalidates the old one
   - **Revocation:** Can be revoked (logout, compromise detection)

**Why Refresh Tokens?**

- Access tokens are short-lived (15 min) → less damage if stolen
- Refresh tokens allow long sessions without re-login
- Refresh tokens can be revoked server-side (logout all devices)
- Rotation pattern detects token theft (if old refresh token used, revoke all)

**Token Refresh Flow:**

```
Client → /api/auth/refresh (with refresh-token cookie)
   ↓
Next.js reads refresh token from cookie
   ↓
Next.js → .NET /api/auth/refresh
   ↓
.NET validates refresh token (checks DB, expiration)
   ↓
.NET generates NEW access token + NEW refresh token
   ↓
.NET marks old refresh token as used/revoked in DB
   ↓
.NET returns { accessToken, refreshToken }
   ↓
Next.js stores both in httpOnly cookies
   ↓
Next.js returns success to client
```

**Race Condition Prevention:**

When multiple requests fail with 401 simultaneously, avoid multiple refresh calls:

```typescript
// Request queue pattern
let isRefreshing = false;
let refreshQueue: Array<{resolve, reject}> = [];

async function handleTokenRefresh() {
  if (isRefreshing) {
    return new Promise((resolve, reject) => {
      refreshQueue.push({ resolve, reject });
    });
  }

  isRefreshing = true;
  try {
    await fetch('/api/auth/refresh', { method: 'POST' });
    refreshQueue.forEach(p => p.resolve());
  } catch (error) {
    refreshQueue.forEach(p => p.reject(error));
    window.location.href = '/login';
  } finally {
    isRefreshing = false;
    refreshQueue = [];
  }
}
```

### Defense-in-Depth Security Model

**Three Layers of Auth Verification:**

1. **Middleware (Optimistic Layer)**
   - **File:** `proxy.ts` (Next.js 16 convention, replaces `middleware.ts`)
   - **Purpose:** Fast redirect for unauthenticated requests
   - **Check:** Presence of `access-token` cookie
   - **Action:** Redirect to `/login?callbackUrl=/original-path`
   - **Limitation:** Can be bypassed (CVE-2025-29927) - never trust alone

2. **Data Access Layer (Critical Security Boundary)**
   - **File:** `lib/dal.ts`
   - **Pattern:** `React.cache()` wrapper around `getSession()`
   - **Purpose:** Verify auth in every Server Component that fetches data
   - **Check:** Read and validate `access-token` from cookie
   - **Action:** Redirect to `/login` if missing/invalid
   - **Key Feature:** `React.cache()` memoizes within a single render pass

3. **Server Actions (Mutation Layer)**
   - **Pattern:** Call `getSession()` at the start of every Server Action
   - **Purpose:** Verify auth before any mutation (create, update, delete)
   - **Check:** Token presence and validity
   - **Action:** Return error or redirect if unauthorized

**Example DAL Implementation:**

```typescript
// lib/dal.ts
import { cache } from 'react';
import { cookies } from 'next/headers';
import { redirect } from 'next/navigation';

export const getSession = cache(async () => {
  const cookieStore = await cookies();
  const token = cookieStore.get('access-token')?.value;

  if (!token) {
    redirect('/login');
  }

  // Optional: Decode and validate token structure
  // (Full validation happens on .NET API)

  return { token };
});

// Authenticated fetch for Server Components
export async function fetchFromApi<T>(endpoint: string): Promise<T> {
  const { token } = await getSession();

  const res = await fetch(`${process.env.BACKEND_BASE_URL}${endpoint}`, {
    headers: {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json'
    }
  });

  if (res.status === 401) {
    redirect('/login');
  }

  if (!res.ok) {
    throw new Error(`API error: ${res.status}`);
  }

  return res.json();
}
```

**Why Three Layers?**

- Middleware stops most unauthenticated traffic (performance)
- DAL ensures no Server Component can fetch without auth (security boundary)
- Server Actions verify auth at mutation points (data integrity)
- Even if middleware is bypassed, DAL and actions still protect

---

## Part 3: Database Schema Changes Required

### New Entity: RefreshToken

**Purpose:** Track active sessions, enable multi-device support, allow token revocation

**Entity Definition:**

```csharp
// OrbitSpace.Domain/Entities/RefreshToken.cs
public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string Token { get; set; }  // Hashed refresh token
    public DateTime CreatedAtUtc { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
    public DateTime? UsedAtUtc { get; set; }
    public string? ReplacedByToken { get; set; }  // For token rotation tracking
    public string? DeviceInfo { get; set; }  // User-Agent, IP for session identification
    public bool IsRevoked => RevokedAtUtc.HasValue;
    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;
    public bool IsActive => !IsRevoked && !IsExpired;

    // Navigation property
    public User User { get; set; } = null!;
}
```

**Database Migration:**

```sql
CREATE TABLE refresh_tokens (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL,
    token VARCHAR(512) NOT NULL,  -- Store hashed token
    created_at_utc TIMESTAMP NOT NULL,
    expires_at_utc TIMESTAMP NOT NULL,
    revoked_at_utc TIMESTAMP NULL,
    used_at_utc TIMESTAMP NULL,
    replaced_by_token VARCHAR(512) NULL,
    device_info VARCHAR(500) NULL,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

CREATE INDEX idx_refresh_tokens_user_id ON refresh_tokens(user_id);
CREATE INDEX idx_refresh_tokens_token ON refresh_tokens(token);
CREATE INDEX idx_refresh_tokens_expires_at ON refresh_tokens(expires_at_utc);
```

**Why Hash Refresh Tokens?**

- Refresh tokens are sensitive (can generate access tokens)
- If database is compromised, plaintext tokens allow full account access
- Store SHA256 hash of token in DB
- When validating, hash incoming token and compare

### User Entity Changes

**Add Fields:**

1. `DateOfBirth` (DateTime) - Required per phase context
2. `EmailVerified` (bool, default false) - For future email verification
3. `EmailVerificationToken` (string?, nullable) - For future email verification
4. `CreatedAtUtc` (DateTime) - User registration timestamp
5. `UpdatedAtUtc` (DateTime) - Last profile update

**Updated Entity:**

```csharp
public class User
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string PasswordHash { get; set; }
    public DateTime DateOfBirth { get; set; }  // NEW
    public bool EmailVerified { get; set; } = false;  // NEW
    public string? EmailVerificationToken { get; set; }  // NEW
    public DateTime CreatedAtUtc { get; set; }  // NEW
    public DateTime UpdatedAtUtc { get; set; }  // NEW

    // Navigation properties
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
```

**Migration Strategy:**

1. Create new migration: `AddRefreshTokensAndUpdateUsers`
2. Add `refresh_tokens` table
3. Add new columns to `users` table
4. Set default `EmailVerified = false` for existing users (if any)
5. Existing users won't have `DateOfBirth` - allow NULL temporarily or require user update

---

## Part 4: Frontend Architecture Patterns

### Entity Layer: auth

**Directory Structure:**

```
src/entities/auth/
├── api/
│   ├── auth-handlers.ts      # Server-side API route logic
│   └── auth-query-keys.ts    # React Query key factory
├── model/
│   ├── schemas.ts            # Zod validation schemas
│   ├── types.ts              # TypeScript types
│   └── use-auth.ts           # Client-side auth hook
├── ui/
│   └── user-menu.tsx         # User profile dropdown (future)
└── index.ts                  # Public API exports
```

**Schemas (model/schemas.ts):**

```typescript
import { z } from 'zod';

export const loginSchema = z.object({
  email: z.string().email('Invalid email address'),
  password: z.string().min(4, 'Password must be at least 4 characters'),
  rememberMe: z.boolean().default(false)
});

export const registerSchema = z.object({
  email: z.string().email('Invalid email address'),
  password: z.string().min(4, 'Password must be at least 4 characters'),
  repeatPassword: z.string(),
  firstName: z.string().min(1, 'First name is required').max(50),
  lastName: z.string().min(1, 'Last name is required').max(50),
  dateOfBirth: z.coerce.date().refine(
    (date) => {
      const age = new Date().getFullYear() - date.getFullYear();
      return age >= 13; // Minimum age requirement
    },
    { message: 'You must be at least 13 years old' }
  )
}).refine((data) => data.password === data.repeatPassword, {
  message: 'Passwords do not match',
  path: ['repeatPassword']
});

export type LoginInput = z.infer<typeof loginSchema>;
export type RegisterInput = z.infer<typeof registerSchema>;
```

**Types (model/types.ts):**

```typescript
export type User = {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  dateOfBirth: string;
  emailVerified: boolean;
};

export type AuthResponse = {
  user: User;
};

export type AuthError = {
  error: string;
  fieldErrors?: Record<string, string[]>;
};
```

**Auth Hook (model/use-auth.ts):**

```typescript
'use client';

import { useQuery } from '@tanstack/react-query';
import { authQueryKeys } from '../api/auth-query-keys';

export function useAuth() {
  const { data, isLoading } = useQuery({
    queryKey: authQueryKeys.session(),
    queryFn: async () => {
      const res = await fetch('/api/auth/session');
      if (!res.ok) return null;
      return res.json();
    },
    staleTime: 5 * 60 * 1000, // 5 minutes
    retry: false
  });

  return {
    user: data?.user ?? null,
    isAuthenticated: !!data?.user,
    isLoading
  };
}
```

### Feature Layer: Auth Forms

**Directory Structure:**

```
src/features/auth/
├── login/
│   ├── ui/
│   │   └── login-form.tsx
│   └── index.ts
└── register/
    ├── ui/
    │   └── register-form.tsx
    └── index.ts
```

**Login Form Pattern:**

```typescript
'use client';

import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useRouter } from 'next/navigation';
import { useState, useTransition } from 'react';
import { loginSchema, type LoginInput } from '@/entities/auth';

export function LoginForm() {
  const router = useRouter();
  const [error, setError] = useState<string | null>(null);
  const [isPending, startTransition] = useTransition();

  const form = useForm<LoginInput>({
    resolver: zodResolver(loginSchema),
    defaultValues: { email: '', password: '', rememberMe: false }
  });

  const onSubmit = form.handleSubmit((data) => {
    startTransition(async () => {
      setError(null);

      const res = await fetch('/api/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
      });

      if (!res.ok) {
        const body = await res.json();
        setError(body.error ?? 'Login failed');
        return;
      }

      const callbackUrl = new URLSearchParams(window.location.search).get('callbackUrl');
      router.push(callbackUrl ?? '/');
      router.refresh();
    });
  });

  return (
    <form onSubmit={onSubmit}>
      {/* Form fields using shadcn/ui components */}
    </form>
  );
}
```

### Pages: Auth Routes

**Directory Structure:**

```
app/(auth)/
├── layout.tsx          # Auth layout (centered card, no sidebar)
├── login/
│   └── page.tsx
└── register/
    └── page.tsx
```

**Auth Layout Pattern:**

```typescript
// app/(auth)/layout.tsx
export default function AuthLayout({ children }: { children: React.ReactNode }) {
  return (
    <div className="min-h-screen flex items-center justify-center bg-background">
      <div className="w-full max-w-md">
        <div className="flex flex-col items-center mb-8">
          {/* Logo component */}
          <h1 className="text-3xl font-bold mt-4">OrbitSpace</h1>
        </div>
        <div className="bg-card border rounded-lg p-8 shadow-sm">
          {children}
        </div>
      </div>
    </div>
  );
}
```

**Login Page:**

```typescript
// app/(auth)/login/page.tsx
import { LoginForm } from '@/features/auth/login';
import Link from 'next/link';

export default function LoginPage() {
  return (
    <div className="space-y-6">
      <div className="text-center">
        <h2 className="text-2xl font-semibold">Welcome back</h2>
        <p className="text-muted-foreground mt-2">Sign in to your account</p>
      </div>

      <LoginForm />

      <div className="text-center space-y-2">
        <Link href="/forgot-password" className="text-sm text-muted-foreground hover:underline">
          Forgot your password?
        </Link>
        <p className="text-sm text-muted-foreground">
          Don't have an account?{' '}
          <Link href="/register" className="text-primary hover:underline font-medium">
            Sign up
          </Link>
        </p>
      </div>
    </div>
  );
}
```

### API Routes: Authentication Endpoints

**Login Route Handler:**

```typescript
// app/api/auth/login/route.ts
import { NextRequest, NextResponse } from 'next/server';
import { cookies } from 'next/headers';
import { loginSchema } from '@/entities/auth';

export async function POST(request: NextRequest) {
  const body = await request.json();
  const parsed = loginSchema.safeParse(body);

  if (!parsed.success) {
    return NextResponse.json(
      { error: 'Invalid input', fieldErrors: parsed.error.flatten().fieldErrors },
      { status: 400 }
    );
  }

  // Forward to .NET API
  const backendRes = await fetch(`${process.env.BACKEND_BASE_URL}/api/authentication/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      email: parsed.data.email,
      password: parsed.data.password
    })
  });

  if (!backendRes.ok) {
    const err = await backendRes.json().catch(() => ({ message: 'Authentication failed' }));
    return NextResponse.json({ error: err.message }, { status: 401 });
  }

  const { accessToken, refreshToken, user } = await backendRes.json();

  // Set httpOnly cookies
  const cookieStore = await cookies();
  const maxAge = parsed.data.rememberMe ? 60 * 60 * 24 * 30 : undefined; // 30 days or session

  cookieStore.set('access-token', accessToken, {
    httpOnly: true,
    secure: process.env.NODE_ENV === 'production',
    sameSite: 'lax',
    maxAge: 60 * 15, // 15 minutes
    path: '/'
  });

  cookieStore.set('refresh-token', refreshToken, {
    httpOnly: true,
    secure: process.env.NODE_ENV === 'production',
    sameSite: 'lax',
    maxAge: maxAge,
    path: '/'
  });

  return NextResponse.json({ user });
}
```

**Register Route Handler:**

```typescript
// app/api/auth/register/route.ts
import { NextRequest, NextResponse } from 'next/server';
import { registerSchema } from '@/entities/auth';

export async function POST(request: NextRequest) {
  const body = await request.json();
  const parsed = registerSchema.safeParse(body);

  if (!parsed.success) {
    return NextResponse.json(
      { error: 'Invalid input', fieldErrors: parsed.error.flatten().fieldErrors },
      { status: 400 }
    );
  }

  // Forward to .NET API
  const backendRes = await fetch(`${process.env.BACKEND_BASE_URL}/api/authentication/register`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      email: parsed.data.email,
      password: parsed.data.password,
      firstName: parsed.data.firstName,
      lastName: parsed.data.lastName,
      dateOfBirth: parsed.data.dateOfBirth.toISOString()
    })
  });

  if (!backendRes.ok) {
    const err = await backendRes.json().catch(() => ({ message: 'Registration failed' }));
    return NextResponse.json({ error: err.message }, { status: 400 });
  }

  // After registration, auto-login
  return NextResponse.json({ success: true });
}
```

**Refresh Route Handler:**

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

  const backendRes = await fetch(`${process.env.BACKEND_BASE_URL}/api/authentication/refresh`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ refreshToken })
  });

  if (!backendRes.ok) {
    // Refresh failed - clear cookies
    cookieStore.delete('access-token');
    cookieStore.delete('refresh-token');
    return NextResponse.json({ error: 'Session expired' }, { status: 401 });
  }

  const { accessToken, refreshToken: newRefreshToken } = await backendRes.json();

  // Update cookies
  cookieStore.set('access-token', accessToken, {
    httpOnly: true,
    secure: process.env.NODE_ENV === 'production',
    sameSite: 'lax',
    maxAge: 60 * 15,
    path: '/'
  });

  cookieStore.set('refresh-token', newRefreshToken, {
    httpOnly: true,
    secure: process.env.NODE_ENV === 'production',
    sameSite: 'lax',
    maxAge: cookieStore.get('refresh-token')?.maxAge, // Preserve original maxAge
    path: '/'
  });

  return NextResponse.json({ success: true });
}
```

**Logout Route Handler:**

```typescript
// app/api/auth/logout/route.ts
import { NextRequest, NextResponse } from 'next/server';
import { cookies } from 'next/headers';

export async function POST(request: NextRequest) {
  const cookieStore = await cookies();
  const refreshToken = cookieStore.get('refresh-token')?.value;

  // Notify backend to revoke refresh token
  if (refreshToken) {
    await fetch(`${process.env.BACKEND_BASE_URL}/api/authentication/revoke`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ refreshToken })
    }).catch(() => {}); // Ignore errors
  }

  // Clear cookies
  cookieStore.delete('access-token');
  cookieStore.delete('refresh-token');

  return NextResponse.json({ success: true });
}
```

**Session Route Handler:**

```typescript
// app/api/auth/session/route.ts
import { NextRequest, NextResponse } from 'next/server';
import { cookies } from 'next/headers';

export async function GET(request: NextRequest) {
  const cookieStore = await cookies();
  const token = cookieStore.get('access-token')?.value;

  if (!token) {
    return NextResponse.json({ user: null }, { status: 401 });
  }

  // Fetch user info from backend
  const res = await fetch(`${process.env.BACKEND_BASE_URL}/api/users/me`, {
    headers: { Authorization: `Bearer ${token}` }
  });

  if (!res.ok) {
    return NextResponse.json({ user: null }, { status: 401 });
  }

  const user = await res.json();
  return NextResponse.json({ user });
}
```

### Middleware: Custom Auth Guard

**Replace Clerk middleware with custom implementation:**

```typescript
// proxy.ts
import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';

export function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl;
  const token = request.cookies.get('access-token')?.value;
  const refreshToken = request.cookies.get('refresh-token')?.value;

  // Public routes that don't require auth
  const publicRoutes = ['/login', '/register', '/forgot-password'];
  const isPublicRoute = publicRoutes.some(route => pathname.startsWith(route));

  if (isPublicRoute) {
    return NextResponse.next();
  }

  // No access token - try refresh
  if (!token && refreshToken) {
    // Redirect to refresh endpoint, which will redirect back
    const refreshUrl = new URL('/api/auth/refresh', request.url);
    refreshUrl.searchParams.set('redirect', pathname);
    return NextResponse.redirect(refreshUrl);
  }

  // No tokens at all - redirect to login
  if (!token) {
    const loginUrl = new URL('/login', request.url);
    loginUrl.searchParams.set('callbackUrl', pathname);
    return NextResponse.redirect(loginUrl);
  }

  return NextResponse.next();
}

export const config = {
  matcher: [
    '/((?!api/auth|_next/static|_next/image|favicon.ico).*)',
  ]
};
```

### Data Access Layer (DAL)

**Critical security boundary:**

```typescript
// lib/dal.ts
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

export async function fetchFromApi<T>(
  endpoint: string,
  options?: RequestInit
): Promise<T> {
  const { token } = await getSession();

  const res = await fetch(`${process.env.BACKEND_BASE_URL}${endpoint}`, {
    ...options,
    headers: {
      ...options?.headers,
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json'
    }
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

---

## Part 5: Backend Implementation Details

### Token Service Enhancement

**Current State:**
- Only generates access tokens
- Expiration hardcoded to 7 days

**Required Changes:**

```csharp
// OrbitSpace.Application/Common/Interfaces/ITokenService.cs
public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();  // NEW
    Task<RefreshToken> CreateRefreshTokenAsync(
        Guid userId,
        string token,
        bool rememberMe,
        string? deviceInfo
    );  // NEW
    Task<RefreshToken?> ValidateRefreshTokenAsync(string token);  // NEW
    Task RevokeRefreshTokenAsync(string token);  // NEW
    Task RevokeAllUserTokensAsync(Guid userId);  // NEW
}
```

**Implementation:**

```csharp
// OrbitSpace.Infrastructure/Services/JwtTokenService.cs
public class JwtTokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public string GenerateAccessToken(User user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(15),  // SHORT-LIVED
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience
        };

        var tokenHandler = new JsonWebTokenHandler();
        return tokenHandler.CreateToken(tokenDescriptor);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public async Task<RefreshToken> CreateRefreshTokenAsync(
        Guid userId,
        string token,
        bool rememberMe,
        string? deviceInfo)
    {
        var hashedToken = HashToken(token);
        var expirationDays = rememberMe ? 30 : 7;

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = hashedToken,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(expirationDays),
            DeviceInfo = deviceInfo
        };

        await _refreshTokenRepository.CreateAsync(refreshToken);
        return refreshToken;
    }

    public async Task<RefreshToken?> ValidateRefreshTokenAsync(string token)
    {
        var hashedToken = HashToken(token);
        var refreshToken = await _refreshTokenRepository.GetByTokenAsync(hashedToken);

        if (refreshToken == null || !refreshToken.IsActive)
        {
            return null;
        }

        return refreshToken;
    }

    public async Task RevokeRefreshTokenAsync(string token)
    {
        var hashedToken = HashToken(token);
        await _refreshTokenRepository.RevokeByTokenAsync(hashedToken);
    }

    public async Task RevokeAllUserTokensAsync(Guid userId)
    {
        await _refreshTokenRepository.RevokeAllByUserIdAsync(userId);
    }

    private static string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
```

### Refresh Token Repository

```csharp
// OrbitSpace.Application/Common/Interfaces/IRefreshTokenRepository.cs
public interface IRefreshTokenRepository
{
    Task CreateAsync(RefreshToken refreshToken);
    Task<RefreshToken?> GetByTokenAsync(string hashedToken);
    Task<List<RefreshToken>> GetActiveByUserIdAsync(Guid userId);
    Task RevokeByTokenAsync(string hashedToken);
    Task RevokeAllByUserIdAsync(Guid userId);
    Task CleanupExpiredTokensAsync();
}

// OrbitSpace.Infrastructure/Persistence/Repositories/RefreshTokenRepository.cs
public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetByTokenAsync(string hashedToken)
    {
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == hashedToken);
    }

    public async Task<List<RefreshToken>> GetActiveByUserIdAsync(Guid userId)
    {
        return await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.IsActive)
            .OrderByDescending(rt => rt.CreatedAtUtc)
            .ToListAsync();
    }

    public async Task RevokeByTokenAsync(string hashedToken)
    {
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == hashedToken);

        if (token != null)
        {
            token.RevokedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task RevokeAllByUserIdAsync(Guid userId)
    {
        var tokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAtUtc == null)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.RevokedAtUtc = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    public async Task CleanupExpiredTokensAsync()
    {
        var expiredTokens = await _context.RefreshTokens
            .Where(rt => rt.ExpiresAtUtc < DateTime.UtcNow)
            .ToListAsync();

        _context.RefreshTokens.RemoveRange(expiredTokens);
        await _context.SaveChangesAsync();
    }
}
```

### Authentication Service Updates

```csharp
// OrbitSpace.Application/Services/AuthenticationService.cs
public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasherService _passwordHasherService;

    public async Task<OperationResult> RegisterAsync(RegisterRequestDto request)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.FirstName) ||
            string.IsNullOrWhiteSpace(request.LastName) ||
            string.IsNullOrWhiteSpace(request.Password) ||
            request.Password.Length < 4)  // Updated per phase context
        {
            return OperationResultError.Validation("Invalid input");
        }

        // Check if email already exists
        if (await _userRepository.GetByEmailAsync(request.Email) != null)
        {
            return OperationResultError.Validation("Email already exists");
        }

        // Create user
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = request.DateOfBirth,  // NEW
            PasswordHash = _passwordHasherService.HashPassword(request.Password),
            EmailVerified = false,  // NEW - not implementing verification yet
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        await _userRepository.CreateAsync(user);

        return OperationResult.Success();
    }

    public async Task<OperationResult<LoginResponseDto>> LoginAsync(
        LoginRequestDto request)  // CHANGED: use DTO instead of separate params
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            return OperationResultError.Validation("Invalid email or password");
        }

        if (!_passwordHasherService.VerifyPassword(user.PasswordHash, request.Password))
        {
            return OperationResultError.Validation("Invalid email or password");
        }

        // Generate tokens
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshTokenValue = _tokenService.GenerateRefreshToken();

        // Store refresh token in database
        await _tokenService.CreateRefreshTokenAsync(
            user.Id,
            refreshTokenValue,
            request.RememberMe,  // NEW
            request.DeviceInfo   // NEW
        );

        return new LoginResponseDto(
            accessToken,
            refreshTokenValue,  // NEW - return unhashed token
            new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                EmailVerified = user.EmailVerified
            }
        );
    }

    public async Task<OperationResult<RefreshResponseDto>> RefreshAsync(
        RefreshRequestDto request)  // NEW METHOD
    {
        var refreshToken = await _tokenService.ValidateRefreshTokenAsync(request.RefreshToken);

        if (refreshToken == null)
        {
            return OperationResultError.Validation("Invalid or expired refresh token");
        }

        // Mark old token as used
        refreshToken.UsedAtUtc = DateTime.UtcNow;

        // Generate new tokens
        var newAccessToken = _tokenService.GenerateAccessToken(refreshToken.User);
        var newRefreshTokenValue = _tokenService.GenerateRefreshToken();

        // Store new refresh token
        var newRefreshToken = await _tokenService.CreateRefreshTokenAsync(
            refreshToken.UserId,
            newRefreshTokenValue,
            refreshToken.ExpiresAtUtc > DateTime.UtcNow.AddDays(7), // Determine if "remember me"
            refreshToken.DeviceInfo
        );

        // Link old token to new token for audit trail
        refreshToken.ReplacedByToken = newRefreshToken.Token;
        await _userRepository.SaveChangesAsync();

        return new RefreshResponseDto(newAccessToken, newRefreshTokenValue);
    }

    public async Task<OperationResult> RevokeAsync(RevokeRequestDto request)  // NEW METHOD
    {
        await _tokenService.RevokeRefreshTokenAsync(request.RefreshToken);
        return OperationResult.Success();
    }

    public async Task<OperationResult> RevokeAllAsync(Guid userId)  // NEW METHOD
    {
        await _tokenService.RevokeAllUserTokensAsync(userId);
        return OperationResult.Success();
    }
}
```

### Controller Updates

```csharp
// OrbitSpace.WebApi/Controllers/AuthenticationController.cs
[AllowAnonymous]
[Route("api/[controller]")]
public class AuthenticationController : ApiControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    /// <summary>
    /// Register a new user account
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var result = await _authenticationService.RegisterAsync(request);
        return result.IsSuccess ? Ok() : Problem(result.Error);
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var result = await _authenticationService.LoginAsync(request);
        return result.IsSuccess ? Ok(result.Data) : Unauthorized(result.Error);
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(RefreshResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto request)
    {
        var result = await _authenticationService.RefreshAsync(request);
        return result.IsSuccess ? Ok(result.Data) : Unauthorized(result.Error);
    }

    /// <summary>
    /// Revoke a refresh token (logout from specific device)
    /// </summary>
    [HttpPost("revoke")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Revoke([FromBody] RevokeRequestDto request)
    {
        await _authenticationService.RevokeAsync(request);
        return Ok();
    }

    /// <summary>
    /// Revoke all refresh tokens for current user (logout from all devices)
    /// </summary>
    [Authorize]
    [HttpPost("revoke-all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RevokeAll()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _authenticationService.RevokeAllAsync(userId);
        return Ok();
    }
}
```

### DTOs

```csharp
// OrbitSpace.Application/Dtos/Authentication/LoginRequestDto.cs
public record LoginRequestDto(
    string Email,
    string Password,
    bool RememberMe,
    string? DeviceInfo
);

// OrbitSpace.Application/Dtos/Authentication/LoginResponseDto.cs
public record LoginResponseDto(
    string AccessToken,
    string RefreshToken,
    UserDto User
);

// OrbitSpace.Application/Dtos/Authentication/RegisterRequestDto.cs
public record RegisterRequestDto(
    string Email,
    string FirstName,
    string LastName,
    string Password,
    DateTime DateOfBirth
);

// OrbitSpace.Application/Dtos/Authentication/RefreshRequestDto.cs
public record RefreshRequestDto(string RefreshToken);

// OrbitSpace.Application/Dtos/Authentication/RefreshResponseDto.cs
public record RefreshResponseDto(string AccessToken, string RefreshToken);

// OrbitSpace.Application/Dtos/Authentication/RevokeRequestDto.cs
public record RevokeRequestDto(string RefreshToken);

// OrbitSpace.Application/Dtos/UserDto.cs
public record UserDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }
    public bool EmailVerified { get; init; }
}
```

### ApplicationUserProvider Fix

**Current Problem:** Hardcoded temp userId on line 14

```csharp
// OrbitSpace.WebApi/Identity/ApplicationUserProvider.cs
public class ApplicationUserProvider : IApplicationUserProvider
{
    private readonly ClaimsPrincipal _claims;

    public ApplicationUserProvider(IHttpContextAccessor httpContextAccessor)
    {
        _claims = httpContextAccessor.HttpContext?.User
            ?? throw new UnauthorizedAccessException();
    }

    // REMOVE THIS LINE:
    // private Guid TempUserId = new Guid("019c577c-c280-7036-a555-36746161bb89");

    // FIX THIS:
    public Guid UserId => Guid.Parse(GetValueFromClaim(ClaimTypes.NameIdentifier));

    public string UserEmail => GetValueFromClaim(ClaimTypes.Email);

    private string GetValueFromClaim(string type)
    {
        var value = _claims.FindFirst(type)?.Value;
        return string.IsNullOrWhiteSpace(value)
            ? throw new UnauthorizedAccessException()
            : value;
    }
}
```

---

## Part 6: Security Considerations

### Password Security

**Current Implementation:**
- BCrypt with default cost factor (10)
- Auto-salting per password
- Timing attack resistant

**Recommendations:**
- Keep BCrypt (good choice for .NET)
- Consider increasing cost factor to 12 if performance allows
- **DO NOT** reduce minimum password length below 4 in production (current 4 is for development only)
- Implement password strength meter on frontend (future enhancement)

### Token Security

**Access Token:**
- Short lifetime (15 min) limits damage if stolen
- Stored in httpOnly cookie (XSS protection)
- sameSite: lax (CSRF protection)
- secure: true in production (HTTPS only)

**Refresh Token:**
- Hashed in database (SHA256)
- Device tracking for session management
- Token rotation on every refresh
- Can be revoked (logout, compromise detection)

**Attack Scenarios:**

1. **XSS Attack:**
   - **Risk:** Attacker injects malicious script
   - **Mitigation:** httpOnly cookies prevent JavaScript access to tokens
   - **Result:** Attack cannot steal tokens

2. **CSRF Attack:**
   - **Risk:** Malicious site triggers authenticated request
   - **Mitigation:** sameSite: lax prevents cross-site cookie sending
   - **Result:** Attack cannot use victim's cookies

3. **Token Theft (Network Sniffing):**
   - **Risk:** Attacker intercepts network traffic
   - **Mitigation:** secure flag requires HTTPS, short token lifetime
   - **Result:** Stolen access token expires in 15 min, refresh token revocation available

4. **Refresh Token Reuse:**
   - **Risk:** Old refresh token used after new one issued
   - **Mitigation:** Token rotation marks old token as used, can detect and revoke all user tokens
   - **Result:** Potential compromise detected and all sessions terminated

5. **Database Compromise:**
   - **Risk:** Attacker gains access to database
   - **Mitigation:** Refresh tokens hashed (SHA256), passwords hashed (BCrypt)
   - **Result:** Tokens and passwords cannot be directly used (must be cracked)

### Rate Limiting (Future Enhancement)

**Not implementing in Phase 1, but plan for:**

1. Login endpoint: 5 attempts per 15 min per IP
2. Register endpoint: 3 attempts per hour per IP
3. Refresh endpoint: 10 attempts per 5 min per user
4. Password reset: 3 attempts per hour per email

**Implementation Options:**
- AspNetCoreRateLimit library
- Redis-based rate limiting
- Middleware with distributed cache

### Email Enumeration Prevention

**Implemented:**
- Registration returns generic "Email already exists" (reveals email is taken - acceptable for UX)
- Login returns generic "Invalid email or password" (prevents enumeration)
- Forgot password will return success even if email doesn't exist (future)

**Trade-off:**
- Registration reveals if email is registered (UX over security)
- Login doesn't reveal which field is wrong (security over UX)
- Consider adding CAPTCHA if abuse detected

---

## Part 7: Testing Strategy

### Backend Unit Tests

**Authentication Service Tests:**
```csharp
[Fact]
public async Task RegisterAsync_ValidInput_CreatesUser()
{
    // Arrange
    var request = new RegisterRequestDto(
        "test@example.com",
        "John",
        "Doe",
        "password123",
        new DateTime(1990, 1, 1)
    );

    // Act
    var result = await _authenticationService.RegisterAsync(request);

    // Assert
    Assert.True(result.IsSuccess);
    var user = await _userRepository.GetByEmailAsync(request.Email);
    Assert.NotNull(user);
    Assert.Equal(request.Email, user.Email);
}

[Fact]
public async Task LoginAsync_InvalidPassword_ReturnsError()
{
    // Arrange
    await SeedUserAsync();
    var request = new LoginRequestDto(
        "test@example.com",
        "wrongpassword",
        false,
        null
    );

    // Act
    var result = await _authenticationService.LoginAsync(request);

    // Assert
    Assert.False(result.IsSuccess);
    Assert.Contains("Invalid email or password", result.Error);
}

[Fact]
public async Task RefreshAsync_ValidToken_ReturnsNewTokens()
{
    // Arrange
    var user = await SeedUserAsync();
    var refreshToken = _tokenService.GenerateRefreshToken();
    await _tokenService.CreateRefreshTokenAsync(user.Id, refreshToken, false, null);

    // Act
    var result = await _authenticationService.RefreshAsync(
        new RefreshRequestDto(refreshToken)
    );

    // Assert
    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Data.AccessToken);
    Assert.NotNull(result.Data.RefreshToken);
    Assert.NotEqual(refreshToken, result.Data.RefreshToken);
}
```

**Token Service Tests:**
```csharp
[Fact]
public void GenerateAccessToken_ValidUser_ContainsClaims()
{
    // Arrange
    var user = new User { Id = Guid.NewGuid(), Email = "test@example.com" };

    // Act
    var token = _tokenService.GenerateAccessToken(user);

    // Assert
    var handler = new JwtSecurityTokenHandler();
    var jwt = handler.ReadJwtToken(token);
    Assert.Contains(jwt.Claims, c => c.Type == JwtRegisteredClaimNames.Sub);
    Assert.Contains(jwt.Claims, c => c.Type == JwtRegisteredClaimNames.Email);
}

[Fact]
public async Task ValidateRefreshTokenAsync_ExpiredToken_ReturnsNull()
{
    // Arrange
    var expiredToken = new RefreshToken
    {
        ExpiresAtUtc = DateTime.UtcNow.AddDays(-1),
        RevokedAtUtc = null
    };
    await _refreshTokenRepository.CreateAsync(expiredToken);

    // Act
    var result = await _tokenService.ValidateRefreshTokenAsync(expiredToken.Token);

    // Assert
    Assert.Null(result);
}
```

### Frontend Tests

**Login Form Tests:**
```typescript
describe('LoginForm', () => {
  it('shows validation errors for invalid email', async () => {
    render(<LoginForm />);

    const emailInput = screen.getByPlaceholderText(/email/i);
    const submitButton = screen.getByRole('button', { name: /sign in/i });

    await userEvent.type(emailInput, 'invalidemail');
    await userEvent.click(submitButton);

    expect(await screen.findByText(/invalid email/i)).toBeInTheDocument();
  });

  it('submits form with valid credentials', async () => {
    const mockFetch = vi.fn(() =>
      Promise.resolve({ ok: true, json: () => Promise.resolve({ user: {} }) })
    );
    global.fetch = mockFetch;

    render(<LoginForm />);

    await userEvent.type(screen.getByPlaceholderText(/email/i), 'test@example.com');
    await userEvent.type(screen.getByPlaceholderText(/password/i), 'password123');
    await userEvent.click(screen.getByRole('button', { name: /sign in/i }));

    expect(mockFetch).toHaveBeenCalledWith('/api/auth/login', expect.any(Object));
  });
});
```

**Auth Hook Tests:**
```typescript
describe('useAuth', () => {
  it('returns null user when not authenticated', async () => {
    server.use(
      http.get('/api/auth/session', () => {
        return HttpResponse.json({ user: null }, { status: 401 });
      })
    );

    const { result } = renderHook(() => useAuth(), { wrapper: QueryWrapper });

    await waitFor(() => expect(result.current.isAuthenticated).toBe(false));
    expect(result.current.user).toBeNull();
  });

  it('returns user when authenticated', async () => {
    server.use(
      http.get('/api/auth/session', () => {
        return HttpResponse.json({
          user: { id: '1', email: 'test@example.com' }
        });
      })
    );

    const { result } = renderHook(() => useAuth(), { wrapper: QueryWrapper });

    await waitFor(() => expect(result.current.isAuthenticated).toBe(true));
    expect(result.current.user?.email).toBe('test@example.com');
  });
});
```

### Integration Tests

**Login Flow:**
```typescript
describe('Login Flow', () => {
  it('allows user to login and redirects to dashboard', async () => {
    render(<LoginPage />);

    await userEvent.type(screen.getByLabelText(/email/i), 'test@example.com');
    await userEvent.type(screen.getByLabelText(/password/i), 'password123');
    await userEvent.click(screen.getByRole('button', { name: /sign in/i }));

    await waitFor(() => {
      expect(window.location.pathname).toBe('/');
    });
  });

  it('shows error for invalid credentials', async () => {
    server.use(
      http.post('/api/auth/login', () => {
        return HttpResponse.json(
          { error: 'Invalid email or password' },
          { status: 401 }
        );
      })
    );

    render(<LoginPage />);

    await userEvent.type(screen.getByLabelText(/email/i), 'test@example.com');
    await userEvent.type(screen.getByLabelText(/password/i), 'wrongpassword');
    await userEvent.click(screen.getByRole('button', { name: /sign in/i }));

    expect(await screen.findByText(/invalid email or password/i)).toBeInTheDocument();
  });
});
```

**Protected Route Test:**
```typescript
describe('Protected Routes', () => {
  it('redirects to login when not authenticated', async () => {
    render(<GoalsPage />);

    await waitFor(() => {
      expect(window.location.pathname).toBe('/login');
    });
  });

  it('renders page when authenticated', async () => {
    server.use(
      http.get('/api/auth/session', () => {
        return HttpResponse.json({ user: { id: '1', email: 'test@example.com' } });
      })
    );

    render(<GoalsPage />);

    expect(await screen.findByText(/your goals/i)).toBeInTheDocument();
  });
});
```

---

## Part 8: Deployment & Configuration

### Environment Variables

**Backend (.NET):**

```bash
# appsettings.Production.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.example.com;Database=orbitspace;Username=app;Password=xxx"
  },
  "JwtSettings": {
    "Key": "your-secret-key-min-32-characters-long-use-env-var",
    "Issuer": "https://api.orbitspace.com",
    "Audience": "https://orbitspace.com"
  },
  "Cors": {
    "AllowedOrigins": ["https://orbitspace.com"]
  }
}
```

**Frontend (Next.js):**

```bash
# .env.production
NODE_ENV=production
BACKEND_BASE_URL=https://api.orbitspace.com
```

### Docker Configuration

**Backend Dockerfile:**
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["OrbitSpace.WebApi/OrbitSpace.WebApi.csproj", "OrbitSpace.WebApi/"]
RUN dotnet restore "OrbitSpace.WebApi/OrbitSpace.WebApi.csproj"
COPY . .
RUN dotnet build "OrbitSpace.WebApi/OrbitSpace.WebApi.csproj" -c Release -o /app/build
RUN dotnet publish "OrbitSpace.WebApi/OrbitSpace.WebApi.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OrbitSpace.WebApi.dll"]
```

**Frontend Dockerfile:**
```dockerfile
FROM node:22-alpine AS base
RUN corepack enable && corepack prepare pnpm@latest --activate

FROM base AS deps
WORKDIR /app
COPY package.json pnpm-lock.yaml ./
RUN pnpm install --frozen-lockfile

FROM base AS builder
WORKDIR /app
COPY --from=deps /app/node_modules ./node_modules
COPY . .
RUN pnpm build

FROM base AS runner
WORKDIR /app
ENV NODE_ENV=production
RUN addgroup --system --gid 1001 nodejs
RUN adduser --system --uid 1001 nextjs

COPY --from=builder --chown=nextjs:nodejs /app/.next/standalone ./
COPY --from=builder --chown=nextjs:nodejs /app/.next/static ./.next/static
COPY --from=builder --chown=nextjs:nodejs /app/public ./public

USER nextjs
EXPOSE 3000
ENV PORT=3000
CMD ["node", "server.js"]
```

**Updated docker-compose.yml:**
```yaml
services:
  db:
    image: postgres:17-alpine
    container_name: orbit-space-db
    restart: always
    environment:
      POSTGRES_USER: orbit_user
      POSTGRES_PASSWORD: orbit_password
      POSTGRES_DB: orbit_db
    ports:
      - '5432:5432'
    volumes:
      - 'F:/Docker-vault/Orbit-space/PostgreSQL:/var/lib/postgresql/data'
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U orbit_user -d orbit_db"]
      interval: 10s
      timeout: 5s
      retries: 5

  api:
    build:
      context: ./dotnet-web-api
      dockerfile: Dockerfile
    container_name: orbit-space-api
    restart: always
    environment:
      ConnectionStrings__DefaultConnection: "Host=db;Database=orbit_db;Username=orbit_user;Password=orbit_password"
      JwtSettings__Key: "${JWT_SECRET_KEY}"
      JwtSettings__Issuer: "https://api.orbitspace.local"
      JwtSettings__Audience: "https://orbitspace.local"
    ports:
      - '7284:8080'
    depends_on:
      db:
        condition: service_healthy

  web:
    build:
      context: ./next-js-app
      dockerfile: Dockerfile
    container_name: orbit-space-web
    restart: always
    environment:
      BACKEND_BASE_URL: "http://api:8080"
    ports:
      - '3000:3000'
    depends_on:
      - api

networks:
  default:
    name: orbit-space-network
```

### Production Checklist

**Backend:**
- [ ] Set strong JWT secret key (min 32 chars, use secrets manager)
- [ ] Set `secure: true` for JWT cookies in production
- [ ] Configure CORS to specific frontend domain
- [ ] Enable HTTPS (reverse proxy or cloud provider)
- [ ] Set up database connection pooling
- [ ] Configure logging (Serilog to file/cloud)
- [ ] Set up health check endpoint
- [ ] Enable response compression
- [ ] Configure rate limiting on auth endpoints

**Frontend:**
- [ ] Set `NODE_ENV=production`
- [ ] Configure correct `BACKEND_BASE_URL`
- [ ] Enable `secure: true` for cookies in middleware
- [ ] Set up monitoring (Sentry, LogRocket)
- [ ] Configure CDN for static assets
- [ ] Enable response caching headers
- [ ] Set up error boundary for runtime errors
- [ ] Configure Content Security Policy headers

---

## Part 9: Knowledge Gaps & Decisions Needed

### Open Questions for Planning Phase

1. **User Sessions Page:**
   - Should users be able to see active sessions (devices, last activity)?
   - Should users be able to revoke specific sessions remotely?
   - **Recommendation:** Defer to Phase 2 (user profile features)

2. **Email Verification:**
   - Which email service? (SendGrid, Resend, AWS SES)
   - Verification flow: click link or enter code?
   - **Decision:** Stub page for now, implement in Phase 2

3. **Password Reset:**
   - Same email service as verification
   - Token expiration time? (1 hour recommended)
   - **Decision:** Stub page for now, implement in Phase 2

4. **Remember Me Duration:**
   - 30 days vs 90 days?
   - Allow user to customize?
   - **Recommendation:** 30 days fixed for Phase 1

5. **Token Cleanup Strategy:**
   - Background job to delete expired tokens?
   - Delete on validation attempt?
   - **Recommendation:** Manual cleanup for Phase 1, background job in Phase 2

6. **Multi-Factor Authentication (MFA):**
   - TOTP (Google Authenticator)?
   - SMS/Email codes?
   - **Decision:** Out of scope for v1 entirely

7. **OAuth Providers:**
   - Google? GitHub? Microsoft?
   - Use library (AspNetCore.Identity.Google) or manual?
   - **Decision:** Phase context says design should be extensible but implementation deferred

### Technical Debt Identified

1. **ApplicationUserProvider Temp UserId:**
   - Currently hardcoded on line 14
   - Must be fixed in this phase

2. **Clerk Removal:**
   - Complete removal required (middleware, API client, dependencies)
   - Search entire codebase for `@clerk` imports

3. **Password Schema Location:**
   - Currently in `src/shared/schemas/` (wrong per FSD)
   - Should be in `src/entities/auth/model/schemas.ts`

4. **Existing Goals/Activities Protected Routes:**
   - Currently protected by Clerk
   - Need to update to use new DAL pattern in Server Components

5. **API Proxy Route:**
   - Currently uses Clerk's `getToken()`
   - Must update to read from cookie

---

## Part 10: Migration Path from Clerk

### Step-by-Step Removal Strategy

**Phase 1: Backend Preparation (No Breaking Changes)**
1. Implement RefreshToken entity and repository
2. Add refresh token methods to ITokenService
3. Update AuthenticationService with refresh logic
4. Add new controller endpoints (refresh, revoke)
5. Create database migration for refresh_tokens table
6. Test backend auth flow with Postman/Insomnia

**Phase 2: Frontend Foundation (Parallel Implementation)**
1. Create `src/entities/auth/` structure
2. Implement auth schemas (move from shared)
3. Create auth API route handlers (login, register, refresh)
4. Implement DAL (`lib/dal.ts`)
5. Create auth pages (login, register) - don't link yet
6. Test auth pages in isolation (navigate directly to /login)

**Phase 3: Protected Routes Update**
1. Update existing Server Components to use `fetchFromApi` from DAL
2. Update `src/shared/api/api-server-client.ts` to read from cookie instead of Clerk
3. Update `app/api/(proxy)/[...path]/route.ts` to use cookies
4. Test protected routes work with both Clerk (for now) and cookies

**Phase 4: Middleware Swap (Breaking Change)**
1. Backup current `proxy.ts`
2. Replace with custom auth middleware
3. Update auth routes to be public in middleware
4. Remove Clerk middleware completely
5. Test all routes redirect to /login when unauthenticated

**Phase 5: Clerk Removal**
1. Remove Clerk dependencies from `package.json`
2. Delete `.clerk/` directory
3. Remove Clerk env vars from `.env.local`
4. Search codebase for remaining `@clerk` imports
5. Update root layout if it uses Clerk components
6. Run `pnpm install` to clean up node_modules
7. Full regression test

**Rollback Plan:**
- Keep Clerk code in git history
- Each phase should be a separate commit
- If Phase 4/5 fails, can revert to previous commit
- Database migration for refresh_tokens is safe (doesn't affect existing tables)

---

## Part 11: Learning Resources

### For Deep Understanding

**JWT & Security:**
1. [RFC 7519: JSON Web Token](https://datatracker.ietf.org/doc/html/rfc7519)
2. [OWASP Authentication Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Authentication_Cheat_Sheet.html)
3. [JWT Best Practices (IETF Draft)](https://datatracker.ietf.org/doc/html/draft-ietf-oauth-jwt-bcp)

**Next.js Security:**
1. [Next.js Data Access Layer Pattern](https://nextjs.org/blog/security-nextjs-server-components-actions#data-access-layer)
2. [Next.js Authentication Guide](https://nextjs.org/docs/app/building-your-application/authentication)
3. [Vercel's Security Best Practices](https://vercel.com/docs/security/best-practices)

**.NET Authentication:**
1. [ASP.NET Core JWT Bearer Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwt-authn)
2. [ASP.NET Core Identity (for reference, not using)](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity)
3. [BCrypt Password Hashing](https://github.com/BcryptNet/bcrypt.net)

**Architecture Patterns:**
1. [Backend-for-Frontend Pattern](https://learn.microsoft.com/en-us/azure/architecture/patterns/backends-for-frontends)
2. [Defense in Depth](https://en.wikipedia.org/wiki/Defense_in_depth_(computing))
3. [Token Rotation](https://auth0.com/docs/secure/tokens/refresh-tokens/refresh-token-rotation)

---

## Summary & Next Steps

### What We Know
1. Backend auth foundation is 80% complete - need refresh token system
2. Frontend requires complete Clerk removal and new auth implementation
3. Architecture guide provides proven pattern for httpOnly cookie auth
4. Security model requires three layers: middleware, DAL, Server Actions
5. Multi-device support requires refresh token database table

### What We Need to Build

**Backend (6 major tasks):**
1. RefreshToken entity, repository, migration
2. Enhanced ITokenService with refresh methods
3. Updated AuthenticationService with refresh/revoke logic
4. New controller endpoints (refresh, revoke, revoke-all)
5. Updated DTOs (LoginRequestDto, RefreshRequestDto, etc.)
6. Fix ApplicationUserProvider temp userId

**Frontend (8 major tasks):**
1. Create auth entity layer (schemas, types, hooks)
2. Create auth pages (login, register) with layouts
3. Create auth API route handlers (login, register, refresh, logout, session)
4. Implement Data Access Layer (DAL)
5. Implement custom middleware (replace Clerk)
6. Update API proxy route (cookie-based auth)
7. Update existing protected routes to use DAL
8. Complete Clerk removal (dependencies, imports, env vars)

### Risk Assessment

**High Risk:**
- Middleware bypass vulnerability if DAL not implemented correctly
- Token theft if cookies not configured with httpOnly + secure flags
- Race conditions during token refresh if queue pattern not used

**Medium Risk:**
- User lockout if refresh token validation too strict
- Session loss if cookie maxAge not handled correctly for "remember me"
- OAuth integration friction if endpoints not designed for extensibility

**Low Risk:**
- Password strength (already using BCrypt with good defaults)
- Email enumeration (acceptable trade-off per phase context)
- Token cleanup (manual for Phase 1, automate later)

### Recommended Planning Approach

1. **Start with Backend** - foundation must be solid
2. **Test Backend in Isolation** - Postman/Insomnia before frontend work
3. **Build Frontend in Parallel** - can develop while backend is being tested
4. **Gradual Clerk Removal** - don't rip out until new system is working
5. **Defense-in-Depth is Non-Negotiable** - all three layers must be implemented
6. **Test Token Refresh Extensively** - most complex part of the flow

### User's Learning Objectives (from phase context)

The phase context states: "User wants to learn auth deeply — plan should be detailed and educational, not just 'get it done'."

**Key Learning Opportunities:**
1. **JWT Anatomy** - understand claims, signatures, expiration
2. **httpOnly Cookie Security** - why it's crucial for XSS prevention
3. **Token Rotation** - how refresh tokens stay secure even if stolen
4. **Defense-in-Depth** - why middleware alone is insufficient (CVE-2025-29927)
5. **BFF Pattern** - separating browser concerns from server-to-server calls
6. **Password Hashing** - BCrypt's adaptive cost factor and auto-salting

Each task in the plan should include:
- **Why** this is necessary (not just what to do)
- **Security implications** of the implementation
- **Common mistakes** to avoid
- **Testing strategy** to verify correctness

---

## Appendix: Code Examples for Common Patterns

### Server Component with TanStack Query Hydration

```typescript
// app/goals/page.tsx (Server Component)
import { dehydrate, HydrationBoundary, QueryClient } from '@tanstack/react-query';
import { fetchFromApi } from '@/lib/dal';
import { GoalsList } from './goals-list';
import { goalsQueryOptions } from '@/entities/goal';

export default async function GoalsPage() {
  const queryClient = new QueryClient();

  // Prefetch on server using DAL
  await queryClient.prefetchQuery({
    queryKey: goalsQueryOptions({ page: 1 }).queryKey,
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

### Client Component with useQuery

```typescript
// app/goals/goals-list.tsx (Client Component)
'use client';

import { useQuery } from '@tanstack/react-query';
import { goalsQueryOptions } from '@/entities/goal';

export function GoalsList() {
  const { data, isLoading, error } = useQuery(goalsQueryOptions({ page: 1 }));

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error: {error.message}</div>;

  return (
    <ul>
      {data?.items.map((goal) => (
        <li key={goal.id}>{goal.title}</li>
      ))}
    </ul>
  );
}
```

### Token Refresh with Race Condition Prevention

```typescript
// lib/token-refresh.ts
let isRefreshing = false;
let refreshQueue: Array<{ resolve: (value: unknown) => void; reject: (reason?: any) => void }> = [];

function processQueue(error: Error | null) {
  refreshQueue.forEach(({ resolve, reject }) => {
    if (error) reject(error);
    else resolve(undefined);
  });
  refreshQueue = [];
}

export async function handleTokenRefresh(): Promise<void> {
  if (isRefreshing) {
    return new Promise((resolve, reject) => {
      refreshQueue.push({ resolve, reject });
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

---

**End of Research Document**

This research provides a comprehensive foundation for planning Phase 1. All architectural patterns, security considerations, and implementation details have been documented. The next step is to create a detailed task breakdown in the PLAN document.
