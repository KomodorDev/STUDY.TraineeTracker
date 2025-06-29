using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.ApplicationUsers {
    public interface IApplicationUserRepository {
        public Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role);

        public Task<IdentityResult> CreateAsync(ApplicationUser user, string password);

        public Task<bool> ExistsAsync(string userId);

        public Task<bool> ExistsAsync(ApplicationUser user);

        public Task<ApplicationUser?> FindByEmailAsync(string email);

        public Task<ApplicationUser?> FindByEmailWithProcessingPausesAndTraineeLessonsAsync(string email);

        public Task<ApplicationUser?> FindByIdAsync(string userId);

        public Task<ApplicationUser?> FindByIdWithLastSelectedTraineesAsync(string userId);

        public Task<ApplicationUser?> FindByIdWithProcessingPausesAndTraineeLessonsAsync(string userId);

        public Task<ApplicationUser?> FindByIdWithTraineeLessonsWithLessonsAndTeachingPlanAsync(string userId);

        public Task<IEnumerable<ApplicationUser>> GetOpenUsersInRoleAsync(string roleName);

        public Task<IEnumerable<ApplicationUser>> GetUsersInRoleAsync(string roleName);

        public Task<IEnumerable<ApplicationUser>> GetUsersInRoleWithProcessingPausesAndTraineeLessonsAsync(string roleName);

        public Task<bool> IsInRoleAsync(ApplicationUser user, string role);

        public Task<IdentityResult> UpdateAsync(ApplicationUser user);
    }
}
