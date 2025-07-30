using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.ProcessingPauses {

    /// <summary>
    /// Defines methods for managing <see cref="ProcessingPause"/> entities in the data store.
    /// </summary>
    public interface IProcessingPauseRepository {

        // ------------------------------------------------------
        /// <summary>
        /// Asynchronously creates a new <see cref="ProcessingPause"/> entry.
        /// </summary>
        /// <param name="processingPause">The processing pause to create.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        Task CreateAsync(ProcessingPause processingPause);

        // ------------------------------------------------------
        /// <summary>
        /// Asynchronously checks if a specified <see cref="ProcessingPause"/> exists.
        /// </summary>
        /// <param name="processingPause">The processing pause to check for existence.</param>
        /// <returns>
        /// A task that returns <c>true</c> if the processing pause exists; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> ExistsAsync(ProcessingPause processingPause);

        // ------------------------------------------------------
        /// <summary>
        /// Asynchronously checks if a specified <see cref="ProcessingPause"/> overlaps with existing pauses.
        /// </summary>
        /// <param name="processingPause">The processing pause to check for overlap.</param>
        /// <param name="newProcessingPause">Indicates whether the pause is new or being updated.</param>
        /// <returns>
        /// A task that returns <c>true</c> if there is an overlap; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> OverlapsAsync(ProcessingPause processingPause, bool newProcessingPause);

        // ------------------------------------------------------
        /// <summary>
        /// Asynchronously finds a <see cref="ProcessingPause"/> by its unique identifier.
        /// </summary>
        /// <param name="processingPauseId">The unique identifier of the processing pause.</param>
        /// <returns>
        /// A task that returns the found <see cref="ProcessingPause"/>, or <c>null</c> if not found.
        /// </returns>
        Task<ProcessingPause?> FindByIdAsync(int processingPauseId);

        // ------------------------------------------------------
        /// <summary>
        /// Asynchronously updates an existing <see cref="ProcessingPause"/>.
        /// </summary>
        /// <param name="processingPause">The processing pause to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateAsync(ProcessingPause processingPause);

        // ------------------------------------------------------
        /// <summary>
        /// Asynchronously deletes a specified <see cref="ProcessingPause"/>.
        /// </summary>
        /// <param name="processingPause">The processing pause to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteAsync(ProcessingPause processingPause);

        // ------------------------------------------------------
        /// <summary>
        /// Asynchronously retrieves all <see cref="ProcessingPause"/> entries for a given trainee.
        /// </summary>
        /// <param name="traineeId">The identifier of the trainee.</param>
        /// <returns>
        /// A task that returns an enumerable collection of <see cref="ProcessingPause"/> objects.
        /// </returns>
        Task<IEnumerable<ProcessingPause>> GetAllPausesAsync(string traineeId);
    }
}
