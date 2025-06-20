using System.Collections.Generic;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.ProcessingPauses
{
    public interface IProcessingPauseRepository
    {
        bool Exists(ProcessingPause processingPause);

        void Create(ProcessingPause processingPause);

        void Update(ProcessingPause processingPause);

        void Delete(ProcessingPause processingPause);

        IEnumerable<ProcessingPause> GetAllPauses();
    }
}
