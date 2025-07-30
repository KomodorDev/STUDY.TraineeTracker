using System.ComponentModel.DataAnnotations;

namespace TraineeTracker.Models.Dtos {

    /// <summary>
    /// Data Transfer Object representing a processing pause for a trainee.
    /// </summary>
    /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
    public class ProcessingPauseDto {
        
        /// <summary>
        /// Gets or sets the unique identifier for the processing pause.
        /// </summary>
        public int? ProcessingPauseId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the trainee associated with the processing pause.
        /// </summary>
        [Required]
        public string TraineeId { get; set; } = "";

        /// <summary>
        /// Gets or sets the start date of the processing pause.
        /// </summary>
        [Required]
        public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        /// <summary>
        /// Gets or sets the end date of the processing pause.
        /// </summary>
        [Required]
        public DateOnly EndDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}