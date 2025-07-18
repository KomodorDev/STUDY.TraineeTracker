using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.ApplicationUsers {
    public interface IApplicationUserRepository {
        Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role);
        Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
        Task<IdentityResult> DeleteAsync(ApplicationUser user);
        Task<bool> ExistsAsync(string userId);
        Task<bool> ExistsAsync(ApplicationUser user);
        Task<ApplicationUser?> FindByEmailAsync(string email);
        Task<ApplicationUser?> FindByEmailWithProcessingPausesAndTraineeLessonsAsync(string email);
        Task<ApplicationUser?> FindByIdAsync(string userId);
        Task<ApplicationUser?> FindByIdWithLastSelectedTraineesAsync(string userId);
        Task<ApplicationUser?> FindByIdWithNotificationSettingAsync(string userId);
        Task<ApplicationUser?> FindByIdWithTeachingPlanAndTraineeLessonsWithLessonsAsync(string userId);
        Task<ApplicationUser?> FindByIdWithTraineeLessonsAndProcessingPausesAsync(string userId);
        Task<ApplicationUser?> FindByIdWithProcessingPausesAsync(string userId);
        Task<ApplicationUser?> FindByIdWithWrittenFeedbacksWithLessonAsync(string userId);
        Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);
        Task<IEnumerable<ApplicationUser>> GetOpenUsersInRoleAsync(string roleName);
        Task<IEnumerable<ApplicationUser>> GetClosedUsersInRoleAsync(string roleName);
        Task<IEnumerable<ApplicationUser>> GetOpenUsersInRoleWithEmailNotificationSettingAsync(string roleName);
        Task<IEnumerable<string>> GetRolesAsync(ApplicationUser user);
        Task<ApplicationUser?> GetUserAsync(ClaimsPrincipal principal);
        Task<IEnumerable<ApplicationUser>> GetUsersInRoleAsync(string roleName);
        Task<IEnumerable<ApplicationUser>> GetUsersInRoleWithProcessingPausesAndTraineeLessonsAsync(string roleName);
        Task<IEnumerable<ApplicationUser>> GetAllAsync();
        Task<IEnumerable<ApplicationUser>> GetAllAsync(bool isClosed);
        Task<bool> IsInRoleAsync(ApplicationUser user, string role);
        Task<IdentityResult> UpdateAsync(ApplicationUser user);
    }
}
