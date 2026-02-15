---
phase: 01-foundation-authentication
plan: 03
subsystem: frontend-auth
tags: [authentication, nextjs, react-hook-form, zod, bff-proxy, httponly-cookies]

# Dependency graph
requires:
  - phase: 01-01
    provides: "Backend JWT authentication with refresh token rotation"
  - phase: 01-02
    provides: "Authentication REST endpoints exposed via OpenAPI"
provides:
  - "Auth entity layer (schemas, types, hooks, query keys) following FSD architecture"
  - "Login and register pages with forms at /login and /register routes"
  - "API route handlers implementing BFF proxy pattern with httpOnly cookies"
  - "Auth layout with centered card design (no dashboard chrome)"
  - "useAuth hook for session management in client components"
affects: [01-04, frontend-integration, protected-routes]

# Tech tracking
tech-stack:
  added:
    - "React Hook Form v7 with zodResolver for form validation"
    - "httpOnly cookies for secure token storage (Next.js cookies() API)"
    - "TanStack Query for session state management"
  patterns:
    - "BFF (Backend-for-Frontend) proxy pattern - all auth requests route through Next.js API routes"
    - "Route groups: (auth) for public pages, (dashboard) for protected app routes"
    - "FSD architecture: entities/auth (domain), features/auth (forms), app/(auth) (pages)"
    - "Server-side Zod validation in API routes before forwarding to backend"

key-files:
  created:
    - "next-js-app/src/entities/auth/model/schemas.ts"
    - "next-js-app/src/entities/auth/model/types.ts"
    - "next-js-app/src/entities/auth/model/use-auth.ts"
    - "next-js-app/src/entities/auth/api/auth-query-keys.ts"
    - "next-js-app/src/entities/auth/index.ts"
    - "next-js-app/src/features/auth/login/ui/login-form.tsx"
    - "next-js-app/src/features/auth/login/index.ts"
    - "next-js-app/src/features/auth/register/ui/register-form.tsx"
    - "next-js-app/src/features/auth/register/index.ts"
    - "next-js-app/app/(auth)/layout.tsx"
    - "next-js-app/app/(auth)/login/page.tsx"
    - "next-js-app/app/(auth)/register/page.tsx"
    - "next-js-app/app/(auth)/forgot-password/page.tsx"
    - "next-js-app/app/api/auth/login/route.ts"
    - "next-js-app/app/api/auth/register/route.ts"
    - "next-js-app/app/api/auth/refresh/route.ts"
    - "next-js-app/app/api/auth/logout/route.ts"
    - "next-js-app/app/api/auth/session/route.ts"
    - "next-js-app/app/(dashboard)/layout.tsx"
  modified:
    - "next-js-app/app/layout.tsx"

key-decisions:
  - title: "Password minimum: 4 characters (dev phase)"
    rationale: "Per user decision from previous plans - simplifies development/testing"
    alternatives: "6-8 characters typical for production"
  - title: "RememberMe controls refresh token expiration"
    rationale: "rememberMe=false: 7 days, rememberMe=true: 30 days - matches backend strategy"
    alternatives: "Single fixed expiration - rejected as less flexible"
  - title: "Route groups for layout separation"
    rationale: "Clean separation: (auth) for public pages without sidebar, (dashboard) for app routes with DashboardLayout"
    alternatives: "Conditional layout in root - rejected as less maintainable"
  - title: "Auto-login after registration"
    rationale: "Better UX - user doesn't need to manually sign in after registering"
    alternatives: "Redirect to login page - rejected as extra friction"
  - title: "JWT decoding in /api/auth/session"
    rationale: "Avoids extra backend call for basic user info; JWT already contains claims"
    alternatives: "Call backend /users/me endpoint - adds latency"

patterns-established:
  - "Pattern 1: Auth forms use useTransition for pending states, not useState(loading)"
  - "Pattern 2: All API routes validate with Zod before forwarding to backend"
  - "Pattern 3: API routes never return raw tokens to browser - only user profile data"
  - "Pattern 4: Register flow auto-logs in on success by calling login endpoint"

