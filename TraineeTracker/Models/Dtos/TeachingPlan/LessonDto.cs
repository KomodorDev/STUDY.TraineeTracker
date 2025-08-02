namespace TraineeTracker.Models.Dtos {

    // ------------------------------------------------------
    /// <summary>
    /// Provides Dto for Lesson
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexandros Blask
    /// </remarks>
    public class LessonDto {

        /// <summary>
        /// Unique Id of the Lesson
        /// </summary>
        public required string Id { get; set; }

        /// <summary>
        /// Title of the Lesson Dto
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Url of the Lesson Dto
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// This is the Estimated Effort of the Lesson Dto
        /// </summary>
        public double? Estimate { get; set; }
        
        /// <summary>
        /// Boolean which if set marks the Lesson Dto as Depracated
        /// </summary>
        public bool Deprecated { get; set; }
    }
}
