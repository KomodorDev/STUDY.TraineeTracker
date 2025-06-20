using Microsoft.AspNetCore.Identity;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Services.Seeders {
    public static class IdentitySeeder {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider) {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var roleNames = new[] { "Admin", "Mentor", "Trainee" };

            foreach (var roleName in roleNames) {
                if (!await roleManager.RoleExistsAsync(roleName)) {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        public static async Task SeedTestUsersAsync(IServiceProvider serviceProvider) {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var userData = new[] {
                new {Email = "simon.hinterreiter@uni-a.de", Role = "Admin"},
                new {Email = "paul.schweizer@uni-a.de", Role = "Mentor"},
                new {Email = "alexander.schlemmer@uni-a.de", Role = "Mentor"},
                new {Email = "alexandros.blask@uni-a.de", Role = "Trainee"},
                new {Email = "nikita.stefan@uni-a.de", Role = "Trainee"}
            };
            string password = "Sopro.2025";

            foreach (var entry in userData) {
                var user = await userManager.FindByEmailAsync(entry.Email);
                if (user == null) {
                    user = new ApplicationUser { Email = entry.Email };
                    await userManager.CreateAsync(user, password);
                }
                if (!await userManager.IsInRoleAsync(user, entry.Role)) {
                    await userManager.AddToRoleAsync(user, entry.Role);
                }
            }
        }
    }
}