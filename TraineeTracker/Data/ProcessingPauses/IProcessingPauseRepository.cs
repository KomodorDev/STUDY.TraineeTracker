using System.Collections.Generic;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.ProcessingPauses
{
    public interface IProcessingPauseRepository
    {
        public Task CreateAsync(ProcessingPause processingPause);

        public Task<bool> ExistsAsync(ProcessingPause processingPause);

        public Task<ProcessingPause?> FindById(int processingPauseId);

        public Task UpdateAsync(ProcessingPause processingPause);

        public Task DeleteAsync(ProcessingPause processingPause);

        public Task<IEnumerable<ProcessingPause>> GetAllPausesAsync(string traineeId);
    }
}