# Metrics
duration: 5min
completed: 2026-02-15
---

# Phase 01 Plan 03: Frontend Authentication Integration Summary

**Complete frontend auth layer with BFF proxy pattern, httpOnly cookies, and FSD architecture**

## Performance

- **Duration:** 5 minutes 57 seconds
- **Started:** 2026-02-15T16:43:49Z
- **Completed:** 2026-02-15T16:49:46Z
- **Tasks:** 2
- **Files created:** 18
- **Files modified:** 1

## Accomplishments

### Task 1: Auth Entity Layer
- Created Zod validation schemas (loginSchema, registerSchema) with 4-char password minimum
- Added registerSchema refinements: password match validation, minimum age 13 check
- Defined TypeScript types: AuthUser, AuthResponse, AuthError
- Implemented useAuth hook with TanStack Query (5min staleTime, no retry)
- Created authQueryKeys factory following existing pattern
- Exported all from index.ts barrel file

### Task 2: Auth Pages, Forms, and API Routes
- **LoginForm**: email, password, rememberMe fields with React Hook Form + Zod validation
- **RegisterForm**: firstName, lastName, email, dateOfBirth, password, repeatPassword with validation
- **Auth pages**: Login at /login, Register at /register, Forgot password stub at /forgot-password
- **Auth layout**: Centered card design with OrbitSpace branding (no dashboard sidebar)
- **5 API route handlers**:
  - `/api/auth/login` - forwards to .NET, sets httpOnly cookies (access: 15min, refresh: 7/30 days)
  - `/api/auth/register` - forwards to .NET, returns success
  - `/api/auth/refresh` - reads refresh cookie, rotates tokens, updates cookies
  - `/api/auth/logout` - revokes token on backend, deletes cookies
  - `/api/auth/session` - decodes JWT payload to extract user info (no backend call needed)

### Layout Architecture Changes
- **Root layout**: Removed unconditional DashboardLayout wrapping
- **Created (auth) route group**: Clean centered layout for login/register/forgot-password
- **Created (dashboard) route group**: Moved existing app routes (goals, activities, task-management, home) with DashboardLayout
- **Result**: Auth pages display without sidebar/nav; app pages keep dashboard chrome

## Task Commits

Each task was committed atomically:

1. **Task 1: Auth entity layer** - `1bad92a` (feat)
2. **Task 2: Auth pages, forms, and API routes** - `7a947f6` (feat)

## Files Created

**Entity layer:**
- `next-js-app/src/entities/auth/model/schemas.ts` - Zod schemas with refinements
- `next-js-app/src/entities/auth/model/types.ts` - AuthUser, AuthResponse, AuthError
- `next-js-app/src/entities/auth/model/use-auth.ts` - useAuth hook with session query
- `next-js-app/src/entities/auth/api/auth-query-keys.ts` - Query key factory
- `next-js-app/src/entities/auth/index.ts` - Barrel exports

**Feature layer:**
- `next-js-app/src/features/auth/login/ui/login-form.tsx` - Login form component
- `next-js-app/src/features/auth/login/index.ts` - Feature barrel
- `next-js-app/src/features/auth/register/ui/register-form.tsx` - Register form component
- `next-js-app/src/features/auth/register/index.ts` - Feature barrel

**Pages:**
- `next-js-app/app/(auth)/layout.tsx` - Auth layout with centered card
- `next-js-app/app/(auth)/login/page.tsx` - Login page
- `next-js-app/app/(auth)/register/page.tsx` - Register page
- `next-js-app/app/(auth)/forgot-password/page.tsx` - Stub page

**API routes:**
- `next-js-app/app/api/auth/login/route.ts` - Login proxy with cookie setting
- `next-js-app/app/api/auth/register/route.ts` - Register proxy
- `next-js-app/app/api/auth/refresh/route.ts` - Token refresh with cookie update
- `next-js-app/app/api/auth/logout/route.ts` - Logout with token revocation
- `next-js-app/app/api/auth/session/route.ts` - Session endpoint with JWT decoding

