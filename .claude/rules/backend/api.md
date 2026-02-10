---
paths:
  - "dotnet-web-api/**"
---

# Backend API Rules

## Architecture

- Follow Clean Architecture: Domain → Application → Infrastructure → WebApi
- Domain layer has no external dependencies
- Application layer contains use cases and interfaces
- Infrastructure layer implements external concerns (MongoDB, auth)

## Tech Stack

- .NET 10, C# 14
- MongoDB for data storage
- JWT Bearer authentication
- Mapster for object mapping
- OpenAPI 3.0 + Scalar for API docs

## Standards

- Use Problem Details (RFC 7807) for error responses
- All endpoints must have OpenAPI documentation
- Use global exception handling — do not swallow exceptions silently
