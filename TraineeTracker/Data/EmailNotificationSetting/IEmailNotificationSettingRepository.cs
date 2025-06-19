using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data
{
    public interface IEmailNotificationSettingRepository
    {
        void Create(EmailNotificationSetting emailNotificationSetting);
        void Update(EmailNotificationSetting emailNotificationSetting);
        void Delete(EmailNotificationSetting emailNotificationSetting);
    }
}
