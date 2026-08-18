# One-Time Solution Scaffold

Run these on your machine, from the root of the cloned repo. **Only the PM/DBA runs
this once** — everyone else just clones the result.

## 1. Create the solution and projects

```bash
dotnet new sln -n LostAndFound

dotnet new classlib -n LostAndFound.Domain         -o src/LostAndFound.Domain
dotnet new classlib -n LostAndFound.Shared         -o src/LostAndFound.Shared
dotnet new classlib -n LostAndFound.Infrastructure -o src/LostAndFound.Infrastructure
dotnet new webapi   -n LostAndFound.Api            -o src/LostAndFound.Api
dotnet new blazorwasm -n LostAndFound.Client       -o src/LostAndFound.Client

dotnet new xunit -n LostAndFound.UnitTests        -o tests/LostAndFound.UnitTests
dotnet new xunit -n LostAndFound.IntegrationTests -o tests/LostAndFound.IntegrationTests
```

## 2. Add every project to the solution

```bash
dotnet sln add src/LostAndFound.Domain/LostAndFound.Domain.csproj
dotnet sln add src/LostAndFound.Shared/LostAndFound.Shared.csproj
dotnet sln add src/LostAndFound.Infrastructure/LostAndFound.Infrastructure.csproj
dotnet sln add src/LostAndFound.Api/LostAndFound.Api.csproj
dotnet sln add src/LostAndFound.Client/LostAndFound.Client.csproj
dotnet sln add tests/LostAndFound.UnitTests/LostAndFound.UnitTests.csproj
dotnet sln add tests/LostAndFound.IntegrationTests/LostAndFound.IntegrationTests.csproj
```

## 3. Wire up project references

Dependency direction matters. Domain depends on nothing.

```bash
# Infrastructure -> Domain
dotnet add src/LostAndFound.Infrastructure reference src/LostAndFound.Domain

# Api -> Infrastructure, Domain, Shared
dotnet add src/LostAndFound.Api reference src/LostAndFound.Infrastructure
dotnet add src/LostAndFound.Api reference src/LostAndFound.Domain
dotnet add src/LostAndFound.Api reference src/LostAndFound.Shared

# Client -> Shared only (never Domain or Infrastructure)
dotnet add src/LostAndFound.Client reference src/LostAndFound.Shared

# Tests
dotnet add tests/LostAndFound.UnitTests reference src/LostAndFound.Domain
dotnet add tests/LostAndFound.UnitTests reference src/LostAndFound.Infrastructure
dotnet add tests/LostAndFound.IntegrationTests reference src/LostAndFound.Api
```

## 4. Install the required packages

```bash
# Infrastructure — EF Core + SQL Server + Identity
dotnet add src/LostAndFound.Infrastructure package Microsoft.EntityFrameworkCore
dotnet add src/LostAndFound.Infrastructure package Microsoft.EntityFrameworkCore.SqlServer
dotnet add src/LostAndFound.Infrastructure package Microsoft.EntityFrameworkCore.Design
dotnet add src/LostAndFound.Infrastructure package Microsoft.AspNetCore.Identity.EntityFrameworkCore

# Api — JWT auth + Swagger
dotnet add src/LostAndFound.Api package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add src/LostAndFound.Api package Swashbuckle.AspNetCore
dotnet add src/LostAndFound.Api package Microsoft.EntityFrameworkCore.Tools

# Client — authenticated HTTP calls
dotnet add src/LostAndFound.Client package Microsoft.AspNetCore.Components.WebAssembly.Authentication

# Tests
dotnet add tests/LostAndFound.IntegrationTests package Microsoft.AspNetCore.Mvc.Testing
dotnet add tests/LostAndFound.UnitTests package Moq
```

## 5. Install the EF Core CLI tool (once per machine)

```bash
dotnet tool install --global dotnet-ef
```

## 6. Verify

```bash
dotnet build
```

If this builds clean, commit and push.

---

# Publishing to GitHub

```bash
git init
git add .
git commit -m "chore: scaffold solution structure and team conventions"

git branch -M main
git remote add origin https://github.com/<org-or-username>/LostAndFound.git
git push -u origin main

# Create develop from main
git checkout -b develop
git push -u origin develop
```

---

# Branch Protection (do this in the GitHub web UI)

Settings → Branches → Add branch protection rule.

Create **two** rules, one for `main` and one for `develop`:

- Branch name pattern: `main` (then repeat for `develop`)
- Require a pull request before merging
- Require approvals: **1**
- Require status checks to pass before merging → select **build-and-test**
- Do not allow bypassing the above settings

Then Settings → Collaborators → add all 11 teammates with **Write** access.

> Note: branch protection requires a public repo or GitHub Pro/Team on private repos.
> If the repo must stay private and protection is unavailable, make the rule social:
> state clearly in the group chat that direct pushes to `main`/`develop` are forbidden.
