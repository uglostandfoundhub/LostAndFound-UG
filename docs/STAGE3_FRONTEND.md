# Stage 3 — Blazor Frontend & End-to-End Flow

Owner: Shadrack Dorkenoo (Frontend Lead) · Joel Adom Yaw Opoku, Samuel Kofi Ntem Amankwah (Frontend)
Branch: `feature/stage2-api` (continued — Stage 3 builds on the Stage 2 API)

---

## What is in this stage

| Project | Contents |
|---|---|
| `LostAndFound.Shared` | `ImageDto` added for item photos |
| `LostAndFound.Infrastructure` | `IItemImageService` (file storage under `wwwroot/uploads`), `ItemService.GetForOwnerAsync`, `ClaimService.GetAllAsync` |
| `LostAndFound.Api` | Image upload (`POST/GET/DELETE /api/items/{id}/images`), `GET /api/items/mine`, `GET /api/claims` (staff/admin), Swagger Bearer, static files enabled |
| `LostAndFound.Client` | Auth/HTTP layer + full UI pages |

---

## Frontend pages

| Route | Purpose | Auth |
|---|---|---|
| `/` | Dashboard / entry | Public |
| `/items` | Search & filter all items (type, category, location, text) | Public |
| `/items/{id}` | Item detail: photos, claim form (found items), owner delete, staff claim review | Public / role-based |
| `/report` | Report a lost/found item + photo upload | User |
| `/my-items` | Items you reported, with delete | User |
| `/claims` | Your claims, or all claims to review (staff/admin) | User / staff+admin |
| `/matches` | Generate & view smart matches for your items | User |
| `/notifications` | Notification inbox, mark read | User |
| `/login`, `/register` | Auth | Anonymous |

## Backend additions (the proposal's "image upload" feature)

- `POST /api/items/{id}/images` stores the file in `wwwroot/uploads` (git-ignored) and creates an
  `ItemImage` (first upload becomes primary). Returns `ImageDto`.
- `GET /api/items/{id}/images` lists an item's images; `DELETE /api/items/{id}/images/{imageId}` removes it.
- Images are served statically from `/uploads/...` and referenced by the client via an absolute URL
  (`ApiClient.GetImageUrl`). For production, swap the local folder for blob storage.
- `ItemImage` entities still cascade-delete with their item (Stage 1 config unchanged).

## Auth flow

`JwtService` (Stage 2) → `AuthMessageHandler` attaches the bearer token on every request →
`ApiAuthStateProvider` maps the JWT to a Blazor `AuthenticationState` (name + roles). `NavMenu` is
role-aware: staff/admin see the claims-review surface; anonymous users see only Login/Register.

---

## Running locally (two projects)

1. Start the API (`src/LostAndFound.Api`) — it applies the EF migration and seeds on first run.
   It must listen on the URL set in the Client's `wwwroot/appsettings.json` (`ApiBaseUrl`).
2. Start the Client (`src/LostAndFound.Client`). Update `ApiBaseUrl` to the API's actual address
   (e.g. `https://localhost:7070`).

> Both projects must run; the Client calls the API over HTTP. CORS is **not** configured yet —
> run them on their dev URLs and keep `ApiBaseUrl` pointing at the API. Add CORS in `Program.cs`
> if you later host them on different origins.

---

## Checklist before opening the PR

- [x] `dotnet build LostAndFound.slnx` succeeds (API + Client + tests)
- [x] Image upload endpoint + service implemented
- [x] Full UI: browse, report (with photos), detail, claim, review, matches, notifications
- [x] Role-aware navigation and auth state
- [ ] CORS for independently-hosted API/Client (optional, noted above)
- [ ] `appsettings.Development.json` (API) NOT committed — git-ignored
- [ ] Uploaded photos NOT committed — git-ignored
