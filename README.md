# 🧅 OnionSetUp

A production-oriented **ASP.NET Core (.NET 9) Web API boilerplate** built on **Onion Architecture** — JWT authentication, role-based authorization, automatic soft-delete & auditing, self-service profile management and file uploads work out of the box, so every new project starts from decisions already made.

![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API-512BD4)
![EF Core](https://img.shields.io/badge/EF%20Core-9.0-6C3483)
![SQL Server](https://img.shields.io/badge/SQL%20Server-LocalDB-CC2927?logo=microsoftsqlserver&logoColor=white)
![JWT](https://img.shields.io/badge/Auth-JWT%20Bearer-F7B93E)
![License](https://img.shields.io/badge/License-MIT-2ea44f)

---

## 📑 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Tech Stack](#-tech-stack)
- [Architecture](#-architecture)
- [Solution Structure](#-solution-structure)
- [Key Design Decisions](#-key-design-decisions)
- [API Endpoints](#-api-endpoints)
- [Getting Started](#-getting-started)
- [Default Accounts & Policies](#-default-accounts--policies)
- [Roadmap](#-roadmap)
- [License](#-license)
- [Author](#-author)

---

## 📖 Overview

`OnionSetUp` is a reference backend template designed to be **cloned, run and extended**. The goal is not just a working REST API — it is a foundation where the expensive part of every new project (architecture decisions) has already been made once, deliberately:

- Dependencies flow strictly **inward** — the Domain layer depends on nothing.
- Cross-cutting concerns (soft delete, auditing) live in **one interceptor**, not scattered across services.
- Errors are **designed**: every domain exception carries its own HTTP status code.
- Entities protect their own invariants — no anemic models with public setters.

---

## ✨ Features

- 🔐 **JWT Bearer authentication** — `Sub` / `Email` / `Name` / `Jti` + role claims, HMAC-SHA256, strict expiry (`ClockSkew = Zero`)
- 🛡 **Lockout-aware login** — failed-attempt counter, automatic account lockout (5 attempts / 5 min)
- 👤 **Self-service profile module** — view profile, update name, change password, upload / reset avatar (identity taken from JWT claims, never from the route)
- 🧑‍💼 **Admin user management** — list users with roles (single-query join projection), change role, soft delete — locked behind `[Authorize(Roles = "Admin")]`
- 🗑 **Automatic soft delete + auditing** — a single `SaveChangesInterceptor` converts hard deletes to `IsDeleted = true` and stamps `CreatedAt` / `ModifiedAt`
- 🕳 **Global query filter** — soft-deleted rows are invisible to every LINQ query, no manual `Where` needed
- 🚨 **Self-describing exception hierarchy** — `BaseException(message, statusCode)`: `ConflictException` = 409, `NotExistException` = 404, `BadRequestException` = 400 — translated by one global middleware
- 📦 **Unified `Response<T>` envelope** — every endpoint returns the same JSON shape (`data`, `isSuccess`, `statusCode`, `errors`)
- 🖼 **Validated file storage** — extension whitelist, 5 MB limit, GUID file names under `wwwroot/uploads/`
- ⚙️ **Clone → run** — automatic migration + idempotent seeding (roles & admin) on startup
- 🧾 **Modern C# throughout** — record DTOs, primary constructors, `GlobalUsings`, nullable reference types

---

## 🧰 Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core Web API (.NET 9) |
| Data Access | Entity Framework Core 9 · SQL Server (LocalDB) |
| Identity & Auth | ASP.NET Core Identity (`IdentityUser<Guid>` / `IdentityRole<Guid>`) · JWT Bearer |
| Mapping | AutoMapper (construction-time mapping for records) |
| Validation | FluentValidation (registered; pipeline integration on roadmap) |
| API Docs | .NET 9 native OpenAPI (`/openapi/v1.json`, Development only) |

---

## 🏛 Architecture

Classic **Onion Architecture** across five projects — all dependencies point inward, and the solution folders themselves mirror the layers (`src/Core`, `src/Infrastructure`, `src/Presentation`):

```
WebAPI  →  Infrastructure / Persistence  →  Application  →  Domain
```

**Architectural principles applied:**

- Dependency Inversion — the Application layer talks to the database only through the `IAppDbContext` abstraction; it has zero knowledge of EF Core
- Separation of Concerns — each layer owns its own DI registration (`AddApplication()`, `AddInfrastructure()`, `AddPersistence()`)
- Rich domain model — invariants enforced inside entities, not in services
- Centralized cross-cutting concerns — auditing, soft delete and error handling each live in exactly one place

---

## 🧩 Solution Structure

```
OnionSetUp/
├── src/
│   ├── Core/
│   │   ├── OnionSetUp.Domain            # Entities, BaseEntity/AuditEntity, ISoftDeletable, BaseException family
│   │   └── OnionSetUp.Application       # DTOs (records), service abstractions, IAppDbContext,
│   │                                    # AutoMapper profile, validators, Response<T>, constants
│   ├── Infrastructure/
│   │   ├── OnionSetUp.Infrastructure    # IdentityService, UserService, ProfileService,
│   │   │                                # FileStorageService, JWT generation & bearer setup
│   │   └── OnionSetUp.Persistence       # AppDbContext, entity configurations, migrations,
│   │                                    # SaveChangesInterceptor, DataInitializer (seed)
│   └── Presentation/
│       └── OnionSetUp.WebAPI            # Controllers, GlobalExceptionMiddleware, Program.cs
└── OnionSetUp.sln
```

---

## 🎯 Key Design Decisions

**Soft delete + audit in one pass.** A single EF Core `SaveChangesInterceptor` walks the change tracker on every save: entities implementing `ISoftDeletable` have their `Deleted` state flipped to `Modified` with `IsDeleted = true` / `DeletedAt` set, and every `AuditEntity` gets `CreatedAt` / `ModifiedAt` stamped automatically. Combined with a global `HasQueryFilter`, no service ever checks deletion state manually.

**Exceptions carry their own status codes.** The Domain defines `BaseException(message, statusCode)`; concrete exceptions *are* their HTTP semantics (`ConflictException` → 409). The global middleware is only a translator — no HTTP concepts leak into services.

**DDD style — with honest boundaries.** No aggregates or bounded contexts are claimed at this scale. What is applied is the tactical side: private setters, guarded constructors, behavior methods (`UpdateFullName`, `UpdateImageUrl`) that validate invariants. A template sets the standard every future project inherits.

**No hand-rolled repository.** EF Core's `DbSet` already implements the repository / unit-of-work patterns. The `IAppDbContext` interface keeps the Application layer decoupled without ceremonial abstraction — knowing when *not* to apply a pattern is part of the design.

**Identity comes from the token, not the route.** All self-service profile endpoints resolve the user via `ClaimTypes.NameIdentifier` from the JWT. A route-supplied id would let a user change someone else's password; the token says who you are — you don't get to choose.

**Strict token expiry.** JWT validation uses `ClockSkew = TimeSpan.Zero` — no default 5-minute grace window; expired means expired.

---

## 🔌 API Endpoints

### Auth — `api/Auth` (anonymous)

| Method | Endpoint | Description |
|---|---|---|
| POST | `/Register` | Register a new user (default role `User`), returns JWT |
| POST | `/Login` | Lockout-aware login, returns JWT |
| POST | `/Logout` | Stateless logout (client discards token) |

### Profile — `api/Profile` (any authenticated user, id from JWT)

| Method | Endpoint | Description |
|---|---|---|
| GET | `/Me` | Current user's profile (name, email, avatar, roles) |
| PUT | `/` | Update full name |
| PATCH | `/Password` | Change password (current + new) |
| POST | `/` | Upload avatar (`multipart/form-data`, key: `file`) |
| DELETE | `/` | Reset avatar to default image |

### Users — `api/User` (requires `Admin` role)

| Method | Endpoint | Description |
|---|---|---|
| GET | `/` | List all users with roles (single-query projection) |
| GET | `/{id}` | Get a single user |
| PATCH | `/{id}` | Replace the user's role |
| DELETE | `/{id}` | Soft-delete the user |

Every response uses the same envelope:

```json
{
  "data": { },
  "isSuccess": true,
  "statusCode": 200,
  "errors": null
}
```

---

## 🚀 Getting Started

**Prerequisites:** .NET 9 SDK · SQL Server LocalDB (ships with Visual Studio)

```bash
git clone https://github.com/vusal016/Onion-SetUp.git
cd Onion-SetUp
dotnet run --project src/Presentation/OnionSetUp.WebAPI
```

That's it — on first run the app **applies migrations and seeds roles + admin automatically**. No manual `database update` required.

Configuration lives in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=OnionSetUpDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-at-least-32-characters-long",
    "Issuer": "OnionSetUp",
    "Audience": "OnionSetUpUsers",
    "ExpirationInMinutes": 60
  }
}
```

In Development the OpenAPI document is available at `/openapi/v1.json` (native .NET 9 generator).

---

## 🔑 Default Accounts & Policies

| Email | Password | Role |
|---|---|---|
| `admin@gmail.com` | `salam123` | Admin |

> ⚠️ Seeded for demo purposes — change these credentials before any real deployment.

**Password policy:** minimum 8 characters, at least one digit · unique email required
**Lockout policy:** 5 failed attempts → 5-minute lockout

---

## 📄 License

MIT — free to use as a starting point for your own projects.

## 👤 Author

**Vusal Mammadov** — .NET Backend Developer

[![GitHub](https://img.shields.io/badge/GitHub-vusal016-181717?logo=github)](https://github.com/vusal016)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-vusalmemmedov-0A66C2?logo=linkedin&logoColor=white)](https://linkedin.com/in/vusalmemmedov)
[![Email](https://img.shields.io/badge/Email-mvusal316%40gmail.com-EA4335?logo=gmail&logoColor=white)](mailto:mvusal316@gmail.com)
