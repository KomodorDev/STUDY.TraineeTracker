using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.ApplicationUsers {
    public class DatabaseApplicationUserRepository : IApplicationUserRepository {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        public DatabaseApplicationUserRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager) {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role) {
            var result = await _userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded) {
                return result;
            }
            if (role == "Admin") {
                var mentorResult = await _userManager.AddToRoleAsync(user, "Mentor");
                if (!mentorResult.Succeeded) {
                    var errors = result.Errors.Concat(mentorResult.Errors);
                    return IdentityResult.Failed(errors.ToArray());
                }
            }
            return result;
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, string password) {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationUser user) {
            return await _userManager.DeleteAsync(user);
        }

        public async Task<bool> ExistsAsync(string userId) {
            var user = await _userManager.FindByIdAsync(userId);
            return user != null;
        }

        public async Task<bool> ExistsAsync(ApplicationUser user) {
            var tmp = await _userManager.FindByIdAsync(user.Id);
            return tmp != null;
        }

        public async Task<ApplicationUser?> FindByEmailAsync(string email) {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<ApplicationUser?> FindByEmailWithProcessingPausesAndTraineeLessonsAsync(string email) {
            return await _context.Users
            .Include(u => u.ProcessingPauses)
            .Include(u => u.TraineeLessons)
            .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<ApplicationUser?> FindByIdAsync(string userId) {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<ApplicationUser?> FindByIdWithLastSelectedTraineesAsync(string userId) {
            return await _context.Users
            .Include(u => u.LastSelectedTrainees)
            .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<ApplicationUser?> FindByIdWithNotificationSettingAsync(string userId) {
            return await _context.Users
                .Include(u => u.EmailNotificationSetting)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<ApplicationUser?> FindByIdWithTeachingPlanAndTraineeLessonsWithLessonsAsync(string userId) {
            return await _context.Users
                .Include(u => u.TraineeLessons)
                    .ThenInclude(tl => tl.Lesson)
                .Include(u => u.TeachingPlan)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<ApplicationUser?> FindByIdWithTraineeLessonsAndProcessingPausesAsync(string userId) {
            return await _context.Users
            .Include(u => u.ProcessingPauses)
            .Include(u => u.TraineeLessons)
            .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<ApplicationUser?> FindByIdWithProcessingPausesAsync(string userId) {
            return await _context.Users
            .Include(u => u.ProcessingPauses)
            .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<ApplicationUser?> FindByIdWithWrittenFeedbacksWithLessonAsync(string userId) {
            return await _context.Users
                .Include(u => u.WrittenFeedbacks)
                    .ThenInclude(f => f.Lesson)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user) {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<IEnumerable<ApplicationUser>> GetOpenUsersInRoleAsync(string roleName) {
            var usersInRole = await GetUsersInRoleAsync(roleName);
            var userIds = usersInRole.Select(u => u.Id).ToList();
            return await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .Where(u => !u.IsClosed)
            .ToListAsync();
        }

        public async Task<IEnumerable<ApplicationUser>> GetClosedUsersInRoleAsync(string roleName) {
            var usersInRole = await GetUsersInRoleAsync(roleName);
            var userIds = usersInRole.Select(u => u.Id).ToList();
            return await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .Where(u => u.IsClosed)
            .ToListAsync();
        }

        public async Task<IEnumerable<ApplicationUser>> GetOpenUsersInRoleWithEmailNotificationSettingAsync(string roleName) {
            var users = await GetOpenUsersInRoleAsync(roleName);
            var userIds = users.Select(u => u.Id);
            return await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .Include(u => u.EmailNotificationSetting)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetRolesAsync(ApplicationUser user) {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin")) {
                roles.Remove("Mentor");
            }
            return roles;
        }

        public async Task<ApplicationUser?> GetUserAsync(ClaimsPrincipal principal) {
            return await _userManager.GetUserAsync(principal);
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersInRoleAsync(string roleName) {
            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
            if (roleName != "Mentor") {
                return usersInRole;
            }
            var filtered = new List<ApplicationUser>();
            foreach (var user in usersInRole) {
                if (!await IsInRoleAsync(user, "Admin")) {
                    filtered.Add(user);
                }
            }
            return filtered;
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersInRoleWithProcessingPausesAndTraineeLessonsAsync(string roleName) {
            var usersInRole = await GetUsersInRoleAsync(roleName);
            var userIds = usersInRole.Select(u => u.Id).ToList();
            return await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .Include(u => u.ProcessingPauses)
            .Include(u => u.TraineeLessons)
            .ToListAsync();
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync() {
            return await _context.Users.ToListAsync();
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync(bool isClosed) {
            return await _context.Users.Where(u => u.IsClosed == isClosed).ToListAsync();
        }

        public async Task<bool> IsInRoleAsync(ApplicationUser user, string role) {
            var roles = await GetRolesAsync(user);
            return roles.Contains(role);
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user) {
            return await _userManager.UpdateAsync(user);
        }
    }
}
