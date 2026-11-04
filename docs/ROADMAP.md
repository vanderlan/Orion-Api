# Architecture roadmap — keeping Orion aligned with .NET 10 and Microsoft's API guidance

Prioritised list of the improvements with the best value/effort ratio for the template.
Each item is self-contained and can be delivered as an independent pull request.
Items are ordered by impact.

---

## 1. One error contract: RFC 9457 `ProblemDetails` via `IExceptionHandler` — ✅ DONE

**Before** the API could answer an error in three different shapes:

| Source | Shape |
|---|---|
| `ExceptionMiddleware` (domain exceptions, serialized with **Newtonsoft**) | `ExceptionResponse { title, notificationType, errors[] }` |
| `AutoValidationActionFilter` (FluentValidation) | `ValidationProblemDetails` |
| `CustomValidationAttribute` (model binding errors) | `ExceptionResponse` built from `ModelState` |

…while the controllers document `400` as `ExceptionResponse`. Clients cannot rely on a single contract.

**Do**
- `builder.Services.AddProblemDetails()` + `app.UseExceptionHandler()` with one `IExceptionHandler`
  (`DomainExceptionHandler`) that maps `NotFoundException → 404`, `ConflictException → 409`,
  `UnauthorizedUserException → 401`, `BusinessException → 400`, everything else → 500 (logged, no details leaked).
- Validation returns `ValidationProblemDetails` (already the case) — make it the only path: delete
  `CustomValidationAttribute` + `SuppressModelStateInvalidFilter`, keep `AutoValidationActionFilter`.
- Drop `Newtonsoft.Json` from the API and the middleware; use `System.Text.Json` everywhere
  (`ExceptionResponse` can be retired or kept as `ProblemDetails.Extensions`).
- Document `400/401/404/409` with `typeof(ProblemDetails)` / `typeof(ValidationProblemDetails)` and add
  the matching example providers.

**Why** Microsoft's recommendation for HTTP APIs, better client generation, `traceId` correlation for free.

**Delivered:** `ExceptionHandling/DomainExceptionHandler.cs` + `Configuration/ProblemDetailsConfiguration.cs`
(`AddProblemDetails`, `AddExceptionHandler`, `UseExceptionHandler`, `UseStatusCodePages`), `AutoValidationActionFilter` now uses
`ProblemDetailsFactory`, `CustomValidationAttribute`/`ExceptionMiddleware`/`ExceptionResponse` removed, Newtonsoft removed from `src`.
**Breaking change for clients:** error bodies are now `ProblemDetails` (`type/title/status/detail/instance/traceId`, plus `errors` for validation).

---

## 2. Native OpenAPI (`Microsoft.AspNetCore.OpenApi`) + Scalar, with transformers — ✅ DONE

**Before** Swashbuckle generated the document. It still works, but since .NET 9 the official templates ship
`Microsoft.AspNetCore.OpenApi`, and .NET 10 adds OpenAPI 3.1 by default, XML doc-comment support through a
source generator, and `IOpenApiDocument/Operation/SchemaTransformer` extensibility. The current document
also has gaps: no security requirement (the *Authorize* button does not attach the token to operations),
enums exposed as integers, API versioning not plugged into ApiExplorer (single `v1` document forever).

**Do**
- `AddOpenApi()` / `MapOpenApi()` + `Scalar.AspNetCore` (`MapScalarApiReference()`) for the UI.
- Port `ExamplesOperationFilter` to an `IOpenApiOperationTransformer` (the `IExampleProvider<T>` contracts
  stay untouched) and the Bearer definition to an `IOpenApiDocumentTransformer` that also adds the
  security requirement to `[Authorize]` operations.
- `Asp.Versioning.Mvc.ApiExplorer` → one document per API version, `x-api-version` parameter documented.
- `JsonStringEnumConverter` in `JsonOptions` so `profile` is `"Admin"` instead of `1`.
- `<GenerateDocumentationFile>true</GenerateDocumentationFile>` and XML comments on controllers/DTOs.

**Why** Keeps the template on the supported, built-in stack; examples and docs generated at build time; cleaner UI.

**Delivered:** `Configuration/OpenApiConfiguration.cs` (`AddOpenApi` per version + `MapOpenApi` + `MapScalarApiReference`),
`OpenApi/Transformers/*` (info, Bearer scheme, security requirement on `[Authorize]`, examples),
`AddApiExplorer` with `'v'VVV` group names, `JsonStringEnumConverter`, `GenerateDocumentationFile` + XML comments on the controllers,
`[ProducesResponseType]` everywhere. Follow-up ideas: generate the document at build time
(`Microsoft.Extensions.ApiDescription.Server`) and publish it as a CI artifact; add `<AdditionalFiles>` with the XML docs of
`Application.Core` so DTO properties are described too.

