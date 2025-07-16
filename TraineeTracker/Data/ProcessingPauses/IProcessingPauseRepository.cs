using System.Collections.Generic;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.ProcessingPauses {
    public interface IProcessingPauseRepository {
        Task CreateAsync(ProcessingPause processingPause);

        Task<bool> ExistsAsync(ProcessingPause processingPause);

        Task<bool> OverlapsAsync(ProcessingPause processingPause, bool newProcessingPause);

        Task<ProcessingPause?> FindByIdAsync(int processingPauseId);

        Task UpdateAsync(ProcessingPause processingPause);

        Task DeleteAsync(ProcessingPause processingPause);

        Task<IEnumerable<ProcessingPause>> GetAllPausesAsync(string traineeId);
    }
}
