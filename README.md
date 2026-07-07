# OnionSetUp

A clean, reusable **ASP.NET Core Web API boilerplate** built on **Onion Architecture** with .NET 9. Designed as a production-ready starting point for backend projects — authentication, soft delete, auditing, global error handling, and admin user management work out of the box.

## Tech Stack

- **.NET 9** / ASP.NET Core Web API
- **Entity Framework Core** + SQL Server
- **ASP.NET Core Identity** (custom `AppUser`, `IdentityRole<Guid>`, Guid keys)
- **JWT Bearer Authentication** (stateless)
- **FluentValidation** — request validation
- **AutoMapper** — entity ↔ DTO mapping
- **Scalar** — interactive API documentation

## Architecture

The solution follows the Onion Architecture dependency rule — all dependencies point inward toward the Domain:

```
src/
├── Core/
│   ├── OnionSetUp.Domain          # Entities, domain rules — depends on nothing
│   └── OnionSetUp.Application    # DTOs, interfaces, validation, business abstractions
├── Infrastructure/
│   ├── OnionSetUp.Persistence    # DbContext, migrations, interceptors, seeding
│   └── OnionSetUp.Infrastructure # Identity/JWT services, file storage
└── Presentation/
    └── OnionSetUp.WebAPI         # Controllers, middleware, startup extensions
```

Each layer registers its own services through a `DependencyInjection` extension method, keeping `Program.cs` minimal:

```csharp
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
```

## Features

### Authentication & Authorization
- Register / Login / Logout / Me endpoints backed by an `IIdentityService` abstraction
- JWT generation with role claims — works directly with `[Authorize(Roles = "...")]`
- **Account lockout** on repeated failed logins (`AccessFailedCount` + configurable lockout window)
- Role-based admin endpoints separated under `api/admin/*`

### Data Layer
- **`IAppDbContext` abstraction** — the Application layer talks to EF Core through an interface, no repository boilerplate
- **SaveChanges interceptor** handling two cross-cutting concerns in one pass:
  - **Soft Delete** — any entity implementing `ISoftDeletable` is never physically removed; `Delete` is converted to `IsDeleted = true` + `DeletedAt`, and global query filters hide deleted rows automatically
  - **Auditing** — `CreatedAt` / `ModifiedAt` are set automatically for audit entities
- **DDD-style entities** — private setters, invariants enforced through constructor and behavior methods (e.g. `AppUser.UpdateFullName`)
- **Auto migration + data seeding** on startup (`DataInitializer`): applies pending migrations, seeds roles (`Admin`, `Student`, `Teacher`) and a default admin user

### API Layer
- **Unified response envelope** — every endpoint returns `Response<T>` with `data`, `isSuccess`, `statusCode`, and `errors`
- **Global exception middleware** — custom exceptions derived from `BaseException` carry their own HTTP status codes; unexpected errors return a consistent 500 payload
- **Centralized constants** — `ErrorMessages` and `FilePaths` keep magic strings out of business code
- **File storage service** — image upload with extension whitelist, size limit, Guid-based file names, and folder separation under `wwwroot/uploads`
- Startup extension creates required storage folders automatically

## Getting Started

### Prerequisites
- .NET 9 SDK
- SQL Server (LocalDB or full instance)

### Setup

1. Clone the repository:
```bash
git clone https://github.com/<your-username>/OnionSetUp.git
cd OnionSetUp
```

2. Configure `appsettings.json` in `OnionSetUp.WebAPI`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=OnionSetUpDb;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-at-least-32-characters-long",
    "Issuer": "OnionSetUp",
    "Audience": "OnionSetUpUsers",
    "ExpirationInMinutes": 60
  }
}
```

3. Run the API:
```bash
dotnet run --project src/Presentation/OnionSetUp.WebAPI
```

Migrations and seed data are applied automatically on first run — no manual `database update` needed.

API documentation is available at `http://localhost:5000/scalar/v1` in development.

### Default Admin Account

| Email | Password | Role |
|---|---|---|
| `admin@gmail.com` | `Admin123!` | Admin |

> Change these credentials before any real deployment.

## API Endpoints

### Auth — `api/auth`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/register` | — | Register a new user, returns JWT |
| POST | `/login` | — | Authenticate, returns JWT |
| POST | `/logout` | ✅ | Logout (client discards token) |
| GET | `/me` | ✅ | Current user info from token claims |

### Admin — `api/admin/users` (requires `Admin` role)

| Method | Endpoint | Description |
|---|---|---|
| GET | `/` | List all users with roles |
| GET | `/{id}` | Get a single user |
| PATCH | `/{id}/role` | Replace the user's role |
| DELETE | `/{id}` | Soft-delete the user |

All responses share the same envelope:

```json
{
  "data": { },
  "isSuccess": true,
  "statusCode": 200,
  "errors": []
}
```

## Design Decisions

- **Task-based endpoints for users** — user fields follow different rules (roles are admin-controlled, profile is self-service, passwords need hashing flows), so instead of one generic `Update`, each intent gets its own endpoint (`/role`, `/me`). Classic full-DTO CRUD remains the right choice for uniform entities.
- **No repository layer** — EF Core's `DbSet` already implements the repository/unit-of-work patterns; `IAppDbContext` keeps the Application layer decoupled without an extra abstraction.
- **Stateless logout** — JWT cannot be revoked server-side without giving up statelessness; logout is a client-side token discard. Refresh-token rotation is the planned production path (see Roadmap).
- **Interceptor over overridden `SaveChanges`** — cross-cutting persistence concerns (audit, soft delete) live in one `SaveChangesInterceptor`, keeping the DbContext clean.

## Roadmap

- [ ] Refresh token pattern (short-lived access + revocable refresh tokens)
- [ ] Result pattern for expected business failures
- [ ] Pagination, filtering, and sorting for list endpoints
- [ ] CQRS + MediatR with validation pipeline behavior
- [ ] Unit and integration tests (xUnit, Testcontainers)
- [ ] Docker support

## License

MIT — free to use as a starting point for your own projects.
