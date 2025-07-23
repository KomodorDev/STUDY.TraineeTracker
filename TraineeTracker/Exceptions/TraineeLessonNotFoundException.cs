namespace TraineeTracker.Exceptions {
    
    /// <summary>
    /// A custom exception class, that should be used when a trainee lesson was not found.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexander Schlemmer (schleale)
    /// </remarks>
    public class TraineeLessonNotFoundException : Exception {

        // : base() passes parameters upward to Exception class
        public TraineeLessonNotFoundException()
            : base("The specified TraineeLesson could not be found.") {

        }

        public TraineeLessonNotFoundException(int traineeLessonId)
            : base($"The TraineeLesson with ID {traineeLessonId} could not be found.") {

        }

        public TraineeLessonNotFoundException(string message)
            : base(message) {

        }

        public TraineeLessonNotFoundException(string message, Exception innerException)
            : base(message, innerException) {

        }
    }
}