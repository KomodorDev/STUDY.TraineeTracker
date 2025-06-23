using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.ApplicationUsers {
    public interface IApplicationUserRepository {
        public Task AddToRoleAsync(ApplicationUser user, string role);

        public Task CreateAsync(ApplicationUser user, string password);

        public Task<bool> ExistsAsync(int applicationUserId);

        public Task<bool> ExistsAsync(ApplicationUser applicationUser);

        public Task<ApplicationUser?> FindByEmailAsync(string email);

        public Task<ApplicationUser?> FindByIdAsync(string userId);

        public Task<IEnumerable<ApplicationUser>> GetUsersInRoleAsync(string roleName);

        public Task<bool> IsInRoleAsync(ApplicationUser user, string role);

        public Task UpdateAsync(ApplicationUser user);
    }
}
