// Infrastructure
using famenova.Domain.Constants;
using famenova.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration config)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
        foreach (var role in new[] { AppRoles.Customer, AppRoles.Pharmacist, AppRoles.Admin })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<int>(role));
        }

        var email = config["SeedAdmin:Email"];
        var password = config["SeedAdmin:Password"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) return;

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        if (await userManager.FindByEmailAsync(email) is not null) return;

        var admin = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FirstName = "Admin",
            LastName = "Famenova"
        };

        var result = await userManager.CreateAsync(admin, password);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(admin, AppRoles.Admin);
    }
}