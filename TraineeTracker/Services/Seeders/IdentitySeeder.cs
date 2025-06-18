using Microsoft.AspNetCore.Identity;

namespace TraineeTracker.Services.Seeders {
    public static class IdentitySeeder {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider) {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roleNames = { "Admin", "Mentor", "Trainee" };
            foreach (var roleName in roleNames) {
                if (!await roleManager.RoleExistsAsync(roleName)) {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}