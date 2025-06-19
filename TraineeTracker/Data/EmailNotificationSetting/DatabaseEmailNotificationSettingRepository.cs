using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data
{
    public class DatabaseEmailNotificationSettingRepository : IEmailNotificationSettingRepository
    {
        private readonly ApplicationDbContext _context;

        public DatabaseEmailNotificationSettingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Create(EmailNotificationSetting emailNotificationSetting)
        {
            _context.EmailNotificationSettings.Add(emailNotificationSetting);
            _context.SaveChanges();
        }

        public void Update(EmailNotificationSetting emailNotificationSetting)
        {
            _context.EmailNotificationSettings.Update(emailNotificationSetting);
            _context.SaveChanges();
        }

        public void Delete(EmailNotificationSetting emailNotificationSetting)
        {
            _context.EmailNotificationSettings.Remove(emailNotificationSetting);
            _context.SaveChanges();
        }
    }
}
