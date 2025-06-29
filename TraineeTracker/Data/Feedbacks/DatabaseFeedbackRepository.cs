using Microsoft.EntityFrameworkCore;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.Feedbacks {
    public class DatabaseFeedbackRepository : IFeedbackRepository {
        private readonly ApplicationDbContext _context;

        public DatabaseFeedbackRepository(ApplicationDbContext context) {
            _context = context;
        }

        public async Task<bool> ExistsAsync(int feedbackId) {
            return await _context.Feedbacks.AnyAsync(f => f.FeedbackId == feedbackId);
        }

        public async Task<bool> ExistsAsync(Feedback feedback) {
            return await _context.Feedbacks.AnyAsync(f => f.FeedbackId == feedback.FeedbackId);
        }

        public async Task CreateAsync(Feedback feedback) {
            await _context.Feedbacks.AddAsync(feedback);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Feedback feedback)
        {
            _context.Feedbacks.Update(feedback);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Feedback feedback) {
            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int feedbackId) {
            var feedback = await _context.Feedbacks.FindAsync(feedbackId);
            if (feedback != null) {
                _context.Feedbacks.Remove(feedback);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Feedback?> GetFeedbackByIDWithLessonAndAuthorAndReadByUsersAsync(int feedbackId) {
            return await _context.Feedbacks
                .Include(f => f.Lesson)
                .Include(f => f.Author)
                .Include(f => f.ReadByUsers)
                .FirstOrDefaultAsync(f => f.FeedbackId == feedbackId);
        }

        public async Task<IEnumerable<Feedback>> GetAllFeedbacksForLessonWithLessonAndAuthorAndReadByUsersAsync(Lesson lesson) {
            return await _context.Feedbacks
                .Include(f => f.Lesson)
                .Include(f => f.Author)
                .Include(f => f.ReadByUsers)
                .Where(f => f.LessonId == lesson.LessonId)
                .ToListAsync();
        }

        public async Task<List<Feedback>> GetAllFeedbacksWithLessonAndAuthorAndReadByUsersAsync() {
            return await _context.Feedbacks
                .Include(f => f.Lesson)
                .Include(f => f.Author)
                .Include(f => f.ReadByUsers)
                .ToListAsync();
        }

        public async Task<IEnumerable<Feedback>> GetAllFeedbacksWrittenByUserWithLessonAndAuthorAndReadByUsersAsync(ApplicationUser user) {
            return await _context.Feedbacks
                .Include(f => f.Lesson)
                .Include(f => f.Author)
                .Include(f => f.ReadByUsers)
                .Where(f => f.AuthorId == user.Id)
                .ToListAsync();
        }

        public async Task<List<Feedback>> GetAllFeedbacksReadByUserWithLessonAndAuthorAndReadByUsersAsync(ApplicationUser user) {
            return await _context.Feedbacks
                .Include(f => f.Lesson)
                .Include(f => f.Author)
                .Include(f => f.ReadByUsers)
                .Where(f => f.ReadByUsers.Any(u => u.Id == user.Id))
                .ToListAsync();
        }

        public async Task<List<Feedback>> GetAllFeedbacksUnreadByUserWithLessonAndAuthorAndReadByUsersAsync(ApplicationUser user) {
            return await _context.Feedbacks
                .Include(f => f.Lesson)
                .Include(f => f.Author)
                .Include(f => f.ReadByUsers)
                .Where(f => !f.ReadByUsers.Any(u => u.Id == user.Id))
                .ToListAsync();
        }

        public async Task<Feedback?> GetFeedbackOfTraineeLessonWithLessonAndAuthorAndReadByUsersAsync(TraineeLesson traineeLesson) {
            return await _context.Feedbacks
                .Include(f => f.Lesson)
                .Include(f => f.Author)
                .Include(f => f.ReadByUsers)
                .FirstOrDefaultAsync(f => f.AuthorId == traineeLesson.TraineeId && f.LessonId == traineeLesson.LessonId);
        }
    }
}
