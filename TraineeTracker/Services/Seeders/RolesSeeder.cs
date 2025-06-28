using Microsoft.AspNetCore.Identity;

namespace TraineeTracker.Services.Seeders {
    public class RolesSeeder {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesSeeder(RoleManager<IdentityRole> roleManager) {
            _roleManager = roleManager;
        }

        public async Task SeedRolesAsync() {
            var roleNames = new[] { "Admin", "Mentor", "Trainee" };

            foreach (var roleName in roleNames) {
                if (!await _roleManager.RoleExistsAsync(roleName)) {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}