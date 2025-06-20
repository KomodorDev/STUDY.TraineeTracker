using System.Collections.Generic;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Repositories
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
