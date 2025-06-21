using System.Collections.Generic;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.ProcessingPauses
{
    public interface IProcessingPauseRepository
    {
        public bool Exists(int processingPauseId);

        public bool Exists(ProcessingPause processingPause);

        public void Create(ProcessingPause processingPause);

        public void Update(ProcessingPause processingPause);

        public void Delete(ProcessingPause processingPause);

        public IEnumerable<ProcessingPause> GetAllPauses();
    }
}
