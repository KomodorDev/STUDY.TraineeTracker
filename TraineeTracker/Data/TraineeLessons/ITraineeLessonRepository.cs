using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.TraineeLessons {
    public interface ITraineeLessonRepository {
        public bool Exists(int traineeLessonId);
        public bool Exists(TraineeLesson traineeLesson);
        public async Task Create(TraineeLesson traineeLesson);
        public async Task CreateRange(IEnumerable<TraineeLesson> traineeLessons);
        public async Task Update(TraineeLesson traineeLesson);
        public TraineeLesson? GetTraineeLessonById(int traineeLessonId);
        public IEnumerable<TraineeLesson> GetAllTraineeLessonsOfTrainee(string traineeId);
        public IEnumerable<TraineeLesson> GetAllTraineeLessonsOfLesson(int lessonId);
    }
}
