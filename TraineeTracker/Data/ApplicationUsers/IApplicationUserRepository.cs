using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.ApplicationUsers {
    public interface IApplicationUserRepository {
        public Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role);
        public Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
        public Task<IdentityResult> DeleteAsync(ApplicationUser user);
        public Task<bool> ExistsAsync(string userId);
        public Task<bool> ExistsAsync(ApplicationUser user);
        public Task<ApplicationUser?> FindByEmailAsync(string email);
        public Task<ApplicationUser?> FindByEmailWithProcessingPausesAndTraineeLessonsAsync(string email);
        public Task<ApplicationUser?> FindByIdAsync(string userId);
        public Task<ApplicationUser?> FindByIdWithLastSelectedTraineesAsync(string userId);
        public Task<ApplicationUser?> FindByIdWithNotificationSettingAsync(string userId);
        public Task<ApplicationUser?> FindByIdWithTeachingPlanAndTraineeLessonsWithLessonsAsync(string userId);
        public Task<ApplicationUser?> FindByIdWithTraineeLessonsAndProcessingPausesAsync(string userId);
        public Task<ApplicationUser?> FindByIdWithProcessingPausesAsync(string userId);
        public Task<ApplicationUser?> FindByIdWithWrittenFeedbacksWithLessonAsync(string userId);
        public Task<IEnumerable<ApplicationUser>> GetOpenUsersInRoleAsync(string roleName);
        public Task<IEnumerable<ApplicationUser>> GetOpenUsersInRoleWithEmailNotificationSettingAsync(string roleName);
        public Task<IEnumerable<string>> GetRolesAsync(ApplicationUser user);
        public Task<ApplicationUser?> GetUserAsync(ClaimsPrincipal principal);
        public Task<IEnumerable<ApplicationUser>> GetUsersInRoleAsync(string roleName);
        public Task<IEnumerable<ApplicationUser>> GetUsersInRoleWithProcessingPausesAndTraineeLessonsAsync(string roleName);
        public Task<IEnumerable<ApplicationUser>> GetAllAsync();
        public Task<bool> IsInRoleAsync(ApplicationUser user, string role);
        public Task<IdentityResult> UpdateAsync(ApplicationUser user);
    }
}
