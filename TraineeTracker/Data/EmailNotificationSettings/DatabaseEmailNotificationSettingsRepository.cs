using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data
{
    public class DatabaseEmailNotificationSettingsRepository : IEmailNotificationSettingsRepository
    {
        private readonly ApplicationDbContext _context;

        public DatabaseEmailNotificationSettingsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Create(EmailNotificationSettings emailNotificationSettings)
        {
            _context.EmailNotificationSettings.Add(emailNotificationSettings);
            _context.SaveChanges();
        }

        public void Update(EmailNotificationSettings emailNotificationSettings)
        {
            _context.EmailNotificationSettings.Update(emailNotificationSettings);
            _context.SaveChanges();
        }

        public void Delete(EmailNotificationSettings emailNotificationSettings)
        {
            _context.EmailNotificationSettings.Remove(emailNotificationSettings);
            _context.SaveChanges();
        }
    }
}
