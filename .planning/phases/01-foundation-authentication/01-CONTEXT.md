# Phase 1: Foundation & Authentication - Context

**Gathered:** 2026-02-15
**Status:** Ready for planning

<domain>
## Phase Boundary

Secure user account creation and login with custom JWT authentication (replacing Clerk). BFF proxy pattern with defense-in-depth security. Includes registration, login, token management, and protected route infrastructure. Email verification and OAuth providers are deferred.

</domain>

<decisions>
## Implementation Decisions

### Auth Architecture
- Custom JWT authentication (remove Clerk completely)
- Access token + refresh token with token rotation
- httpOnly cookies for token storage (never exposed to browser JS)
- BFF proxy pattern — all client-side API calls route through Next.js proxy
- Auto token refresh on client with race condition prevention (request queue pattern)
- Multi-device support (multiple active sessions per user)
- proxy.ts middleware for route protection
- Future-ready for OAuth providers (Google, etc.) — design login endpoint to accommodate

### Registration Flow
- Fields: Email, Password, Name, Date of Birth
- Password: minimum 4 characters (development phase, tighten later)
- Email verification: required but deferred (no email sender setup now)
- After registration: auto-login and redirect to Dashboard (empty state)

### Login Experience
- "Remember me" toggle: checked = 30-day session, unchecked = session-only
- Error messages: generic ("Invalid email or password") — no email enumeration
- Post-login redirect: return to last visited page (callbackUrl pattern)
- "Forgot password" link shown but leads to stub page ("Coming soon")

### Auth Pages Design
- Separate pages: `/login` and `/register` as distinct routes with links between them
- Centered card layout on clean background
- Logo + "OrbitSpace" name displayed above form card
- System color preference (follows OS dark/light mode)

### Claude's Discretion
- Loading states and button disabled patterns during auth requests
- Form validation UX (inline vs on-submit)
- Exact token expiration times (access: ~15min, refresh: based on remember-me)
- Password hashing algorithm on backend (bcrypt/argon2)
- Database schema for users and refresh tokens
- Error boundary and fallback UI design

</decisions>

<specifics>
## Specific Ideas

- User wants to learn auth deeply — plan should be detailed and educational, not just "get it done"
- The architecture guide in `.claude/rules/Next.js 16 + .NET API production-ready JWT architecture guide.md` is the reference implementation pattern
- Token refresh must handle concurrent requests without race conditions (request queue pattern from the architecture guide)
- Design should accommodate future OAuth providers without major refactoring

</specifics>

<deferred>
## Deferred Ideas

- Email verification flow — requires email sender setup (SendGrid/Resend/SES decision pending)
- Password reset flow — stub page shown, implementation when email service is ready
- OAuth providers (Google, GitHub, etc.) — future phase, but auth architecture should be extensible
- Rate limiting on auth endpoints — important for production, not critical for v1

</deferred>

---

*Phase: 01-foundation-authentication*
*Context gathered: 2026-02-15*
