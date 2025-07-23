using Microsoft.AspNetCore.Identity;
using EventManagementSystem.Models;

namespace EventManagementSystem.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            // Създаваме ролите, ако не съществуват
            if (!await roleManager.RoleExistsAsync("Administrator"))
                await roleManager.CreateAsync(new IdentityRole("Administrator"));
            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new IdentityRole("User"));

            string adminEmail = "admin@events.com";
            string adminPassword = "adminpassword";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var user = new User
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Administrator");
                }
            }
        }

        public static async Task SeedCategoriesAsync(IServiceProvider services)
        {
            var dbContext = services.GetRequiredService<ApplicationDbContext>();
            if (!dbContext.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Музика" },
                    new Category { Name = "Спорт" },
                    new Category { Name = "Технологии" },
                    new Category { Name = "Изкуство" },
                    new Category { Name = "Бизнес" }
                };
                await dbContext.Categories.AddRangeAsync(categories);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
