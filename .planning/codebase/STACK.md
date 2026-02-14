# Technology Stack

**Analysis Date:** 2026-02-14

## Languages

**Primary:**
- TypeScript 5.9.3 - Frontend (React components, utilities, types)
- C# 14 - Backend (.NET 10 REST API, Clean Architecture)

**Secondary:**
- JavaScript (configuration files, build setup)
- JSON (OpenAPI schemas, configuration)

## Runtime

**Environment:**
- Node.js (version specified via package manager, exact version in .npmrc or .nvmrc if present)
- .NET 10 Runtime

**Package Manager:**
- pnpm - JavaScript/Node.js package manager (frontend only)
- .NET Package Manager (NuGet) - Backend dependencies

## Frameworks

**Core:**
- Next.js 16.0.7 - React framework with App Router and Turbopack bundler
- React 19.2.1 - UI library with Server and Client Components
- ASP.NET Core 10.0 - Web API framework (REST endpoints)
- Entity Framework Core 10.0.0 - ORM for database access

**UI & Styling:**
- Tailwind CSS 4 - Utility-first CSS framework
- Radix UI (via shadcn/ui) - Accessible component library
- Lucide React 0.544.0 - Icon library
- class-variance-authority 0.7.1 - Component variant management
- clsx 2.1.1 + tailwind-merge 3.3.1 - Conditional CSS class utilities

**Data & Forms:**
- TanStack React Query 5.90.5 - Server state management and caching
- TanStack React Table 8.21.3 - Headless table component library
- React Hook Form 7.67.0 - Form state management
- Zod 4.1.13 - Runtime schema validation and type inference

**API Integration:**
- openapi-fetch 0.15.0 - Typed fetch client from OpenAPI schemas (browser)
- openapi-typescript 7.10.1 - Code generator for OpenAPI types (dev tool)

**Object Mapping:**
- Mapster 7.4.0 - DTO mapping library (backend)

**Authentication & Security:**
- Clerk - Authentication provider (frontend sign-in/sign-up, JWT generation)
- JWT Bearer - Token-based authentication (backend validation)
- BCrypt.Net-Next 4.0.3 - Password hashing (backend)
- Microsoft.IdentityModel.JsonWebTokens 8.14.0 - JWT token handling (backend)

**API Documentation:**
- Microsoft.AspNetCore.OpenApi 10.0.0 - OpenAPI schema generation
- Scalar.AspNetCore 2.10.3 - Interactive API documentation UI (Swagger alternative)

**Testing:**
- Not explicitly configured in package.json (Jest, Vitest, or xUnit likely added separately)

**Build/Dev Tools:**
- Turbopack - Next.js bundler (development and build)
- ESLint 9 - JavaScript/TypeScript linting
- ESLint config for Next.js - Next.js specific linting rules
- TypeScript - Static type checking
- Tailwind CSS PostCSS - CSS processing

## Key Dependencies

**Critical (Frontend):**
- `@clerk/nextjs` 6.34.0 - Clerk authentication middleware and hooks
- `@tanstack/react-query` 5.90.5 - Async state and server-state management (crucial for data fetching)
- `openapi-fetch` 0.15.0 - Type-safe API client generation from OpenAPI specs

**Critical (Backend):**
- `Microsoft.AspNetCore.Authentication.JwtBearer` 10.0.0 - JWT authentication middleware
- `Microsoft.EntityFrameworkCore` 10.0.0 - Data access layer
- `Npgsql.EntityFrameworkCore.PostgreSQL` 10.0.0 - PostgreSQL database provider
- `BCrypt.Net-Next` 4.0.3 - Password hashing
- `Mapster` 7.4.0 - DTO mapping

**Infrastructure (Backend):**
- `EFCore.NamingConventions` 10.0.0 - Convert C# PascalCase to snake_case (PostgreSQL convention)
- `Microsoft.Extensions.Options` 10.0.0 - Configuration management
- `Microsoft.Extensions.DependencyInjection.Abstractions` 10.0.0 - Dependency injection

**UI Components:**
- `@radix-ui/react-*` (multiple) - Unstyled, accessible component primitives
- `react-day-picker` 9.11.3 - Calendar component
- `date-fns` 4.1.0 - Date utility library

## Configuration

**Environment:**
- `NEXT_PUBLIC_BACKEND_BASE_URL` - Backend API URL (exposed to browser)
- `NEXT_PUBLIC_CLERK_PUBLISHABLE_KEY` - Clerk public key (exposed to browser, required for frontend auth)
- `CLERK_SECRET_KEY` - Clerk secret key (server-only, never exposed to client)
- `NEXT_PUBLIC_` prefix indicates variables exposed to browser JavaScript
- Secrets stored in server-only environment (.env.local, server actions, Route Handlers)

**Build (Frontend):**
- `next.config.ts` - Next.js configuration (minimal, empty template)
- `tsconfig.json` - TypeScript compiler options with path aliases (`@/*` → `./src/*`, layer-specific aliases)
- `package.json` - Scripts: `dev`, `build`, `start`, `lint`, `generate-api-types`

**Build (Backend):**
- `.csproj` files - Project file structure (OrbitSpace.WebApi, OrbitSpace.Application, OrbitSpace.Infrastructure, OrbitSpace.Domain)
- `appsettings.json` - Configuration (logging, connection string, JWT settings, CORS)
- Solution file: `dotnet-web-api.slnx`

## Platform Requirements

**Development:**
- Node.js (LTS recommended, check for .nvmrc if present)
- pnpm package manager
- .NET 10 SDK
- Visual Studio 2022+ or VS Code with C# Dev Kit
- PostgreSQL 14+ (database)

**Production:**
- Node.js runtime (for Next.js deployed to Vercel, AWS, etc.)
- .NET 10 Runtime
- PostgreSQL 14+ database
- Clerk account and API credentials
- HTTPS support (JWT Bearer requires secure transport)
- CORS configuration for frontend origin

**Database Migration:**
- Recently migrated from MongoDB to PostgreSQL (see commit 4cd4157)
- Uses Entity Framework Core with Npgsql provider
- Entity configurations defined in `OrbitSpace.Infrastructure/Persistence/Configurations/`

---

*Stack analysis: 2026-02-14*
