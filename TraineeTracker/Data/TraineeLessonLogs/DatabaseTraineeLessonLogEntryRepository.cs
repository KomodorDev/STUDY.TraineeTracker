using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.TraineeLessonLog {

    /// <summary>
    /// Repository implementation for persisting and retrieving trainee lesson log entries using Entity Framework Core.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Simon Hinterreiter (hintsimo)
    /// </remarks>
    public class DatabaseTraineeLessonLogEntryRepository : ITraineeLessonLogEntryRepository {

        /// <summary>
        /// The Entity Framework Core database context used for accessing and modifying notification settings.
        /// </summary>
        private readonly ApplicationDbContext _context;

        // ------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseTraineeLessonLogEntryRepository"/> class.
        /// </summary>
        /// <param name="context">The application's database context.</param>
        /// <remarks>
        /// Code Ownership: Simon Hinterreiter (hintsimo)
        /// </remarks>
        public DatabaseTraineeLessonLogEntryRepository(ApplicationDbContext context) {
            _context = context;
        }

        // ------------------------------------------------------
        /// <summary>
        /// Persists a new trainee lesson log entry to the database.
        /// </summary>
        /// <param name="log">The log entry to create.</param>
        /// <remarks>
        /// Code Ownership: Simon Hinterreiter (hintsimo)
        /// </remarks>
        public void Create(TraineeLessonLogEntry log) {
            _context.TraineeLessonLogEntries.Add(log);
            _context.SaveChanges();
        }

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves all log entries for a specific trainee lesson, sorted by timestamp in descending order.
        /// </summary>
        /// <param name="traineeLessonId">The ID of the trainee lesson.</param>
        /// <returns>An enumerable collection of <see cref="TraineeLessonLogEntry"/> for the specified lesson.</returns>
        /// <remarks>
        /// Code Ownership: Simon Hinterreiter (hintsimo)
        /// </remarks>
        public IEnumerable<TraineeLessonLogEntry> GetAllLogsForTraineeLesson(int traineeLessonId) {
            return _context.TraineeLessonLogEntries
                .Where(l => l.TraineeLessonId == traineeLessonId)
                .OrderByDescending(l => l.Timestamp)
                .ToList();
        }

        // ------------------------------------------------------
    }
}
