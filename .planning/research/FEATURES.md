# Feature Research

**Domain:** Life OS / Personal Productivity Platform
**Researched:** 2026-02-14
**Confidence:** MEDIUM

## Feature Landscape

### Table Stakes (Users Expect These)

#### Authentication & Account

| Feature | Why Expected | Complexity | Notes |
|---------|--------------|------------|-------|
| Email/password registration | Basic account creation | MEDIUM | Custom JWT, not OAuth — keep simple |
| Login with session persistence | Users expect to stay logged in | MEDIUM | httpOnly cookies, refresh tokens, 7-day sessions |
| Password reset via email | Standard security flow | MEDIUM | Requires email service integration |
| Logout from any page | Basic session control | LOW | Clear cookies, redirect |

#### Manifest (Identity Module)

| Feature | Why Expected | Complexity | Notes |
|---------|--------------|------------|-------|
| Create/edit free-form text document | Core module — must work on day one | LOW | Rich text or markdown editor |
| Auto-save while editing | Users expect no data loss | LOW | Debounced save on change |
| View current Manifest | Read mode separate from edit | LOW | Clean typography, no distractions |
| Yearly snapshot (Jan 1st archive) | Core design principle | MEDIUM | Background job or triggered on first visit of new year |

#### Activity Grid

| Feature | Why Expected | Complexity | Notes |
|---------|--------------|------------|-------|
| Monthly grid view (days x activities) | Core module visualization | HIGH | Complex table with mixed data types |
| Add/remove activities and metrics | Grid must be customizable | MEDIUM | Global repository + monthly active context |
| Boolean activity check-in (fact mode) | Primary daily interaction | LOW | Click to toggle cell |
| Metric value entry | Quantitative tracking (weight, sleep, energy) | MEDIUM | Different input types: number, time, rating 1-10 |
| Intent mode (plan for the day) | Key differentiator but expected for planning | MEDIUM | Visual distinction: outline vs filled |
| Monthly migration | Carry activities to next month | MEDIUM | Suggest last month's set, allow changes |
| Visual distinction: intent vs fact | Users must see plan vs done at a glance | LOW | Color/opacity difference |

#### Goals

| Feature | Why Expected | Complexity | Notes |
|---------|--------------|------------|-------|
| Create goal with title, description, deadline | Basic goal management | LOW | Standard CRUD |
| Link goal to Balance Wheel sector | Core identity-goal connection | LOW | Dropdown selection |
| Add tasks to a goal | Goals need actionable steps | LOW | Flat task list, no hierarchy |
| Mark tasks complete | Basic task management | LOW | Checkbox |
| View goal progress | Users expect progress feedback | MEDIUM | Calculate from tasks, activities |
| Goal feed (chronological log) | Shows activity over time | MEDIUM | Auto-log task completions, activity check-ins |

#### Balance Wheel

| Feature | Why Expected | Complexity | Notes |
|---------|--------------|------------|-------|
| Define custom sectors | Personalization is key | LOW | CRUD for sector names |
| Score sectors 1-10 | Core assessment mechanic | LOW | Slider or number input |
| Radar/wheel chart visualization | Visual representation of life balance | MEDIUM | Recharts RadarChart component |
| View previous scores for comparison | Progress tracking over time | MEDIUM | Overlay previous scores on chart |

### Differentiators (Competitive Advantage)

| Feature | Value Proposition | Complexity | Notes |
|---------|-------------------|------------|-------|
| Identity-First philosophy | No competitor does "who before what" | LOW | Manifest as center of navigation, framing |
| Zero double-entry | Check activity once, updates everywhere | HIGH | Event-driven sync between grid, goals, habits |
| No guilt accumulation | Ephemeral tasks vanish at midnight | MEDIUM | Midnight cleanup job, no overdue shame |
| Monthly active context | Conscious monthly selection of tracking | MEDIUM | Unlike always-on tracking in competitors |
| Goal path inheritance | Continue B1 → B2 with context preserved | MEDIUM | Link to predecessor, inherit activities/sector |
| Manual goal closure with stats | Forces conscious reflection | MEDIUM | Aggregate stats, prompt for summary |
| Activity visual codes | 1-5 char codes (P, ENG, DOG) for grid compactness | LOW | Unique to grid-based tracking |

### Anti-Features (Commonly Requested, Often Problematic)

| Feature | Why Requested | Why Problematic | Alternative |
|---------|---------------|-----------------|-------------|
| Task hierarchy/subtasks | "I need subtasks for complex work" | Adds complexity, infinite nesting, loses atomic simplicity | If task is complex, convert to Goal. Tasks stay flat |
| Real-time collaboration | "I want to share with partner/coach" | Massively increases complexity, auth, data model | Solo-first. Share via export/screenshot later |
| AI-generated goals/plans | "AI should suggest what to do" | Undermines identity-first philosophy — user must choose | AI can suggest activities based on Manifest, but never auto-create goals |
| Gamification (points, streaks, badges) | "Streaks motivate me" | Creates guilt when broken, extrinsic motivation undermines intrinsic identity work | Activity Grid itself IS the visual streak. No explicit streak counters |
| Calendar sync (Google/Outlook) | "I want all events in one place" | Scope creep, complex OAuth, sync conflicts | Daily Planner (v2) with manual time-blocking. No external sync in v1 |
| Mobile push notifications | "Remind me to check in" | Notification fatigue, complex infrastructure | v1 is web-only Station. Probe (future) handles mobile |

