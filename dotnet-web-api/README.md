# OrbitSpace Web API

[![.NET 10](https://img.shields.io/badge/.NET-10-blue.svg)](https://dotnet.microsoft.com/)
[![C# 14](https://img.shields.io/badge/C%23-14-green.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Description

OrbitSpace Web API is a modern REST API built on .NET 10 that serves as the backend for a task management and planning web application. The API provides functionality for authentication, user management, and task operations.

## Features

- 🔐 **JWT Authentication** — secure authentication using JSON Web Tokens
- 📊 **OpenAPI/Swagger** — automatic API documentation generation via Scalar UI
- 🏗️ **Clean Architecture** — clear separation of layers (Presentation, Application, Infrastructure)
- 🗃️ **MongoDB** — modern NoSQL database for data storage
- ⚡ **Mapster** — fast and efficient object mapping
- 🛡️ **Global Exception Handling** — centralized error handling
- 📝 **Problem Details** — standardized error responses (RFC 7807)

## Technology Stack

- **Framework**: .NET 10
- **Language**: C# 14.0
- **Database**: MongoDB
- **Authentication**: JWT Bearer
- **API Documentation**: OpenAPI 3.0 + Scalar
- **Object Mapping**: Mapster
- **Architecture Pattern**: Clean Architecture

## **API Access**

- **API**: `https://localhost:5001`
- **Swagger UI**: `https://localhost:5001/` (Development only)

## API Documentation

The API documentation is automatically generated using OpenAPI and is available through Scalar UI in development mode:

- **Base URL**: `https://localhost:5001`
- **Documentation**: `https://localhost:5001/` (Development only)
- **OpenAPI JSON**: `https://localhost:5001/openapi/v1.json`
