using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.TraineeStatistics {
    public interface ITraineeStatisticsRepository {
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsAsync(TraineeStatisticsSnapshot snapshot);
        Task CreateAsync(TraineeStatisticsSnapshot snapshot);
        Task UpdateAsync(TraineeStatisticsSnapshot snapshot);
        Task DeleteAsync(TraineeStatisticsSnapshot snapshot);
        Task<TraineeStatisticsSnapshot> GetTraineeStatisticsSnapshotAsync(string traineeId);
    }
}