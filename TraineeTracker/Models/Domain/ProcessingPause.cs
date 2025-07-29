namespace TraineeTracker.Models.Domain {
    /// <summary>
    /// Represents a pause in processing for a trainee, defined by a start and end date.
    /// </summary>
    /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
    public class ProcessingPause {
        /// <summary>
        /// Gets the unique identifier for the processing pause.
        /// </summary>
        public int ProcessingPauseId { get; private set; }

        /// <summary>
        /// Gets or sets the unique identifier of the trainee associated with this processing pause.
        /// </summary>
        public required string TraineeId { get; set; }

        /// <summary>
        /// Gets or sets the trainee associated with this processing pause.
        /// </summary>
        public required ApplicationUser Trainee { get; set; }

        /// <summary>
        /// Gets or sets the start date of the processing pause.
        /// </summary>
        public required DateOnly StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date of the processing pause.
        /// </summary>
        public required DateOnly EndDate { get; set; }

        /// <summary>
        /// Determines whether the specified object is equal to the current <see cref="ProcessingPause"/>.
        /// </summary>
        /// <param name="obj">The object to compare with the current <see cref="ProcessingPause"/>.</param>
        /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public override bool Equals(object? obj) {
            if (obj is not ProcessingPause other) {
                return false;
            }
            return TraineeId == other.TraineeId && StartDate == other.StartDate && EndDate == other.EndDate;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current <see cref="ProcessingPause"/>.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public override int GetHashCode() {
            return HashCode.Combine(TraineeId, StartDate, EndDate);
        }
    }
}