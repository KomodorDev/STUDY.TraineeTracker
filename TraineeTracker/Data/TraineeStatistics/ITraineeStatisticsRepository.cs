using TraineeTracker.Models.Domain;
using System.Collections.Generic;

namespace TraineeTracker.Data.TraineeStatistics {
    public interface ITraineeStatisticsRepository {
        bool Exists(int id);
        bool Exists(TraineeStatisticsSnapshot snapshot);
        void Create(TraineeStatisticsSnapshot snapshot);
        void Update(TraineeStatisticsSnapshot snapshot);
        void Delete(TraineeStatisticsSnapshot snapshot);
        TraineeStatisticsSnapshot GetTraineeStatisticsSnapshot(string traineeId);
    }
}