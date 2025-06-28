using TraineeTracker.Models.Domain;

namespace TraineeTracker.Models.Dtos {
    public class ProcessingPauseDto {
        public required string TraineeId { get; set; }
        public required DateOnly StartDate { get; set; }

        public required DateOnly EndDate { get; set; }
    }
}