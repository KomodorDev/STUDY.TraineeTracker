using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.TraineeStatistics {

    /// <summary>
    /// Defines data access operations for managing <see cref="TraineeStatisticsSnapshot"/> records.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Nikita Stefan (stefanni)
    /// </remarks>
    public interface ITraineeStatisticsRepository {

        // ------------------------------------------------------
        /// <summary>
        /// Checks whether a snapshot exists for the given unique ID.
        /// </summary>
        /// <param name="id">The unique ID of the statistics snapshot.</param>
        /// <returns>
        /// A Task that resolves to true if a snapshot with the specified ID exists, otherwise false.
        /// </returns>
        Task<bool> ExistsAsync(int id);

        // ------------------------------------------------------
        /// <summary>
        /// Checks whether a snapshot exists for a given trainee and snapshot timestamp.
        /// </summary>
        /// <param name="snapshot">The snapshot instance to check for existence.</param>
        /// <returns>
        /// A Task that resolves to true if an equivalent snapshot already exists, otherwise false.
        /// </returns>
        Task<bool> ExistsAsync(TraineeStatisticsSnapshot snapshot);

        // ------------------------------------------------------
        /// <summary>
        /// Adds a new <see cref="TraineeStatisticsSnapshot"/> to the data store.
        /// </summary>
        /// <param name="snapshot">The snapshot to be created and persisted.</param>
        /// <returns>
        /// A Task representing the asynchronous creation operation.
        /// </returns>
        Task CreateAsync(TraineeStatisticsSnapshot snapshot);

        // ------------------------------------------------------
        /// <summary>
        /// Updates an existing <see cref="TraineeStatisticsSnapshot"/> in the data store.
        /// </summary>
        /// <param name="snapshot">The modified snapshot to persist.</param>
        /// <returns>
        /// A Task representing the asynchronous update operation.
        /// </returns>
        Task UpdateAsync(TraineeStatisticsSnapshot snapshot);

        // ------------------------------------------------------
        /// <summary>
        /// Removes a specified <see cref="TraineeStatisticsSnapshot"/> from the data store.
        /// </summary>
        /// <param name="snapshot">The snapshot to be deleted.</param>
        /// <returns>
        /// A Task representing the asynchronous deletion operation.
        /// </returns>
        Task DeleteAsync(TraineeStatisticsSnapshot snapshot);

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves the most recent <see cref="TraineeStatisticsSnapshot"/> for a given trainee.
        /// </summary>
        /// <param name="traineeId">The unique identifier of the trainee.</param>
        /// <returns>
        /// A Task that resolves to the latest statistics snapshot for the specified trainee.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when no snapshot is found for the given trainee ID.
        /// </exception>
        Task<TraineeStatisticsSnapshot> GetTraineeStatisticsSnapshotAsync(string traineeId);

        // ------------------------------------------------------
    }
}
