namespace TraineeTracker.Models.Domain {
    public class ProcessingPause {
        public int Id { get; private set; }

        public required string TraineeId { get; set; }
        
        public required ApplicationUser Trainee { get; set; }

        public required DateTime StartDate { get; set; }

        public required DateTime EndDate { get; set; }

        public override bool Equals(object? obj) {
            if (obj is not ProcessingPause other) {
                return false;
            }
            return TraineeId == other.TraineeId && StartDate == other.StartDate && EndDate == other.EndDate;
        }

        public override int GetHashCode() {
            return HashCode.Combine(TraineeId, StartDate, EndDate);
        }
    }
}