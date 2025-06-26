using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.TraineeStatistics {
    public class DatabaseTraineeStatisticsRepository : ITraineeStatisticsRepository {
        private readonly ApplicationDbContext _context;

        public DatabaseTraineeStatisticsRepository(ApplicationDbContext context) {
            _context = context;
        }

        public bool Exists(int id) {
            return _context.TraineeStatisticsSnapshots.Any(s => s.TraineeStatisticsSnapshotId == id);
        }

        public bool Exists(TraineeStatisticsSnapshot snapshot) {
            return _context.TraineeStatisticsSnapshots.Any(s =>
                s.TraineeId == snapshot.TraineeId &&
                s.SnapshotDate == snapshot.SnapshotDate);
        }

        public void Create(TraineeStatisticsSnapshot snapshot) {
            _context.TraineeStatisticsSnapshots.Add(snapshot);
            _context.SaveChanges();
        }

        public void Update(TraineeStatisticsSnapshot snapshot) {
            _context.TraineeStatisticsSnapshots.Update(snapshot);
            _context.SaveChanges();
        }

        public void Delete(TraineeStatisticsSnapshot snapshot) {
            _context.TraineeStatisticsSnapshots.Remove(snapshot);
            _context.SaveChanges();
        }

        public TraineeStatisticsSnapshot GetTraineeStatisticsSnapshot(string traineeId) {
            var snapshot = _context.TraineeStatisticsSnapshots
            .FirstOrDefault(s => s.TraineeId == traineeId);

            if (snapshot == null) {
            throw new InvalidOperationException($"No snapshot for trainee with ID '{traineeId}' found.");
            }

            return snapshot;
        }
    }
}