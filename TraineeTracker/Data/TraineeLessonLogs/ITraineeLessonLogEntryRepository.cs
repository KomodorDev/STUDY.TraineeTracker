using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.TraineeLessonLog {
    public interface ITraineeLessonLogEntryRepository {
        void Create(TraineeLessonLogEntry log);
        IEnumerable<TraineeLessonLogEntry> GetAllLogs();
        IEnumerable<TraineeLessonLogEntry> GetAllLogsForTraineeLesson(int traineeLessonId);
    }
}
