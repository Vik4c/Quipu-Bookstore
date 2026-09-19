# Quipu Bookstore API

A .NET 8 REST API for managing books and authors, built with ASP.NET Core, Entity Framework Core, SQL Server, OpenIddict, and Docker.

## Features

- Book CRUD operations.
- Search by title or author with pagination.
- Request and domain validation.
- OAuth2 authentication and scope-based authorization.
- SQL Server persistence with EF Core.
- Automatic database initialization and seed data.
- Swagger/OpenAPI documentation.
- Postman collection for API testing.

## Structure

```text
src/
- Quipu.Bookstore.Api
- Quipu.Bookstore.Application
- Quipu.Bookstore.Contracts
- Quipu.Bookstore.Domain
- Quipu.Bookstore.Storage

docs/postman/
```

## Quick start

Prerequisite: Docker Desktop.

```powershell
docker compose up --build
```

API: `http://localhost:5107`  
Swagger: `http://localhost:5107/swagger`

Stop the stack:

```powershell
docker compose down
```

The database and seed data are initialized automatically. Docker SQL Server data is stored in a persistent named volume.

## Local development

```powershell
dotnet run --project src/Quipu.Bookstore.Api
```

- HTTP: `http://localhost:5107`
- HTTPS: `https://localhost:7152`

Use `.env.example` as the configuration template. Never commit `.env`, passwords, tokens, or other secrets.

## Authentication

OpenIddict is embedded in the API.

- `books.manage`: book CRUD and get-by-ID operations.
- `books.read`: book search.

Token endpoint:

```text
POST /connect/token
```

## Main endpoints

| Method | Endpoint | Scope |
|---|---|---|
| GET | `/api/books/{id}` | `books.manage` |
| GET | `/api/books` | `books.read` |
| POST | `/api/books` | `books.manage` |
| PUT | `/api/books/{id}` | `books.manage` |
| DELETE | `/api/books/{id}` | `books.manage` |

See Swagger for complete request and response schemas.

## Local development credentials

Development values are configured locally through `appsettings.Development.json`, `.env`, user secrets, or environment variables. They are not included in this README and must never be committed or reused in production.

## Acquiring an implicit-flow token (search)

Use the Swagger Authorize dialog or the development sign-in flow with the `books.read` scope. The token is returned in the redirect fragment.

## Status codes

- `200` successful reads and searches.
- `201` successful creation.
- `204` successful updates and deletes.
- `400` invalid input.
- `401` missing or invalid authentication.
- `403` insufficient scope.
- `404` missing resources.

## Resetting the development database

Docker:

```powershell
docker compose down -v
docker compose up --build
```

For LocalDB, remove the `QuipuBookstore` database with a SQL Server tool and run the API again.

## Postman

Import:

```text
docs/postman/quipu-bookstore.postman_collection.json
```

The collection covers authentication, CRUD, search, validation, missing resources, and authorization failures. Detailed instructions are in [docs/postman/README.md](docs/postman/README.md).

## Overriding the defaults

Docker Compose reads local overrides from `.env`. Application settings are loaded from the base settings, Development settings, user secrets, and environment variables. See `.env.example` for available variables.

## Known limitations

- The schema uses EF Core `EnsureCreated()`; migrations are not included.
- Authors are seeded and do not have CRUD endpoints.
- No automated test project is included; Docker, Swagger, and Postman provide the validation workflow.

## About

This project was created as a technical assessment submission.