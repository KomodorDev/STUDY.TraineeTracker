namespace TraineeTracker.Models.Dtos {

    // ------------------------------------------------------
    /// <summary>
    /// Provides Dto for Lesson
    /// Code Ownership: Alexandros Blask
    /// </summary>
    public class LessonDto {

        // ------------------------------------------------------
        /// <summary>
        /// Unique Id of the Lesson
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public required string Id { get; set; }

        // ------------------------------------------------------
        /// <summary>
        /// Title of the Lesson Dto
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public string Title { get; set; } = string.Empty;

        // ------------------------------------------------------
        /// <summary>
        /// Url of the Lesson Dto
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public string Url { get; set; } = string.Empty;

        // ------------------------------------------------------
        /// <summary>
        /// This is the Estimated Effort of the Lesson Dto
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public double? Estimate { get; set; }
        
        // ------------------------------------------------------
        /// <summary>
        /// Boolean which if set marks the Lesson Dto as Depracated
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public bool Deprecated { get; set; }
    }
}
