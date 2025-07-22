using TraineeTracker.Models.Dtos;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Models.ViewModels.Admin {
    public class ManageProcessingPausesViewModel {
        public string UserName { get; set; } = "";
        public string TraineeId { get; set; } = "";
        public List<ProcessingPause> ProcessingPauses { get; set; } = new();
        public ProcessingPauseDto NewProcessingPause { get; set; } = new();
    }

}