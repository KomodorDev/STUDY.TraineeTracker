namespace TraineeTracker.Models.Domain {
    
    /// <summary>
    /// Represents a single trainee lesson, which holds all the information of a trainee and
    /// their progress with one of their corresponding lessons. 
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexander Schlemmer (schleale)
    /// </remarks>
    public class TraineeLesson {

        /// <summary>
        /// Primary key: The unique ID of a trainee lesson
        /// </summary>
        // Id or {ClassName}Id -> treated as primary key, not nullable, auto-incremented
        public int TraineeLessonId { get; set; }

        /// <summary>
        /// The ID of the trainee who owns / edits the trainee lesson
        /// </summary>
        public required string TraineeId { get; set; }

        /// <summary>
        /// The trainee who owns / edits the trainee lesson
        /// </summary>
        public required ApplicationUser Trainee { get; set; }

        /// <summary>
        /// The current state of the trainee lesson the trainee has achieved.
        /// The standard state is Open.
        /// </summary>
        public TraineeLessonState State { get; set; } = TraineeLessonState.Open;

        /// <summary>
        /// The reason why the 'Finished' trainee lesson was rejected by a Mentor (or higher).
        /// This property is required if the state changes to 'Rejected'
        /// </summary>
        public string? RejectionReason { get; set; }

        /// <summary>
        /// The Date when the trainee lesson was started.
        /// Resets to null if it enters 'Open' again.
        /// </summary>
        public DateOnly? DayStarted { get; set; }

        /// <summary>
        /// The Date when the trainee lesson was finished.
        /// Resets to null, if it enters 'Rejected' or re-enters 'Started'.
        /// </summary>
        public DateOnly? DayFinished { get; set; }

        /// <summary>
        /// The ID of the lesson corresponding to the trainee lesson.
        /// </summary>
        public required int LessonId { get; set; }

        /// <summary>
        /// The lesson corresponding to the trainee lesson.
        /// </summary>
        public required Lesson Lesson { get; set; }

    }
}
