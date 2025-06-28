using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.Feedbacks {
    public interface IFeedbackRepository {
        Task<bool> ExistsAsync(int feedbackId);
        Task<bool> ExistsAsync(Feedback feedback);

        Task CreateAsync(Feedback feedback);

        Task UpdateAsync(Feedback feedback);

        Task DeleteAsync(Feedback feedback);
        Task DeleteAsync(int feedbackId);

        Task<IEnumerable<Feedback>> GetAllFeedbacksForLessonAsync(Lesson lesson);
        Task<IEnumerable<Feedback>> GetAllFeedbacksWrittenByUserAsync(ApplicationUser user);
        Task<IEnumerable<Feedback>> GetAllFeedbacksReadByUserAsync(ApplicationUser user);
        Task<IEnumerable<Feedback>> GetAllFeedbacksUnreadByUserAsync(ApplicationUser user);

        Task<Feedback?> GetFeedbackOfTraineeLessonAsync(TraineeLesson traineeLesson);
    }
}