**Layout changes:**
- `next-js-app/app/(dashboard)/layout.tsx` - Dashboard layout wrapper

## Files Modified

- `next-js-app/app/layout.tsx` - Removed DashboardLayout, now just provides fonts and Providers

## Decisions Made

**1. Route group architecture for layout separation:**
- Auth pages need clean centered layout without sidebar
- App pages need DashboardLayout with navigation
- Solution: Split into (auth) and (dashboard) route groups with separate layouts
- Alternative considered: Conditional layout in root based on pathname - rejected as harder to maintain

**2. Auto-login after registration:**
- After successful registration, immediately call /api/auth/login with same credentials
- Better UX - user starts using app immediately
- Alternative: Redirect to login page with success message - rejected as unnecessary friction

**3. JWT decoding in session endpoint:**
- Session route decodes JWT payload directly instead of calling backend
- Saves network round-trip for basic user info
- JWT already contains all needed claims (sub, email, given_name, family_name, birthdate)
- Alternative: Call backend /api/users/me - adds latency, unnecessary for session checks

**4. RegisterForm date input approach:**
- Using HTML5 date input (type="date") for simplicity
- Zod's z.coerce.date() handles string-to-Date conversion
- Minimum age 13 validated with refine() comparing against current date minus 13 years
- Alternative: DatePicker component from shadcn/ui - deferred to avoid complexity in this plan

## Deviations from Plan

None - plan executed exactly as written. All verification criteria met.

## Key Implementation Details

### BFF Proxy Pattern Flow

**Login:**
1. Browser → POST /api/auth/login (email, password, rememberMe)
2. Next.js API route validates with Zod
3. Next.js → POST ${BACKEND_BASE_URL}/api/authentication/login
4. Backend returns { accessToken, refreshToken, user }
5. Next.js sets httpOnly cookies (access-token: 15min, refresh-token: 7/30 days)
6. Next.js → returns { user } only (no tokens)
7. Browser receives user profile, redirects to callbackUrl or "/"
8. router.refresh() forces Server Components to re-render with auth

**Register:**
1. Browser → POST /api/auth/register (email, password, firstName, lastName, dateOfBirth)
2. Next.js validates, converts dateOfBirth to ISO string
3. Next.js → POST ${BACKEND_BASE_URL}/api/authentication/register
4. Backend creates user, returns success
5. RegisterForm then calls /api/auth/login with same credentials
6. Login flow proceeds as above

**Refresh:**
1. Browser → POST /api/auth/refresh (no body)
2. Next.js reads refresh-token from cookies
3. Next.js → POST ${BACKEND_BASE_URL}/api/authentication/refresh { refreshToken }
4. Backend rotates tokens, returns new pair
5. Next.js updates both cookies
6. Returns { success: true }

**Session:**
1. Browser → GET /api/auth/session
2. Next.js reads access-token from cookies
3. Decodes JWT payload (base64 decode middle segment)
4. Extracts claims: sub, email, given_name, family_name, birthdate
5. Returns { user: { id, email, firstName, lastName, dateOfBirth, emailVerified } }
6. useAuth hook calls this endpoint, caches result for 5 minutes

### Security Properties

