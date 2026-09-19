# Quipu Bookstore

A .NET 8 Web API backend for a bookstore: `Author`/`Book` domain, Book CRUD, and a title/author search endpoint with pagination — protected by OAuth2 (OpenIddict), backed by SQL Server via EF Core.

## Prerequisites

- .NET 8 SDK
- One of:
  - Docker Desktop (runs the API and SQL Server together), or
  - SQL Server LocalDB (ships with Visual Studio's ASP.NET workload) for running the API directly

## Local development credentials

All values below are **local assessment-only mock values — not production credentials**. They are defaults so the solution runs with no setup; override any of them without editing code (see [Overriding the defaults](#overriding-the-defaults)).

| What | Value | Used for |
|---|---|---|
| Demo user | `quipu-manager` / `bookstore123` | Interactive sign-in for the implicit flow (search) |
| CRUD client | `bookstore-crud-client` / `crud123!` | client_credentials token (CRUD) |
| SQL Server `sa` password | `SqlDev12345!` | Docker SQL Server container only |

The demo user, CRUD secret and seeded OAuth clients only exist when `ASPNETCORE_ENVIRONMENT=Development`. A real deployment needs a real identity provider and real client registration, which is out of scope here.

## Running with Docker

Prerequisite: Docker Desktop, installed and running. From the repository root:

```powershell
docker compose up --build
```

This builds the API image and starts both containers. The `api` container waits for the `sqlserver` container to report healthy (via its SQL healthcheck) before it starts, so the database is always ready before the app creates and seeds it. No `.env` file is needed — `docker-compose.yml` falls back to the mock values above.

Once up:

- Swagger: http://localhost:5107/swagger
- Token endpoint: http://localhost:5107/connect/token

### Stopping the stack

```powershell
docker compose down
```

This stops the containers and keeps the SQL Server data volume intact. Do **not** run `docker compose down -v` unless you intend to permanently delete that volume — it removes the persisted database, and the next `docker compose up` starts from a fresh, re-seeded database.

## Running locally (without Docker)

```powershell
dotnet restore
dotnet build Quipu.Bookstore.sln
dotnet run --project src\Quipu.Bookstore.Api
```

The default LocalDB connection string lives in `appsettings.Development.json` and uses Windows-integrated auth. The API listens on http://localhost:5107 (and https://localhost:7152 with the `https` profile); Swagger opens at `/swagger`. For any other environment, set `ConnectionStrings__BookstoreDatabase` — the base `appsettings.json` intentionally ships it empty.

## Database initialization

In Development, startup does two things:

1. **Schema and seed data** — `Database.EnsureCreatedAsync()` creates the `QuipuBookstore` database with 3 authors and 6 books (seeded through `HasData` in the entity configurations). `EnsureCreated` only runs against a missing database; it never alters an existing schema. There are no EF Core migrations yet.
2. **OAuth2 scopes and clients** — `books.manage` and `books.read` are created if missing, and both clients are created **or updated** from current configuration on every startup. Changing the CRUD secret or the Swagger redirect URIs therefore takes effect on the next start without resetting anything.

### Resetting the development database

A reset is only needed to get fresh seed data, or when changing the SQL Server `sa` password (SQL Server applies `MSSQL_SA_PASSWORD` only when the volume is first created).

- Docker: `docker compose down -v`, then `docker compose up --build`. This permanently deletes the database volume.
- LocalDB: `sqlcmd -S "(localdb)\mssqllocaldb" -Q "DROP DATABASE QuipuBookstore"`, then run the API again.

## Architecture

Five projects, with dependencies pointing inward to `Domain`:

```
Quipu.Bookstore.Api          → Application, Contracts, Domain, Storage
Quipu.Bookstore.Application  → Domain
Quipu.Bookstore.Contracts    → Domain
Quipu.Bookstore.Storage      → Domain
Quipu.Bookstore.Domain       → (none)
```

| Project | Responsibility |
|---|---|
| `Api` | Controllers, OpenIddict server hosting and endpoints, authorization policies, Swagger, middleware, composition root (`Registers/Register.*.cs`), mapping `Result` to HTTP (`Extensions/ResultExtensions.cs`) |
| `Application` | CQRS commands, queries and handlers (`Books/<UseCase>/`), request/handler abstractions, DTOs. No EF Core, SQL Server, HTTP contracts or OpenIddict |
| `Contracts` | HTTP request and response models, paging response. Only binding/validation attributes that the HTTP boundary needs |
| `Domain` | `Author` and `Book` entities with their invariants, `Result`/`Result<T>`, result codes and error types, paging primitives, repository interfaces. No framework dependencies |
| `Storage` | `AppDbContext`, entity configurations and seed data, repository implementations, database initialization, SQL Server and OpenIddict EF Core registration |

There is no separate `Infrastructure` project: the only external dependency is the database, which `Storage` owns.

### Request flow

```
BooksController ──(IMediator.Send)──▶ <UseCase>Handler ──▶ IBookRepository / IAuthorRepository (Domain)
      ▲                                     │                          │
      │                                     ▼                          ▼
 ToActionResult ◀──── Result / Result<T> ◀──┘                BookRepository (Storage, EF Core)
```

1. The controller binds a `Contracts` request, builds a command or query, and sends it through MediatR.
2. The handler (`CreateBookHandler`, `UpdateBookHandler`, `DeleteBookHandler`, `GetBookByIdHandler`, `SearchBooksHandler`) orchestrates repositories and returns `Result` or `Result<T>` — it never throws for expected failures such as a missing book or author.
3. `BaseController.Result(...)` calls `ToActionResult`, which returns the success response the action asks for, or turns a failure into an RFC 7807 `ProblemDetails` response with an `errorCode` extension.

Handlers are registered by scanning the Application assembly for `IRequestWithResultHandler<,>` (Scrutor), and `IRequestWithResult<T>` constrains every request to return a `Result`.

### Status codes

| Situation | Status | Body |
|---|---|---|
| `GET /api/books/{id}`, `GET /api/books` succeed | 200 | `BookResponse` / `PagedResponse<BookResponse>` |
| `POST /api/books` succeeds | 201 | `BookResponse`, `Location` header |
| `PUT` / `DELETE /api/books/{id}` succeed | 204 | none |
| Invalid input (title length, missing fields, `authorId < 1`, `page < 1`, `pageSize` outside 1–100) | 400 | `ValidationProblemDetails` |
| Author does not exist (create/update) | 400 | `ProblemDetails`, `errorCode: AUTHOR_NOT_FOUND` |
| Book does not exist | 404 | `ProblemDetails`, `errorCode: BOOK_NOT_FOUND` |
| Missing, invalid or expired token | 401 | none |
| Valid token with the wrong scope | 403 | none |

## Authentication and authorization

Book **CRUD** (`GET/PUT/DELETE /api/books/{id}`, `POST /api/books`) requires an OAuth2 **client_credentials** token with the `books.manage` scope. Book **search** (`GET /api/books`) requires an OAuth2 **implicit-flow** token with the `books.read` scope. Both flows are issued and validated by an embedded OpenIddict authorization/resource server running in the same process as the API — there is no separate Auth service.

### Why two different flows, and why implicit is used literally

The assessment specifies client_credentials for CRUD and implicit for search. Implicit flow is deprecated (OAuth 2.0 Security BCP recommends against it; OAuth 2.1 removes it in favor of Authorization Code + PKCE) and requires a real signed-in user, which nothing else in this assessment's object model provides. This implementation adds the smallest possible piece of infrastructure to make that literal — a single **development-only** demo user and a minimal interactive login page — rather than silently substituting a different flow. In a production system, the search endpoint should use Authorization Code + PKCE against a real identity provider instead.

### Acquiring a client_credentials token (CRUD)

```powershell
curl.exe -X POST http://localhost:5107/connect/token `
  -d "grant_type=client_credentials" `
  -d "client_id=bookstore-crud-client" `
  -d "client_secret=crud123!" `
  -d "scope=books.manage"
```

Use the returned `access_token` as a `Bearer` token against `/api/books/*` (all verbs except the plain `GET /api/books` search).

### Acquiring an implicit-flow token (search)

This requires a browser (or the Swagger UI, below) — implicit flow returns the token directly in a redirect fragment, not via a token endpoint call. Navigate to:

```
http://localhost:5107/connect/authorize?client_id=bookstore-swagger-client&response_type=token&scope=books.read&redirect_uri=http://localhost:5107/swagger/oauth2-redirect.html&state=xyz
```

Sign in as `quipu-manager` / `bookstore123`; you're redirected back with `#access_token=...` in the URL fragment.

### Using Swagger

Swagger UI (`/swagger`, Development only) shows two separate OAuth2 lock icons — one per scheme, applied per-operation automatically:

- **`oauth2_client_credentials`** on the four CRUD operations — in the Authorize dialog enter `bookstore-crud-client` / `crud123!`.
- **`oauth2_implicit`** on the search operation — click Authorize and sign in as `quipu-manager` / `bookstore123`.

Manual API testing: see [docs/postman/README.md](docs/postman/README.md).

### Scopes, clients, and endpoints

| | |
|---|---|
| Token endpoint | `/connect/token` |
| Authorization endpoint | `/connect/authorize` |
| Scopes | `books.manage` (CRUD), `books.read` (search) |
| CRUD client | `bookstore-crud-client` — confidential, client_credentials only |
| Search client | `bookstore-swagger-client` — public, implicit only |
| Audience | `quipu-bookstore-api`, required on every token |

Each client is permission-restricted to exactly one grant type and one scope at the authorization-server level, so "CRUD only via client_credentials" and "search only via implicit" are enforced structurally, not by inspecting token provenance after the fact.

## Overriding the defaults

Configuration precedence, lowest to highest: `appsettings.Development.json` → .NET user secrets → environment variables. Values set in a higher source replace the mock defaults.

Running locally, use user secrets from `src\Quipu.Bookstore.Api`:

```powershell
dotnet user-secrets set "DevelopmentAuth:Password" "<your-dev-password>"
dotnet user-secrets set "DevelopmentAuth:CrudClientSecret" "<your-dev-secret>"
```

or environment variables (`DevelopmentAuth__Username`, `DevelopmentAuth__Password`, `DevelopmentAuth__CrudClientSecret`, `ConnectionStrings__BookstoreDatabase`).

With Docker, copy `.env.example` to `.env` and edit it (`Copy-Item .env.example .env` in PowerShell, `copy .env.example .env` in CMD). `.env` is gitignored and must never be committed or submitted.

If user secrets or a `.env` file are present, they take precedence — use those values instead of the table above when requesting tokens or signing in.

## Known limitations

- No EF Core migrations — the schema is created with `EnsureCreated()` in Development, which does not evolve an existing schema. Migrations are the intended next step.
- No Author CRUD — authors come only from seed data; the assessment doesn't require Author endpoints.
- The demo login page, mock credentials and seeded OAuth clients are Development-only by design. OpenIddict uses development signing/encryption certificates kept in the current user's certificate store; inside Docker that store lives in the container, so tokens issued before the `api` container is recreated stop validating.
- ASP.NET Core data-protection keys are not persisted in Docker (logged as a warning at startup); the demo sign-in cookie does not survive the `api` container being recreated.
- Failures produced by handlers (`AUTHOR_NOT_FOUND`, `BOOK_NOT_FOUND`) return `ProblemDetails` without the `traceId` field that ASP.NET adds to its own model-validation responses.
- No automated tests yet.
