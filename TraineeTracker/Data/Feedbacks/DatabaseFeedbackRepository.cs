using Microsoft.EntityFrameworkCore;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.Feedbacks {

    // ------------------------------------------------------
    /// <summary>
    /// Provides CRUD operations for <see cref="Feedback"/> entities using Entity Framework Core.
    /// Implements the <see cref="IFeedbackRepository"/> interface.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexandros Blask
    /// </remarks>
    public class DatabaseFeedbackRepository : IFeedbackRepository {

        /// <summary>
        /// The Entity Framework Core database context used for accessing and modifying feedback entities.
        /// </summary>
        private readonly ApplicationDbContext _context;

        // ------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseFeedbackRepository"/> class
        /// with a provided <see cref="ApplicationDbContext"/>.
        /// </summary>
        /// <param name="context">The database context used for accessing the Feedbacks table.</param>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        public DatabaseFeedbackRepository(ApplicationDbContext context) {
            _context = context;
        }

        // ------------------------------------------------------
        /// <summary>
        /// Checks whether a <see cref="Feedback"/> entry with the given ID exists in the database.
        /// </summary>
        /// <param name="feedbackId">The ID of the feedback to check.</param>
        /// <returns>True if the feedback exists; otherwise, false.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        public async Task<bool> ExistsAsync(int feedbackId) {
            return await _context.Feedbacks.AnyAsync(f => f.FeedbackId == feedbackId);
        }

        // ------------------------------------------------------
        /// <summary>
        /// Checks whether a <see cref="Feedback"/> entity already exists in the database.
        /// </summary>
        /// <param name="feedback">The feedback entity to check for existence.</param>
        /// <returns>True if the feedback exists; otherwise, false.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        public async Task<bool> ExistsAsync(Feedback feedback) {
            return await _context.Feedbacks.AnyAsync(f => f.FeedbackId == feedback.FeedbackId);
        }

        // ------------------------------------------------------
        /// <summary>
        /// Adds a new <see cref="Feedback"/> entry to the database.
        /// </summary>
        /// <param name="feedback">The feedback entity to be persisted.</param>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        public async Task CreateAsync(Feedback feedback) {
            await _context.Feedbacks.AddAsync(feedback);
            await _context.SaveChangesAsync();
        }

        // ------------------------------------------------------
        /// <summary>
        /// Updates an existing <see cref="Feedback"/> entry in the database.
        /// </summary>
        /// <param name="feedback">The updated feedback entity.</param>
        /// <returns>A Task representing the asynchronous update operation.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        public async Task UpdateAsync(Feedback feedback) {
            _context.Feedbacks.Update(feedback);
            await _context.SaveChangesAsync();
        }

        // ------------------------------------------------------
        /// <summary>
        /// Deletes a specific <see cref="Feedback"/> entity from the database.
        /// </summary>
        /// <param name="feedback">The feedback entity to delete.</param>
        /// <returns>A Task representing the asynchronous delete operation.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        public async Task DeleteAsync(Feedback feedback) {
            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();
        }

        // ------------------------------------------------------
        /// <summary>
        /// Deletes a <see cref="Feedback"/> entry from the database by its ID.
        /// </summary>
        /// <param name="feedbackId">The ID of the feedback to delete.</param>
        /// <returns>A Task representing the asynchronous delete operation.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        public async Task DeleteAsync(int feedbackId) {
            var feedback = await _context.Feedbacks.FindAsync(feedbackId);
            if (feedback != null) {
                _context.Feedbacks.Remove(feedback);
                await _context.SaveChangesAsync();
            }
        }

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves a <see cref="Feedback"/> by its ID, including related Lesson, Author, and ReadByUsers.
        /// </summary>
        /// <param name="feedbackId">The ID of the feedback to retrieve.</param>
        /// <returns>The matching Feedback, or null if not found.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        public async Task<Feedback?> GetFeedbackByIDWithLessonAndAuthorAndReadByUsersAsync(int feedbackId) {
            return await _context.Feedbacks
                .Include(f => f.Lesson)
                .Include(f => f.Author)
                .Include(f => f.ReadByUsers)
                .FirstOrDefaultAsync(f => f.FeedbackId == feedbackId);
        }

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves all <see cref="Feedback"/> entries for a given Lesson, including related entities.
        /// </summary>
        /// <param name="lesson">The Lesson to filter feedback by.</param>
        /// <returns>An enumerable of matching Feedback entities.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        public async Task<IEnumerable<Feedback>> GetAllFeedbacksForLessonWithLessonAndAuthorAndReadByUsersAsync(Lesson lesson) {
            return await _context.Feedbacks
                .Include(f => f.Lesson)
                .Include(f => f.Author)
                .Include(f => f.ReadByUsers)
                .Where(f => f.LessonId == lesson.LessonId)
                .ToListAsync();
        }

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves all <see cref="Feedback"/> entries, including related entities.
        /// </summary>
        /// <returns>A list of all Feedback entries.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        public async Task<List<Feedback>> GetAllFeedbacksWithLessonAndAuthorAndReadByUsersAsync() {
            return await _context.Feedbacks
                .Include(f => f.Lesson)
                .Include(f => f.Author)
                .Include(f => f.ReadByUsers)
                .ToListAsync();
        }

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves all Feedback written by a specific user, including related entities.
        /// </summary>
        /// <param name="user">The authoring user.</param>
        /// <returns>An enumerable of Feedback authored by the user.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        public async Task<IEnumerable<Feedback>> GetAllFeedbacksWrittenByUserWithLessonAndAuthorAndReadByUsersAsync(ApplicationUser user) {
            return await _context.Feedbacks
                .Include(f => f.Lesson)
                .Include(f => f.Author)
                .Include(f => f.ReadByUsers)
                .Where(f => f.AuthorId == user.Id)
                .ToListAsync();
        }

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves all Feedback read by a specific user, including related entities.
        /// </summary>
        /// <param name="user">The user who read the feedback.</param>
        /// <returns>A list of Feedback read by the user.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        public async Task<List<Feedback>> GetAllFeedbacksReadByUserWithLessonAndAuthorAndReadByUsersAsync(ApplicationUser user) {
            return await _context.Feedbacks
                .Include(f => f.Lesson)
                .Include(f => f.Author)
                .Include(f => f.ReadByUsers)
                .Where(f => f.ReadByUsers.Any(u => u.Id == user.Id))
                .ToListAsync();
        }

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves all Feedback unread by a specific user, including related entities.
        /// </summary>
        /// <param name="user">The user to filter unread feedback by.</param>
        /// <returns>A list of unread Feedback for the user.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        public async Task<List<Feedback>> GetAllFeedbacksUnreadByUserWithLessonAndAuthorAndReadByUsersAsync(ApplicationUser user) {
            return await _context.Feedbacks
                .Include(f => f.Lesson)
                .Include(f => f.Author)
                .Include(f => f.ReadByUsers)
                .Where(f => !f.ReadByUsers.Any(u => u.Id == user.Id))
                .ToListAsync();
        }
        
        // ------------------------------------------------------
        /// <summary>
        /// Retrieves an <see cref="IQueryable{Feedback}"/> of unread Feedback for a specific user, including related entities.
        /// </summary>
        /// <param name="user">The user to filter unread feedback by.</param>
        /// <returns>An IQueryable of unread Feedback for the user.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        public IQueryable<Feedback> GetAllFeedbacksUnreadByUserWithLessonAndAuthorAndReadByUsers(ApplicationUser user) {
            return _context.Feedbacks
                .Include(f => f.Lesson)
                .Include(f => f.Author)
                .Include(f => f.ReadByUsers)
                .Where(f => !f.ReadByUsers.Any(u => u.Id == user.Id));
        }
        
        // ------------------------------------------------------
        /// <summary>
        /// Retrieves the feedback associated with a specific TraineeLesson, including related entities.
        /// </summary>
        /// <param name="traineeLesson">The TraineeLesson linking trainee and lesson.</param>
        /// <returns>The matching Feedback, or null if not found.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        public async Task<Feedback?> GetFeedbackOfTraineeLessonWithLessonAndAuthorAndReadByUsersAsync(TraineeLesson traineeLesson) {
            return await _context.Feedbacks
                .Include(f => f.Lesson)
                .Include(f => f.Author)
                .Include(f => f.ReadByUsers)
                .FirstOrDefaultAsync(f => f.AuthorId == traineeLesson.TraineeId && f.LessonId == traineeLesson.LessonId);
        }

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves an <see cref="IQueryable{Feedback}"/> of all Feedback including Lesson and Author.
        /// </summary>
        /// <returns>An IQueryable of all Feedback entities with related Lesson and Author.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        public IQueryable<Feedback> GetAllFeedbacksWithLessonAndAuthor() {
            return _context.Feedbacks
                .Include(f => f.Lesson)
                .Include(f => f.Author);

        }

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves an <see cref="IQueryable{Feedback}"/> of Feedback read by a specific user, including Lesson and Author.
        /// </summary>
        /// <param name="user">The user who read the feedback.</param>
        /// <returns>An IQueryable of read Feedback for the user.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        public IQueryable<Feedback> GetAllFeedbacksReadByUserWithLessonAndAuthor(ApplicationUser user) {
            return _context.Feedbacks
                .Include(f => f.Lesson)
                .Include(f => f.Author)
                .Where(f => f.ReadByUsers.Any(u => u.Id == user.Id));
        }

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves an <see cref="IQueryable{Feedback}"/> of Feedback unread by a specific user, including Lesson and Author.
        /// </summary>
        /// <param name="user">The user to filter unread feedback by.</param>
        /// <returns>An IQueryable of unread Feedback for the user.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        public IQueryable<Feedback> GetAllFeedbacksUnreadByUserWithLessonAndAuthor(ApplicationUser user) {
            return _context.Feedbacks
                .Include(f => f.Lesson)
                .Include(f => f.Author)
                .Where(f => f.ReadByUsers.All(u => u.Id != user.Id));
        }
        
        // ------------------------------------------------------
        /// <summary>
        /// Retrieves an <see cref="IQueryable{Feedback}"/> of all Feedback including Lesson, Author, and ReadByUsers.
        /// </summary>
        /// <returns>An IQueryable of all Feedback entities with related Lesson, Author, and ReadByUsers.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        public IQueryable<Feedback> GetAllFeedbacksWithLessonAndAuthorAndReadByUsers() {
            return _context.Feedbacks
                .Include(f => f.Lesson)
                .Include(f => f.Author)
                .Include(f => f.ReadByUsers);


        }
    }

}
