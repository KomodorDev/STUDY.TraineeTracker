using Microsoft.AspNetCore.Identity;

namespace TraineeTracker.Services.Seeders {

    /// <summary>
    /// Provides functionality to seed predefined roles into the application's identity system.
    /// </summary>
    /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
    public class RolesSeeder {
        private readonly RoleManager<IdentityRole> _roleManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="RolesSeeder"/> class with the specified <see cref="RoleManager{IdentityRole}"/>.
        /// </summary>
        /// <param name="roleManager">The role manager used to manage identity roles.</param>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public RolesSeeder(RoleManager<IdentityRole> roleManager) {
            _roleManager = roleManager;
        }

        // ------------------------------------------------------
        /// <summary>
        /// Seeds the predefined roles ("Admin", "Mentor", "Trainee") into the identity system if they do not already exist.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task SeedRolesAsync() {
            var roleNames = new[] { "Admin", "Mentor", "Trainee" };

            foreach (var roleName in roleNames) {
                if (!await _roleManager.RoleExistsAsync(roleName)) {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        // ------------------------------------------------------
    }
}