- **Tokens never exposed to browser JavaScript** - stored in httpOnly cookies
- **Browser automatically attaches cookies** - no manual header management
- **Same-origin requests only** - /api/auth/* are same-origin to Next.js app
- **CSRF protection** - sameSite: 'lax' prevents cross-site cookie sending
- **Short access token lifetime** - 15 minutes limits exposure window
- **Refresh token rotation** - backend marks old token as used, issues new one
- **Server-side validation** - Zod schemas run in API routes before backend call

### FSD Architecture Compliance

**Layers:**
- **Entity (auth)**: Domain logic, schemas, types, hooks - no UI dependencies
- **Feature (auth/login, auth/register)**: User-facing forms, business logic for auth flows
- **App (auth)**: Pages and layouts composing features and entities

**Import rules respected:**
- Features import from entities: ✓ (LoginForm imports loginSchema, LoginInput)
- Pages import from features: ✓ (login/page.tsx imports LoginForm)
- No upward imports: ✓ (entities don't import from features/pages)

## Testing Recommendations

Before next plan (01-04 - Middleware and Protected Routes), manually verify:

1. **Navigation**: Visit http://localhost:3000/login - see centered card layout without sidebar
2. **Login flow**: Fill form, submit - should set cookies and redirect to "/"
3. **Register flow**: Fill all fields including dateOfBirth, submit - should auto-login and redirect
4. **Forgot password**: Visit /forgot-password - see "Coming soon" stub
5. **Cookie inspection**: After login, check browser DevTools → Application → Cookies - see access-token and refresh-token (httpOnly: true)
6. **Session check**: After login, open Network tab, trigger useAuth hook - see GET /api/auth/session return user object
7. **Validation**: Submit forms with invalid data (bad email, short password, age < 13) - see error messages
8. **Dashboard routes**: Visit http://localhost:3000/goals - should show DashboardLayout with sidebar

## What's Next

The frontend auth foundation is complete. Next plan (01-04) will implement:

- **Middleware**: Route protection, automatic token refresh on 401
- **Data Access Layer (DAL)**: getSession() helper for Server Components (defense-in-depth)
- **Protected route enforcement**: Redirect unauthenticated users to /login
- **Proxy route handler**: /api/proxy/[...path] for authenticated backend requests
- **TanStack Query integration**: Server-side prefetching with HydrationBoundary

This plan provides the user-facing auth UI and BFF proxy foundation. Next plan adds the protection layer ensuring only authenticated users access app routes.

## Self-Check: PASSED

**Files created verification:**
- FOUND: next-js-app/src/entities/auth/model/schemas.ts
- FOUND: next-js-app/src/entities/auth/model/types.ts
- FOUND: next-js-app/src/entities/auth/model/use-auth.ts
- FOUND: next-js-app/src/entities/auth/api/auth-query-keys.ts
- FOUND: next-js-app/src/entities/auth/index.ts
- FOUND: next-js-app/src/features/auth/login/ui/login-form.tsx
- FOUND: next-js-app/src/features/auth/register/ui/register-form.tsx
- FOUND: next-js-app/app/(auth)/layout.tsx
- FOUND: next-js-app/app/(auth)/login/page.tsx
- FOUND: next-js-app/app/(auth)/register/page.tsx
- FOUND: next-js-app/app/(auth)/forgot-password/page.tsx
- FOUND: next-js-app/app/api/auth/login/route.ts
- FOUND: next-js-app/app/api/auth/register/route.ts
- FOUND: next-js-app/app/api/auth/refresh/route.ts
- FOUND: next-js-app/app/api/auth/logout/route.ts
- FOUND: next-js-app/app/api/auth/session/route.ts
- FOUND: next-js-app/app/(dashboard)/layout.tsx

**Commits verification:**
- FOUND: 1bad92a (Task 1 - Auth entity layer)
- FOUND: 7a947f6 (Task 2 - Auth pages, forms, API routes)

**Key features verification:**
- loginSchema has 4-char minimum: YES (schemas.ts line 7)
- registerSchema has password match refine: YES (schemas.ts line 28-31)
- registerSchema has age 13 check: YES (schemas.ts line 18-26)
- useAuth hook has 5min staleTime: YES (use-auth.ts line 17)
- Login route sets httpOnly cookies: YES (login/route.ts lines 42-53)
- Register auto-logs in: YES (register-form.tsx lines 48-60)
- Session route decodes JWT: YES (session/route.ts lines 15-23)
- Root layout no DashboardLayout: YES (layout.tsx line 34)
- Auth layout has centered card: YES ((auth)/layout.tsx lines 3-10)

All checks passed.
