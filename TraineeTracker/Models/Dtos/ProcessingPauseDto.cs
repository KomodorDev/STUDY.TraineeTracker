using TraineeTracker.Models.Domain;

namespace TraineeTracker.Models.Dtos {
    public class ProcessingPauseDto {
        public required DateTime StartDate { get; set; }

        public required DateTime EndDate { get; set; }
    }
}