# Requirements: OrbitSpace

**Defined:** 2026-02-14
**Core Value:** User can define their identity through the Manifest and see every daily action as a confirmation of who they're becoming

## v1 Requirements

### Authentication

- [ ] **AUTH-01**: User can create account with email and password
- [ ] **AUTH-02**: User can log in and stay logged in across sessions via httpOnly cookies

### Manifest

- [ ] **MNFST-01**: User can create and edit their identity Manifest (design TBD during phase planning)
- [ ] **MNFST-02**: User can view their Manifest in read mode

### Activity Grid

- [ ] **GRID-01**: User can view monthly grid (days as rows, activities/metrics as columns)
- [ ] **GRID-02**: User can add and remove activities and metrics to their grid
- [ ] **GRID-03**: User can check in activities for the day (fact mode — boolean toggle)
- [ ] **GRID-04**: User can enter metric values for the day (number, time, rating 1-10)
- [ ] **GRID-05**: User can mark intent for activities (plan for the day — visual outline)
- [ ] **GRID-06**: User can distinguish intent cells from fact cells visually

### Goals

- [ ] **GOAL-01**: User can create a goal with title, description, and deadline
- [ ] **GOAL-02**: User can link a goal to a Balance Wheel sector
- [ ] **GOAL-03**: User can add flat tasks to a goal and mark them complete
- [ ] **GOAL-04**: User can view a personal goal page with info, tasks, and progress

### Balance Wheel

- [ ] **WHEEL-01**: User can define custom life sectors
- [ ] **WHEEL-02**: User can score each sector from 1 to 10
- [ ] **WHEEL-03**: User can view their scores as a radar/wheel chart
- [ ] **WHEEL-04**: User can see previous scores overlaid when making a new assessment

## v2 Requirements

### Authentication (Extended)

- **AUTH-03**: User can reset password via email link
- **AUTH-04**: User can log out from any page

### Manifest (Extended)

- **MNFST-03**: Manifest auto-saves while editing
- **MNFST-04**: Manifest yearly snapshot archived on Jan 1st
- **MNFST-05**: Foundation block displays cemented habits (requires Habits module)

### Activity Grid (Extended)

- **GRID-07**: Monthly migration suggests carrying last month's activities
- **GRID-08**: Activity visual codes (1-5 char codes like P, ENG, DOG)

### Goals (Extended)

- **GOAL-05**: Goal feed shows chronological log of activities and task completions
- **GOAL-06**: User can manually close a goal with aggregated stats and summary
- **GOAL-07**: Goal path inheritance (continue B1 → B2 with context)

### Daily Planner

- **PLAN-01**: User can view all tasks for today aggregated from multiple sources
- **PLAN-02**: User can time-block tasks in calendar view
- **PLAN-03**: User can use Pomodoro timer for deep work

### Habits

- **HABIT-01**: User can create habits with formula/trigger
- **HABIT-02**: Habit lifecycle: Installation → Brick → Archive

### Dashboard

- **DASH-01**: Year-scoped command center with stats
- **DASH-02**: Memento Mori life grid
- **DASH-03**: Time Machine for historical years

### Reflection

- **REFL-01**: Activity Reflection with aggregated stats
- **REFL-02**: Balance Wheel Reflection updates sector scores
- **REFL-03**: Custom reflection types

## Out of Scope

| Feature | Reason |
|---------|--------|
| Real-time collaboration | Solo-first app. Massively increases complexity |
| Mobile app / PWA (Probe) | Web-first Station. Mobile deferred to future |
| AI-generated goals | Undermines identity-first philosophy — user must choose |
| Gamification (streaks, badges) | Creates guilt, extrinsic motivation undermines identity work |
| Calendar sync (Google/Outlook) | Scope creep, complex OAuth, sync conflicts |
| Task hierarchy/subtasks | Violates flat task design. Complex tasks → convert to Goal |
| Push notifications | Web-only in v1, no mobile infrastructure |
| Knowledge Base | Not core to identity-action loop |

## Traceability

| Requirement | Phase | Status |
|-------------|-------|--------|
| AUTH-01 | Phase 1 | Pending |
| AUTH-02 | Phase 1 | Pending |
| WHEEL-01 | Phase 2 | Pending |
| WHEEL-02 | Phase 2 | Pending |
| WHEEL-03 | Phase 2 | Pending |
| WHEEL-04 | Phase 2 | Pending |
| MNFST-01 | Phase 3 | Pending |
| MNFST-02 | Phase 3 | Pending |
| GRID-01 | Phase 4 | Pending |
| GRID-02 | Phase 4 | Pending |
| GRID-03 | Phase 4 | Pending |
| GRID-04 | Phase 4 | Pending |
| GRID-05 | Phase 4 | Pending |
| GRID-06 | Phase 4 | Pending |
| GOAL-01 | Phase 5 | Pending |
| GOAL-02 | Phase 5 | Pending |
| GOAL-03 | Phase 5 | Pending |
| GOAL-04 | Phase 5 | Pending |

**Coverage:**
- v1 requirements: 18 total
- Mapped to phases: 18
- Unmapped: 0

---
*Requirements defined: 2026-02-14*
*Last updated: 2026-02-14 after roadmap creation*
