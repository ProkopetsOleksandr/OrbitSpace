# OrbitSpace Web API

## Tech Stack

- .NET 10, C# 14
- MongoDB for data storage
- JWT Bearer authentication
- Mapster for object mapping
- OpenAPI 3.0 + Scalar for API docs

## Project Structure (Clean Architecture)

```
dotnet-web-api/
├── OrbitSpace.Domain/           # Entities, interfaces (no external deps)
├── OrbitSpace.Application/      # Use cases, DTOs, service interfaces
├── OrbitSpace.Infrastructure/   # MongoDB repos, JWT, external services
├── OrbitSpace.WebApi/           # Endpoints, middleware, OpenAPI config
└── dotnet-web-api.slnx         # Solution file
```

## Commands

```bash
dotnet run --project OrbitSpace.WebApi    # Run the API
dotnet build                              # Build
dotnet test                               # Run tests
```

## API

- Base URL: `https://localhost:5001`
- OpenAPI spec: `https://localhost:5001/openapi/v1.json`
- Scalar docs UI: `https://localhost:5001/` (dev only)

## Frontend Integration

- OpenAPI spec is consumed by frontend to generate TypeScript types
- Frontend proxies all requests through Next.js API routes
- Authentication: Clerk JWT in Authorization header
