using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.TraineeLessons {
    public interface ITraineeLessonRepository {
        Task<bool> Exists(int traineeLessonId);
        Task<bool> Exists(TraineeLesson traineeLesson);
        Task Create(TraineeLesson traineeLesson);
        Task CreateRange(IEnumerable<TraineeLesson> traineeLessons);
        Task Update(TraineeLesson traineeLesson);
        Task<TraineeLesson?> GetTraineeLessonById(int traineeLessonId);
        Task<IEnumerable<TraineeLesson>> GetAllTraineeLessonsOfTrainee(string traineeId);
        Task<IEnumerable<TraineeLesson>> GetAllTraineeLessonsOfLesson(int lessonId);
    }
}