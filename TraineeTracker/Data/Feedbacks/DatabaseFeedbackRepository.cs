using Microsoft.EntityFrameworkCore;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.Feedbacks {

    /// <summary>
    /// Provides CRUD operations for <see cref="Feedback"/> entities using Entity Framework Core.
    /// Implements the <see cref="IFeedbackRepository"/> interface.
    /// </summary>
    /// <remarks>
    /// Code Ownership: [Dein Name hier]
    /// </remarks>
    public class DatabaseFeedbackRepository : IFeedbackRepository {

        /// <summary>
        /// The Entity Framework Core database context used for accessing and modifying feedback entities.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseFeedbackRepository"/> class
        /// with a provided <see cref="ApplicationDbContext"/>.
        /// </summary>
        /// <param name="context">The database context used for accessing the Feedbacks table.</param>
        public DatabaseFeedbackRepository(ApplicationDbContext context) {
            _context = context;
        }

        
        /// <summary>
        /// Checks whether a <see cref="Feedback"/> entry with the given ID exists in the database.
        /// </summary>
        /// <param name="feedbackId">The ID of the feedback to check.</param>
        /// <returns>True if the feedback exists; otherwise, false.</returns>
        public async Task<bool> ExistsAsync(int feedbackId) {
            return await _context.Feedbacks.AnyAsync(f => f.FeedbackId == feedbackId);
        }

        /// <summary>
        /// Checks whether a <see cref="Feedback"/> entity already exists in the database.
        /// </summary>
        /// <param name="feedback">The feedback entity to check for existence.</param>
        /// <returns>True if the feedback exists; otherwise, false.</returns>
        public async Task<bool> ExistsAsync(Feedback feedback) {
            return await _context.Feedbacks.AnyAsync(f => f.FeedbackId == feedback.FeedbackId);
        }

        /// <summary>
        /// Adds a new <see cref="Feedback"/> entry to the database.
        /// </summary>
        /// <param name="feedback">The feedback entity to be persisted.</param>
        public async Task CreateAsync(Feedback feedback) {
            await _context.Feedbacks.AddAsync(feedback);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing <see cref="Feedback"/> entry in the database.
        /// </summary>
        /// <param name="feedback">The updated feedback entity.</param>
        /// <returns>A Task representing the asynchronous update operation.</returns>
        public async Task UpdateAsync(Feedback feedback) {
            _context.Feedbacks.Update(feedback);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a specific <see cref="Feedback"/> entity from the database.
        /// </summary>
        /// <param name="feedback">The feedback entity to delete.</param>
        /// <returns>A Task representing the asynchronous delete operation.</returns>
        public async Task DeleteAsync(Feedback feedback) {
            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a <see cref="Feedback"/> entry from the database by its ID.
        /// </summary>
        /// <param name="feedbackId">The ID of the feedback to delete.</param>
        /// <returns>A Task representing the asynchronous delete operation.</returns>
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
        public IQueryable<Feedback> GetAllFeedbacksUnreadByUserWithLessonAndAuthorAndReadByUsers(ApplicationUser user) {
            return _context.Feedbacks
                .Include(f => f.Lesson)
                .Include(f => f.Author)
                .Include(f => f.ReadByUsers)
                .Where(f => !f.ReadByUsers.Any(u => u.Id == user.Id));
        }
        public async Task<Feedback?> GetFeedbackOfTraineeLessonWithLessonAndAuthorAndReadByUsersAsync(TraineeLesson traineeLesson) {
            return await _context.Feedbacks
                .Include(f => f.Lesson)
                .Include(f => f.Author)
                .Include(f => f.ReadByUsers)
                .FirstOrDefaultAsync(f => f.AuthorId == traineeLesson.TraineeId && f.LessonId == traineeLesson.LessonId);
        }

        public IQueryable<Feedback> GetAllFeedbacksWithLessonAndAuthor() {
            return _context.Feedbacks
                .Include(f => f.Lesson)
                .Include(f => f.Author);

        }

        public IQueryable<Feedback> GetAllFeedbacksReadByUserWithLessonAndAuthor(ApplicationUser user) {
            return _context.Feedbacks
                .Include(f => f.Lesson)
                .Include(f => f.Author)
                .Where(f => f.ReadByUsers.Any(u => u.Id == user.Id));
        }

        public IQueryable<Feedback> GetAllFeedbacksUnreadByUserWithLessonAndAuthor(ApplicationUser user) {
            return _context.Feedbacks
                .Include(f => f.Lesson)
                .Include(f => f.Author)
                .Where(f => f.ReadByUsers.All(u => u.Id != user.Id));
        }
        public IQueryable<Feedback> GetAllFeedbacksWithLessonAndAuthorAndReadByUsers() {
            return _context.Feedbacks
                .Include(f => f.Lesson)
                .Include(f => f.Author)
                .Include(f => f.ReadByUsers);


        }
    }

}
