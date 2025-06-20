using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.TraineeLessonLog {
    public class DatabaseTraineeLessonLogEntryRepository : ITraineeLessonLogEntryRepository {
        private readonly ApplicationDbContext _context;

        public DatabaseTraineeLessonLogEntryRepository(ApplicationDbContext context) {
            _context = context;
        }

        public void Create(TraineeLessonLogEntry log) {
            _context.TraineeLessonLogEntries.Add(log);
            _context.SaveChanges();
        }

        public IEnumerable<TraineeLessonLogEntry> GetAllLogs() {
            return _context.TraineeLessonLogEntries
                .OrderByDescending(l => l.Timestamp)
                .ToList();
        }

        public IEnumerable<TraineeLessonLogEntry> GetAllLogsForTraineeLesson(int traineeLessonId) {
            return _context.TraineeLessonLogEntries
                .Where(l => l.TraineeLessonId == traineeLessonId)
                .OrderByDescending(l => l.Timestamp)
                .ToList();
        }
    }
}
