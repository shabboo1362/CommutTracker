

# CommuteTracker 🚗🚶

## Overview
CommuteTracker is an ASP.NET Core Web API project designed for tracking daily commutes. It allows users to record trips, manage locations, calculate traveled distances, and analyze commute information.

The application is built with a clean and maintainable architecture using modern .NET technologies.

## Features
- User registration and login
- JWT authentication
- Secure password hashing
- Trip management
- Location point tracking
- Distance calculation using GPS coordinates
- Average speed calculation
- Entity Framework Core with SQL Database
- Swagger API documentation and testing
- Clean separation of responsibilities using services and dependency injection

## Technologies
- C#
- .NET 8 / ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- LINQ
- JWT Authentication
- Swagger / OpenAPI
- Dependency Injection
- Git & GitHub

## Architecture
The project follows a layered architecture:

```text
CommuteTracker/
├── CommuteTracker.Api
│   └── Controllers
│       └── Handles HTTP requests and responses
├── CommuteTracker.Core
│   ├── Entities
│   ├── Interfaces
│   └── Business contracts
└── CommuteTracker.Infrastructure
    ├── Database
    ├── Services
    └── External implementations
```

## Authentication Flow

```text
User
  ↓
Register / Login
  ↓
AuthController
  ↓
IAuthService
  ↓
AuthService
  ↓
Database
  ↓
JWT Token
```

## Trip Flow

```text
User creates a trip
  ↓
TripController
  ↓
TripService
  ↓
Database
  ↓
Distance & speed calculation
```

## API Documentation
Swagger UI is available for testing API endpoints:

- `/swagger`

## Future Improvements
Planned features include:
- Refresh token support
- Role-based authorization
- Better GPS route visualization
- Unit testing
- Repository pattern
- Docker support
- Cloud deployment

## Developer
Full-Stack Developer

Technologies:
- .NET Core
- C#
- Angular
- SQL
- REST API