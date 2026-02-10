---
paths:
  - "next-js-app/**"
---

# FSD Architecture

## Layers

```
src/
├── app/          # Next.js App Router (layouts, providers, global styles)
├── pages/        # Page-level components (assemble widgets)
├── widgets/      # Complex UI compositions combining features
├── features/     # User-facing features with business logic
├── entities/     # Business domain entities (goal, todo-item)
├── shared/       # Shared utilities, UI components, types (no business logic)
```

## Import Rules

- Lower layers CANNOT import from upper layers
- Direction: shared → entities → features → widgets → pages → app
- Each layer can only import from layers below it

## Layer Responsibilities

- **Entities**: domain models, API queries/mutations, query keys, minimal UI components
- **Features**: specific user actions (create, update, filter, delete)
- **Widgets**: compose multiple features/entities into complex UI blocks
- **Pages**: assemble widgets into complete page views
- **Shared**: truly cross-cutting concerns only

## Code Organization

### Entity Structure

```
entities/[entity-name]/
├── api/              # API route handlers (server-side)
├── model/            # Queries, mutations, query keys, types
├── ui/               # Minimal entity UI components
└── index.ts          # Public API
```

### Feature Structure

```
features/[namespace]/[feature-name]/
├── ui/               # UI components
├── model/            # Business logic, schemas
└── index.ts          # Public API
```

### Widget Structure

```
widgets/[widget-name]/
├── ui/               # Widget components
├── model/            # Widget-specific logic (if needed)
└── index.ts          # Public API
```
