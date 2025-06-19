using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data
{
    public interface IEmailNotificationSettingsRepository
    {
        void Create(EmailNotificationSettings emailNotificationSettings);
        void Update(EmailNotificationSettings emailNotificationSettings);
        void Delete(EmailNotificationSettings emailNotificationSettings);
    }
}
