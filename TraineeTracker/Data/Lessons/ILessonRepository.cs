using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.Lessons {
    public interface ILessonRepository {
        Task<bool> ExistsAsync(int id);
        
        Task<bool> ExistsAsync(string makandraId, int teachingPlanId);

        Task CreateAsync(Lesson lesson);

        Task UpdateAsync(Lesson lesson);

        Task DeleteAsync(Lesson lesson);

        Task<Lesson?> GetLessonByIdAsync(int id);

        Task<IEnumerable<Lesson>> GetAllLessonsAsync();

        Task<IEnumerable<Lesson>> GetAllLessonsWithFeedbacksAsync();

    }
}

