using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.ApplicationUsers {
    /// <summary>
    /// Repository implementation for managing <see cref="ApplicationUser"/> entities using Entity Framework and ASP.NET Identity.
    /// Provides methods for user creation, deletion, role management, and retrieval with related entities.
    /// </summary>
    /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
    public class DatabaseApplicationUserRepository : IApplicationUserRepository {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseApplicationUserRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        /// <param name="userManager">The ASP.NET Identity user manager.</param>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public DatabaseApplicationUserRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager) {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Adds the specified user to a role. If the role is "Admin", also adds the user to the "Mentor" role.
        /// </summary>
        /// <param name="user">The user to add to the role.</param>
        /// <param name="role">The role name.</param>
        /// <returns>The result of the operation.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
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

        /// <summary>
        /// Creates a new user with the specified password.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <param name="password">The password for the user.</param>
        /// <returns>The result of the operation.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<IdentityResult> CreateAsync(ApplicationUser user, string password) {
            return await _userManager.CreateAsync(user, password);
        }

        /// <summary>
        /// Deletes the specified user.
        /// </summary>
        /// <param name="user">The user to delete.</param>
        /// <returns>The result of the operation.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<IdentityResult> DeleteAsync(ApplicationUser user) {
            return await _userManager.DeleteAsync(user);
        }

        /// <summary>
        /// Checks if a user with the specified ID exists.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>True if the user exists; otherwise, false.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<bool> ExistsAsync(string userId) {
            var user = await _userManager.FindByIdAsync(userId);
            return user != null;
        }

        /// <summary>
        /// Checks if the specified user exists.
        /// </summary>
        /// <param name="user">The user to check.</param>
        /// <returns>True if the user exists; otherwise, false.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<bool> ExistsAsync(ApplicationUser user) {
            var tmp = await _userManager.FindByIdAsync(user.Id);
            return tmp != null;
        }

        /// <summary>
        /// Finds a user by email.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<ApplicationUser?> FindByEmailAsync(string email) {
            return await _userManager.FindByEmailAsync(email);
        }

        /// <summary>
        /// Finds a user by email and includes related ProcessingPauses and TraineeLessons.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<ApplicationUser?> FindByEmailWithProcessingPausesAndTraineeLessonsAsync(string email) {
            return await _context.Users
            .Include(u => u.ProcessingPauses)
            .Include(u => u.TraineeLessons)
            .FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// Finds a user by ID.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<ApplicationUser?> FindByIdAsync(string userId) {
            return await _userManager.FindByIdAsync(userId);
        }

        /// <summary>
        /// Finds a user by ID and includes related LastSelectedTrainees.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<ApplicationUser?> FindByIdWithLastSelectedTraineesAsync(string userId) {
            return await _context.Users
            .Include(u => u.LastSelectedTrainees)
            .FirstOrDefaultAsync(u => u.Id == userId);
        }

        /// <summary>
        /// Finds a user by ID and includes related EmailNotificationSetting.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<ApplicationUser?> FindByIdWithNotificationSettingAsync(string userId) {
            return await _context.Users
                .Include(u => u.EmailNotificationSetting)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        /// <summary>
        /// Finds a user by ID and includes related TeachingPlan and TraineeLessons with Lessons.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<ApplicationUser?> FindByIdWithTeachingPlanAndTraineeLessonsWithLessonsAsync(string userId) {
            return await _context.Users
                .Include(u => u.TraineeLessons)
                    .ThenInclude(tl => tl.Lesson)
                .Include(u => u.TeachingPlan)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        /// <summary>
        /// Finds a user by ID and includes related TraineeLessons and ProcessingPauses.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<ApplicationUser?> FindByIdWithTraineeLessonsAndProcessingPausesAsync(string userId) {
            return await _context.Users
            .Include(u => u.ProcessingPauses)
            .Include(u => u.TraineeLessons)
            .FirstOrDefaultAsync(u => u.Id == userId);
        }

        /// <summary>
        /// Finds a user by ID and includes related ProcessingPauses.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<ApplicationUser?> FindByIdWithProcessingPausesAsync(string userId) {
            return await _context.Users
            .Include(u => u.ProcessingPauses)
            .FirstOrDefaultAsync(u => u.Id == userId);
        }

        /// <summary>
        /// Finds a user by ID and includes related WrittenFeedbacks with Lessons.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<ApplicationUser?> FindByIdWithWrittenFeedbacksWithLessonAsync(string userId) {
            return await _context.Users
                .Include(u => u.WrittenFeedbacks)
                    .ThenInclude(f => f.Lesson)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        /// <summary>
        /// Generates an email confirmation token for the specified user.
        /// </summary>
        /// <param name="user">The user for whom to generate the token.</param>
        /// <returns>The email confirmation token.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user) {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        /// <summary>
        /// Gets all open users in the specified role.
        /// </summary>
        /// <param name="roleName">The role name.</param>
        /// <returns>A collection of open users in the role.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<IEnumerable<ApplicationUser>> GetOpenUsersInRoleAsync(string roleName) {
            var usersInRole = await GetUsersInRoleAsync(roleName);
            var userIds = usersInRole.Select(u => u.Id).ToList();
            return await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .Where(u => !u.IsClosed)
            .ToListAsync();
        }

        /// <summary>
        /// Gets all closed users in the specified role.
        /// </summary>
        /// <param name="roleName">The role name.</param>
        /// <returns>A collection of closed users in the role.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<IEnumerable<ApplicationUser>> GetClosedUsersInRoleAsync(string roleName) {
            var usersInRole = await GetUsersInRoleAsync(roleName);
            var userIds = usersInRole.Select(u => u.Id).ToList();
            return await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .Where(u => u.IsClosed)
            .ToListAsync();
        }

        /// <summary>
        /// Gets all open users in the specified role including their EmailNotificationSetting.
        /// </summary>
        /// <param name="roleName">The role name.</param>
        /// <returns>A collection of open users in the role with EmailNotificationSetting.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<IEnumerable<ApplicationUser>> GetOpenUsersInRoleWithEmailNotificationSettingAsync(string roleName) {
            var users = await GetOpenUsersInRoleAsync(roleName);
            var userIds = users.Select(u => u.Id);
            return await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .Include(u => u.EmailNotificationSetting)
                .ToListAsync();
        }

        /// <summary>
        /// Gets the roles assigned to the specified user. If the user is an Admin, removes the Mentor role from the result.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>A collection of role names.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<IEnumerable<string>> GetRolesAsync(ApplicationUser user) {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin")) {
                roles.Remove("Mentor");
            }
            return roles;
        }

        /// <summary>
        /// Gets the user associated with the specified claims principal.
        /// </summary>
        /// <param name="principal">The claims principal.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<ApplicationUser?> GetUserAsync(ClaimsPrincipal principal) {
            return await _userManager.GetUserAsync(principal);
        }

        /// <summary>
        /// Gets all users in the specified role. If the role is "Mentor", excludes users who are also in the "Admin" role.
        /// </summary>
        /// <param name="roleName">The role name.</param>
        /// <returns>A collection of users in the role.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
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

        /// <summary>
        /// Gets all users in the specified role including their ProcessingPauses and TraineeLessons.
        /// </summary>
        /// <param name="roleName">The role name.</param>
        /// <returns>A collection of users in the role with related entities.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<IEnumerable<ApplicationUser>> GetUsersInRoleWithProcessingPausesAndTraineeLessonsAsync(string roleName) {
            var usersInRole = await GetUsersInRoleAsync(roleName);
            var userIds = usersInRole.Select(u => u.Id).ToList();
            return await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .Include(u => u.ProcessingPauses)
            .Include(u => u.TraineeLessons)
            .ToListAsync();
        }

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <returns>A collection of all users.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<IEnumerable<ApplicationUser>> GetAllAsync() {
            return await _context.Users.ToListAsync();
        }

        /// <summary>
        /// Gets all users filtered by their closed status.
        /// </summary>
        /// <param name="isClosed">True to get closed users; false to get open users.</param>
        /// <returns>A collection of users filtered by closed status.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<IEnumerable<ApplicationUser>> GetAllAsync(bool isClosed) {
            return await _context.Users.Where(u => u.IsClosed == isClosed).ToListAsync();
        }

        /// <summary>
        /// Determines whether the specified user is in the given role.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="role">The role name.</param>
        /// <returns>True if the user is in the role; otherwise, false.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<bool> IsInRoleAsync(ApplicationUser user, string role) {
            var roles = await GetRolesAsync(user);
            return roles.Contains(role);
        }

        /// <summary>
        /// Updates the specified user.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <returns>The result of the operation.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<IdentityResult> UpdateAsync(ApplicationUser user) {
            return await _userManager.UpdateAsync(user);
        }
    }
}
