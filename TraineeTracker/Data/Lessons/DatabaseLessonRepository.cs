using TraineeTracker.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace TraineeTracker.Data.Lessons {

    /// <summary>
    /// Provides CRUD operations for <see cref="Lesson"/> entities using Entity Framework Core.
    /// Implements the <see cref="ILessonRepository"/> interface.
    /// Code Ownership: Alexandros Blask
    /// </summary>
    public class DatabaseLessonRepository : ILessonRepository {
        
        /// <summary>
        /// The Entity Framework Core database context used for accessing and modifying lesson entities.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseLessonRepository"/> class.
        /// </summary>
        /// <param name="context">The database context used for accessing the Lessons table.</param>
        public DatabaseLessonRepository(ApplicationDbContext context) {
            _context = context;
        }

        /// <summary>
        /// Checks whether a <see cref="Lesson"/> entry with the given ID exists in the database.
        /// </summary>
        /// <param name="id">The lesson ID to check.</param>
        /// <returns>True if the lesson exists; otherwise, false.</returns>
        public async Task<bool> ExistsAsync(int id) {
            return await _context.Lessons.AnyAsync(l => l.LessonId == id);
        }

        /// <summary>
        /// Checks whether a <see cref="Lesson"/> with the specified Makandra ID and TeachingPlan ID exists.
        /// </summary>
        /// <param name="makandraId">The Makandra ID.</param>
        /// <param name="teachingPlanId">The associated Teaching Plan ID.</param>
        /// <returns>True if a matching lesson exists; otherwise, false.</returns>
        public async Task<bool> ExistsAsync(string makandraId, int teachingPlanId) {
            return await _context.Lessons
                .AnyAsync(l => l.MakandraId == makandraId && l.TeachingPlanId == teachingPlanId);
        }

        /// <summary>
        /// Adds a new <see cref="Lesson"/> entry to the database.
        /// </summary>
        /// <param name="lesson">The lesson entity to be created.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task CreateAsync(Lesson lesson) {
            await _context.Lessons.AddAsync(lesson);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing <see cref="Lesson"/> in the database.
        /// </summary>
        /// <param name="lesson">The updated lesson entity.</param>
        /// <returns>A Task representing the asynchronous update operation.</returns>
        public async Task UpdateAsync(Lesson lesson) {
            _context.Lessons.Update(lesson);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a specific <see cref="Lesson"/> entity from the database.
        /// </summary>
        /// <param name="lesson">The lesson entity to delete.</param>
        /// <returns>A Task representing the asynchronous delete operation.</returns>
        public async Task DeleteAsync(Lesson lesson) {
            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves a <see cref="Lesson"/> by its ID, including related feedbacks.
        /// </summary>
        /// <param name="id">The lesson ID to retrieve.</param>
        /// <returns>The matching <see cref="Lesson"/> entity, or null if not found.</returns>
        public async Task<Lesson?> GetLessonByIdAsync(int id) {
            return await _context.Lessons
            .Include(l => l.Feedbacks)
            .FirstOrDefaultAsync(l => l.LessonId == id);
        }

        /// <summary>
        /// Retrieves all <see cref="Lesson"/> entities from the database, including their feedbacks.
        /// </summary>
        /// <returns>A list of all lessons with related feedbacks.</returns>
        public async Task<IEnumerable<Lesson>> GetAllLessonsAsync() {
            return await _context.Lessons
            .Include(l => l.Feedbacks)
            .ToListAsync();
        }
        
        /// <summary>
        /// Retrieves all <see cref="Lesson"/> entities along with their feedbacks.
        /// </summary>
        /// <returns>A list of all lessons including associated feedbacks.</returns>
        public async Task<IEnumerable<Lesson>> GetAllLessonsWithFeedbacksAsync() {
            return await _context.Lessons
                .Include(l => l.Feedbacks)
                .ToListAsync();
        }
    }
}
