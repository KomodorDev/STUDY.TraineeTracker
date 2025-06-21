using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.ApplicationUsers {
    public class DatabaseApplicationUserRepository : IApplicationUserRepository {
        private readonly UserManager<ApplicationUser> _userManager;

        public DatabaseApplicationUserRepository(UserManager<ApplicationUser> userManager) {
            _userManager = userManager;
        }

        public async Task<bool> ExistsAsync(int applicationUserId) {
            var user = await _userManager.FindByIdAsync(applicationUserId.ToString());
            return user != null;
        }

        public async Task<bool> ExistsAsync(ApplicationUser applicationUser) {
            var user = await _userManager.FindByIdAsync(applicationUser.Id);
            return user != null;
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersInRoleAsync(string roleName) {
            return await _userManager.GetUsersInRoleAsync(roleName);
        }
    }
}
