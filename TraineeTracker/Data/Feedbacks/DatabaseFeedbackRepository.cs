using Microsoft.EntityFrameworkCore;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.Feedbacks {
    public class DatabaseFeedbackRepository : IFeedbackRepository {
        private readonly ApplicationDbContext _context;

        public DatabaseFeedbackRepository(ApplicationDbContext context) {
            _context = context;
        }

        // -------------------------------------------
        public bool Exists(int feedbackId) {
            return _context.Feedbacks.Any(f => f.FeedbackId == feedbackId);
        }

        // -------------------------------------------
        public bool Exists(Feedback feedback) {
            return _context.Feedbacks.Any(f => f.FeedbackId == feedback.FeedbackId);
        }

        // -------------------------------------------
        public void Create(Feedback feedback) {
            _context.Feedbacks.Add(feedback);
            _context.SaveChanges();
        }
        // -------------------------------------------
        public void Update(Feedback feedback)
        {
            _context.Feedbacks.Update(feedback);
            _context.SaveChanges();
        }

        // -------------------------------------------
        public void Delete(Feedback feedback) {
            _context.Feedbacks.Remove(feedback);
            _context.SaveChanges();
        }

        // -------------------------------------------
        public void Delete(int feedbackId) {
            var feedback = _context.Feedbacks.Find(feedbackId);
            if (feedback != null) {
                _context.Feedbacks.Remove(feedback);
                _context.SaveChanges();
            }
        }
        // -------------------------------------------
        public IEnumerable<Feedback> GetAllFeedbacksForLesson(Lesson lesson) {
            return _context.Feedbacks
                .Include(f => f.Lesson)
                .Include(f => f.Author)
                .Include(f => f.ReadByUsers)
                .Where(f => f.LessonId == lesson.LessonId)
                .ToList();
        }

        // -------------------------------------------
        public IEnumerable<Feedback> GetAllFeedbacksWrittenByUser(ApplicationUser user) {
            return _context.Feedbacks
                .Include(f => f.Lesson)
                .Include(f => f.Author)
                .Include(f => f.ReadByUsers)
                .Where(f => f.AuthorId == user.Id)
                .ToList();
        }

        // -------------------------------------------
        public IEnumerable<Feedback> GetAllFeedbacksReadByUser(ApplicationUser user) {
            return _context.Feedbacks
                .Include(f => f.Lesson)
                .Include(f => f.Author)
                .Include(f => f.ReadByUsers)
                .Where(f => f.ReadByUsers.Any(u => u.Id == user.Id))
                .ToList();
        }
        // -------------------------------------------
        public IEnumerable<Feedback> GetAllFeedbacksUnreadByUser(ApplicationUser user) {
            return _context.Feedbacks
                .Include(f => f.Lesson)
                .Include(f => f.Author)
                .Include(f => f.ReadByUsers)
                .Where(f => !f.ReadByUsers.Any(u => u.Id == user.Id))
                .ToList();
        }

        public Feedback? GetFeedbackOfTraineeLesson(TraineeLesson traineeLesson) {
            return _context.Feedbacks
                .Include(f => f.Lesson)
                .Include(f => f.Author)
                .Include(f => f.ReadByUsers)
                .FirstOrDefault(f => f.AuthorId == traineeLesson.TraineeId && f.LessonId == traineeLesson.LessonId);
        }
        // -------------------------------------------




    }
}
