# Stage 1 — Database Layer

Owner: Emmanuel Jerry Kuake (Database Admin)
Branch: `feature/db-schema-and-migrations`

---

## What is in this stage

| Project | Contents |
|---|---|
| `LostAndFound.Domain` | Entities, enums, role constants |
| `LostAndFound.Infrastructure` | `ApplicationDbContext`, Fluent API configurations, seeder, DI |

---

## 1. Extra package required on Domain

`ApplicationUser` inherits from `IdentityUser`, so the Domain project needs the Identity
stores abstraction:

```bash
dotnet add src/LostAndFound.Domain package Microsoft.Extensions.Identity.Stores
```

This is a deliberate trade-off. Strict Clean Architecture would keep Domain free of all
framework references and put `ApplicationUser` in Infrastructure — but then `Item` could
not hold a navigation property to its reporter without an awkward indirection. For a
project this size, one package reference is the cheaper cost.

---

## 2. Wire up `Program.cs` in the API

```csharp
using LostAndFound.Domain.Entities;
using LostAndFound.Infrastructure;
using LostAndFound.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    await DbSeeder.SeedAsync(context, userManager, roleManager);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

> JWT configuration is Kennedy's work in Stage 2. `AddInfrastructure` registers Identity
> with cookie defaults for now; he will layer JWT bearer authentication on top.

---

## 3. Create and apply the migration

```bash
dotnet ef migrations add InitialCreate \
  --project src/LostAndFound.Infrastructure \
  --startup-project src/LostAndFound.Api \
  --output-dir Data/Migrations

dotnet ef database update \
  --project src/LostAndFound.Infrastructure \
  --startup-project src/LostAndFound.Api
```

Verify in SSMS: expect the seven project tables plus the seven `AspNet*` Identity tables.

---

## 4. Schema reference

### Core tables

| Table | Purpose |
|---|---|
| `AspNetUsers` | Users, extended with FullName, StudentId, Department |
| `Categories` | Item categories (seeded: 11 rows) |
| `Locations` | Campus locations (seeded: 18 rows) |
| `Items` | Both lost and found reports, separated by `Type` |
| `ItemImages` | Photos attached to an item |
| `ItemClaims` | Ownership claims with verification answers |
| `ItemMatches` | Suggested lost/found pairings with a score |
| `Notifications` | Per-user alerts |

### Key design decisions

**One `Items` table, not two.** A `Type` discriminator (`Lost` / `Found`) keeps search,
filtering, and matching against a single table. Two tables would force a UNION on every
query and duplicate the claim logic.

**`VerificationQuestion` / `VerificationAnswer`.** The finder sets a question only the
true owner could answer. The claimant's answer is stored on the claim for staff review.
This is what separates a real claim workflow from a "click to claim" button.

**Delete behaviour is deliberate.** SQL Server rejects a schema with multiple cascade
paths to the same table. Because `ItemClaims` is reachable from `AspNetUsers` both
directly (`ClaimantId`) and indirectly (`AspNetUsers` → `Items` → `ItemClaims`), all
user-facing foreign keys use `Restrict`. Only `ItemImages`, `ItemClaims` (from `Items`),
and `Notifications` cascade.

**Unique index on `(ItemId, ClaimantId)`.** One person cannot file two claims on the
same item.

**`ItemClaim`, not `Claim`.** `System.Security.Claims.Claim` ships with Identity, and a
type named `Claim` would collide in any file that uses both. `ItemMatch` follows the same
convention.

---

## 5. Default seeded admin

```
Email:    admin@ug.edu.gh
Password: Admin@123!
```

**Change this before deployment.** It is fine for local development and the demo, but it
must not survive into anything public.

---

## 6. Checklist before opening the PR

- [ ] `dotnet build` succeeds
- [ ] Migration generated and applied without error
- [ ] Tables visible in SSMS with seed rows present
- [ ] `appsettings.Development.json` NOT committed
- [ ] Branch is `feature/db-schema-and-migrations`, PR targets `develop`
