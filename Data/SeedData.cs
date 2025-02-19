using Assignment1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles = { "Admin", "Contributor" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        if (await userManager.FindByEmailAsync("a@a.a") == null)
        {
            var adminUser = new User
            {
                UserName = "a@a.a",
                Email = "a@a.a",
                FirstName = "Admin",      
                LastName = "User",       
                Role = "Admin",          
                Approved = true
            };

            var result = await userManager.CreateAsync(adminUser, "P@$$w0rd");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        if (await userManager.FindByEmailAsync("c@c.c") == null)
        {
            var contributorUser = new User
            {
                UserName = "c@c.c",
                Email = "c@c.c",
                FirstName = "Contributor", 
                LastName = "User",          
                Role = "Contributor",
                Approved = true
            };

            var result = await userManager.CreateAsync(contributorUser, "P@$$w0rd");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(contributorUser, "contributor");
            }
        }
    }
}
