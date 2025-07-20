using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.Feedbacks {
    public interface IFeedbackRepository {
        Task<bool> ExistsAsync(int feedbackId);
        Task<bool> ExistsAsync(Feedback feedback);

        Task CreateAsync(Feedback feedback);

        Task UpdateAsync(Feedback feedback);

        Task DeleteAsync(Feedback feedback);
        Task DeleteAsync(int feedbackId);

        Task<Feedback?> GetFeedbackByIDWithLessonAndAuthorAndReadByUsersAsync(int feedbackId);

        Task<List<Feedback>> GetAllFeedbacksWithLessonAndAuthorAndReadByUsersAsync();

        Task<IEnumerable<Feedback>> GetAllFeedbacksForLessonWithLessonAndAuthorAndReadByUsersAsync(Lesson lesson);
        Task<IEnumerable<Feedback>> GetAllFeedbacksWrittenByUserWithLessonAndAuthorAndReadByUsersAsync(ApplicationUser user);

        Task<List<Feedback>> GetAllFeedbacksReadByUserWithLessonAndAuthorAndReadByUsersAsync(ApplicationUser user);
        Task<List<Feedback>> GetAllFeedbacksUnreadByUserWithLessonAndAuthorAndReadByUsersAsync(ApplicationUser user);

        Task<Feedback?> GetFeedbackOfTraineeLessonWithLessonAndAuthorAndReadByUsersAsync(TraineeLesson traineeLesson);

        IQueryable<Feedback> GetAllFeedbacksWithLessonAndAuthor();

        IQueryable<Feedback> GetAllFeedbacksReadByUserWithLessonAndAuthor(ApplicationUser user);

        IQueryable<Feedback> GetAllFeedbacksUnreadByUserWithLessonAndAuthor(ApplicationUser user);

        IQueryable<Feedback> GetAllFeedbacksUnreadByUserWithLessonAndAuthorAndReadByUsers(ApplicationUser user);

        IQueryable<Feedback> GetAllFeedbacksWithLessonAndAuthorAndReadByUsers();


    }
}
