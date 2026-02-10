---
paths:
  - "next-js-app/**"
---

# Component & Styling Patterns

## General

- Prefer Server Components; use `"use client"` only when necessary
- Composition over complex prop APIs
- Use `cn()` (clsx + tailwind-merge) for conditional class names

## shadcn/ui & Radix

- Use shadcn/ui components (Radix UI-based)
- Use `class-variance-authority (cva)` for component variants
- Lucide React for icons — import from specific paths

## Form Pattern

1. Define Zod schema in `model/schemas.ts`
2. Create form component in `ui/` using React Hook Form + zodResolver
3. Wrap in dialog/sheet in `ui/[feature]-dialog.tsx`

```
features/[ns]/[feature]/
├── model/schemas.ts         # Zod schema + inferred type
├── ui/[feature]-form.tsx    # Form with useForm + mutation
└── ui/[feature]-dialog.tsx  # Dialog wrapping the form
```

- Always use Zod schemas for validation
- Define schemas in feature/entity model layer
- Use TypeScript inference from Zod: `z.infer<typeof schema>`

## Tailwind CSS

- Utility classes directly in JSX
- Group: layout → spacing → colors → typography
- Use semantic color tokens: `bg-background`, `text-foreground`
- Use CSS variables from `app/globals.css` (shadcn/ui theming)
- Mobile-first with Tailwind breakpoints: `sm:`, `md:`, `lg:`

## Authentication

- All routes protected by Clerk middleware
- Server: `const { userId } = await auth()`
- Client: `useAuth()`, `useUser()` hooks
- Never expose backend URL to client
