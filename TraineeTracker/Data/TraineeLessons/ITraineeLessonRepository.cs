using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.TraineeLessons {
    public interface ITraineeLessonRepository {
        Task<bool> ExistsAsync(int traineeLessonId);
        Task<bool> ExistsAsync(TraineeLesson traineeLesson);
        Task CreateAsync(TraineeLesson traineeLesson);
        Task CreateRangeAsync(IEnumerable<TraineeLesson> traineeLessons);
        Task UpdateAsync(TraineeLesson traineeLesson);
        Task<TraineeLesson?> GetTraineeLessonByIdWithLessonAsync(int traineeLessonId);
        Task<IEnumerable<TraineeLesson>> GetAllTraineeLessonsOfTraineeWithLessonAsync(string traineeId);
        Task<IEnumerable<TraineeLesson>> GetAllTraineeLessonsOfLessonWithLessonAsync(int lessonId);
    }
}