## Feature Dependencies

```
[Auth System]
    └──requires──> [User Entity + JWT]

[Manifest]
    └──requires──> [Auth System]

[Balance Wheel]
    └──requires──> [Auth System]

[Goals]
    └──requires──> [Auth System]
    └──requires──> [Balance Wheel] (sector linking)
    └──enhances──> [Activity Grid] (activity linking)

[Activity Grid]
    └──requires──> [Auth System]
    └──enhances──> [Goals] (zero double-entry)

[Manifest] ──independent──> [Activity Grid] (no hard DB links)
[Manifest] ──independent──> [Goals] (connection is through user consciousness)
```

### Dependency Notes

- **Goals requires Balance Wheel:** Each goal links to a sector. Sectors must exist first.
- **Activity Grid enhances Goals:** Activities can link to goals. Both must exist for zero double-entry.
- **Manifest is independent:** No DB links to other modules. Can be built in any order.
- **Auth is foundation:** Everything requires authenticated user.

## MVP Definition

### Launch With (v1)

- [ ] Auth system (register, login, logout, password reset) — gate to everything
- [ ] Manifest CRUD with auto-save — identity foundation
- [ ] Balance Wheel with sectors and scoring — life assessment
- [ ] Goals with tasks and sector linking — tactical projects
- [ ] Activity Grid with fact mode — daily tracking
- [ ] Activity Grid intent mode — daily planning
- [ ] Zero double-entry (activity check-in updates goal) — key differentiator

### Add After Validation (v1.x)

- [ ] Goal path inheritance — when users complete first goals
- [ ] Activity Grid monthly migration — when month 2 starts
- [ ] Balance Wheel reflection with historical comparison — when second assessment happens
- [ ] Manifest yearly snapshot — when Jan 1st arrives

### Future Consideration (v2+)

- [ ] Daily Planner with time-blocking — adds convenience over grid
- [ ] Habits module with installation/brick lifecycle — extends identity confirmation
- [ ] Dashboard with Memento Mori — needs data to visualize
- [ ] Knowledge Base — not core to identity-action loop
- [ ] Reflection module — structured reflection replaces freeform

## Feature Prioritization Matrix

| Feature | User Value | Implementation Cost | Priority |
|---------|------------|---------------------|----------|
| Auth system | HIGH | MEDIUM | P1 |
| Manifest CRUD | HIGH | LOW | P1 |
| Balance Wheel sectors + chart | HIGH | MEDIUM | P1 |
| Goals with tasks | HIGH | MEDIUM | P1 |
| Activity Grid (fact mode) | HIGH | HIGH | P1 |
| Activity Grid (intent mode) | MEDIUM | MEDIUM | P1 |
| Zero double-entry sync | HIGH | HIGH | P1 |
| Goal feed | MEDIUM | MEDIUM | P2 |
| Goal path inheritance | LOW | MEDIUM | P2 |
| Monthly migration | MEDIUM | LOW | P2 |
| Manifest yearly snapshot | LOW | LOW | P3 |

## Competitor Feature Analysis

| Feature | Notion | Habitica | Todoist | OrbitSpace Approach |
|---------|--------|----------|---------|---------------------|
| Identity/Manifest | No — pages are blank slate | No — gamification focus | No — task-only | Core philosophy — identity document drives everything |
| Activity Grid | No — custom databases possible but complex | Yes — habit tracking | No | Monthly table with visual codes, intent/fact modes |
| Goal tracking | Basic via databases/boards | Quests (gamified) | Projects (task-oriented) | Goals as micro-hubs linked to sectors + activities |
| Life balance | No | No | No | Balance Wheel with radar chart, periodic reflection |
| Daily tracking | No built-in | Daily habits | Today view | Activity Grid + intent mode ephemeral tasks |
| Guilt-free design | No — overdue items pile up | Gamification = guilt by design | Overdue tasks pile up | Ephemeral tasks vanish, no streaks to break |

## Sources

- Competitor analysis: Notion, Habitica, Todoist, Obsidian productivity workflows
- [Mobile app retention statistics 2026](https://www.remoteface.com/top-mobile-app-retention-features-in-2026-and-why-most-apps-still-lose-users/) — 17.1% day-one retention for productivity apps
- [Productivity app trends 2026](https://future.forem.com/matt_iscanner/7-productivity-app-trends-in-2026-59a7) — AI integration, minimalism trends
- OrbitSpace domain documentation (domain.md, domain-mechanics.md)

---
*Feature research for: Life OS / Personal Productivity Platform*
*Researched: 2026-02-14*
