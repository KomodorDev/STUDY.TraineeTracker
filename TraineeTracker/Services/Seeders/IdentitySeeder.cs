using Microsoft.AspNetCore.Identity;
using TraineeTracker.Data.ApplicationUsers;
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
            var applicationUserRepository = serviceProvider.GetRequiredService<IApplicationUserRepository>();

            var userData = new[] {
                new {Email = "simon.hinterreiter@uni-a.de", Role = "Admin"},
                new {Email = "paul.schweizer@uni-a.de", Role = "Mentor"},
                new {Email = "alexander.schlemmer@uni-a.de", Role = "Mentor"},
                new {Email = "alexandros.blask@uni-a.de", Role = "Trainee"},
                new {Email = "nikita.stefan@uni-a.de", Role = "Trainee"}
            };
            string password = "Sopro.2025";

            foreach (var entry in userData) {
                var user = await applicationUserRepository.FindByEmailAsync(entry.Email);
                if (user == null) {
                    user = new ApplicationUser {
                        UserName = entry.Email,
                        Email = entry.Email,
                        EmailConfirmed = true
                    };
                    await applicationUserRepository.CreateAsync(user, password);
                }
                if (!await applicationUserRepository.IsInRoleAsync(user, entry.Role)) {
                    await applicationUserRepository.AddToRoleAsync(user, entry.Role);
                }
            }
        }
    }
}