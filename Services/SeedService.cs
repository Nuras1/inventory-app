using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using project_itransition.Data;
using project_itransition.Models.Entities;

namespace project_itransition.Services
{
    public class SeedService
    {
        public static async Task SeedDatabase(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<SeedService>>();

            try
            {
                logger.LogInformation("Ensuring tha database is created.");
                await context.Database.MigrateAsync();

                logger.LogInformation("Sending roles.");
                await AddRoleAsync(roleManager, "Admin");
                await AddRoleAsync(roleManager, "User");

                logger.LogInformation("Sending admin user");
                var adminEmail = "admin@gmail.com";
                if (await userManager.FindByEmailAsync(adminEmail) == null)
                {
                    var adminUser = new ApplicationUser
                    {
                        FullName = "Nuras Bekmurad",
                        UserName = adminEmail,
                        NormalizedEmail = adminEmail.ToUpper(),
                        Email = adminEmail,
                        NormalizedUserName = adminEmail.ToUpper(),
                        EmailConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString()
                    };
                    var result = await userManager.CreateAsync(adminUser, "Admin@123");
                    if (result.Succeeded)
                    {
                        logger.LogInformation("Assigning Admin role to the admin user");
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                    else
                    {
                        logger.LogError("Failed to create admin user: {Errors}", string.Join(",", result.Errors.Select(e => e.Description)));
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occured while seeding the database.");
            }
        }
        public static async Task AddRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to create role '{roleName}': {string.Join(",", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}