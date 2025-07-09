using TraineeTracker.Models.Dtos;

namespace TraineeTracker.Models.ViewModels.Admin {
    public class CreateProcessingPauseViewModel {
        public ProcessingPauseDto ProcessingPause { get; set; } = new ProcessingPauseDto();
        public required string UserName { get; set; }
    }
}