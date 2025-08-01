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
    public interface IFeedbackRepository {

        // ------------------------------------------------------
        /// <summary>
        /// Checks whether a <see cref="Feedback"/> entry with the given ID exists in the database.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="feedbackId">The ID of the feedback to check.</param>
        /// <returns>True if the feedback exists; otherwise, false.</returns>
        Task<bool> ExistsAsync(int feedbackId);

        // ------------------------------------------------------
        /// <summary>
        /// Checks whether a <see cref="Feedback"/> entity already exists in the database.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="feedback">The feedback entity to check for existence.</param>
        /// <returns>True if the feedback exists; otherwise, false.</returns>
        Task<bool> ExistsAsync(Feedback feedback);

        // ------------------------------------------------------
        /// <summary>
        /// Adds a new <see cref="Feedback"/> entry to the database.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="feedback">The feedback entity to be persisted.</param>
        Task CreateAsync(Feedback feedback);

        // ------------------------------------------------------
        /// <summary>
        /// Updates an existing <see cref="Feedback"/> entry in the database.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="feedback">The updated feedback entity.</param>
        /// <returns>A Task representing the asynchronous update operation.</returns>
        Task UpdateAsync(Feedback feedback);

        // ------------------------------------------------------
        /// <summary>
        /// Deletes a specific <see cref="Feedback"/> entity from the database.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="feedback">The feedback entity to delete.</param>
        /// <returns>A Task representing the asynchronous delete operation.</returns>
        Task DeleteAsync(Feedback feedback);

        // ------------------------------------------------------
        /// <summary>
        /// Deletes a <see cref="Feedback"/> entry from the database by its ID.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="feedbackId">The ID of the feedback to delete.</param>
        /// <returns>A Task representing the asynchronous delete operation.</returns>
        Task DeleteAsync(int feedbackId);

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves a <see cref="Feedback"/> by its ID, including related Lesson, Author, and ReadByUsers.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="feedbackId">The ID of the feedback to retrieve.</param>
        /// <returns>The matching Feedback, or null if not found.</returns>
        Task<Feedback?> GetFeedbackByIDWithLessonAndAuthorAndReadByUsersAsync(int feedbackId);

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves all <see cref="Feedback"/> entries for a given Lesson, including related entities.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="lesson">The Lesson to filter feedback by.</param>
        /// <returns>An enumerable of matching Feedback entities.</returns>
        Task<List<Feedback>> GetAllFeedbacksWithLessonAndAuthorAndReadByUsersAsync();

        
        Task<IEnumerable<Feedback>> GetAllFeedbacksForLessonWithLessonAndAuthorAndReadByUsersAsync(Lesson lesson);

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves all Feedback written by a specific user, including related entities.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="user">The authoring user.</param>
        /// <returns>An enumerable of Feedback authored by the user.</returns>
        Task<IEnumerable<Feedback>> GetAllFeedbacksWrittenByUserWithLessonAndAuthorAndReadByUsersAsync(ApplicationUser user);

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves all Feedback read by a specific user, including related entities.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="user">The user who read the feedback.</param>
        /// <returns>A list of Feedback read by the user.</returns>
        Task<List<Feedback>> GetAllFeedbacksReadByUserWithLessonAndAuthorAndReadByUsersAsync(ApplicationUser user);

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves all Feedback unread by a specific user, including related entities.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="user">The user to filter unread feedback by.</param>
        /// <returns>A list of unread Feedback for the user.</returns>
        Task<List<Feedback>> GetAllFeedbacksUnreadByUserWithLessonAndAuthorAndReadByUsersAsync(ApplicationUser user);

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves the feedback associated with a specific TraineeLesson, including related entities.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="traineeLesson">The TraineeLesson linking trainee and lesson.</param>
        /// <returns>The matching Feedback, or null if not found.</returns>
        Task<Feedback?> GetFeedbackOfTraineeLessonWithLessonAndAuthorAndReadByUsersAsync(TraineeLesson traineeLesson);

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves an <see cref="IQueryable{Feedback}"/> of all Feedback including Lesson and Author.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <returns>An IQueryable of all Feedback entities with related Lesson and Author.</returns>
        IQueryable<Feedback> GetAllFeedbacksWithLessonAndAuthor();

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves an <see cref="IQueryable{Feedback}"/> of Feedback read by a specific user, including Lesson and Author.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="user">The user who read the feedback.</param>
        /// <returns>An IQueryable of read Feedback for the user.</returns>
        IQueryable<Feedback> GetAllFeedbacksReadByUserWithLessonAndAuthor(ApplicationUser user);

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves an <see cref="IQueryable{Feedback}"/> of Feedback unread by a specific user, including Lesson and Author.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="user">The user to filter unread feedback by.</param>
        /// <returns>An IQueryable of unread Feedback for the user.</returns>
        IQueryable<Feedback> GetAllFeedbacksUnreadByUserWithLessonAndAuthor(ApplicationUser user);

        IQueryable<Feedback> GetAllFeedbacksUnreadByUserWithLessonAndAuthorAndReadByUsers(ApplicationUser user);

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves an <see cref="IQueryable{Feedback}"/> of all Feedback including Lesson, Author, and ReadByUsers.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <returns>An IQueryable of all Feedback entities with related Lesson, Author, and ReadByUsers.</returns>
        IQueryable<Feedback> GetAllFeedbacksWithLessonAndAuthorAndReadByUsers();
    }
}
