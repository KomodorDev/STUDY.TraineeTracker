using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.EmailNotificationSettings {

    /// <summary>
    /// Defines data access operations for managing <see cref="EmailNotificationSetting"/> records.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Simon Hinterreiter (hintsimo)
    /// </remarks>
    public interface IEmailNotificationSettingRepository {

        // ------------------------------------------------------
        /// <summary>
        /// Adds a new email notification setting to the data store.
        /// </summary>
        /// <param name="emailNotificationSetting">The notification setting to be created.</param>
        /// <returns>A Task representing the asynchronous creation operation.</returns>
        Task CreateAsync(EmailNotificationSetting emailNotificationSetting);

        // ------------------------------------------------------
        /// <summary>
        /// Updates an existing email notification setting in the data store.
        /// </summary>
        /// <param name="emailNotificationSetting">The updated notification setting to store.</param>
        /// <returns>A Task representing the asynchronous update operation.</returns>
        Task UpdateAsync(EmailNotificationSetting emailNotificationSetting);

        // ------------------------------------------------------
        /// <summary>
        /// Deletes a specific email notification setting from the data store.
        /// </summary>
        /// <param name="emailNotificationSetting">The setting to be removed.</param>
        /// <returns>A Task representing the asynchronous delete operation.</returns>
        Task DeleteAsync(EmailNotificationSetting emailNotificationSetting);

        // ------------------------------------------------------
        /// <summary>
        /// Checks whether a notification setting exists for the specified user ID.
        /// </summary>
        /// <param name="userId">The ID of the user to check for.</param>
        /// <returns>A Task that resolves to true if a setting exists, otherwise false.</returns>
        Task<bool> ExistsAsync(string userId);

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves the notification setting for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose setting is being requested.</param>
        /// <returns>A Task resolving to the <see cref="EmailNotificationSetting"/> of the user.</returns>
        Task<EmailNotificationSetting> GetByUserIdAsync(string userId);

        // ------------------------------------------------------
    }
}
