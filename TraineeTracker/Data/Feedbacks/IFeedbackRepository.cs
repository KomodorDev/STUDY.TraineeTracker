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
        /// </summary>
        /// <param name="feedbackId">The ID of the feedback to check.</param>
        /// <returns>True if the feedback exists; otherwise, false.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        Task<bool> ExistsAsync(int feedbackId);

        // ------------------------------------------------------
        /// <summary>
        /// Checks whether a <see cref="Feedback"/> entity already exists in the database.
        /// </summary>
        /// <param name="feedback">The feedback entity to check for existence.</param>
        /// <returns>True if the feedback exists; otherwise, false.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        Task<bool> ExistsAsync(Feedback feedback);

        // ------------------------------------------------------
        /// <summary>
        /// Adds a new <see cref="Feedback"/> entry to the database.
        /// </summary>
        /// <param name="feedback">The feedback entity to be persisted.</param>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        Task CreateAsync(Feedback feedback);

        // ------------------------------------------------------
        /// <summary>
        /// Updates an existing <see cref="Feedback"/> entry in the database.
        /// </summary>
        /// <param name="feedback">The updated feedback entity.</param>
        /// <returns>A Task representing the asynchronous update operation.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        Task UpdateAsync(Feedback feedback);

        // ------------------------------------------------------
        /// <summary>
        /// Deletes a specific <see cref="Feedback"/> entity from the database.
        /// </summary>
        /// <param name="feedback">The feedback entity to delete.</param>
        /// <returns>A Task representing the asynchronous delete operation.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        Task DeleteAsync(Feedback feedback);

        // ------------------------------------------------------
        /// <summary>
        /// Deletes a <see cref="Feedback"/> entry from the database by its ID.
        /// </summary>
        /// <param name="feedbackId">The ID of the feedback to delete.</param>
        /// <returns>A Task representing the asynchronous delete operation.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        Task DeleteAsync(int feedbackId);

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves a <see cref="Feedback"/> by its ID, including related Lesson, Author, and ReadByUsers.
        /// </summary>
        /// <param name="feedbackId">The ID of the feedback to retrieve.</param>
        /// <returns>The matching Feedback, or null if not found.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        Task<Feedback?> GetFeedbackByIDWithLessonAndAuthorAndReadByUsersAsync(int feedbackId);

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves all <see cref="Feedback"/> entries for a given Lesson, including related entities.
        /// </summary>
        /// <param name="lesson">The Lesson to filter feedback by.</param>
        /// <returns>An enumerable of matching Feedback entities.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        Task<List<Feedback>> GetAllFeedbacksWithLessonAndAuthorAndReadByUsersAsync();

        
        Task<IEnumerable<Feedback>> GetAllFeedbacksForLessonWithLessonAndAuthorAndReadByUsersAsync(Lesson lesson);

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves all Feedback written by a specific user, including related entities.
        /// </summary>
        /// <param name="user">The authoring user.</param>
        /// <returns>An enumerable of Feedback authored by the user.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        Task<IEnumerable<Feedback>> GetAllFeedbacksWrittenByUserWithLessonAndAuthorAndReadByUsersAsync(ApplicationUser user);

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves all Feedback read by a specific user, including related entities.
        /// </summary>
        /// <param name="user">The user who read the feedback.</param>
        /// <returns>A list of Feedback read by the user.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        Task<List<Feedback>> GetAllFeedbacksReadByUserWithLessonAndAuthorAndReadByUsersAsync(ApplicationUser user);

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves all Feedback unread by a specific user, including related entities.
        /// </summary>
        /// <param name="user">The user to filter unread feedback by.</param>
        /// <returns>A list of unread Feedback for the user.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        Task<List<Feedback>> GetAllFeedbacksUnreadByUserWithLessonAndAuthorAndReadByUsersAsync(ApplicationUser user);

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves the feedback associated with a specific TraineeLesson, including related entities.
        /// </summary>
        /// <param name="traineeLesson">The TraineeLesson linking trainee and lesson.</param>
        /// <returns>The matching Feedback, or null if not found.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        Task<Feedback?> GetFeedbackOfTraineeLessonWithLessonAndAuthorAndReadByUsersAsync(TraineeLesson traineeLesson);

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves an <see cref="IQueryable{Feedback}"/> of all Feedback including Lesson and Author.
        /// </summary>
        /// <returns>An IQueryable of all Feedback entities with related Lesson and Author.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        IQueryable<Feedback> GetAllFeedbacksWithLessonAndAuthor();

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves an <see cref="IQueryable{Feedback}"/> of Feedback read by a specific user, including Lesson and Author.
        /// </summary>
        /// <param name="user">The user who read the feedback.</param>
        /// <returns>An IQueryable of read Feedback for the user.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        IQueryable<Feedback> GetAllFeedbacksReadByUserWithLessonAndAuthor(ApplicationUser user);

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves an <see cref="IQueryable{Feedback}"/> of Feedback unread by a specific user, including Lesson and Author.
        /// </summary>
        /// <param name="user">The user to filter unread feedback by.</param>
        /// <returns>An IQueryable of unread Feedback for the user.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        IQueryable<Feedback> GetAllFeedbacksUnreadByUserWithLessonAndAuthor(ApplicationUser user);

        IQueryable<Feedback> GetAllFeedbacksUnreadByUserWithLessonAndAuthorAndReadByUsers(ApplicationUser user);

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves an <see cref="IQueryable{Feedback}"/> of all Feedback including Lesson, Author, and ReadByUsers.
        /// </summary>
        /// <returns>An IQueryable of all Feedback entities with related Lesson, Author, and ReadByUsers.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        IQueryable<Feedback> GetAllFeedbacksWithLessonAndAuthorAndReadByUsers();
    }
}
