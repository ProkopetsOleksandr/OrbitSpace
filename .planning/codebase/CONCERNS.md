# Codebase Concerns

**Analysis Date:** 2026-02-14

## Security Concerns

**Authentication Parameter Binding:**
- Issue: Login endpoint accepts email and password as query string parameters instead of request body
- Files: `dotnet-web-api/OrbitSpace/Controllers/AuthenticationController.cs` (lines 23)
- Impact: Credentials can be logged in URLs, cached in browser history, and transmitted in plaintext in logs. This is a critical security vulnerability
- Fix approach: Change `Login(string email, string password)` to accept `[FromBody] LoginRequestDto` with proper validation. Create a `LoginRequestDto` DTO

**Missing Refresh Token Mechanism:**
- Issue: JWT tokens are issued with 7-day expiry but there is no refresh token endpoint to extend sessions
- Files: `dotnet-web-api/OrbitSpace.Infrastructure/Services/JwtTokenService.cs` (line 29)
- Impact: Users cannot maintain sessions longer than 7 days. Long-lived tokens increase compromised token attack surface
- Fix approach: Implement refresh token generation, storage in secure database, and `/api/auth/refresh` endpoint. Return both access and refresh tokens on login

**No Revocation/Logout Mechanism:**
- Issue: Once a JWT is issued, there is no way to invalidate it (no token blacklist or revocation endpoint)
- Files: `dotnet-web-api/OrbitSpace/Controllers/AuthenticationController.cs`
- Impact: Compromised tokens cannot be revoked until natural expiry. User logout has no effect on token validity
- Fix approach: Implement token blacklist (Redis or database) with cleanup job, or add `/api/auth/logout` endpoint that stores token on blacklist

**Empty JWT Key Configuration:**
- Issue: `appsettings.json` has empty JWT signing key with placeholder value
- Files: `dotnet-web-api/OrbitSpace/appsettings.json` (line 13)
- Impact: Development config must never be deployed to production. Risk of secret exposure in version control
- Fix approach: Use environment variables or Azure Key Vault for JWT key. Never commit `appsettings.Development.json` with real credentials. Validate JWT key length on startup

**Exposed Database Credentials:**
- Issue: Development MongoDB connection string visible in source control (though now migrated to PostgreSQL)
- Files: `dotnet-web-api/OrbitSpace/appsettings.Development.json`
- Impact: Database credentials were exposed in git history
- Fix approach: Store in `.env` file with `.env` in `.gitignore`. Use environment variables or secrets manager. Run `git-secrets` or similar in CI

## Database & Query Concerns

**No Eager Loading / N+1 Risk:**
- Issue: `ActivityRepository.GetByIdAsync()` uses `FindAsync(id)` without checking user ownership; repositories lack `Include()` for related entities
- Files: `dotnet-web-api/OrbitSpace.Infrastructure/Persistence/Repositories/ActivityRepository.cs` (line 18)
- Impact: If goals/activities have related entities (comments, attachments), each access causes additional database queries. Scale degradation with data growth
- Fix approach: Implement `Include()` for commonly accessed relations. Use query specs or projection. Add `AsNoTracking()` for read-only queries. Profile slow queries with EF Core logging

**Unvalidated Database Lookups:**
- Issue: `GetByIdAsync()` in ActivityRepository returns entity without user authorization check (only DeleteAsync validates ownership)
- Files: `dotnet-web-api/OrbitSpace.Infrastructure/Persistence/Repositories/ActivityRepository.cs` (lines 16-18)
- Impact: If controller doesn't validate ownership, users could read/update other users' data (authorization bypass vulnerability)
- Fix approach: Add `Guid userId` parameter to all repository methods. Always filter by `userId` in WHERE clause. Never trust controller-level checks alone

