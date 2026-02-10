---
paths:
  - "next-js-app/**"
---

# TypeScript & Naming Conventions

## Strict Mode

- No `any` types (use `unknown` if truly needed)
- Always define return types for exported functions
- Prefer `type` over `interface` for object shapes
- Use `const` assertions for literal types

## File Naming

```
Components:     PascalCase.tsx     (GoalCard.tsx)
Utilities:      kebab-case.ts      (query-client.ts)
Hooks:          use-something.ts   (use-create-goal.ts)
Types:          types.ts
Schemas:        schemas.ts
Constants:      constants.ts
API handlers:   route.ts           (Next.js convention)
```

## Naming Conventions

- **Server handlers**: suffix with `-handler` (`getGoalsHandler`), place in `entities/[entity]/api/`
- **Components**: PascalCase (`CreateGoalDialog`, `GoalsTable`)
- **Hooks**: prefix with `use` (`useCreateGoal`, `useGoalsList`), place in `model/`
- **Schemas**: suffix with `Schema` (`createGoalSchema`), place in `model/schemas.ts`

## Import Order

```typescript
// 1. External dependencies
import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';

// 2. Internal absolute imports (by FSD layer, bottom-up)
import { goalQueryKeys } from '@/entities/goal';
import { CreateGoalDialog } from '@/features/goals/create-goal';
import { Button } from '@/shared/ui/button';

// 3. Relative imports
import { GoalFilters } from './types';

// 4. Type-only imports
import type { Goal } from '@/shared/api/v1';
```

## Type Organization

- `shared/types/` — truly shared types (`UUID`, `ISODateString`)
- `entities/[entity]/model/types.ts` — entity-specific types
- `features/[feature]/model/types.ts` — feature-specific types
- API types — always from `@/shared/api/v1` (generated), never manual
