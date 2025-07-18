using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.TraineeLessons {

    /// <summary>
    /// Defines data access operations for managing <see cref="TraineeLesson"/> records.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexander Schlemmer (schleale)
    /// </remarks>
    public interface ITraineeLessonRepository {

        // ------------------------------------------------------
        /// <summary>
        /// Checks whether a trainee lesson exists by its unique ID.
        /// </summary>
        /// <param name="traineeLessonId">The ID of the trainee lesson to check.</param>
        /// <returns>A Task that resolves to true if the lesson exists; otherwise, false.</returns>
        Task<bool> ExistsAsync(int traineeLessonId);

        // ------------------------------------------------------
        /// <summary>
        /// Checks whether a specific <see cref="TraineeLesson"/> entity exists.
        /// </summary>
        /// <param name="traineeLesson">The trainee lesson instance to check.</param>
        /// <returns>A Task that resolves to true if the lesson exists; otherwise, false.</returns>
        Task<bool> ExistsAsync(TraineeLesson traineeLesson);

        // ------------------------------------------------------
        /// <summary>
        /// Adds a new trainee lesson to the data store.
        /// </summary>
        /// <param name="traineeLesson">The trainee lesson to create.</param>
        /// <returns>A Task representing the asynchronous create operation.</returns>
        Task CreateAsync(TraineeLesson traineeLesson);

        // ------------------------------------------------------
        /// <summary>
        /// Adds a collection of trainee lessons to the data store.
        /// </summary>
        /// <param name="traineeLessons">The trainee lessons to be added.</param>
        /// <returns>A Task representing the asynchronous batch creation operation.</returns>
        Task CreateRangeAsync(IEnumerable<TraineeLesson> traineeLessons);

        // ------------------------------------------------------
        /// <summary>
        /// Updates an existing trainee lesson in the data store.
        /// </summary>
        /// <param name="traineeLesson">The trainee lesson entity with updated values.</param>
        /// <returns>A Task representing the asynchronous update operation.</returns>
        Task UpdateAsync(TraineeLesson traineeLesson);

        // ------------------------------------------------------
        /// <summary>
        /// Deletes a trainee lesson from the data store using its ID.
        /// </summary>
        /// <param name="traineeLessonId">The ID of the trainee lesson to delete.</param>
        /// <returns>A Task representing the asynchronous delete operation.</returns>
        Task DeleteAsync(int traineeLessonId);

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves a specific trainee lesson by its ID, including the associated lesson details.
        /// </summary>
        /// <param name="traineeLessonId">The ID of the trainee lesson to retrieve.</param>
        /// <returns>
        /// A Task resolving to the <see cref="TraineeLesson"/> entity if found; otherwise, null.
        /// </returns>
        Task<TraineeLesson?> GetTraineeLessonByIdWithLessonAsync(int traineeLessonId);

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves all trainee lessons for a specific trainee, including associated lesson details.
        /// </summary>
        /// <param name="traineeId">The ID of the trainee whose lessons are to be retrieved.</param>
        /// <returns>
        /// A Task resolving to a collection of <see cref="TraineeLesson"/> entities.
        /// </returns>
        Task<IEnumerable<TraineeLesson>> GetAllTraineeLessonsOfTraineeWithLessonAsync(string traineeId);

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves all trainee lessons for a specific lesson, including associated lesson details.
        /// </summary>
        /// <param name="lessonId">The ID of the lesson whose trainee associations are to be retrieved.</param>
        /// <returns>
        /// A Task resolving to a collection of <see cref="TraineeLesson"/> entities.
        /// </returns>
        Task<IEnumerable<TraineeLesson>> GetAllTraineeLessonsOfLessonWithLessonAsync(int lessonId);

        // ------------------------------------------------------
    }
}
