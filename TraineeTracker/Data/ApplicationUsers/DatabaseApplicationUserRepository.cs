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

        public async Task<ApplicationUser?> FindByIdWithProcessingPausesAndTraineeLessonsAsync(string userId) {
            return await _context.Users
            .Include(u => u.ProcessingPauses)
            .Include(u => u.TraineeLessons)
            .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<ApplicationUser?> FindByIdWithTraineeLessonsWithLessonsAndTeachingPlanAsync(string userId) {
            return await _context.Users
                .Include(u => u.TraineeLessons)
                    .ThenInclude(tl => tl.Lesson)
                .Include(u => u.TeachingPlan)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }


        public async Task<IEnumerable<ApplicationUser>> GetOpenUsersInRoleAsync(string roleName) {
            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
            var userIds = usersInRole.Select(u => u.Id).ToList();
            return await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .Where(u => !u.IsClosed)
            .ToListAsync();
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersInRoleAsync(string roleName) {
            return await _userManager.GetUsersInRoleAsync(roleName);
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersInRoleWithProcessingPausesAndTraineeLessonsAsync(string roleName) {
            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
            var userIds = usersInRole.Select(u => u.Id).ToList();
            return await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .Include(u => u.ProcessingPauses)
            .Include(u => u.TraineeLessons)
            .ToListAsync();
        }

        public async Task<bool> IsInRoleAsync(ApplicationUser user, string role) {
            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user) {
            return await _userManager.UpdateAsync(user);
        }
    }
}
