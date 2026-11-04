<div align="center">

# Orion API

**A production-ready .NET 10 Web API template with Clean Architecture, CQRS, JWT auth and real-database tests — pick SQL Server, PostgreSQL or MongoDB and start coding your domain in minutes.**

[![Build and Test](https://github.com/vanderlan/Orion-Api/actions/workflows/build.yml/badge.svg)](https://github.com/vanderlan/Orion-Api/actions/workflows/build.yml)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=vanderlan_Orion-Api&metric=coverage)](https://sonarcloud.io/summary/overall?id=vanderlan_Orion-Api)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=vanderlan_Orion-Api&metric=alert_status)](https://sonarcloud.io/summary/overall?id=vanderlan_Orion-Api)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=vanderlan_Orion-Api&metric=sqale_index)](https://sonarcloud.io/summary/overall?id=vanderlan_Orion-Api)
[![GitHub release](https://img.shields.io/github/release/vanderlan/Orion-Api.svg)](https://github.com/vanderlan/Orion-Api/releases)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)

[Quick start](#-quick-start) •
[Features](#-whats-inside) •
[Architecture](#-architecture) •
[Add a feature](#-adding-a-new-feature-in-5-steps) •
[Testing](#-testing) •
[Contributing](#-contributing) •

</div>

---

## Why Orion?

Every new API starts the same way: authentication, validation, error handling, logging, versioning, docs, Docker, CI… and only *then* the actual domain. Orion is that first week of work, done once, kept up to date with .NET and Microsoft's recommendations, and shipped as a `dotnet new` template.

- **Start from a clean architecture**, not from an empty `Program.cs`.
- **Choose your database at creation time** — SQL Server, PostgreSQL (EF Core) or MongoDB — the application layer does not change.
- **Tests that actually hit a database** using Testcontainers, locally and in CI.
- **Batteries included, opinions explicit**: every pattern is documented in [`AGENTS.md`](AGENTS.md) so humans *and* AI coding agents keep the structure consistent.

## 🚀 Quick start

### 1. Create your project from the template

```bash
dotnet new install .        # from the cloned repository root

dotnet new orion-api --companyName "MyCompany" --output "MyProjectName" --systemDatabase "PostgreSql"
dotnet new orion-api --companyName "MyCompany" --output "MyProjectName" --systemDatabase "SqlServer"
dotnet new orion-api --companyName "MyCompany" --output "MyProjectName" --systemDatabase "MongoDb"
```

### 2. Run the infrastructure and the API

```bash
docker compose up -d            # database (+ seed script for SQL Server)
cd src/Company.Orion.Api
dotnet run                      # http://localhost:5100/scalar/v1
```

Or run everything in containers: `docker compose up --build` → API on `http://localhost:5010/scalar/v1`, telemetry on `http://localhost:18888` (Aspire dashboard).

### 3. Make your first request

Open **`/scalar/v1`** (API reference) or **`/openapi/v1.json`** (raw document): every endpoint comes with ready-to-use **request and response examples** — pick an example, press *Send* and you are done. The *Authorize* action is only required where the padlock shows.

```bash
# 1. Authenticate (the default admin user is created on startup)
curl -X POST http://localhost:5100/Auth/Login \
  -H "Content-Type: application/json" \
  -d '{ "email": "adm@orion.com", "password": "123" }'

# 2. Use the token
curl http://localhost:5100/Users?page=1&quantity=10 \
  -H "Authorization: Bearer <token>" \
  -H "x-api-version: 1.0"
```

### Migrations *(SQL Server and PostgreSQL only)*

```bash
# in the src/ folder
dotnet ef migrations add MigrationName -p Company.Orion.Infra.Data -s Company.Orion.Api
dotnet ef database update -p Company.Orion.Infra.Data -s Company.Orion.Api --verbose
```

Pending migrations are also applied automatically at startup.

## ✨ What's inside

| Area | What you get |
|---|---|
| **Architecture** | Clean Architecture (Domain → Application → Infra → Api), CQRS with MediatR, Notification pattern, Repository + Unit of Work |
| **Persistence** | EF Core (SQL Server / PostgreSQL) with **pooled `DbContext` from DI**, Fluent API mappings, migrations, `CreatedAt`/`LastUpdated`/`PublicId` stamped by a `SaveChangesInterceptor` (`TimeProvider`-based), pagination **or** MongoDB driver with `BsonClassMap`, buffered writes committed on `CommitAsync`, indexes and seed data created on startup. Scoped Unit of Work, `CancellationToken` propagated end-to-end |
| **Security** | JWT Bearer (`JsonWebTokenHandler`, HS512) behind an `ITokenService`, **PBKDF2-SHA512 salted password hashing** (`IPasswordHasher`, transparent upgrade of legacy hashes), refresh tokens with **expiration and atomic rotation** (re-login revokes old tokens), role-based authorization (`[AuthorizeFor(Roles.Admin)]`), `ICurrentUser`, **rate limiting** on the auth endpoints, CORS policy from configuration, JWT options **validated at startup** |
| **API surface** | Controllers with API versioning (`x-api-version` header), FluentValidation, **one error contract** for everything — RFC 9457 `ProblemDetails` (`application/problem+json`) via `IExceptionHandler`, CORS |
| **Documentation** | Built-in **`Microsoft.AspNetCore.OpenApi`** (OpenAPI 3.1, XML comments) + **Scalar** API reference, one document per API version, Bearer security wired to `[Authorize]` endpoints, and **pluggable request/response example providers** (`IExampleProvider<T>`, `IMultipleExamplesProvider<T>`, `IResponseExampleProvider<T>`) |
| **Observability** | **OpenTelemetry** traces, metrics and logs (OTLP export switched on by `OTEL_EXPORTER_OTLP_ENDPOINT`; `docker compose up` ships an **Aspire dashboard** at `http://localhost:18888`), Serilog as the logging pipeline (request logging, Correlation Id + TraceId in every line), Kubernetes-style probes `/health/live` and `/health/ready` |
| **Globalization** | Resource-based messages (`en-US`, `pt-BR`) resolved with `Accept-Language` |
| **Testing** | Unit, integration and API tests with xUnit, Moq and Bogus; real database in tests with Testcontainers |
| **DevOps** | Multi-stage Dockerfile on a **chiseled, non-root** runtime image (port 8080, cached restore layer), docker-compose per database + Aspire dashboard, GitHub Actions (build, test, SonarCloud, Docker Hub), Dependabot, central package management |

## 🏛 Architecture

```mermaid
flowchart LR
    Client((Client)) -->|HTTP / JSON| Api

    subgraph Api["Company.Orion.Api"]
        direction TB
        Controllers --> Filters["Validation filter\nProblemDetails exception handler"]
        OpenApi["OpenAPI + Scalar + Examples"]
    end

    subgraph Application["Company.Orion.Application.Core"]
        UseCases["UseCases\nCommands · Queries · Notifications"]
    end

    subgraph Domain["Company.Orion.Domain.Core"]
        Entities["Entities · Repositories (interfaces)\nExceptions · Filters · Value Objects"]
    end

    subgraph Infra["Company.Orion.Infra.Data"]
        Ef["EF Core\nSQL Server / PostgreSQL"]
        Mongo["MongoDB Driver"]
    end

    Crosscutting["Crosscutting\nIoc · Resources"]

    Api -->|MediatR| Application --> Domain
    Infra -.implements.-> Domain
    Crosscutting -.wires.-> Infra
```

**Dependency rule:** source code dependencies only point inwards. `Domain` knows nothing about the database, `Application` knows nothing about HTTP, and `Infra` is swappable (that is how the MongoDB flavour works: same `IUnitOfWork`/repository contracts, different implementation).

### Request lifecycle

```
HTTP request
 └─ ExceptionHandler + StatusCodePages  (domain exceptions / empty 401-404 → RFC 9457 ProblemDetails)
     └─ Authentication / Authorization   (JWT Bearer, roles)
         └─ AutoValidationActionFilter   (FluentValidation → 400 ValidationProblemDetails)
             └─ Controller               (thin: builds the request and sends it to MediatR)
                 └─ RequestHandler       (use case: repositories + unit of work + notifications, CancellationToken flows down)
                     └─ IUnitOfWork      (scoped: one EF Core DataContext / MongoDataContext per request)
```

### Solution layout

```
src/
├── Company.Orion.Api                  HTTP layer: controllers, filters, exception handling, OpenAPI, configuration
├── Company.Orion.Application.Core     Use cases (MediatR requests, handlers, validators, notifications)
├── Company.Orion.Domain.Core          Entities, repository contracts, exceptions, filters, value objects
├── Company.Orion.Infra.Data           EF Core (Context, Mapping, Migrations, Repository, UnitOfWork) or Mongo/
├── Company.Orion.Crosscutting.Ioc     Dependency injection registration of the data layer
└── Company.Orion.Crosscutting.Resources  Localized messages (resx) and message keys
tests/
└── Company.Orion.Test                 Unit/, Integration/ (fixtures), Api/ (end-to-end), Shared/ (fakers)
```

## 🧩 Adding a new feature in 5 steps

Suppose you want a `Products` resource:

1. **Domain** – add `Entities/Product.cs`, `Repositories/IProductRepository.cs`, expose it on `IUnitOfWork`.
2. **Infra** – add the mapping (`Mapping/ProductMapping.cs` or the Mongo `BsonClassMap`), the repository implementation, register it on the `UnitOfWork`, create a migration.
3. **Application** – create `UseCases/Products/Commands/Create/` with `ProductCreateRequest`, `ProductCreateRequestHandler`, `ProductCreateRequestValidator` and `ProductCreateResponse`.
4. **Api** – add `Controllers/V1/ProductsController.cs` (inherit `ApiController`, send the request through `Mediator`, document every status code with `[ProducesResponseType]` + XML `<response>` comments) and an `OpenApi/Examples/Products/ProductCreateRequestExample.cs` implementing `IExampleProvider<ProductCreateRequest>` so the docs show a real payload.
5. **Tests** – a handler unit test, a controller unit test and an API test using `IntegrationTestsBootstrapper`; add fakers to `Shared/Faker`.

The full set of conventions (naming, folders, what goes where, what to avoid) lives in [`AGENTS.md`](AGENTS.md).

### Error contract

Every error — domain exceptions, validation, model binding or framework 401/403/404 — is an RFC 9457 `ProblemDetails` (`application/problem+json`):

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Not Found",
  "status": 404,
  "detail": "Resource Not Found with id: 3e7a9d21-0f4b-4b6c-8c2d-7a1e5f9b3c44",
  "instance": "GET /Users/3e7a9d21-0f4b-4b6c-8c2d-7a1e5f9b3c44",
  "traceId": "00-0af7651916cd43dd8448eb211c80319c-b7ad6b7169203331-01"
}
```

| Exception (Domain) | Status |
|---|---|
| `NotFoundException` | 404 |
| `ConflictException` | 409 |
| `UnauthorizedUserException` | 401 |
| `BusinessException` | 400 |
| anything else | 500 (generic message, details only in the logs) |

Validation failures (FluentValidation) add the `errors` dictionary (`ValidationProblemDetails`).

### Authentication flow

1. `POST /Auth/Login` with e-mail + password → `{ token, refreshToken, expiration }`. Passwords are verified with PBKDF2-SHA512 (salted, 210k iterations); hashes stored in an older format are upgraded on the fly.
2. Send `Authorization: Bearer {token}` (15 min by default).
3. When it expires, `POST /Auth/RefreshToken` with the expired `token` + `refreshToken`. The expired token's **signature is still validated**, the refresh token must exist, belong to the same user and be younger than `RefreshTokenExpirationDays`; it is then **rotated** (old one revoked, new one issued) atomically.
4. Logging in again revokes any previous refresh token of the user.

### Documenting requests with examples

Examples are discovered automatically: implement `IExampleProvider<T>` (one example), `IMultipleExamplesProvider<T>` (a dropdown of named scenarios) or `IResponseExampleProvider<T>` (one example per HTTP status code) anywhere in the API project and they are attached to every operation that receives or returns `T`.

```csharp
public sealed class ProductCreateRequestExample : IExampleProvider<ProductCreateRequest>
{
    public ProductCreateRequest GetExample() => new() { Name = "Telescope", Price = 1299.90m };
}
```

## 🍃 MongoDB flavour

When the project is created with `--systemDatabase "MongoDb"`, the EF Core infrastructure (DbContext, Fluent API mappings and migrations) is replaced by the implementation in `Company.Orion.Infra.Data/Mongo`. The domain contracts (`IUnitOfWork`, `IUserRepository`, `IRefreshTokenRepository`) are the same, so the application layer does not change.

**Configuration**

```json
"ConnectionStrings": {
  "MongoDb": "mongodb://mongo:CHANGE_ME_Passw0rd!@localhost:27017/?authSource=admin"
},
"MongoDb": {
  "DatabaseName": "OrionDev"
}
```

The database name can also be informed on the connection string, e.g. `mongodb://localhost:27017/OrionDev`.

**How it works**

1. The `MongoClient` is registered as a singleton (connection pool) and each unit of work owns its own context;
2. Writes are buffered by the `MongoDataContext` and only applied on `CommitAsync`, like the EF Core implementation;
3. `PublicId` is mapped as the document `_id`, and `CreatedAt`/`LastUpdated` are filled on commit;
4. Collections use the entity name (`User`, `RefreshToken`);
5. There are no migrations: the indexes and the default admin user are created on startup (idempotent);
6. Integration tests run against a throwaway MongoDB container, each fixture using its own database;
7. The MongoDB data layer is unit tested with a mocked driver (`tests/Company.Orion.Test/Unit/Infra/Mongo`), no server needed.

## 🧪 Testing

```bash
dotnet test Orion.sln
```

| Suite | Location | Needs Docker? |
|---|---|---|
| Unit | `tests/Company.Orion.Test/Unit` | No |
| Integration / API | `tests/Company.Orion.Test/Api`, `Integration` | Yes — a database container is started automatically with Testcontainers |

Coverage is collected with Coverlet and published to SonarCloud on every push/PR (see `.github/workflows/build.yml`).

## 🧱 Build conventions

`Directory.Build.props` centralizes the settings every project shares (`net10.0`, `ImplicitUsings`, `LangVersion`, deterministic builds) and enables **nullable reference types** solution-wide. Projects not migrated yet opt out with `<Nullable>disable</Nullable>` in their `.csproj` (currently `Api`, `Application.Core`, `Infra.Data`, `Test`); `Domain.Core`, `Crosscutting.*` are already annotated. Package versions live only in `Directory.Packages.props`.

## ⚙️ Configuration

| Setting | Where | Notes |
|---|---|---|
| `ConnectionStrings:SqlServer` / `PostgreSql` / `MongoDb` | `appsettings.{Environment}.json` | Use user-secrets or environment variables outside development |
| `JwtOptions` | `appsettings.{Environment}.json` | `SymmetricSecurityKey` (≥ 64 chars, HS512 — **replace it**, the app refuses to start otherwise), `TokenExpirationMinutes`, `RefreshTokenExpirationDays` |
| `Cors:AllowedOrigins` | `appsettings.{Environment}.json` | List of allowed origins; `["*"]` (development) allows any origin |
| `RateLimiting:Auth` | `appsettings.{Environment}.json` | `PermitLimit` requests per `WindowSeconds` per client IP on `/Auth/*` (429 + ProblemDetails when exceeded) |
| `OTEL_EXPORTER_OTLP_ENDPOINT` (+ `OTEL_EXPORTER_OTLP_PROTOCOL`, `OTEL_SERVICE_NAME`) | environment | When set, traces/metrics/logs are exported via OTLP; unset = instrumentation only, nothing exported |

### Health probes

| Endpoint | Checks | Use as |
|---|---|---|
| `GET /health/live` | nothing (process is up) | Kubernetes `livenessProbe` / Docker `HEALTHCHECK` |
| `GET /health/ready` | database (checks tagged `ready`) | Kubernetes `readinessProbe` / load balancer |
| `GET /health` | everything, HealthChecks-UI JSON | dashboards |
| `Serilog` | `appsettings.json` | Sinks, levels and enrichers |
| `ASPNETCORE_ENVIRONMENT` | environment | `Development`, `Test`, `Production` |

## 🤝 Contributing

Contributions are very welcome — bug reports, new database flavours or docs

1. Fork and create a branch: `git checkout -b feature/my-improvement`
2. Read [`AGENTS.md`](AGENTS.md): it is the contract for the project structure and it applies to humans too.
3. Keep the three template flavours compiling (`SqlServer`, `PostgreSql`, `MongoDb`) — look at the `#if (systemDatabase == ...)` blocks.
4. Add or update tests; `dotnet test Orion.sln` must be green.
5. Open a pull request describing *why* — the CI (build, tests, SonarCloud quality gate) runs automatically.

Found a bug or have an idea? [Open an issue](https://github.com/vanderlan/Orion-Api/issues).

## 📄 License

[MIT](LICENSE) — *Maintainer:* [@vanderlan](https://github.com/vanderlan)