**No Query Pagination:**
- Issue: `GetAllAsync()` methods return entire list without pagination; frontend may load thousands of records
- Files: `dotnet-web-api/OrbitSpace.Infrastructure/Persistence/Repositories/ActivityRepository.cs` (line 9)
- Impact: Memory exhaustion on large datasets. Slow API responses. Poor UX on slow networks
- Fix approach: Add `Take(pageSize).Skip((page-1)*pageSize)` with proper ordering. Return total count for pagination UI. Frontend must implement infinite scroll or page buttons

## Data Validation & Input Handling

**Minimal Registration Validation:**
- Issue: Registration only checks for null/empty and password length ≥ 6; no email format validation, no password complexity requirements
- Files: `dotnet-web-api/OrbitSpace.Application/Services/AuthenticationService.cs` (lines 19-25)
- Impact: Weak passwords accepted. Invalid emails may cause send failures. Users can register with easily-guessable credentials
- Fix approach: Use FluentValidation or data annotations for email format, password strength (uppercase, numbers, special chars), name length. Add backend validation separate from frontend

**Missing API Request Validation:**
- Issue: DTOs have no data annotations or validation attributes
- Files: `dotnet-web-api/OrbitSpace.Application/Dtos/**/*.cs`
- Impact: Invalid data reaches service layer. No automatic 400 errors for malformed requests
- Fix approach: Add `[Required]`, `[StringLength]`, `[EmailAddress]`, `[Range]` attributes to all DTOs. Enable model validation middleware globally

**Frontend-Only Validation Risk:**
- Issue: `create-goal-schema.ts` validates URL format on client side, but backend has no validation for `imageUrl` field
- Files: `next-js-app/src/features/goals/create-goal/model/create-goal-schema.ts` (lines 12-23), `dotnet-web-api/OrbitSpace.Application/Dtos/Goal/CreateGoalRequest.cs`
- Impact: Bypass via direct API call allows invalid URLs. Frontend validation is not a security boundary
- Fix approach: Implement same validation rules on backend. Use Uri.IsWellFormedUriString() or regex in C#. Always validate at the data boundary

## Testing & Code Quality

**Zero Test Coverage:**
- Issue: No unit tests, integration tests, or e2e tests found in codebase
- Files: None (testing infrastructure missing)
- Impact: Refactoring is high-risk. Regressions go undetected. New developers cannot verify behavior. Difficult to maintain code quality
- Fix approach: Start with critical path tests (authentication, authorization, CRUD operations). Use xUnit + Moq for backend. Add Jest/Vitest for frontend. Aim for 80%+ coverage on business logic

**Large Components:**
- Issue: `CreateGoalForm.tsx` is 232 lines; `GoalTableColumns.tsx` is 104 lines; `Sidebar.tsx` is 628 lines
- Files: `next-js-app/src/features/goals/create-goal/ui/CreateGoalForm.tsx`, etc.
- Impact: Hard to test. Hard to reuse. Difficult to reason about. Poor maintainability
- Fix approach: Break into smaller focused components. Extract custom hooks for logic. Use composition. Aim for 40-80 line components max

**Generated API Types Not Committed:**
- Issue: `v1.ts` (984 lines) is generated from backend OpenAPI spec but not version-controlled
- Files: `next-js-app/src/shared/api/v1.ts`
- Impact: Type mismatch if backend changes before types are regenerated. Different developers may have different versions. CI cannot catch type errors
- Fix approach: Commit generated types to git. Add `npm run generate-api-types` to CI pipeline. Fail build if types are out of sync with backend

## Architecture & Design Concerns

**Missing Authorization in Repositories:**
- Issue: No authorization checks at persistence layer; user ID filtering relies on service/controller layer only
- Files: `dotnet-web-api/OrbitSpace.Infrastructure/Persistence/Repositories/**/*.cs`
- Impact: A bug in a single controller could expose all user data. Authorization scattered across multiple layers. Defense-in-depth violated
- Fix approach: Implement IApplicationUserProvider in repositories. Always filter by current user ID in WHERE clause. Make unauthorized access impossible at data layer

