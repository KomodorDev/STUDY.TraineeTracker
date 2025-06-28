using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.Feedbacks {
    public interface IFeedbackRepository {
        Task<bool> ExistsAsync(int feedbackId);
        Task<bool> ExistsAsync(Feedback feedback);

        Task CreateAsync(Feedback feedback);

        Task UpdateAsync(Feedback feedback);

        Task DeleteAsync(Feedback feedback);
        Task DeleteAsync(int feedbackId);

        Task<IEnumerable<Feedback>> GetAllFeedbacksForLessonWithLessonAndAuthorAndReadByUsersAsync(Lesson lesson);
        Task<IEnumerable<Feedback>> GetAllFeedbacksWrittenByUserWithLessonAndAuthorAndReadByUsersAsync(ApplicationUser user);
        Task<IEnumerable<Feedback>> GetAllFeedbacksReadByUserWithLessonAndAuthorAndReadByUsersAsync(ApplicationUser user);
        Task<IEnumerable<Feedback>> GetAllFeedbacksUnreadByUserWithLessonAndAuthorAndReadByUsersAsync(ApplicationUser user);

        Task<Feedback?> GetFeedbackOfTraineeLessonWithLessonAndAuthorAndReadByUsersAsync(TraineeLesson traineeLesson);
    }
}
