using Microsoft.EntityFrameworkCore;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.TraineeStatistics {

    /// <summary>
    /// Provides CRUD operations for <see cref="TraineeStatisticsSnapshot"/> entities using Entity Framework Core.
    /// Implements the <see cref="ITraineeStatisticsRepository"/> interface.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Nikita Stefan (stefanni)
    /// </remarks>
    public class DatabaseTraineeStatisticsRepository : ITraineeStatisticsRepository {

        /// <summary>
        /// The Entity Framework Core database context used for accessing and modifying trainee statistics.
        /// </summary>
        private readonly ApplicationDbContext _context;

        // ------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseTraineeStatisticsRepository"/> class.
        /// </summary>
        /// <param name="context">The database context used to access trainee statistics data.</param>
        /// <remarks>
        /// Code Ownership: Nikita Stefan (stefanni)
        /// </remarks>
        public DatabaseTraineeStatisticsRepository(ApplicationDbContext context) {
            _context = context;
        }

        // ------------------------------------------------------
        /// <summary>
        /// Checks whether a trainee statistics snapshot exists by its unique ID.
        /// </summary>
        /// <param name="id">The ID of the snapshot to check.</param>
        /// <returns>
        /// A Task representing the asynchronous operation. The task result is true if the snapshot exists; otherwise, false.
        /// </returns>
        /// <remarks>
        /// Code Ownership: Nikita Stefan (stefanni)
        /// </remarks>
        public async Task<bool> ExistsAsync(int id) {
            return await _context.TraineeStatisticsSnapshots.AnyAsync(s => s.TraineeStatisticsSnapshotId == id);
        }

        // ------------------------------------------------------
        /// <summary>
        /// Checks whether an identical trainee statistics snapshot already exists based on trainee ID and snapshot time.
        /// </summary>
        /// <param name="snapshot">The snapshot to compare against existing entries.</param>
        /// <returns>
        /// A Task representing the asynchronous operation. The task result is true if a matching snapshot exists; otherwise, false.
        /// </returns>
        /// <remarks>
        /// Code Ownership: Nikita Stefan (stefanni)
        /// </remarks>
        public async Task<bool> ExistsAsync(TraineeStatisticsSnapshot snapshot) {
            return await _context.TraineeStatisticsSnapshots.AnyAsync(s =>
                s.TraineeId == snapshot.TraineeId &&
                s.SnapshotDateTime == snapshot.SnapshotDateTime);
        }

        // ------------------------------------------------------
        /// <summary>
        /// Persists a new trainee statistics snapshot to the database.
        /// </summary>
        /// <param name="snapshot">The snapshot to add.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <remarks>
        /// Code Ownership: Nikita Stefan (stefanni)
        /// </remarks>
        public async Task CreateAsync(TraineeStatisticsSnapshot snapshot) {
            await _context.TraineeStatisticsSnapshots.AddAsync(snapshot);
            await _context.SaveChangesAsync();
        }

        // ------------------------------------------------------
        /// <summary>
        /// Updates an existing trainee statistics snapshot in the database.
        /// </summary>
        /// <param name="snapshot">The updated snapshot instance.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <remarks>
        /// Code Ownership: Nikita Stefan (stefanni)
        /// </remarks>
        public async Task UpdateAsync(TraineeStatisticsSnapshot snapshot) {
            _context.TraineeStatisticsSnapshots.Update(snapshot);
            await _context.SaveChangesAsync();
        }

        // ------------------------------------------------------
        /// <summary>
        /// Deletes an existing trainee statistics snapshot from the database.
        /// </summary>
        /// <param name="snapshot">The snapshot to delete.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <remarks>
        /// Code Ownership: Nikita Stefan (stefanni)
        /// </remarks>
        public async Task DeleteAsync(TraineeStatisticsSnapshot snapshot) {
            _context.TraineeStatisticsSnapshots.Remove(snapshot);
            await _context.SaveChangesAsync();
        }

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves the most recent statistics snapshot for a given trainee by ID.
        /// </summary>
        /// <param name="traineeId">The ID of the trainee whose snapshot is to be retrieved.</param>
        /// <returns>
        /// A Task representing the asynchronous operation. The task result contains the matching <see cref="TraineeStatisticsSnapshot"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown when no snapshot is found for the given trainee ID.</exception>
        /// <remarks>
        /// Code Ownership: Nikita Stefan (stefanni)
        /// </remarks>
        public async Task<TraineeStatisticsSnapshot> GetTraineeStatisticsSnapshotAsync(string traineeId) {
            var snapshot = await _context.TraineeStatisticsSnapshots
                .FirstOrDefaultAsync(s => s.TraineeId == traineeId);

            if (snapshot == null) {
                throw new InvalidOperationException($"No snapshot for trainee with ID '{traineeId}' found.");
            }

            return snapshot;
        }

        // ------------------------------------------------------
    }
}
