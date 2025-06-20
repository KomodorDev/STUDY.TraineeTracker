namespace TraineeTracker.Models.Domain {
    public class ProcessingPause {
        public int ProcessingPauseId { get; private set; }

        public required string TraineeId { get; set; }

        public required ApplicationUser Trainee { get; set; }

        public required DateTime StartDate { get; set; }

        public required DateTime EndDate { get; set; }
    }
}