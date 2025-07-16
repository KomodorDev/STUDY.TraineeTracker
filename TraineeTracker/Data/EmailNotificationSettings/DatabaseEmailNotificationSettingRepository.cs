using Microsoft.EntityFrameworkCore;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.EmailNotificationSettings {

    /// <summary>
    /// Provides CRUD operations for <see cref="EmailNotificationSetting"/> entities using Entity Framework Core.
    /// Implements the <see cref="IEmailNotificationSettingRepository"/> interface.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Simon Hinterreiter (hintsimo)
    /// </remarks>
    public class DatabaseEmailNotificationSettingRepository : IEmailNotificationSettingRepository {

        /// <summary>
        /// The Entity Framework Core database context used for accessing and modifying notification settings.
        /// </summary>
        private readonly ApplicationDbContext _context;

        // ------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseEmailNotificationSettingRepository"/> class
        /// with a provided <see cref="ApplicationDbContext"/>.
        /// </summary>
        /// <param name="context">The database context used for accessing the EmailNotificationSettings table.</param>
        /// <remarks>
        /// Code Ownership: Simon Hinterreiter (hintsimo)
        /// </remarks>
        public DatabaseEmailNotificationSettingRepository(ApplicationDbContext context) {
            _context = context;
        }

        // ------------------------------------------------------
        /// <summary>
        /// Adds a new <see cref="EmailNotificationSetting"/> entry to the database.
        /// </summary>
        /// <param name="emailNotificationSetting">The setting to be persisted.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <remarks>
        /// Code Ownership: Simon Hinterreiter (hintsimo)
        /// </remarks>
        public async Task CreateAsync(EmailNotificationSetting emailNotificationSetting) {
            _context.EmailNotificationSettings.Add(emailNotificationSetting);
            await _context.SaveChangesAsync();
        }

        // ------------------------------------------------------
        /// <summary>
        /// Updates an existing <see cref="EmailNotificationSetting"/> in the database.
        /// </summary>
        /// <param name="emailNotificationSetting">The updated setting instance.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <remarks>
        /// Code Ownership: Simon Hinterreiter (hintsimo)
        /// </remarks>
        public async Task UpdateAsync(EmailNotificationSetting emailNotificationSetting) {
            _context.EmailNotificationSettings.Update(emailNotificationSetting);
            await _context.SaveChangesAsync();
        }

        // ------------------------------------------------------
        /// <summary>
        /// Removes an existing <see cref="EmailNotificationSetting"/> from the database.
        /// </summary>
        /// <param name="emailNotificationSetting">The setting instance to delete.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <remarks>
        /// Code Ownership: Simon Hinterreiter (hintsimo)
        /// </remarks>
        public async Task DeleteAsync(EmailNotificationSetting emailNotificationSetting) {
            _context.EmailNotificationSettings.Remove(emailNotificationSetting);
            await _context.SaveChangesAsync();
        }

        // ------------------------------------------------------
        /// <summary>
        /// Checks whether an email notification setting exists for a given user ID.
        /// </summary>
        /// <param name="userId">The ID of the user to check for.</param>
        /// <returns>
        /// A Task representing the asynchronous operation. The task result is true if a setting exists; otherwise, false.
        /// </returns>
        /// <remarks>
        /// Code Ownership: Simon Hinterreiter (hintsimo)
        /// </remarks>
        public async Task<bool> ExistsAsync(string userId) {
            return await _context.EmailNotificationSettings.AnyAsync(s => s.UserId == userId);
        }

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves the email notification setting for a specific user ID.
        /// Throws an <see cref="InvalidOperationException"/> if no entry is found.
        /// </summary>
        /// <param name="userId">The ID of the user whose settings are to be retrieved.</param>
        /// <returns>
        /// A Task representing the asynchronous operation. The task result contains the <see cref="EmailNotificationSetting"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown when no setting is found for the specified user ID.</exception>
        /// <remarks>
        /// Code Ownership: Simon Hinterreiter (hintsimo)
        /// </remarks>
        public async Task<EmailNotificationSetting> GetByUserIdAsync(string userId) {
            var setting = await _context.EmailNotificationSettings
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (setting == null)
                throw new InvalidOperationException($"No EmailNotificationSetting found for user with ID '{userId}'.");

            return setting;
        }

        // ------------------------------------------------------

    }
}
