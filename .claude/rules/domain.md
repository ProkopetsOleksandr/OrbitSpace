# OrbitSpace Domain Model

## Philosophy: Identity-First

The system's core principle is "First WHO, then WHAT":
1. **Declaration** — user defines desired identity in the Manifest ("I am an athlete")
2. **Confirmation** — every habit, goal, and activity is a vote confirming that identity
3. **Filter** — the Manifest acts as a filter: if a goal doesn't align with any identity paragraph, it's either unnecessary or the Manifest needs updating

---

## Modules & Entities

### 1. Identity / Manifest (The Core)

The "Living Manifest" — a free-form text document describing who the user wants to become.

- **Not a list of entities** — it's a narrative document that evolves over time
- **No hard DB links** to Goals/Tasks — connection is through human consciousness and context
- **Yearly Snapshot** — on Jan 1st, the system archives the current Manifest state; a new year starts with a fresh working copy
- **Foundation block** — displays "Bricks" (cemented habits) as proof of identity
- **Optional** — user can write it whenever they feel ready

### 2. Balance Wheel

Visual indicator of life quality across customizable sectors (e.g., Health, Career, Relationships).

- Each sector scored 1–10 by the user during strategic reflection
- Goals are linked to sectors — shows "weight" of each area on Dashboard
- Updated 2–4 times per year via Balance Wheel Reflection
- Displayed on Dashboard as a radar/wheel chart

### 3. Goals

Tactical projects driving identity forward. The bridge between Manifest (who) and Activities (daily actions).

**Entity fields**: Title, Description, Deadline, Balance Wheel Sector, Manifest Role (optional)

**Key integrations**:
- **Balance Wheel** — each goal linked to one sector
- **Activities** — regular processes connected to the goal; visualized as GitHub-style contribution grid on goal page
- **Tasks** — discrete steps (milestones) created inside the goal, auto-marked as `goal_linked`
- **Feed** — chronological log of all activities, completed tasks, and comments

**Goal page anatomy**: info block, activity grid, task list, event feed, notes block

**Lifecycle**:
- Never auto-closed — requires conscious analysis
- On close: user sees aggregated stats + writes final summary
- **Path Inheritance** — "Continue path" creates a new goal inheriting sector, activities, and link to predecessor (e.g., English B1 → B2)

### 4. Tasks

Atomic, flat planning elements. No hierarchy, no subtasks. If a task becomes complex — convert to Goal.

**Entity fields**: Title, Description, Date/Time, Goal Link (optional)

**Functional types**:
- **Goal Tasks** — steps toward a goal; auto-logged in goal's Feed on completion
- **Floating Tasks** — standalone daily/one-off items; no impact on Balance Wheel
- **System Tasks** — backend-generated (run reflection, update Manifest, log metrics)
- **Ephemeral Activity Tasks** — generated from Activity Grid planning; exist only for current day, vanish at midnight without "overdue" status

### 5. Activity Grid

The "Flight Log" — monthly table where columns = activities/metrics selected for the month, rows = days.

**Key entities**:
- **Activity** — boolean action (Programming, English, Walk). Visual code: 1–5 chars (P, ENG, DOG)
- **Metric** — quantitative value (Weight, Energy, Sleep). Types: Number, Time, Rating 1-10, Boolean

**Active Context mechanic**:
- Global repository of all user's activities/metrics
- Monthly slice: user selects what to track this month
- Migration: system suggests copying last month's set

**Two modes**:
- **Fact** — filled/colored cells (action completed)
- **Intent** — outlined cells (planned for the day); auto-creates ephemeral task

**Zero Double-Entry**: if Activity is linked to a Habit or Goal, checking it in the grid auto-updates the linked entity's progress.

### 6. Habits (Atomic Habits & Bricks)

Algorithms for confirming identity. Atomic, concrete actions.

**Entity fields**: Name, Formula/Trigger ("After [trigger], I will [action]"), Manifest Link, Schedule, Status