---

## 3. Security hardening of the authentication flow — ✅ DONE

**Before**
- Passwords are hashed with **unsalted SHA-512** (`StringExtensions.ToSha512`) — not acceptable for production.
- Refresh tokens have **no expiration / revocation** (`RefreshToken` has no `ExpiresAt`), and `Login` reuses an
  existing token instead of issuing a new one.
- Tokens are created with the legacy `JwtSecurityTokenHandler`; `JwtOptions` are read ad hoc with
  `configuration.GetSection(...).Get<>()` in two places and never validated at startup.
- CORS is `AllowAnyOrigin/Method/Header`; there is no rate limiting on `/Auth/Login`.

**Do**
- `PasswordHasher<User>` (PBKDF2, `Microsoft.Extensions.Identity.Core`) or Argon2 behind an `IPasswordHasher`
  domain abstraction; keep a migration path for existing SHA-512 hashes.
- Add `ExpiresAt`/`RevokedAt` to `RefreshToken`, validate them on rotation, issue a new token on every login,
  and purge expired tokens.
- `Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler` for creation/validation; move token creation
  out of a static helper into an `ITokenService` registered in DI.
- `services.AddOptions<JwtOptions>().BindConfiguration("JwtOptions").ValidateDataAnnotations().ValidateOnStart()`
  (same for `MongoDbOptions`); inject `IOptions<JwtOptions>`.
- Named CORS policy bound from configuration; `AddRateLimiter` (fixed window) on the auth endpoints.

**Why** These are the findings any security review of a generated project would raise first.

**Delivered:** `Domain.Core/Security` (`IPasswordHasher`, `Pbkdf2PasswordHasher` — PBKDF2-SHA512, 210k iterations,
16-byte salt, legacy SHA-512 accepted + `SuccessRehashNeeded`), `Domain.Core/Authentication` (`ITokenService`, `JwtOptions`
validated with `ValidateOnStart`), `Api/Security/JwtTokenService` (`JsonWebTokenHandler`; signature/issuer/audience validated
on refresh), `RefreshToken.IsExpired` + `RefreshTokenExpirationDays`, login revokes previous refresh tokens, `IUserRepository.LoginAsync`
removed (no lookup by hash), `Cors:AllowedOrigins`, `RateLimiting:Auth` fixed-window per IP on `/Auth/*` (429 ProblemDetails).
Follow-ups: a background purge of expired refresh tokens; `ExpiresAt`/`RevokedAt` columns if auditing is required (needs a migration);
seed the admin with a PBKDF2 hash once a migration is generated.

---

## 4. DI lifetimes, data-access correctness and compiler safety nets — ✅ DONE (nullable: in progress)

**Before**
- `DataContext` is registered as scoped **and** `UnitOfWork` (transient) creates its own
  `new DataContext(configuration)` — two contexts per request, options built outside DI, no pooling.
- `DataContext.SaveChangesAsync()` *hides* the base method instead of overriding
  `SaveChangesAsync(CancellationToken)`; `CancellationToken`s received by handlers are never propagated
  to repositories (`IBaseEntityRepository` has no token parameters).
- `DateTime.UtcNow` is called directly (timestamps, JWT expiration) → hard to test.
- No project enables `<Nullable>enable</Nullable>`; no `Directory.Build.props`, so `TargetFramework`,
  `ImplicitUsings`, analyzers and warnings-as-errors are repeated (or missing) per project.

**Do**
- `AddDbContextPool<DataContext>(o => o.UseSqlServer/UseNpgsql(...))`, inject the context into a **scoped**
  `UnitOfWork`; same lifetime for `MongoUnitOfWork` (`MongoDataContext` via DI).
- `override Task<int> SaveChangesAsync(CancellationToken)`; add `CancellationToken` to every repository/UoW
  method and pass it from handlers.
- Inject `TimeProvider` (built-in since .NET 8) instead of `DateTime.UtcNow`.
- Add `Directory.Build.props` with `TargetFramework`, `Nullable=enable`, `ImplicitUsings=enable`,
  `TreatWarningsAsErrors`, `EnforceCodeStyleInBuild`, `AnalysisLevel=latest`; fix the resulting warnings.

