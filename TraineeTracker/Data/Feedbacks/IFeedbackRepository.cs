using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.Feedbacks {
    public interface IFeedbackRepository {
        bool Exists(int feedbackId);
        bool Exists(Feedback feedback);

        void Create(Feedback feedback);

        void Delete(Feedback feedback);
        void Delete(int feedbackId);

        IEnumerable<Feedback> GetAllFeedbacksForLesson(Lesson lesson);
        IEnumerable<Feedback> GetAllFeedbacksWrittenByUser(ApplicationUser user);
        IEnumerable<Feedback> GetAllFeedbacksReadByUser(ApplicationUser user);
        IEnumerable<Feedback> GetAllFeedbacksUnreadByUser(ApplicationUser user);
    }
}