using Microsoft.EntityFrameworkCore;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.ProcessingPauses {
    /// <summary>
    /// Repository implementation for managing <see cref="ProcessingPause"/> entities in the database.
    /// </summary>
    /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
    public class DatabaseProcessingPauseRepository : IProcessingPauseRepository {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseProcessingPauseRepository"/> class.
        /// </summary>
        /// <param name="context">The application's database context.</param>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public DatabaseProcessingPauseRepository(ApplicationDbContext context) {
            _context = context;
        }

        /// <summary>
        /// Adds a new <see cref="ProcessingPause"/> to the database asynchronously.
        /// </summary>
        /// <param name="processingPause">The processing pause to add.</param>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task CreateAsync(ProcessingPause processingPause) {
            await _context.ProcessingPauses.AddAsync(processingPause);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Checks asynchronously if a specific <see cref="ProcessingPause"/> already exists in the database.
        /// </summary>
        /// <param name="processingPause">The processing pause to check for existence.</param>
        /// <returns>True if the processing pause exists; otherwise, false.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<bool> ExistsAsync(ProcessingPause processingPause) {
            return await _context.ProcessingPauses.AnyAsync(p =>
                p.TraineeId == processingPause.TraineeId &&
                p.StartDate == processingPause.StartDate &&
                p.EndDate == processingPause.EndDate
            );
        }

        /// <summary>
        /// Determines asynchronously if the given <see cref="ProcessingPause"/> overlaps with any existing pauses for the same trainee.
        /// </summary>
        /// <param name="processingPause">The processing pause to check for overlap.</param>
        /// <param name="newProcessingPause">Indicates if the pause is new (true) or being updated (false).</param>
        /// <returns>True if an overlap exists; otherwise, false.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<bool> OverlapsAsync(ProcessingPause processingPause, bool newProcessingPause) {
            return await _context.ProcessingPauses.AnyAsync(p =>
                (newProcessingPause || p.ProcessingPauseId != processingPause.ProcessingPauseId) &&    // if in database (Id != 1): don't compare with same instance of processingPause
                p.TraineeId == processingPause.TraineeId &&
                ((p.StartDate == processingPause.StartDate && p.EndDate == processingPause.EndDate) ||                      // Same
                (processingPause.StartDate < p.StartDate && p.StartDate < processingPause.EndDate) ||                       // StartDate strictly inside new pause
                (processingPause.StartDate < p.EndDate && p.EndDate < processingPause.EndDate) ||                           // EndDate strictly inside new Pause
                (p.StartDate == processingPause.StartDate && p.EndDate > processingPause.StartDate) ||                      // Enddate lurks inside new Pause
                (p.EndDate == processingPause.EndDate && p.StartDate < processingPause.EndDate))                            // Startdate lurks inside new Pause
            );
        }

        /// <summary>
        /// Finds a <see cref="ProcessingPause"/> by its unique identifier asynchronously.
        /// </summary>
        /// <param name="processingPauseId">The unique identifier of the processing pause.</param>
        /// <returns>The found processing pause, or null if not found.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<ProcessingPause?> FindByIdAsync(int processingPauseId) {
            return await _context.ProcessingPauses.FirstOrDefaultAsync(p => p.ProcessingPauseId == processingPauseId);
        }

        /// <summary>
        /// Updates an existing <see cref="ProcessingPause"/> in the database asynchronously.
        /// </summary>
        /// <param name="processingPause">The processing pause to update.</param>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task UpdateAsync(ProcessingPause processingPause) {
            _context.ProcessingPauses.Update(processingPause);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a <see cref="ProcessingPause"/> from the database asynchronously.
        /// </summary>
        /// <param name="processingPause">The processing pause to delete.</param>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task DeleteAsync(ProcessingPause processingPause) {
            _context.ProcessingPauses.Remove(processingPause);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves all <see cref="ProcessingPause"/> entries for a specific trainee asynchronously.
        /// </summary>
        /// <param name="traineeId">The unique identifier of the trainee.</param>
        /// <returns>A collection of processing pauses for the specified trainee.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<IEnumerable<ProcessingPause>> GetAllPausesAsync(string traineeId) {
            return await _context.ProcessingPauses.Where(p => p.TraineeId == traineeId).ToListAsync();
        }
    }
}
