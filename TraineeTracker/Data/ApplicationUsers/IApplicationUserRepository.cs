using System.Collections.Generic;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.ApplicationUsers {
    public interface IApplicationUserRepository {
        public Task<bool> ExistsAsync(int applicationUserId);

        public Task<bool> ExistsAsync(ApplicationUser applicationUser);

        public Task<bool> RoleExistsAsync(string roleName);

        public Task<IEnumerable<ApplicationUser>> GetUsersInRoleAsync(string roleName);
    }
}
