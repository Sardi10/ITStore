using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NewarkITStore.Models;  // Needed for ApplicationUser

namespace NewarkITStore.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Step 1: Create roles
            string[] roles = { "Admin", "Customer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Step 2: Create Super Admin
            string firstName = "Super";
            string lastName = "Admin";
            string email = "admin@store.com";
            string password = "Admin123!";


            if (await userManager.FindByEmailAsync(email) == null)
            {
                var adminUser = new ApplicationUser
                {
                    FirstName = firstName,
                    LastName = lastName,
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
