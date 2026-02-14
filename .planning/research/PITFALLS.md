# Pitfalls Research

**Domain:** Life OS / Personal Productivity Platform
**Researched:** 2026-02-14
**Confidence:** MEDIUM

## Critical Pitfalls

### Pitfall 1: Zero Double-Entry Becomes Zero Reliability

**What goes wrong:**
The zero double-entry system (checking an activity updates linked goals/habits) has complex cascading logic. When a single check-in triggers multiple updates across entities, partial failures leave data inconsistent — the grid shows "done" but the goal feed is missing the entry.

**Why it happens:**
Developers implement cascading updates as sequential operations without transactions. Or they use fire-and-forget events that silently fail. The happy path works in development, but edge cases (concurrent requests, timeout mid-cascade) break in production.

**How to avoid:**
- Wrap the entire check-in flow in a database transaction. Activity entry + goal feed update + any linked updates are atomic.
- Use domain events within the same transaction boundary (not async message queues for v1).
- If an event handler fails, the entire check-in rolls back — better to show "not checked" than have inconsistent state.

**Warning signs:**
- Goal feed entries missing for checked activities
- Activity Grid shows "done" but goal progress doesn't reflect it
- Inconsistent counts between grid and goal page

**Phase to address:** Activity Grid implementation phase + Goals integration

---

### Pitfall 2: Activity Grid Performance Death by Query

**What goes wrong:**
Loading a monthly Activity Grid requires fetching 30 days x N activities x M metrics = potentially hundreds of individual records. Naive implementation makes N+1 queries or loads everything in a single massive join that's slow.

**Why it happens:**
The grid is a matrix (2D), but the database is rows (1D). The translation between them is non-trivial. Developers either: (a) query per cell (N*30 queries), (b) load all entries and reshape in application code (memory spike), or (c) build a complex pivot query that's hard to maintain.

**How to avoid:**
- Single query: `SELECT * FROM activity_entries WHERE user_id = @id AND date BETWEEN @start AND @end` — returns all entries for the month in one roundtrip.
- Same for metric_entries. Two queries total for the entire grid.
- Reshape to matrix structure in application layer (not database).
- Index: `(user_id, date)` composite index is critical.

**Warning signs:**
- Grid page load > 500ms
- Database query count > 5 for a single grid load
- Memory usage spikes on grid page

**Phase to address:** Activity Grid phase (database design)

---

### Pitfall 3: Ephemeral Task Midnight Cleanup Data Loss

**What goes wrong:**
Ephemeral tasks (from Activity Grid intent mode) should vanish at midnight. But "midnight" is ambiguous — user's timezone? Server timezone? What if the user is actively working at midnight? What if the cleanup job fails?

**Why it happens:**
Timezone handling is notoriously hard. Developers pick a single timezone, forget that midnight is a moving target, or implement cleanup as a cron job that can miss or double-run.

**How to avoid:**
- Store all dates in UTC. Store user's timezone preference.
- Don't use a background job for cleanup. Instead, query "today's tasks" using user's current date. Yesterday's ephemeral tasks simply don't appear because the query filters by today's date.
- Never delete ephemeral task records — mark them with a `date` field and filter in queries. Historical data preserved for analytics.

**Warning signs:**
- Tasks disappearing mid-session for users in different timezones
- Tasks persisting past midnight
- Duplicate task creation when user crosses midnight while app is open

**Phase to address:** Foundation/Auth phase (timezone handling) + Activity Grid phase

---

### Pitfall 4: Monthly Active Context Migration Confusion

**What goes wrong:**
Users must select which activities/metrics to track each month. When a new month starts, the system should suggest carrying over last month's set. But edge cases abound: what if the user hasn't configured the new month yet? What do grid queries return? What happens to entries from activities that were active last month but not this month?

