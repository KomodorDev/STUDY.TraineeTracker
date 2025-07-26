namespace TraineeTracker.Models.Dtos {

    /// <summary>
    /// Data Transfer Object (DTO) that represents the requested state change of a lesson.
    /// A rejection reason must be provided, if the targeted state is 'Rejected'.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexander Schlemmer (schleale)
    /// </remarks>
    public class TraineeLessonDto {

        /// <summary>
        /// The ID of the trainee lesson, of which the state is requested to be changed.
        /// </summary>
        public required int TraineeLessonId { get; set; }

        /// <summary>
        /// The ID of the trainee corresponding to the lesson
        /// </summary>
        public string? TraineeId { get; set; }

        /// <summary>
        /// The ID of the lesson corresponding to the lesson
        /// </summary>
        public int? LessonId { get; set; }

        /// <summary>
        /// The requested state, to which the trainee lesson is requested to change.
        /// </summary>
        public required string TargetStateName { get; set; }

        /// <summary>
        /// The reason, why a trainee lesson may be rejected.
        /// Only required when changing to 'Rejected', ignored otherwise.
        /// </summary>
        public string? RejectionReason { get; set; }
    }
}