**Why** Correctness under load, testability, and the defaults every new .NET 10 project already has.

**Delivered:** `AddDbContextPool<DataContext>` with options from DI (`DataContext(DbContextOptions<DataContext>)`),
`Infra.Data/Interceptors/TimestampsInterceptor` (`SaveChangesInterceptor` + `TimeProvider`, replaces the hidden `SaveChangesAsync`),
scoped `UnitOfWork(DataContext, ILogger)` / `MongoUnitOfWork(MongoDataContext, ILogger)`, `CancellationToken` on every
repository/unit-of-work method and propagated from all handlers, `TimeProvider` in `MongoDataContext`, `DeleteAsync` no longer clears
the change tracker, `Directory.Build.props` (TargetFramework, ImplicitUsings, LangVersion, deterministic builds, `Nullable=enable`).
Nullable is fully applied to `Domain.Core`, `Crosscutting.Ioc` and `Crosscutting.Resources`; `Api`, `Application.Core`, `Infra.Data`
and `Test` opt out per project — **next steps:** migrate them one by one (remove the `<Nullable>disable</Nullable>` override, fix the
warnings), then add `TreatWarningsAsErrors` to the props.

---

## 5. Cloud-native hosting & observability: OpenTelemetry, health probes, modern CI/Docker — ✅ DONE

**Before** Logging is Serilog with a correlation id; there are no traces/metrics; a single `/health`
endpoint; Dockerfile runs as root on port 80 with `aspnet:10.0`; `Program.cs` still uses
`UseRouting/UseEndpoints/MapDefaultControllerRoute`; CI uses `actions/setup-dotnet@v3`.

**Do**
- `AddOpenTelemetry().WithTracing().WithMetrics()` + OTLP exporter and `Serilog`/`ILogger` → OpenTelemetry
  logs (or adopt **.NET Aspire** `ServiceDefaults` + an `AppHost` that replaces `docker-compose.yml` for
  local orchestration of the database containers).
- `MapHealthChecks("/health/live")` (no dependencies) and `/health/ready` (database tag) instead of
  `UseHealthChecks`; `app.MapControllers()` instead of `UseEndpoints`; `MapHealthChecks`/`MapOpenApi` on
  the endpoint route builder.
- Dockerfile: `mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled` (non-root, port 8080), copy `*.csproj`
  first for layer caching, `dotnet publish --no-restore -p:UseAppHost=false`.
- CI: `actions/setup-dotnet@v4`, `dotnet format --verify-no-changes`, xunit v3 / Microsoft.Testing.Platform,
  upload the generated `openapi.json` as an artifact (build-time document generation).

**Why** Production readiness and parity with what `dotnet new webapi` + Aspire provide out of the box.

**Delivered:** `Configuration/OpenTelemetryConfiguration.cs` (ASP.NET Core + HttpClient + runtime instrumentation, `WithLogging`,
`UseOtlpExporter` when `OTEL_EXPORTER_OTLP_ENDPOINT` is set), Serilog finally plugged into the host (`AddSerilog(writeToProviders: true)`
— it was only a static logger before), `UseSerilogRequestLogging`, TraceId in the console template, `/health/live` + `/health/ready`
(`MapHealthChecks` with tag predicates) + `/health`, chiseled non-root runtime image with cached restore layer, Aspire dashboard in
`docker-compose.yml`, `setup-dotnet@v4` / `build-push-action@v6`.
Follow-ups: xunit v3 + Microsoft.Testing.Platform; `dotnet format --verify-no-changes` in CI (after a one-time format commit);
build-time OpenAPI document (`Microsoft.Extensions.ApiDescription.Server`) uploaded as CI artifact; full .NET Aspire AppHost
(orchestrating the database containers) as an alternative to docker-compose.

---

### Honourable mentions (smaller, but worth doing)

- Minimal APIs are now fully featured in .NET 10 (validation, OpenAPI, filters); keep controllers, but add a
  sample Minimal API endpoint group so template users can choose.
- `PagedList<T>` → add `Page`, `PageSize`, `TotalPages` to the response contract.
- Replace the `PUT /Users/{id}` body+route mutation (`userUpdateRequest.PublicId = id`) by a record
  `UserUpdateRequest(string PublicId, ...)` built in the controller, so requests stay immutable.
- Integration fixture: use `IAsyncLifetime` instead of `.GetAwaiter().GetResult()` in the constructor.
