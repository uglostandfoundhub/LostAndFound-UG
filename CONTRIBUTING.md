# Contributing Guide

We are twelve people working in one repository. These rules exist so that nobody
loses work and nobody spends an afternoon resolving merge conflicts. Read this
before your first push.

---

## 1. Branch Strategy

We use three levels of branches.

```
main                    Protected. Release-ready only. Tagged at each milestone.
 └── develop            Protected. Integration branch. All PRs target this.
      └── feature/...   Your working branch. Short-lived.
```

**Never commit directly to `main` or `develop`.** Both are protected on GitHub.
All work reaches `develop` through a pull request.

### Branch naming

```
feature/<area>-<short-description>
fix/<area>-<short-description>
docs/<short-description>
```

The `<area>` tells everyone which part of the system you are touching:

| Area | Meaning |
|---|---|
| `db` | Domain entities, DbContext, migrations |
| `api` | Controllers, endpoints, business logic |
| `auth` | Identity, JWT, roles, permissions |
| `ui` | Blazor pages, components, styling |
| `test` | Unit and integration tests |
| `ci` | Workflows, deployment, tooling |

Examples:

```
feature/db-item-and-claim-entities
feature/api-items-crud-endpoints
feature/auth-jwt-token-service
feature/ui-item-feed-page
fix/api-claim-status-not-updating
```

### Daily workflow

```bash
# Start from an up-to-date develop
git checkout develop
git pull origin develop

# Create your branch
git checkout -b feature/api-items-crud-endpoints

# ... work, commit ...

# Before pushing, pull in anything new from develop
git pull origin develop

# Push and open a PR
git push -u origin feature/api-items-crud-endpoints
```

**Pull from `develop` every morning.** A branch that has drifted for a week is a
merge conflict waiting to happen.

---

## 2. Commit Messages

Format:

```
<type>: <what changed, in the imperative>
```

Types: `feat`, `fix`, `refactor`, `test`, `docs`, `chore`

```
feat: add Claim entity with verification fields
fix: correct cascade delete on ItemImage
test: add unit tests for ClaimService approval flow
docs: document authentication endpoints
```

Keep commits focused. One logical change per commit beats one giant commit per week.

---

## 3. Pull Requests

**Every PR needs at least one approving review before merge.**

| Area changed | Reviewer |
|---|---|
| `Domain/`, `Infrastructure/`, migrations | Jerry (DBA) |
| `Api/` | Ishmael (Backend Lead) |
| `Auth/`, Identity | Kennedy (Security) |
| `Client/` | Shadrack (Frontend Lead) |
| `tests/` | Prince (QA Lead) |
| `.github/`, deployment | Melchizedek (DevOps) |

### PR rules

1. **Keep it small.** A PR touching 40 files will not get a real review. Aim for one feature.
2. **It must build.** Run `dotnet build` and `dotnet test` before you open the PR.
3. **Describe what you did** using the PR template — reviewers should not have to guess.
4. **Do not merge your own PR** unless it is documentation only.
5. **Delete your branch** after the merge.

---

## 4. Migration Rules (Important)

EF Core migrations are the single most conflict-prone part of this project, because
they are ordered and timestamped.

- **Only the DBA (Jerry) and Backend Developer (Caleb) create migrations.**
  If you need a schema change, open an issue or message the DBA — do not run
  `dotnet ef migrations add` yourself.
- Never edit a migration that is already merged into `develop`. Add a new one instead.
- Always pull `develop` immediately before generating a migration.

---

## 5. Things That Must Never Be Committed

- `appsettings.Development.json` (contains your connection string)
- Any file with a password, JWT secret, or API key
- `bin/`, `obj/`, `.vs/` — these are gitignored, keep it that way
- `.mdf` / `.ldf` database files
- Uploaded user images

If you accidentally commit a secret, tell Melchizedek and Kennedy immediately.
Changing the secret matters more than deleting the commit.

---

## 6. Code Style

- Follow standard C# conventions: `PascalCase` for public members, `_camelCase` for private fields.
- One class per file, file named after the class.
- Prefer clear names over comments. Comment *why*, not *what*.
- Run `dotnet format` before committing if your editor does not format on save.

---

## 7. If You Get Stuck

Do not sit on a blocker for three days. Post in the team group chat with:
what you are trying to do, what you tried, and the exact error message.
