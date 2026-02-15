---
phase: 01-foundation-authentication
plan: 01
subsystem: backend-auth
tags: [jwt, refresh-tokens, authentication, security]
dependency_graph:
  requires: []
  provides:
    - RefreshToken entity with rotation support
    - Token generation and hashing service methods
    - Authentication endpoints (login, refresh, revoke)
    - Database migration for refresh tokens and user fields
  affects:
    - User entity (added DateOfBirth, EmailVerified, timestamps)
    - ITokenService (added refresh token methods)
    - IAuthenticationService (added refresh/revoke methods)
    - AuthenticationController (updated endpoints)
tech_stack:
  added:
    - SHA256 token hashing for refresh tokens
    - RandomNumberGenerator for cryptographic token generation
  patterns:
    - Token rotation (old token marked as used when refreshed)
    - RememberMe strategy (7 vs 30 day expiration)
    - httpOnly cookie preparation (returns unhashed tokens to client)
key_files:
  created:
    - dotnet-web-api/OrbitSpace.Domain/Entities/RefreshToken.cs
    - dotnet-web-api/OrbitSpace.Application/Common/Interfaces/IRefreshTokenRepository.cs
    - dotnet-web-api/OrbitSpace.Application/Dtos/Authentication/LoginRequestDto.cs
    - dotnet-web-api/OrbitSpace.Application/Dtos/Authentication/RefreshRequestDto.cs
    - dotnet-web-api/OrbitSpace.Application/Dtos/Authentication/RefreshResponseDto.cs
    - dotnet-web-api/OrbitSpace.Application/Dtos/Authentication/RevokeRequestDto.cs
    - dotnet-web-api/OrbitSpace.Infrastructure/Persistence/Repositories/RefreshTokenRepository.cs
    - dotnet-web-api/OrbitSpace.Infrastructure/Persistence/Configurations/RefreshTokenConfiguration.cs
    - dotnet-web-api/OrbitSpace.Infrastructure/Migrations/20260215163526_AddRefreshTokensAndUpdateUsers.cs
  modified:
    - dotnet-web-api/OrbitSpace.Domain/Entities/User.cs
    - dotnet-web-api/OrbitSpace.Application/Common/Interfaces/ITokenService.cs
    - dotnet-web-api/OrbitSpace.Application/Common/Models/OperationResult.cs
    - dotnet-web-api/OrbitSpace.Application/Dtos/Authentication/LoginResponseDto.cs
    - dotnet-web-api/OrbitSpace.Application/Dtos/Authentication/RegisterRequestDto.cs
    - dotnet-web-api/OrbitSpace.Application/Dtos/UserDto.cs
    - dotnet-web-api/OrbitSpace.Application/Services/Interfaces/IAuthenticationService.cs
    - dotnet-web-api/OrbitSpace.Application/Services/AuthenticationService.cs
    - dotnet-web-api/OrbitSpace.Infrastructure/Services/JwtTokenService.cs
    - dotnet-web-api/OrbitSpace.Infrastructure/Persistence/AppDbContext.cs
    - dotnet-web-api/OrbitSpace.Infrastructure/Persistence/Configurations/UserConfiguration.cs
    - dotnet-web-api/OrbitSpace.Infrastructure/DependencyInjection.cs
    - dotnet-web-api/OrbitSpace/Controllers/AuthenticationController.cs
decisions:
  - title: "Access token expiration: 15 minutes"
    rationale: "Security best practice - short-lived access tokens limit exposure window if compromised"
    alternatives: "7 days (previous value) - rejected as too long for production"
  - title: "Refresh token storage: SHA256 hashing"
    rationale: "One-way hash prevents token theft from database breach; client holds unhashed version"
    alternatives: "Plain text storage - rejected as security vulnerability"
  - title: "RememberMe strategy: 7 vs 30 days"
    rationale: "Balance between convenience (30 days for trusted devices) and security (7 days default)"
    alternatives: "Single expiration - rejected as less flexible"
  - title: "Password minimum: 4 characters (dev phase)"
    rationale: "Per user decision - simplify development/testing; will increase for production"
    alternatives: "6 characters (previous) - rejected for current phase"
metrics:
  duration_minutes: 5
  tasks_completed: 2
  files_created: 9
  files_modified: 15
  commits: 2
  completed_at: "2026-02-15T16:36:14Z"
---

# Phase 01 Plan 01: Backend Authentication Foundation Summary

JWT-based authentication with refresh token rotation, secure SHA256 hashing, and RememberMe support.

## What Was Built

This plan implemented the complete backend authentication foundation required for the Next.js BFF proxy pattern. The system now issues both access tokens (15-minute expiration) and refresh tokens (7 or 30 days based on RememberMe flag), stores refresh tokens as SHA256 hashes in PostgreSQL, and implements token rotation on refresh.

**Core entities:**
- RefreshToken entity with rotation tracking (UsedAtUtc, ReplacedByToken fields)
- Enhanced User entity with DateOfBirth, EmailVerified, EmailVerificationToken, and timestamps

**Service layer:**
- ITokenService extended with GenerateRefreshToken() and HashToken() methods
- JwtTokenService implements cryptographically secure token generation (64-byte random, SHA256 hashing)
- AuthenticationService implements full auth flows: register (with DateOfBirth), login (returns both tokens), refresh (with rotation), revoke, and revoke-all

