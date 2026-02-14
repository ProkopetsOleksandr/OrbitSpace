# Roadmap: OrbitSpace

## Overview

OrbitSpace delivers the Identity-First life operating system through six phases that build from authentication foundation to the complete identity-action loop. Each phase delivers a coherent, verifiable capability: secure user accounts, life quality visualization, identity documentation, daily tracking infrastructure, goal management with sector linkage, and finally the zero double-entry system that connects everything. The journey moves from infrastructure to identity to action tracking to strategic goals, culminating in the seamless integration that makes every daily check-in a confirmation of chosen identity.

## Phases

**Phase Numbering:**
- Integer phases (1, 2, 3): Planned milestone work
- Decimal phases (2.1, 2.2): Urgent insertions (marked with INSERTED)

Decimal phases appear between their surrounding integers in numeric order.

- [ ] **Phase 1: Foundation & Authentication** - Custom JWT auth with BFF proxy pattern
- [ ] **Phase 2: Balance Wheel** - Life sector visualization and scoring
- [ ] **Phase 3: Identity Manifest** - Free-form identity document with read/edit modes
- [ ] **Phase 4: Activity Grid** - Monthly tracking with fact/intent modes
- [ ] **Phase 5: Goals System** - Tactical projects with sector and activity linking
- [ ] **Phase 6: Integration & Polish** - Zero double-entry wiring and onboarding

## Phase Details

### Phase 1: Foundation & Authentication
**Goal**: Users can securely create accounts and access the application with JWT-based authentication following BFF proxy pattern and defense-in-depth security
**Depends on**: Nothing (first phase)
**Requirements**: AUTH-01, AUTH-02
**Success Criteria** (what must be TRUE):
  1. User can create a new account with email and password
  2. User can log in and stay logged in across browser sessions
  3. JWT tokens are stored in httpOnly cookies (never exposed to browser JS)
  4. Server Components can fetch from .NET API with automatic auth header injection
  5. Client Components can call proxy routes that forward to .NET API with cookies
**Plans**: TBD

Plans:
- [ ] 01-01: TBD during phase planning

### Phase 2: Balance Wheel
**Goal**: Users can visualize and track their life quality across custom sectors with radar chart representation
**Depends on**: Phase 1 (requires authentication)
**Requirements**: WHEEL-01, WHEEL-02, WHEEL-03, WHEEL-04
**Success Criteria** (what must be TRUE):
  1. User can define their own life sectors with custom names
  2. User can score each sector from 1 to 10
  3. User can see their sector scores as a radar/wheel chart visualization
  4. User can see previous scores overlaid when making a new assessment
  5. Wheel data persists and loads correctly across sessions
**Plans**: TBD

Plans:
- [ ] 02-01: TBD during phase planning

### Phase 3: Identity Manifest
**Goal**: Users can declare and refine their identity through a free-form document that serves as the foundation for all other modules
**Depends on**: Phase 1 (requires authentication)
**Requirements**: MNFST-01, MNFST-02
**Success Criteria** (what must be TRUE):
  1. User can create their Manifest with free-form text (design approach determined during planning)
  2. User can edit their Manifest and see changes persist
  3. User can view their Manifest in read-only mode
  4. Manifest is accessible from navigation as central identity reference
**Plans**: TBD

Plans:
- [ ] 03-01: TBD during phase planning

### Phase 4: Activity Grid
**Goal**: Users can track daily activities and metrics in a monthly grid with intent/fact distinction and monthly active context
**Depends on**: Phase 1 (requires authentication)
**Requirements**: GRID-01, GRID-02, GRID-03, GRID-04, GRID-05, GRID-06
**Success Criteria** (what must be TRUE):
  1. User can view a monthly grid with days as rows and activities/metrics as columns
  2. User can add activities and metrics to their active tracking set for the month
  3. User can remove activities and metrics from their active set
  4. User can mark activities complete for a day (fact mode - filled/colored cells)
  5. User can enter metric values for a day (number, time, rating 1-10)
  6. User can mark intent for activities (outlined cells that distinguish from facts)
  7. Grid correctly displays current month and allows navigation to other months
  8. Grid state persists and reloads correctly across sessions
**Plans**: TBD

Plans:
- [ ] 04-01: TBD during phase planning

### Phase 5: Goals System
**Goal**: Users can create tactical goals linked to Balance Wheel sectors with task management and progress tracking
**Depends on**: Phase 2 (Goals link to sectors)
**Requirements**: GOAL-01, GOAL-02, GOAL-03, GOAL-04
**Success Criteria** (what must be TRUE):
  1. User can create a goal with title, description, and deadline
  2. User can link a goal to a Balance Wheel sector during creation or editing
  3. User can add flat tasks to a goal and mark them complete
  4. User can view a goal's dedicated page showing info, task list, and progress
  5. Goals display in a list view with filtering by sector
  6. Task completion updates goal progress indicators
**Plans**: TBD

Plans:
- [ ] 05-01: TBD during phase planning

### Phase 6: Integration & Polish
**Goal**: All modules work together seamlessly with zero double-entry, cross-module relationships function correctly, and new users experience clear onboarding
**Depends on**: Phase 5 (all core modules must exist)
**Requirements**: None (integration phase addresses cross-cutting concerns)
**Success Criteria** (what must be TRUE):
  1. Goals can link to Activity Grid activities (relationship established)
  2. Checking an activity in Grid that is linked to a Goal updates goal state (zero double-entry)
  3. Empty states in each module provide clear guidance for first-time users
  4. Navigation between modules feels natural and preserves context
  5. Edge cases are handled gracefully (empty months, deleted sectors still linked to goals, etc.)
**Plans**: TBD

Plans:
- [ ] 06-01: TBD during phase planning

## Progress

**Execution Order:**
Phases execute in numeric order: 1 → 2 → 3 → 4 → 5 → 6

| Phase | Plans Complete | Status | Completed |
|-------|----------------|--------|-----------|
| 1. Foundation & Authentication | 0/0 | Not started | - |
| 2. Balance Wheel | 0/0 | Not started | - |
| 3. Identity Manifest | 0/0 | Not started | - |
| 4. Activity Grid | 0/0 | Not started | - |
| 5. Goals System | 0/0 | Not started | - |
| 6. Integration & Polish | 0/0 | Not started | - |
