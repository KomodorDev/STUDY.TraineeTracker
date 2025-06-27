using Microsoft.EntityFrameworkCore;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data {
    public class DatabaseEmailNotificationSettingRepository : IEmailNotificationSettingRepository {
        private readonly ApplicationDbContext _context;

        public DatabaseEmailNotificationSettingRepository(ApplicationDbContext context) {
            _context = context;
        }

        public async Task CreateAsync(EmailNotificationSetting emailNotificationSetting) {
            _context.EmailNotificationSettings.Add(emailNotificationSetting);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(EmailNotificationSetting emailNotificationSetting) {
            _context.EmailNotificationSettings.Update(emailNotificationSetting);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(EmailNotificationSetting emailNotificationSetting) {
            _context.EmailNotificationSettings.Remove(emailNotificationSetting);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string userId) {
            return await _context.EmailNotificationSettings.AnyAsync(s => s.UserId == userId);
        }

        public async Task<EmailNotificationSetting> GetByUserIdAsync(string userId) {
            var setting = await _context.EmailNotificationSettings
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (setting == null)
                throw new InvalidOperationException($"No EmailNotificationSetting found for user with ID '{userId}'.");

            return setting;
        }

    }
}
