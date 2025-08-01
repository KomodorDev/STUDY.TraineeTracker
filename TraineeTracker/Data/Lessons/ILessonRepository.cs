using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.Lessons {

    // ------------------------------------------------------
    /// <summary>
    /// Code Ownership: Alexandros Blask
    /// </summary>
    public interface ILessonRepository {

        // ------------------------------------------------------
        /// <summary>
        /// Checks whether a <see cref="Lesson"/> entry with the given ID exists in the database.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="id">The lesson ID to check.</param>
        /// <returns>True if the lesson exists; otherwise, false.</returns>
        Task<bool> ExistsAsync(int id);
        
        // ------------------------------------------------------
        /// <summary>
        /// Checks whether a <see cref="Lesson"/> with the specified Makandra ID and TeachingPlan ID exists.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="makandraId">The Makandra ID.</param>
        /// <param name="teachingPlanId">The associated Teaching Plan ID.</param>
        /// <returns>True if a matching lesson exists; otherwise, false.</returns>
        Task<bool> ExistsAsync(string makandraId, int teachingPlanId);

        // ------------------------------------------------------
        /// <summary>
        /// Adds a new <see cref="Lesson"/> entry to the database.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="lesson">The lesson entity to be created.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        Task CreateAsync(Lesson lesson);

        /// <summary>
        /// Updates an existing <see cref="Lesson"/> in the database.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="lesson">The updated lesson entity.</param>
        /// <returns>A Task representing the asynchronous update operation.</returns>
        Task UpdateAsync(Lesson lesson);

        // ------------------------------------------------------
        /// <summary>
        /// Deletes a specific <see cref="Lesson"/> entity from the database.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="lesson">The lesson entity to delete.</param>
        /// <returns>A Task representing the asynchronous delete operation.</returns>
        Task DeleteAsync(Lesson lesson);

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves a <see cref="Lesson"/> by its ID, including related feedbacks.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="id">The lesson ID to retrieve.</param>
        /// <returns>The matching <see cref="Lesson"/> entity, or null if not found.</returns>
        Task<Lesson?> GetLessonByIdAsync(int id);

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves all <see cref="Lesson"/> entities from the database, including their feedbacks.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <returns>A list of all lessons with related feedbacks.</returns>
        Task<IEnumerable<Lesson>> GetAllLessonsAsync();

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves all <see cref="Lesson"/> entities along with their feedbacks.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <returns>A list of all lessons including associated feedbacks.</returns>
        Task<IEnumerable<Lesson>> GetAllLessonsWithFeedbacksAsync();
    }
}