**No Audit Logging:**
- Issue: No logs of who created/updated/deleted entities or when. No record of admin actions
- Files: All entity repositories
- Impact: Cannot detect unauthorized access patterns. GDPR compliance gaps. Incident investigation impossible
- Fix approach: Add `CreatedBy`, `CreatedAt`, `UpdatedBy`, `UpdatedAt`, `DeletedBy`, `DeletedAt` fields to entities. Log sensitive operations to audit table

**Missing Soft Deletes:**
- Issue: `ExecuteDeleteAsync()` permanently removes data; no way to recover deleted entities
- Files: `dotnet-web-api/OrbitSpace.Infrastructure/Persistence/Repositories/**/*.cs` (delete methods)
- Impact: Accidental deletion is unrecoverable. GDPR compliance (data retention) gaps. Data loss from bugs
- Fix approach: Implement soft delete pattern with `IsDeleted` flag. Archive to separate table. Query filters exclude soft-deleted items

**Incomplete Login Response Type:**
- Issue: `LoginResponseDto` only returns `AccessToken` without refresh token; no token expiry information
- Files: `dotnet-web-api/OrbitSpace.Application/Dtos/Authentication/LoginResponseDto.cs`
- Impact: Frontend cannot implement token refresh. Session management incomplete
- Fix approach: Return `{ accessToken, refreshToken, expiresIn, user }` from login endpoint. Frontend stores refresh token for session extension

**Error Handling Exposes Internal Details:**
- Issue: `GlobalExceptionHandler` returns full exception message to client in development
- Files: `dotnet-web-api/OrbitSpace/Exceptions/GlobalExceptionHandler.cs` (line 16)
- Impact: Stack traces or internal error details could leak sensitive system information
- Fix approach: Return generic "An unexpected error occurred" to clients. Log full details server-side only. Use problem details for structured errors

## Performance Concerns

**All Goals Loaded at Once:**
- Issue: `GoalsController.GetAll()` calls `goalService.GetAllAsync()` which loads all user goals without pagination
- Files: `dotnet-web-api/OrbitSpace/Controllers/GoalsController.cs` (lines 22-26)
- Impact: Users with 1000+ goals experience slow API responses. Client-side rendering of all rows causes browser freeze
- Fix approach: Add `page`, `pageSize`, `status` query parameters to `GetAll()`. Implement cursor-based pagination for Activity Grid

**Missing Database Indexes:**
- Issue: No explicit indexes on `UserId` or other frequently-queried columns
- Files: `dotnet-web-api/OrbitSpace.Infrastructure/Persistence/Configurations/*.cs`
- Impact: Table scans on large datasets. Slow queries as data grows. User lookup by email is not indexed
- Fix approach: Add HasIndex() in EF Core configurations for UserId, Email. Add composite indexes for common filter combinations

**No Caching:**
- Issue: No response caching, Redis, or query result caching implemented
- Files: Throughout backend
- Impact: Repeated identical requests cause repeated database queries. No performance optimization for read-heavy operations
- Fix approach: Add [ResponseCache] attributes to read-only endpoints. Implement Redis for user profile cache. Use query result caching strategically

## Deployment & Configuration Concerns

**Hardcoded CORS Origins:**
- Issue: `AllowedOrigins` configured in `appsettings.json` but only includes localhost
- Files: `dotnet-web-api/OrbitSpace/appsettings.json` (line 18)
- Impact: Production deployment requires manual config change. Risk of wrong CORS settings in production
- Fix approach: Store CORS origins in environment variable `CORS__ALLOWEDORIGINS=https://example.com,https://app.example.com`. Fail fast if not configured

**Next.js Client Lacks Environment Validation:**
- Issue: `NEXT_PUBLIC_BACKEND_BASE_URL` is used but no startup validation that it's set
- Files: `next-js-app/src/shared/config`
- Impact: Application starts with undefined backend URL. API calls fail at runtime with confusing errors
- Fix approach: Create config validation function that runs at app startup. Throw error if required env vars are missing

