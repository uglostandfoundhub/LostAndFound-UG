# University Lost and Found Hub

A centralized web platform for reporting, searching, and claiming lost and found items across campus.

**DCIT 318 — Programming II | Semester Project**
Lecturer: Mr. Paul Ammah, Computer Science Department, University of Ghana

---

## Tech Stack

| Layer | Technology |
|---|---|
| Frontend | Blazor WebAssembly (.NET 8) |
| Backend | ASP.NET Core 8 Web API |
| Data Access | Entity Framework Core 8 |
| Database | SQL Server |
| Auth | ASP.NET Core Identity + JWT |
| Testing | xUnit |
| CI | GitHub Actions |

---

## Solution Structure

```
LostAndFound/
├── src/
│   ├── LostAndFound.Domain/          Entities, enums, domain rules (no dependencies)
│   ├── LostAndFound.Infrastructure/  DbContext, migrations, repositories, seeding
│   ├── LostAndFound.Api/             Controllers, auth, DI wiring, middleware
│   ├── LostAndFound.Client/          Blazor WebAssembly UI
│   └── LostAndFound.Shared/          DTOs shared between Api and Client
├── tests/
│   ├── LostAndFound.UnitTests/
│   └── LostAndFound.IntegrationTests/
├── docs/                             Schema diagrams, API docs, meeting notes
└── LostAndFound.sln
```

**Dependency direction:** `Domain` depends on nothing. `Infrastructure` depends on `Domain`.
`Api` depends on `Infrastructure` + `Shared`. `Client` depends on `Shared` only.
Never add a reference that points backwards.

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (Express, Developer, or LocalDB)
- Visual Studio 2022 / VS Code / JetBrains Rider
- Git

### Setup

```bash
git clone https://github.com/<org>/LostAndFound.git
cd LostAndFound
git checkout develop

dotnet restore
dotnet build
```

### Database

Copy the example settings and set your own connection string:

```bash
cp src/LostAndFound.Api/appsettings.Development.example.json \
   src/LostAndFound.Api/appsettings.Development.json
```

Then apply migrations:

```bash
dotnet ef database update \
  --project src/LostAndFound.Infrastructure \
  --startup-project src/LostAndFound.Api
```

> `appsettings.Development.json` is gitignored. Never commit connection strings or secrets.

### Run

```bash
# Terminal 1 — API
dotnet run --project src/LostAndFound.Api

# Terminal 2 — Client
dotnet run --project src/LostAndFound.Client
```

API: `https://localhost:7001` · Swagger: `https://localhost:7001/swagger` · Client: `https://localhost:7002`

---

## Contributing

Read [CONTRIBUTING.md](CONTRIBUTING.md) before your first push. It covers the branch
strategy, commit format, and pull request rules. **Direct pushes to `main` and `develop`
are blocked.**

---

## Team

| Name | Role | Primary Area |
|---|---|---|
| Wilfred Adu Otabil | Project Manager | Timeline & coordination |
| Emmanuel Jerry Kuake | Database Admin *(acting PM)* | `Domain/`, `Infrastructure/` |
| Kwakye Ishmael Affum | Backend Lead | `Api/Controllers/` |
| Ablorh-Adjei Caleb | Backend Developer | `Infrastructure/Repositories/` |
| Shadrack Dorkenoo | Frontend Lead | `Client/` layout & design system |
| Joel Adom Yaw Opoku | Frontend Developer | `Client/Components/` |
| Samuel Kofi Ntem Amankwah | Frontend Developer | `Client/Pages/Search/` |
| Kennedy Sarfo | Security & Auth | `Api/Auth/`, Identity, JWT |
| Melchizedek Sensemore D.A | DevOps | `.github/workflows/`, deployment |
| Prince Boateng | QA Lead | `tests/` strategy |
| Prosper Sokari | QA Tester | `tests/UnitTests/`, `tests/IntegrationTests/` |
| Peter Paul Didemudo | Documentation | `docs/`, Swagger annotations |
