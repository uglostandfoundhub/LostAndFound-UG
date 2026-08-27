# Stage 2 — Web API & Business Logic

Owner: Kwakye Ishmael Affum (Backend Lead) · Security: Kennedy Sarfo (JWT)
Branch: `feature/stage2-api`

---

## What is in this stage

| Project | Contents |
|---|---|
| `LostAndFound.Shared` | DTOs, `Result<T>` / `Result`, `PagedResult<T>` used by API and Client |
| `LostAndFound.Infrastructure` | Services (`ItemService`, `ClaimService`, `CategoryService`, `LocationService`, `NotificationService`, `MatchService`) + DI registration |
| `LostAndFound.Api` | JWT auth (`JwtService`, `AuthService`), controllers, Swagger with Bearer security |

---

## Authentication

JWT Bearer. Obtain a token from `POST /api/auth/login`, then send
`Authorization: Bearer <token>` on protected routes.

| Endpoint | Auth | Body |
|---|---|---|
| `POST /api/auth/register` | Anonymous | `RegisterRequest` (default role `Student`) |
| `POST /api/auth/login` | Anonymous | `LoginRequest` |
| `GET /api/auth/me` | Any user | — |

Roles: `Student`, `Staff`, `Admin` (seeded). Staff/Admin review claims.

---

## Endpoints

| Method & Route | Auth | Notes |
|---|---|---|
| `GET /api/items` | Anonymous | Search: `type`, `status`, `categoryId`, `locationId`, `searchTerm`, `fromDate`, `toDate`, `page`, `pageSize` |
| `GET /api/items/{id}` | Anonymous | Item detail |
| `POST /api/items` | User | Create (reporter = current user) |
| `PUT /api/items/{id}` | Owner | Update own report |
| `DELETE /api/items/{id}` | Owner | Delete own report |
| `POST /api/items/{itemId}/claims` | User | Claim a found item (submit verification answer) |
| `GET /api/items/{itemId}/claims` | Staff/Admin | All claims for an item |
| `GET /api/claims/mine` | User | Claims you submitted |
| `POST /api/claims/{id}/review` | Staff/Admin | Approve/Reject (`Pending` → `Approved`/`Rejected`) |
| `GET /api/matches` | User | Matches involving your items |
| `POST /api/items/{itemId}/matches` | Owner | Generate candidate matches (also notifies the other party) |
| `POST /api/matches/{id}/dismiss` | Owner | Dismiss a match |
| `GET /api/notifications` | User | Your notifications |
| `POST /api/notifications/{id}/read` | User | Mark notification read |
| `GET /api/categories` · `GET /api/categories/{id}` | Anonymous | Lookup lists (seeded) |
| `GET /api/locations` · `GET /api/locations/{id}` | Anonymous | Lookup lists (seeded) |

---

## Key behaviours

- **Claims** target `Found` items only. A pending claim moves the item to `UnderReview`;
  approval sets `Claimed` and notifies the claimant; rejection reverts to `Open` if no
  other pending claim remains. One claim per user per item (unique index).
- **Matching** (`MatchService`) scores a lost item against open found items (or vice-versa)
  by category (+0.4), location (+0.3), and title word overlap (+0.3). Threshold 0.4.
- **Notifications** are raised on claim approval/rejection and on new matches.
- Item status flow: `Open → UnderReview → Claimed → Returned/Archived`.

---

## Running locally

The EF migration `InitialCreate` is committed. To run:

1. Provide `src/LostAndFound.Api/appsettings.Development.json` (git-ignored). Copy
   `appsettings.Development.example.json` and set your SQL Server `DefaultConnection`
   plus a real `Jwt:Key` (≥32 chars).
2. `dotnet tool restore` (installs `dotnet-ef` from the repo tool manifest).
3. `dotnet run --project src/LostAndFound.Api` — `DbSeeder` applies the migration and
   seeds roles, the admin user, 11 categories and 18 locations on first start.

> Default admin: `admin@ug.edu.gh` / `Admin@123!` — change before any public deployment.

---

## Checklist before opening the PR

- [x] `dotnet build` succeeds
- [x] `InitialCreate` migration generated (schema validates, incl. cascade paths)
- [x] JWT auth wired with Swagger Bearer support
- [x] Controllers cover items, claims, matches, notifications, categories, locations, auth
- [ ] Frontend (Blazor) integration against these contracts — Shadrack/Joel/Samuel
- [ ] `appsettings.Development.json` NOT committed (git-ignored)