**No Database Migration Strategy Documented:**
- Issue: Schema changes and data migration approach not documented
- Files: Missing migration guide
- Impact: Unclear how to update production database. Risk of schema mismatches
- Fix approach: Document EF Core migration process. Add migration validation to startup. Implement rollback strategy

## Known Bugs & Fragile Areas

**Activity Repository GetByIdAsync Missing User Check:**
- Issue: `GetByIdAsync(Guid id)` does not verify user ownership; caller must check (unlike DeleteAsync which does)
- Files: `dotnet-web-api/OrbitSpace.Infrastructure/Persistence/Repositories/ActivityRepository.cs` (lines 16-19)
- Impact: Authorization check can be forgotten in controller. Cross-user data access possible
- Safe modification: Always add `Guid userId` parameter and filter by both ID and UserID. See DeleteAsync as pattern

**Duplicate Validation Messages:**
- Issue: "Invalid username or password" appears in both Register and Login error messages but means different things
- Files: `dotnet-web-api/OrbitSpace.Application/Services/AuthenticationService.cs` (lines 15, 25)
- Impact: Confusing error messages. Cannot distinguish registration failure from login failure from API response alone
- Safe modification: Use distinct error messages and error codes for register vs login failures

**Missing Error Details on Client:**
- Issue: `goal-queries.ts` throws `res.error` but error structure is unknown
- Files: `next-js-app/src/entities/goal/api/goal-queries.ts` (line 15)
- Impact: Error handling UI cannot display helpful messages. Error boundaries don't know error type
- Safe modification: Define error response shape from OpenAPI spec. Use typed error handling in query error callbacks

## Dependencies at Risk

**Clerk JWT Passing:**
- Issue: Frontend extracts Clerk JWT and passes to backend, but backend doesn't validate Clerk as issuer
- Files: `next-js-app/app/api/proxy/[...path]/route.ts` (lines 7-8), `dotnet-web-api/OrbitSpace.Infrastructure/Services/JwtTokenService.cs`
- Impact: Backend expects its own JWT format but receives Clerk JWT. User ID extraction assumes `ClaimTypes.NameIdentifier` which may not exist in Clerk token
- Fix approach: Backend should validate `iss` (issuer) claim matches Clerk issuer. Or implement separate login flow that returns backend JWT

**Generated Types Out of Sync:**
- Issue: `openapi-fetch` in frontend generated from backend schema but no validation that types match runtime API
- Files: `next-js-app/src/shared/api/v1.ts`
- Impact: Backend response structure could change without frontend knowing. Type checking provides false confidence
- Fix approach: Add API contract tests. Regenerate types in CI. Run schema validation in tests

## Test Coverage Gaps

**Authentication Flow Untested:**
- Issue: No tests for login, register, token generation, or authorization checks
- Files: None (no test project)
- Impact: Regressions in auth go undetected. Credentials bug fixes cannot be verified
- Priority: **High** - authentication is security-critical

**Database Query Tests Missing:**
- Issue: Repository methods have no tests for authorization filtering or pagination
- Files: None (no test project)
- Impact: N+1 bugs, authorization bypasses discovered in production
- Priority: **High** - data access is critical

**Frontend Component Interaction Tests Missing:**
- Issue: Form validation, mutation error handling, query loading states not tested
- Files: None (no test project)
- Impact: UX bugs and error handling failures discovered by users
- Priority: **Medium** - affects user experience

**CORS and Security Headers Not Tested:**
- Issue: No tests verifying CORS rules, Content-Security-Policy, or other security headers
- Files: None (no test project)
- Impact: Security configuration drifts. Vulnerabilities appear in production
- Priority: **Medium** - security-related

---

*Concerns audit: 2026-02-14*
