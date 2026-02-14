# External Integrations

**Analysis Date:** 2026-02-14

## APIs & External Services

**Authentication & Identity:**
- Clerk - User authentication and session management
  - SDK/Client: `@clerk/nextjs` (frontend)
  - Auth: `NEXT_PUBLIC_CLERK_PUBLISHABLE_KEY`, `CLERK_SECRET_KEY`
  - Flow: Frontend sign-in/sign-up via Clerk UI → Clerk generates JWT → JWT stored in httpOnly cookie via Next.js Route Handler → Backend validates JWT signature
  - Implementation: `app/providers/Providers.tsx` (ClerkProvider wrapper), `proxy.ts` (Clerk middleware), `src/shared/api/api-server-client.ts` (Clerk server-side auth)

## Data Storage

**Databases:**
- PostgreSQL 14+ (primary data storage, recently migrated from MongoDB)
  - Connection: `ConnectionStrings.DefaultConnection` in `appsettings.json` (empty in template, set via environment variable)
  - Client: Entity Framework Core 10.0.0 with Npgsql provider
  - ORM: `Microsoft.EntityFrameworkCore` + `Npgsql.EntityFrameworkCore.PostgreSQL`
  - DbContext: `OrbitSpace.Infrastructure/Persistence/AppDbContext.cs`
  - Tables: Users, Goals, Activities, TodoItems
  - Convention: Uses `EFCore.NamingConventions` to auto-convert C# PascalCase to PostgreSQL snake_case

**Caching:**
- TanStack React Query 5.90.5 (browser client-side cache, not a separate service)
  - Handles: Server state hydration, request deduplication, background refetching, stale-while-revalidate patterns
  - Configuration via `queryClient` in React

**File Storage:**
- Not detected - no S3, Cloud Storage, or file upload integration
- Architecture supports local file operations if needed, but not implemented in core entities

## Authentication & Identity

**Auth Provider:**
- Clerk (third-party authentication and user management)
  - Implementation approach: BFF (Backend-for-Frontend) hybrid pattern
  - Frontend: Clerk SDK handles sign-in/sign-up UI, JWT generation
  - Next.js Route Handler: Receives JWT from Clerk, stores in httpOnly cookie (never exposed to browser JavaScript)
  - Proxy Route: `app/api/proxy/[...path]/route.ts` forwards authenticated requests to .NET backend with JWT in Authorization header
  - Backend: Validates JWT signature using configured issuer and audience
  - Config: `OrbitSpace/Startup/AuthenticationConfig.cs` sets up JWT Bearer scheme, validates tokens with issuer URL

**Password Hashing:**
- BCrypt.Net-Next 4.0.3 (for any local password-based auth fallback, not primary flow)
  - Used in: Backend service layer for password hashing if needed
  - Service: `OrbitSpace.Infrastructure/Services/BCryptPasswordHasherService.cs`

**Token Management:**
- JWT (JSON Web Tokens) via Clerk
  - Short-lived access tokens (typically 15 minutes, configured in Next.js Route Handler)
  - Refresh tokens for extended sessions
  - Validation: Backend checks issuer, signature, lifetime
  - Storage: httpOnly cookies (server-set, cannot be accessed by JavaScript, auto-sent by browser on requests)

## Monitoring & Observability

**Error Tracking:**
- Built-in exception handling
  - Backend: Global exception handler in `OrbitSpace/Exceptions/GlobalExceptionHandler.cs`, returns RFC 7807 Problem Details responses
  - Frontend: TanStack Query error boundary patterns, React Error Boundary components
- Not detected: Sentry, DataDog, or external error tracking service

**Logs:**
- Backend: ASP.NET Core logging (configured in `appsettings.json`, writes to console by default)
  - LogLevel: Information default, Warning for AspNetCore
  - No external log aggregation detected
- Frontend: Browser console logs, React development tools
  - TanStack Query DevTools available in development via `@tanstack/react-query-devtools`

## CI/CD & Deployment

**Hosting:**
- Not yet specified in codebase
- Frontend: Ready for deployment to Vercel (Next.js native), AWS Amplify, Netlify, or self-hosted
- Backend: Ready for deployment to Azure App Service, AWS Elastic Beanstalk, Docker, or self-hosted

**CI Pipeline:**
- Not detected in codebase (no GitHub Actions, GitLab CI, or Jenkins config files found)
- Monorepo structure (dotnet-web-api/, next-js-app/) suggests potential for separate deployment pipelines per component

## Environment Configuration

**Required env vars:**

**Frontend (.env.local or deployment platform):**
- `NEXT_PUBLIC_BACKEND_BASE_URL` - Backend API URL (e.g., `https://localhost:7284` for dev, production URL for prod)
- `NEXT_PUBLIC_CLERK_PUBLISHABLE_KEY` - Clerk public key (exposed to browser)
- `CLERK_SECRET_KEY` - Clerk secret key (server-only, never exposed)

**Backend (appsettings.json or environment variables):**
- `ConnectionStrings__DefaultConnection` - PostgreSQL connection string (format: `Server=host;Port=5432;Database=orbitspace;User Id=user;Password=pwd`)
- `JwtSettings__Key` - JWT signing key (base64-encoded secret, minimum 32 bytes for HS256)
- `JwtSettings__Issuer` - JWT issuer URL (matches Clerk issuer)
- `Cors__AllowedOrigins` - Frontend origin(s) to allow CORS (e.g., `http://localhost:3000`, `https://yourdomain.com`)

**Optional env vars:**
- `NODE_TLS_REJECT_UNAUTHORIZED=0` - Development only, disables TLS verification (current in .env.local, remove in production)

**Secrets location:**
- Frontend: `.env.local` (git-ignored, never committed) or deployment platform secrets (Vercel env, AWS Secrets Manager, etc.)
- Backend: `appsettings.json` (template, secrets via environment variables or Azure Key Vault in production)
- Both: Follow the principle of never committing secrets to version control

## Webhooks & Callbacks

**Incoming:**
- Not detected in current implementation
- API endpoints are RESTful (no webhook listeners)
- Future candidates: Clerk webhook events (user.created, user.updated), payment webhooks if monetized

**Outgoing:**
- Not detected
- No external service callbacks configured
- Clerk integration is pull-based (JWT validation on request), not push-based webhooks

## API Communication Pattern

**Frontend → Backend Flow:**
1. User action triggers client-side mutation or query
2. Browser makes request to Next.js Route Handler: `/api/proxy/[...path]`
3. Route Handler intercepts, reads Clerk JWT from httpOnly cookie via `auth()` middleware
4. Route Handler forwards request to backend with JWT in Authorization header
5. Backend validates JWT, processes request, returns response
6. Route Handler streams response back to browser

**Backend API Specification:**
- OpenAPI 3.0 schema auto-generated from .NET endpoints
- Scalar documentation UI available at `https://localhost:7284/` (dev only)
- Schema endpoint: `https://localhost:7284/openapi/v1.json`
- Frontend regenerates types from schema: `pnpm generate-api-types` (via openapi-typescript)
- Generated types location: `src/shared/api/v1.ts`

**Error Response Format:**
- RFC 7807 Problem Details standard
- Example: `{ "type": "...", "title": "...", "status": 400, "detail": "..." }`

---

*Integration audit: 2026-02-14*
