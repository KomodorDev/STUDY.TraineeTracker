using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.TraineeStatistics {
    public interface ITraineeStatisticsRepository {
        Task<bool> Exists(int id);
        Task<bool> Exists(TraineeStatisticsSnapshot snapshot);
        Task Create(TraineeStatisticsSnapshot snapshot);
        Task Update(TraineeStatisticsSnapshot snapshot);
        Task Delete(TraineeStatisticsSnapshot snapshot);
        Task<TraineeStatisticsSnapshot> GetTraineeStatisticsSnapshot(string traineeId);
    }
}