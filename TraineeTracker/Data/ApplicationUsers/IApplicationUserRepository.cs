using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.ApplicationUsers {

    /// <summary>
    /// Defines methods for managing <see cref="ApplicationUser"/> entities, including creation, deletion, role assignment,
    /// retrieval with related data, and user existence checks.
    /// </summary>
    /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
    public interface IApplicationUserRepository {

        // ------------------------------------------------------
        /// <summary>
        /// Adds the specified user to the given role asynchronously.
        /// </summary>
        /// <param name="user">The user to add to the role.</param>
        /// <param name="role">The name of the role.</param>
        /// <returns>The result of the role assignment operation.</returns>
        Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role);

        // ------------------------------------------------------
        /// <summary>
        /// Creates a new user with the specified password asynchronously.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <param name="password">The password for the user.</param>
        /// <returns>The result of the creation operation.</returns>
        Task<IdentityResult> CreateAsync(ApplicationUser user, string password);

        // ------------------------------------------------------
        /// <summary>
        /// Deletes the specified user asynchronously.
        /// </summary>
        /// <param name="user">The user to delete.</param>
        /// <returns>The result of the deletion operation.</returns>
        Task<IdentityResult> DeleteAsync(ApplicationUser user);

        // ------------------------------------------------------
        /// <summary>
        /// Checks if the specified user exists asynchronously.
        /// </summary>
        /// <param name="user">The user to check.</param>
        /// <returns><c>true</c> if the user exists; otherwise, <c>false</c>.</returns>
        Task<bool> ExistsAsync(ApplicationUser user);

        // ------------------------------------------------------
        /// <summary>
        /// Asynchronously checks if an application user exists with the specified email address.
        /// </summary>
        /// <param name="email">The email address to check for existence.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains <c>true</c> if a user with the specified email exists; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> ExistsByEmailAsync(string email);

        // ------------------------------------------------------
        /// <summary>
        /// Asynchronously checks whether a user with the specified ID exists in the repository.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to check for existence.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains <c>true</c> if the user exists; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> ExistsByIdAsync(string userId);

        // ------------------------------------------------------
        /// <summary>
        /// Asynchronously finds an <see cref="ApplicationUser"/> by their email address.
        /// </summary>
        /// <param name="email">The email address of the user to find.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the <see cref="ApplicationUser"/> if found; otherwise, <c>null</c>.
        /// </returns>
        Task<ApplicationUser?> FindByEmailAsync(string email);

        // ------------------------------------------------------
        /// <summary>
        /// Finds a user by their email address and includes processing pauses and trainee lessons asynchronously.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <returns>The user with related data if found; otherwise, <c>null</c>.</returns>
        Task<ApplicationUser?> FindByEmailWithProcessingPausesAndTraineeLessonsAsync(string email);

        // ------------------------------------------------------
        /// <summary>
        /// Finds a user by their ID asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The user if found; otherwise, <c>null</c>.</returns>
        Task<ApplicationUser?> FindByIdAsync(string userId);

        // ------------------------------------------------------
        /// <summary>
        /// Finds a user by their ID and includes last selected trainees asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The user with last selected trainees if found; otherwise, <c>null</c>.</returns>
        Task<ApplicationUser?> FindByIdWithLastSelectedTraineesAsync(string userId);

        // ------------------------------------------------------
        /// <summary>
        /// Finds a user by their ID and includes notification settings asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The user with notification settings if found; otherwise, <c>null</c>.</returns>
        Task<ApplicationUser?> FindByIdWithNotificationSettingAsync(string userId);

        // ------------------------------------------------------
        /// <summary>
        /// Finds a user by their ID and includes teaching plan and trainee lessons with lessons asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The user with teaching plan and lessons if found; otherwise, <c>null</c>.</returns>
        Task<ApplicationUser?> FindByIdWithTeachingPlanAndTraineeLessonsWithLessonsAsync(string userId);

        // ------------------------------------------------------
        /// <summary>
        /// Finds a user by their ID and includes trainee lessons and processing pauses asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The user with trainee lessons and processing pauses if found; otherwise, <c>null</c>.</returns>
        Task<ApplicationUser?> FindByIdWithTraineeLessonsAndProcessingPausesAsync(string userId);

        // ------------------------------------------------------
        /// <summary>
        /// Finds a user by their ID and includes processing pauses asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The user with processing pauses if found; otherwise, <c>null</c>.</returns>
        Task<ApplicationUser?> FindByIdWithProcessingPausesAsync(string userId);

        // ------------------------------------------------------
        /// <summary>
        /// Finds a user by their ID and includes written feedbacks with lesson asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The user with written feedbacks and lessons if found; otherwise, <c>null</c>.</returns>
        Task<ApplicationUser?> FindByIdWithWrittenFeedbacksWithLessonAsync(string userId);

        // ------------------------------------------------------
        /// <summary>
        /// Generates an email confirmation token for the specified user asynchronously.
        /// </summary>
        /// <param name="user">The user for whom to generate the token.</param>
        /// <returns>The email confirmation token.</returns>
        Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);

        // ------------------------------------------------------
        /// <summary>
        /// Gets all open users in the specified role asynchronously.
        /// </summary>
        /// <param name="roleName">The name of the role.</param>
        /// <returns>A collection of open users in the role.</returns>
        Task<IEnumerable<ApplicationUser>> GetOpenUsersInRoleAsync(string roleName);

        // ------------------------------------------------------
        /// <summary>
        /// Gets all closed users in the specified role asynchronously.
        /// </summary>
        /// <param name="roleName">The name of the role.</param>
        /// <returns>A collection of closed users in the role.</returns>
        Task<IEnumerable<ApplicationUser>> GetClosedUsersInRoleAsync(string roleName);

        // ------------------------------------------------------
        /// <summary>
        /// Gets all open users in the specified role with email notification settings asynchronously.
        /// </summary>
        /// <param name="roleName">The name of the role.</param>
        /// <returns>A collection of open users in the role with email notification settings.</returns>
        Task<IEnumerable<ApplicationUser>> GetOpenUsersInRoleWithEmailNotificationSettingAsync(string roleName);

        // ------------------------------------------------------
        /// <summary>
        /// Gets all roles assigned to the specified user asynchronously.
        /// </summary>
        /// <param name="user">The user whose roles to retrieve.</param>
        /// <returns>A collection of role names.</returns>
        Task<IEnumerable<string>> GetRolesAsync(ApplicationUser user);

        // ------------------------------------------------------
        /// <summary>
        /// Gets the user associated with the specified claims principal asynchronously.
        /// </summary>
        /// <param name="principal">The claims principal.</param>
        /// <returns>The user if found; otherwise, <c>null</c>.</returns>
        Task<ApplicationUser?> GetUserAsync(ClaimsPrincipal principal);

        // ------------------------------------------------------
        /// <summary>
        /// Gets all users in the specified role asynchronously.
        /// </summary>
        /// <param name="roleName">The name of the role.</param>
        /// <returns>A collection of users in the role.</returns>
        Task<IEnumerable<ApplicationUser>> GetUsersInRoleAsync(string roleName);

        // ------------------------------------------------------
        /// <summary>
        /// Gets all users in the specified role with processing pauses and trainee lessons asynchronously.
        /// </summary>
        /// <param name="roleName">The name of the role.</param>
        /// <returns>A collection of users in the role with related data.</returns>
        Task<IEnumerable<ApplicationUser>> GetUsersInRoleWithProcessingPausesAndTraineeLessonsAsync(string roleName);

        // ------------------------------------------------------
        /// <summary>
        /// Gets all users asynchronously.
        /// </summary>
        /// <returns>A collection of all users.</returns>
        Task<IEnumerable<ApplicationUser>> GetAllAsync();

        // ------------------------------------------------------
        /// <summary>
        /// Gets all users filtered by their closed status asynchronously.
        /// </summary>
        /// <param name="isClosed">Whether to retrieve closed users.</param>
        /// <returns>A collection of users filtered by closed status.</returns>
        Task<IEnumerable<ApplicationUser>> GetAllAsync(bool isClosed);

        // ------------------------------------------------------
        /// <summary>
        /// Determines whether the specified user is in the given role asynchronously.
        /// </summary>
        /// <param name="user">The user to check.</param>
        /// <param name="role">The name of the role.</param>
        /// <returns><c>true</c> if the user is in the role; otherwise, <c>false</c>.</returns>
        Task<bool> IsInRoleAsync(ApplicationUser user, string role);

        // ------------------------------------------------------
        /// <summary>
        /// Updates the specified user asynchronously.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <returns>The result of the update operation.</returns>
        Task<IdentityResult> UpdateAsync(ApplicationUser user);
    }

    // ------------------------------------------------------
}
