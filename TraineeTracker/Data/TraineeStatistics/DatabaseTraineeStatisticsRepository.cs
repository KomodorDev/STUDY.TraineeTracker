using System.Collections.Generic;
using System.Linq;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.TraineeStatistics {
    public class DatabaseTraineeStatisticsRepository : ITraineeStatisticsRepository {
        private readonly ApplicationDbContext _context;

        public DatabaseTraineeStatisticsRepository(ApplicationDbContext context) {
            _context = context;
        }

        public bool Exists(int id) {
            return _context.TraineeStatisticsSnapshots.Any(s => s.Id == id);
        }

        public bool Exists(TraineeStatisticsSnapshot snapshot) {
            return _context.TraineeStatisticsSnapshots.Any(s =>
                s.userId == snapshot.userId &&
                s.SnapshotDate == snapshot.SnapshotDate);
        }

        public bool Create(TraineeStatisticsSnapshot snapshot) {
            _context.TraineeStatisticsSnapshots.Add(snapshot);
            _context.SaveChanges();
        }

        public bool Update(TraineeStatisticsSnapshot snapshot) {
            _context.TraineeStatisticsSnapshots.Update(snapshot);
            _context.SaveChanges();
        }

        public bool Delete(TraineeStatisticsSnapshot snapshot) {
            _context.TraineeStatisticsSnapshots.Remove(snapshot);
            _context.SaveChanges();
        }

        public IEnumerable<TraineeStatisticsSnapshot> GetAllTraineeStatisticsSnapshots(string traineeId) {
            return _context.TraineeStatisticsSnapshots
                .Where(s => s.userId == traineeId)
                .OrderByDescending(s => s.SnapshotDate)
                .ToList();
        }
    }
}