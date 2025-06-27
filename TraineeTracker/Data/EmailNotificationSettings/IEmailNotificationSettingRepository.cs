using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.EmailNotificationSettings {
    public interface IEmailNotificationSettingRepository {
        Task CreateAsync(EmailNotificationSetting emailNotificationSetting);
        Task UpdateAsync(EmailNotificationSetting emailNotificationSetting);
        Task DeleteAsync(EmailNotificationSetting emailNotificationSetting);
        Task<bool> ExistsAsync(string userId);
        Task<EmailNotificationSetting> GetByUserIdAsync(string userId);
    }
}