**Why it happens:**
The "active context" mechanic is unique to OrbitSpace. No standard pattern exists. Developers handle the happy path (user configures new month) but not the edges (month starts, user hasn't configured, what shows?).

**How to avoid:**
- Auto-create new monthly context on first visit to a new month, copying previous month's configuration.
- Allow users to modify the auto-created context (add/remove activities).
- Grid queries always filter by the monthly context. No context = show setup prompt, not empty grid.
- Historical entries from deactivated activities are preserved but not shown in grid (still visible in goal feeds).

**Warning signs:**
- Blank grid on month boundary
- Users confused about why activities disappeared
- Historical data gaps

**Phase to address:** Activity Grid phase

---

### Pitfall 5: Productivity App Abandonment (17% Day-1 Retention)

**What goes wrong:**
Productivity apps have the worst retention in mobile: 17.1% day-one, 4.1% day-30. Users download, see empty state, don't know what to do, leave forever. OrbitSpace has this risk amplified because "identity-first" is unfamiliar — users don't know what a "Manifest" is.

**Why it happens:**
Developers build features but not onboarding. Empty states show "no data" instead of guiding the user. The value proposition requires multiple steps before becoming clear (write manifest → set up wheel → create goal → track activities → see connection).

**How to avoid:**
- First-run experience that demonstrates value in under 2 minutes
- Pre-populated example data that shows what a "filled" system looks like
- Progressive disclosure: don't show all modules at once. Start with Manifest, then reveal others
- System tasks that guide setup ("Fill your Balance Wheel", "Create your first goal")
- Each module's empty state teaches what it's for and how to use it

**Warning signs:**
- Users sign up but never complete Manifest
- High bounce rate on Activity Grid (too complex without context)
- Users create account, visit once, never return

**Phase to address:** Every phase — each module needs an onboarding state

---

## Technical Debt Patterns

| Shortcut | Immediate Benefit | Long-term Cost | When Acceptable |
|----------|-------------------|----------------|-----------------|
| Skip refresh token rotation | Simpler auth flow | Less secure if refresh token stolen | Never in production — always rotate |
| Store Activity Grid as JSON blob | Faster initial implementation | Can't query individual entries, no indexes | Never — normalize from day one |
| Skip input validation on .NET | Faster endpoint development | Security vulnerabilities, bad data in DB | Never |
| Hardcode timezone to UTC | Simpler date handling | Wrong midnight for every non-UTC user | Only if single-user and you know the timezone |
| Skip OpenAPI type generation | Faster initial frontend dev | Type drift between frontend and backend | Only for prototyping, add before v1 |

## Integration Gotchas

| Integration | Common Mistake | Correct Approach |
|-------------|----------------|------------------|
| Next.js ↔ .NET API | Forgetting to handle 401 in proxy (token expired) | Proxy catches 401, attempts refresh, retries original request |
| TanStack Query hydration | Using same QueryClient for multiple requests (data leak between users) | Create new QueryClient per Server Component request |
| EF Core migrations | Running migrations in production without backup | Always backup before migration. Use `dotnet ef migrations script` for production |
| OpenAPI type generation | Generated types get stale, not regenerated after API changes | Add type generation to CI pipeline or pre-build script |
| Cookie-based auth | SameSite=None without Secure flag | Use SameSite=Lax + Secure in production |

## Performance Traps

| Trap | Symptoms | Prevention | When It Breaks |
|------|----------|------------|----------------|
| N+1 queries on Goal list with tasks | Goal list page slow, DB CPU high | Include tasks in initial query with `Include()` or batch query | 10+ goals with 5+ tasks each |
| Unindexed date queries | Activity Grid loads slowly | Composite index on (user_id, date) | 6+ months of daily data |
| Sending full grid data on each check-in | Large payloads, slow updates | Mutation endpoints return minimal data. TanStack Query invalidates cached grid | 20+ activities tracked |
| Server Component waterfall | Sequential server-to-server fetches | Use `Promise.all()` for independent data fetches in Server Components | 3+ independent data sources on one page |

## Security Mistakes

| Mistake | Risk | Prevention |
|---------|------|------------|
| JWT in localStorage | XSS steals auth tokens | httpOnly cookies via BFF proxy (already planned) |
| No rate limiting on auth endpoints | Brute force attacks | Rate limit login/register to 5 attempts per minute |
| Trusting Next.js middleware alone for auth | CVE-2025-29927 middleware bypass | Defense-in-depth: middleware + DAL + Server Action auth checks |
| Returning full user object from login | Leaking password hashes or internal IDs | Return only safe fields: id, name, email |
| No CSRF protection on mutations | Cross-site request forgery | SameSite=Lax cookies + verify Origin header on mutations |

## UX Pitfalls

| Pitfall | User Impact | Better Approach |
|---------|-------------|-----------------|
| Showing all 4 modules at once | Overwhelm, decision paralysis | Progressive disclosure — Manifest first, reveal others after setup |
| Activity Grid with no activities | Empty grid means nothing — user bounces | Pre-suggest common activities, show example grid |
| Balance Wheel with generic sectors | "Career, Health, Relationships" feels impersonal | Let user define sectors from scratch, provide examples as inspiration |
| Manifest as blank text area | "What do I write?" paralysis | Provide prompts: "Who do you want to be in 1 year?", example manifest |
| No visual feedback on check-in | User unsure if action registered | Immediate cell color change + subtle animation |

## "Looks Done But Isn't" Checklist

- [ ] **Auth:** Often missing refresh token flow — verify tokens auto-refresh before expiry
- [ ] **Activity Grid:** Often missing monthly boundary handling — verify what happens on month change
- [ ] **Goals:** Often missing the close flow — verify manual closure with stats and summary
- [ ] **Balance Wheel:** Often missing historical comparison — verify previous scores visible during new assessment
- [ ] **Manifest:** Often missing auto-save — verify edits persist without explicit save button
- [ ] **Zero double-entry:** Often missing rollback on failure — verify partial updates don't persist
- [ ] **Grid intent mode:** Often missing visual distinction — verify intent vs fact cells are clearly different

## Recovery Strategies

| Pitfall | Recovery Cost | Recovery Steps |
|---------|---------------|----------------|
| Inconsistent zero double-entry data | MEDIUM | Write reconciliation script to rebuild goal feeds from activity entries |
| Wrong timezone handling | HIGH | Migrate all dates to UTC, add timezone column to users, recompute all date-dependent queries |
| Unindexed grid queries | LOW | Add indexes, no data migration needed |
| Missing refresh token rotation | LOW | Add rotation logic to refresh endpoint, invalidate existing tokens |
| Poor onboarding | MEDIUM | Add empty states and system tasks retroactively |

## Pitfall-to-Phase Mapping

| Pitfall | Prevention Phase | Verification |
|---------|------------------|--------------|
| Zero double-entry reliability | Activity Grid + Goals integration | Test: check activity → verify goal feed entry created atomically |
| Grid performance | Activity Grid (schema design) | Test: load month grid in < 300ms with 20 activities |
| Midnight cleanup | Foundation (timezone) + Activity Grid | Test: intent tasks from yesterday don't appear today |
| Monthly migration | Activity Grid | Test: new month auto-creates context from previous |
| User abandonment | Every phase (onboarding states) | Test: new user reaches first check-in within 2 minutes |
| Auth security | Foundation/Auth phase | Test: no tokens visible in browser JS, refresh works, rate limiting active |

## Sources

- [Mobile app retention 2026](https://www.remoteface.com/top-mobile-app-retention-features-in-2026-and-why-most-apps-still-lose-users/) — 17.1% day-1 retention for productivity apps
- [CVE-2025-29927](https://nextjs.org/docs/app/guides/upgrading/version-16) — Next.js middleware bypass vulnerability
- [TanStack Query SSR gotchas](https://tanstack.com/query/latest/docs/framework/react/guides/ssr) — QueryClient per-request requirement
- OrbitSpace domain mechanics documentation

---
*Pitfalls research for: Life OS / Personal Productivity Platform*
*Researched: 2026-02-14*
