# OrbitSpace

## Project Overview

OrbitSpace is a personal "Life Operating System" (Life OS) built around the Identity-First philosophy. It is NOT a task manager — it's a platform for conscious personality design through daily confirmed actions.

**Core idea**: Instead of "what do I need to do?", the system asks "who do I want to be?". Every goal, habit, and activity is a vote confirming the user's chosen identity.

**Problems it solves**:
1. **Data fragmentation** — goals, habits, tasks scattered across apps with no unified picture
2. **Erased memory effect** — without visual history, completed work is forgotten, killing motivation
3. **No foundation** — traditional systems focus on results (Goals), ignoring personality (Identity)

## Platform Strategy: Station + Probe

- **Orbital Station (Desktop/Web)** — strategic planning, deep reflection, goal architecture, analytics. Designed for focused desk sessions.
- **Probe (Mobile/PWA)** — instant fact capture, activity check-ins, metric logging. Any action under 30 seconds. (Future)

## System Levels

| Level | Name | Modules |
|---|---|---|
| Strategic (Orbit) | Long-term identity | Manifest, Balance Wheel, Timeline |
| Tactical (Engine) | Months/Quarters | Goals, Habits, Knowledge Base |
| Operational (Flight Log) | Day/Week | Activity Grid, Daily Planner, Reflection |

## Domain Model Summary

See @.claude/rules/domain.md for full module descriptions and entity relationships.
See @.claude/rules/domain-mechanics.md for system mechanics, task lifecycle, and user flows.

## Monorepo Structure

- `dotnet-web-api/` — .NET 10 REST API (Clean Architecture, C# 14, MongoDB)
- `next-js-app/` — Next.js 16 frontend (React 19, TypeScript, FSD architecture)

## How It Works

- Frontend communicates with backend exclusively through Next.js API proxy routes (never directly from browser)
- Backend exposes OpenAPI spec; frontend generates TypeScript types from it
- Authentication handled by Clerk (frontend) with JWT passed to backend

## Commit Conventions

We use [Conventional Commits](https://www.conventionalcommits.org/):

- Format: `<type> (<scope>): <description>`
- Scopes: `api` (backend), `frontend` (frontend)
- Types: `feat`, `fix`, `docs`, `refactor`, `perf`, `test`, `chore`, `ci`, `build`
- Examples:
  - `feat (api): add user registration endpoint`
  - `fix (frontend): resolve login button issue`

## General Guidelines

- Always read existing code before modifying it
- Prefer editing existing files over creating new ones
- When adding new features, always look for similar existing patterns first
- Do not introduce security vulnerabilities (OWASP top 10)
- Keep changes focused and minimal — no over-engineering
- Do not create new patterns — follow existing conventions
