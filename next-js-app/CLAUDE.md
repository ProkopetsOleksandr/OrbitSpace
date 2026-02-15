# OrbitSpace Frontend

## Tech Stack

- Next.js 16.0.7 with App Router and Turbopack
- React 19.2.1, TypeScript 5.9.3 (strict mode)
- Tailwind CSS 4, shadcn/ui (Radix UI)
- TanStack React Query v5, TanStack Table
- React Hook Form v7 + Zod v4
- openapi-fetch + openapi-typescript
- Custom JWT authentication with httpOnly cookies (BFF proxy pattern)
- pnpm as package manager

## Project Structure (FSD)

```
src/
├── app/          # App Router: layouts, providers, API proxy routes
├── pages/        # Page-level components
├── widgets/      # Composite UI blocks
├── features/     # User interactions and business logic
├── entities/     # Business entities (goal, todo-item)
└── shared/       # UI components, utilities, API client, types
```

## Commands

```bash
pnpm dev                  # Start dev server (Turbopack)
pnpm build                # Production build
pnpm lint                 # ESLint
pnpm generate-api-types   # Regenerate types from backend OpenAPI spec
```

## API Integration

- Generated types: `src/shared/api/v1.ts`
- Browser client: `getApiClient()` — proxies through `/api/proxy` routes
- Server client: `getServerApiClient()` — direct backend connection with JWT from httpOnly cookie
- Data Access Layer: `fetchFromApi()` and `getSession()` for Server Components (defense-in-depth auth)
- Backend must be running at `https://localhost:7284` for type generation

## Environment Variables

```bash
BACKEND_BASE_URL=https://localhost:7284  # Server-side only
NODE_TLS_REJECT_UNAUTHORIZED=0           # Dev only: accept self-signed certs
```

- Never commit `.env.local`
- `NEXT_PUBLIC_` prefix = exposed to browser (avoid for sensitive data)
- JWTs stored in httpOnly cookies, never exposed to client JavaScript

## Adding New Features

1. Decide which FSD layer it belongs to
2. Define Zod validation schema in `model/schemas.ts`
3. Add API route handler in `entities/[entity]/api/`
4. Add query/mutation in `entities/[entity]/model/`
5. Create UI component in `features/[feature]/ui/`
6. Compose in `widgets/` if needed
7. Use in `pages/` or `app/`

## Adding New Entities

1. Create `src/entities/[entity-name]/`
2. Add API route handlers in `api/`
3. Set up query keys in `model/query-keys.ts`
4. Add queries and mutations in `model/`
5. Create UI components in `ui/`
6. Export public API from `index.ts`

## Performance

- Use `dynamic()` for heavy components (lazy load dialogs/modals)
- Import icons from specific paths: `lucide-react`
- Use selective query invalidation
- Prefetch data on hover/focus when possible
