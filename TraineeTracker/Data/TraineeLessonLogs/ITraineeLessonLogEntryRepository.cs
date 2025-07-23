using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.TraineeLessonLog {

    /// <summary>
    /// Defines methods for persisting and querying trainee lesson log entries.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Simon Hinterreiter (hintsimo)
    /// </remarks>
    public interface ITraineeLessonLogEntryRepository {

        /// <summary>
        /// Persists a new trainee lesson log entry to the database.
        /// </summary>
        /// <param name="log">The log entry to be stored.</param>
        void Create(TraineeLessonLogEntry log);

        /// <summary>
        /// Retrieves all log entries associated with a specific trainee lesson.
        /// </summary>
        /// <param name="traineeLessonId">The ID of the trainee lesson.</param>
        /// <returns>A collection of <see cref="TraineeLessonLogEntry"/> objects for the given lesson.</returns>
        IEnumerable<TraineeLessonLogEntry> GetAllLogsForTraineeLesson(int traineeLessonId);
    }
}