**Lifecycle phases**:
1. **Installation** — active tracking in Daily Planner as ephemeral task; user consciously builds the neural pathway
2. **Brick (Foundation)** — disappears from Planner, appears in Manifest's "Foundation" block; user just lives this way now
3. **Archive/Upgrade** — replaced by a more advanced version or no longer relevant

**Note**: Habits do NOT auto-fill Activity Grid. They are different levels — habit = minimum, activity = context.

### 7. Metrics

Quantitative/qualitative indicators tracked daily.

**Data types**: Number, Time/Duration, Rating (1-10), Boolean

- Displayed as rows in Activity Grid
- Each metric has its own analytics page (charts, trends, min/max, comments)
- Can be linked to Goals as progress indicators
- Active/Deactivated states (deactivated preserves history)

### 8. Knowledge Base

Personal repository of external content (books, articles, courses, videos).

**Source structure**:
- **Insights** — short phrases, quotes, key ideas. Can be starred (`high_priority`) for "Favorites" list
- **Personal Notes** — free-form commentary on the material

**Metadata**: Type (Book/Video/Article/Course), Status (Queue/In Progress/Read), Rating

**Balance Wheel link**: optional sector tag for categorization

### 9. Reflection

Periodic calibration tool. Does not block system — serves as analysis checkpoint.

**System types** (cannot be deleted):
1. **Activity Reflection** — operational review with stats (completed tasks, grid data) for a date range
2. **Balance Wheel Reflection** — strategic assessment, 2-4x/year, updates sector scores with comparison to previous
3. **Manifest Reflection** — reviewing and editing the living document

**Custom types**: user-created categories (e.g., "Birthday reflections", "Project retrospectives") — plain text editor, no stats panel

**Trigger**: system task auto-generated every N days (configurable)

**Archive**: grid of reflection cards with date, type, title. Editable — `updated_at` changes but chronological date stays.

### 10. Daily Planner (Focus Hub)

Main operational screen. Aggregates all tasks for the day from four sources:

1. **Planned activities** — from Activity Grid intent mode
2. **Scheduled tasks** — goal/floating tasks dated for today
3. **Overdue tasks** — unfulfilled past tasks (except ephemeral)
4. **Ad-hoc tasks** — created directly in planner

**Tools**: Time-blocking (Google Calendar-style), Pomodoro timer, Calendar navigator with task count indicators

**Mechanics**: Drag-and-drop prioritization, add from backlog dialog, ephemeral tasks vanish at day end

### 11. Dashboard

Year-scoped command center. Filters out past years, focuses on current 365 days.

**Sections**:
1. **Memento Mori** — life grid (1 cell = 1 year), lived years filled, current highlighted
2. **Year Progress Bar** — percentage of year elapsed
3. **Yearly Stats** — goals (active/completed/failed), habits cemented, tasks completed
4. **Monthly Trends** — charts for comparative analysis
5. **Time Machine** — switch between years; shows Manifest snapshot and stats for that year

---

## Entity Relationships

```
Manifest (Identity)
  ├── Goals ──→ Balance Wheel Sector
  │     ├── Tasks (goal_linked)
  │     ├── Activities (linked)
  │     └── Metrics (linked)
  ├── Habits ──→ Installation → Brick (in Manifest)
  └── Balance Wheel ──→ Sectors (1-10 scores)

Activity Grid (monthly)
  ├── Activities (boolean check-ins)
  ├── Metrics (value entries)
  └── Intent mode → Ephemeral Tasks → Daily Planner

Daily Planner
  ├── Ephemeral tasks (from Grid)
  ├── Goal tasks + Floating tasks
  ├── Overdue tasks
  ├── System tasks (reflection, manifest)
  └── Habit tasks (installation phase)

Reflection
  ├── Activity Reflection ← aggregated stats
  ├── Balance Wheel Reflection → updates sector scores
  ├── Manifest Reflection → edits identity document
  └── Custom Reflections (free text)

Knowledge Base
  └── Sources → Insights (starred) + Notes
```
