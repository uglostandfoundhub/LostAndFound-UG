using LostAndFound.Domain.Constants;
using LostAndFound.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LostAndFound.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        await context.Database.MigrateAsync();

        await SeedRolesAsync(roleManager);
        await SeedAdminAsync(userManager);
        await SeedCategoriesAsync(context);
        await SeedLocationsAsync(context);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (var role in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager)
    {
        const string adminEmail = "admin@ug.edu.gh";

        if (await userManager.FindByEmailAsync(adminEmail) is not null)
            return;

        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = "System Administrator",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(admin, "Admin@123!");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, Roles.Admin);
        }
    }

    private static async Task SeedCategoriesAsync(ApplicationDbContext context)
    {
        if (await context.Categories.AnyAsync())
            return;

        var categories = new List<Category>
        {
            new() { Name = "Electronics", Description = "Phones, laptops, chargers, earphones" },
            new() { Name = "Identification", Description = "Student ID cards, passports, licences" },
            new() { Name = "Books & Stationery", Description = "Textbooks, notebooks, calculators" },
            new() { Name = "Bags & Luggage", Description = "Backpacks, handbags, laptop bags" },
            new() { Name = "Clothing", Description = "Jackets, caps, shoes, uniforms" },
            new() { Name = "Keys", Description = "Room keys, car keys, padlock keys" },
            new() { Name = "Wallets & Purses", Description = "Wallets, purses, card holders" },
            new() { Name = "Jewellery & Watches", Description = "Rings, chains, bracelets, watches" },
            new() { Name = "Sports Equipment", Description = "Boots, kits, water bottles, gear" },
            new() { Name = "Documents", Description = "Certificates, receipts, printed material" },
            new() { Name = "Other", Description = "Anything not covered above" }
        };

        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();
    }

    private static async Task SeedLocationsAsync(ApplicationDbContext context)
    {
        if (await context.Locations.AnyAsync())
            return;

        var locations = new List<Location>
        {
            new() { Name = "Balme Library", Building = "Balme Library" },
            new() { Name = "Great Hall", Building = "Great Hall" },
            new() { Name = "Department of Computer Science", Building = "School of Physical and Mathematical Sciences" },
            new() { Name = "UGBS Main Building", Building = "University of Ghana Business School" },
            new() { Name = "N Block Lecture Halls", Building = "N Block" },
            new() { Name = "JQB Lecture Halls", Building = "JQB" },
            new() { Name = "Night Market", Building = "Night Market" },
            new() { Name = "Legon Sports Stadium", Building = "Sports Complex" },
            new() { Name = "Commonwealth Hall", Building = "Commonwealth Hall" },
            new() { Name = "Legon Hall", Building = "Legon Hall" },
            new() { Name = "Akuafo Hall", Building = "Akuafo Hall" },
            new() { Name = "Volta Hall", Building = "Volta Hall" },
            new() { Name = "Mensah Sarbah Hall", Building = "Mensah Sarbah Hall" },
            new() { Name = "Pentagon Hostel", Building = "Pentagon" },
            new() { Name = "University Square", Building = "Main Campus" },
            new() { Name = "Bush Canteen", Building = "Bush Canteen" },
            new() { Name = "Main Gate", Building = "Main Gate" },
            new() { Name = "Other / Not Listed", Building = null }
        };

        await context.Locations.AddRangeAsync(locations);
        await context.SaveChangesAsync();
    }
}
