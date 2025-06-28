using Microsoft.EntityFrameworkCore;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.TraineeStatistics {
    public class DatabaseTraineeStatisticsRepository : ITraineeStatisticsRepository {
        private readonly ApplicationDbContext _context;

        public DatabaseTraineeStatisticsRepository(ApplicationDbContext context) {
            _context = context;
        }

        public async Task<bool> ExistsAsync(int id) {
            return await _context.TraineeStatisticsSnapshots.AnyAsync(s => s.TraineeStatisticsSnapshotId == id);
        }

        public async Task<bool> ExistsAsync(TraineeStatisticsSnapshot snapshot) {
            return await _context.TraineeStatisticsSnapshots.AnyAsync(s =>
                s.TraineeId == snapshot.TraineeId &&
                s.SnapshotDateTime == snapshot.SnapshotDateTime);
        }

        public async Task CreateAsync(TraineeStatisticsSnapshot snapshot) {
            await _context.TraineeStatisticsSnapshots.AddAsync(snapshot);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TraineeStatisticsSnapshot snapshot) {
            _context.TraineeStatisticsSnapshots.Update(snapshot);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TraineeStatisticsSnapshot snapshot) {
            _context.TraineeStatisticsSnapshots.Remove(snapshot);
            await _context.SaveChangesAsync();
        }

        public async Task<TraineeStatisticsSnapshot> GetTraineeStatisticsSnapshotAsync(string traineeId) {
            var snapshot = await _context.TraineeStatisticsSnapshots
            .FirstOrDefaultAsync(s => s.TraineeId == traineeId);

            if (snapshot == null) {
            throw new InvalidOperationException($"No snapshot for trainee with ID '{traineeId}' found.");
            }

            return snapshot;
        }
    }
}