**Data layer:**
- IRefreshTokenRepository interface with full CRUD operations
- RefreshTokenRepository implements token lookup with User navigation, active token queries, and revocation
- RefreshTokenConfiguration with indexes on Token, UserId, and ExpiresAtUtc columns
- Database migration for refresh_tokens table and User field additions

**API endpoints:**
- POST /api/authentication/login (accepts LoginRequestDto, returns access + refresh tokens)
- POST /api/authentication/refresh (rotates tokens, marks old as used)
- POST /api/authentication/revoke (revokes single token)

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 3 - Blocking Issue] Fixed AuthenticationController endpoint signature**
- **Found during:** Building solution after Task 2 implementation
- **Issue:** AuthenticationController.Login() still used old signature `LoginAsync(string email, string password)` causing compilation error
- **Fix:** Updated Login endpoint to accept LoginRequestDto parameter, added Refresh and Revoke endpoints
- **Files modified:** dotnet-web-api/OrbitSpace/Controllers/AuthenticationController.cs
- **Commit:** 769c4ec (included in Task 2 commit)

No other deviations - plan executed exactly as written.

## Key Implementation Details

**Token rotation flow:**
1. Client sends refresh token to /api/authentication/refresh
2. Backend hashes token, looks up in database
3. If valid and active, marks old token UsedAtUtc = now
4. Generates new access + refresh token pair
5. Creates new RefreshToken entity in database
6. Sets old token's ReplacedByToken field to new hashed token (audit trail)
7. Returns unhashed new tokens to client

**RememberMe strategy:**
- RememberMe = false: 7-day refresh token expiration
- RememberMe = true: 30-day refresh token expiration
- Strategy preserved during token rotation by checking old token's expiration

**Security properties:**
- Refresh tokens stored as SHA256 hashes (cannot be used if database is compromised)
- Client receives unhashed tokens (stores in httpOnly cookies via Next.js BFF proxy)
- Access tokens expire in 15 minutes (limits exposure window)
- Token rotation prevents reuse attacks (old token marked as used)
- ReplacedByToken audit trail enables revocation cascade if needed

**Database schema:**
- refresh_tokens table with indexes on token (lookup), user_id (user sessions), expires_at_utc (cleanup queries)
- Cascade delete on User → RefreshTokens relationship
- users table extended with date_of_birth, email_verified, email_verification_token, created_at_utc, updated_at_utc

## Testing Recommendations

Before moving to next plan, verify:

1. **Registration flow:** POST /api/authentication/register with DateOfBirth field
2. **Login flow:** POST /api/authentication/login returns both accessToken and refreshToken
3. **Token rotation:** POST /api/authentication/refresh with valid refresh token returns new pair
4. **Token expiration:** Access token expires in 15 minutes (check JWT claims)
5. **RememberMe:** Login with RememberMe=true creates 30-day refresh token
6. **Revocation:** POST /api/authentication/revoke marks token as revoked
7. **Database state:** Refresh tokens stored as SHA256 hashes (not plain text)
8. **Used token detection:** Refreshing same token twice fails (second attempt sees UsedAtUtc is set)

## What's Next

The backend auth foundation is complete. Next plan (01-02) will implement the Next.js frontend:
- Route Handlers for /api/auth/login, /api/auth/refresh, /api/auth/revoke
- httpOnly cookie management (store tokens, auto-attach to requests)
- Data Access Layer (DAL) with getSession() for Server Components
- Proxy Route Handler (/api/proxy/[...path]) for authenticated requests
- TanStack Query setup with server-side prefetching and hydration

This backend serves as the secure foundation - frontend will never see raw JWTs in browser JavaScript.

## Self-Check: PASSED

**Files created verification:**
- FOUND: dotnet-web-api/OrbitSpace.Domain/Entities/RefreshToken.cs
- FOUND: dotnet-web-api/OrbitSpace.Application/Common/Interfaces/IRefreshTokenRepository.cs
- FOUND: dotnet-web-api/OrbitSpace.Application/Dtos/Authentication/LoginRequestDto.cs
- FOUND: dotnet-web-api/OrbitSpace.Application/Dtos/Authentication/RefreshRequestDto.cs
- FOUND: dotnet-web-api/OrbitSpace.Application/Dtos/Authentication/RefreshResponseDto.cs
- FOUND: dotnet-web-api/OrbitSpace.Application/Dtos/Authentication/RevokeRequestDto.cs
- FOUND: dotnet-web-api/OrbitSpace.Infrastructure/Persistence/Repositories/RefreshTokenRepository.cs
- FOUND: dotnet-web-api/OrbitSpace.Infrastructure/Persistence/Configurations/RefreshTokenConfiguration.cs
- FOUND: dotnet-web-api/OrbitSpace.Infrastructure/Migrations/20260215163526_AddRefreshTokensAndUpdateUsers.cs

**Commits verification:**
- FOUND: c9adeeb (Task 1 - RefreshToken entity and auth DTOs)
- FOUND: 769c4ec (Task 2 - Token service, auth service, database migration)

**Build verification:**
- Solution builds without errors: YES
- Migration file created: YES
- Access token expiration: 15 minutes (verified in JwtTokenService.cs line 29)
- Refresh token hashing: SHA256 (verified in JwtTokenService.cs lines 47-51)

All checks passed.
