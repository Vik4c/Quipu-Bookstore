# Manual API testing with Postman

`quipu-bookstore.postman_collection.json` exercises every Book endpoint, both OAuth2 flows, and the expected success and error status codes. It contains no credentials; secrets are entered locally in Postman.

For prerequisites, architecture, the OAuth2 design and the full endpoint reference, see the root [README](../../README.md).

## Start the Docker stack

From the repository root:

```powershell
docker compose up --build
```

Continue once http://localhost:5107/swagger loads.

## Import the collection

Postman → **Import** → select `docs/postman/quipu-bookstore.postman_collection.json`.

## Configure variables

Open the collection → **Variables** tab and edit the **Current value** column only. Current values stay on your machine and are not exported or synced.

| Variable | Default | Purpose |
|---|---|---|
| `baseUrl` | `http://localhost:5107` | API root; change it if you set a different `API_HTTP_PORT` |
| `crudClientId` | `bookstore-crud-client` | client_credentials client |
| `crudClientSecret` | *(empty)* | **set locally**: see [Local development credentials](../../README.md#local-development-credentials), or your override |
| `readClientId` | `bookstore-swagger-client` | implicit-flow client |
| `scopeManage` / `scopeRead` | `books.manage` / `books.read` | OAuth2 scopes |
| `implicitRedirectUri` | `http://localhost:5107/swagger/oauth2-redirect.html` | redirect URI registered for the implicit client |
| `bookId` / `authorId` | `1` / `1` | existing seeded ids |
| `missingBookId` / `missingAuthorId` | `999999` | ids that do not exist |
| `accessToken` | *(empty)* | filled by **Get CRUD token** |
| `readAccessToken` | *(empty)* | filled by you after the implicit flow |
| `createdBookId` | *(empty)* | filled by **Create book** |

Never put the secret or a token in the **Initial value** column, and never export or commit the collection with them filled in.

## Get a client-credentials token

Run **02 Client credentials → Get CRUD token**. It posts `grant_type=client_credentials` with `{{crudClientId}}`, `{{crudClientSecret}}` and `{{scopeManage}}` to `/connect/token`, and its test script stores the token in `accessToken` without logging it.

## Test CRUD

Run folders **03 CRUD** and **04 Validation and not found** in order (for example with the Collection Runner). They use `{{accessToken}}` as a Bearer token; **Create book** stores the new id in `createdBookId`; the requests that follow update it, read it back to confirm the update, delete it, and check that it is gone.

## Test search (implicit flow)

Search needs a `books.read` token from the interactive implicit flow:

1. Open folder **05 Search (implicit flow)** → **Authorization**. The OAuth 2.0 *Implicit* settings are preconfigured from the collection variables.
2. Keep **Authorize using browser** off. The callback URL must remain `{{implicitRedirectUri}}`, the only redirect URI registered for the client.
3. Click **Get New Access Token** and sign in with the demo user.
4. Copy the token into the **Current value** of `readAccessToken`.
5. Run **05 Search (implicit flow)** and **06 Wrong scope (403)**.

Alternatively, get the token in a browser as described in [Acquiring an implicit-flow token](../../README.md#acquiring-an-implicit-flow-token-search) and paste it into `readAccessToken`.

## Expected status codes

| Status | Requests |
|---|---|
| 200 | Swagger UI, OpenAPI document, Get CRUD token, Get book by id, Get updated book (confirms the PUT cleared `subTitle`), all successful searches |
| 201 | Create book (with `Location` header) |
| 204 | Update created book, Delete created book |
| 400 | Create with too-short title, Create with unknown author (`AUTHOR_NOT_FOUND`), Search with invalid page |
| 401 | Search without token, Get book without token, Token with wrong secret (`invalid_client`) |
| 403 | Search with CRUD token, Get book with read token |
| 404 | Get deleted book (`BOOK_NOT_FOUND`), Update missing book, Delete missing book |

Every request has a test script asserting its status code and, where relevant, the response shape or `errorCode`. The complete status-code reference is in the root README under [Status codes](../../README.md#status-codes).

## Stop the Docker stack

```powershell
docker compose down
```

This keeps the SQL Server data volume. Do not add `-v` unless you intend to delete the database (see [Resetting the development database](../../README.md#resetting-the-development-database)).

## Troubleshooting

| Symptom | Cause and fix |
|---|---|
| **Get CRUD token** fails its first assertion, or returns 401 `invalid_client` | `crudClientSecret` is empty or wrong. Set the current value; if you override credentials, use your value ([Overriding the defaults](../../README.md#overriding-the-defaults)). |
| Search or CRUD requests return 401 although a token was set | The token expired (about one hour), or the `api` container was recreated and its development signing keys changed. Request a new token. |
| Search returns 403 | `readAccessToken` holds a CRUD token, or the reverse. Each token only carries its own scope. |
| **Get New Access Token** fails with a redirect-URI error | **Authorize using browser** is on, or the callback URL was changed. Use `{{implicitRedirectUri}}` with the in-app window. |
| Requests fail to connect | The stack is not running, or `API_HTTP_PORT` differs from `baseUrl`. |
| The `sqlserver` container stays unhealthy after changing `MSSQL_SA_PASSWORD` | SQL Server applies that password only when the volume is first created. Restore the previous value, or reset the database as described in [Resetting the development database](../../README.md#resetting-the-development-database). |
