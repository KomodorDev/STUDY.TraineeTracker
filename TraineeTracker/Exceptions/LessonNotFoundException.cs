namespace TraineeTracker.Exceptions {

    /// <summary>
    /// A custom exception class, that should be used when a lesson was not found.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexander Schlemmer (schleale)
    /// </remarks>
    public class LessonNotFoundException : Exception {

        // : base() passes parameters upward to Exception class
        public LessonNotFoundException()
            : base("The specified Lesson could not be found.") {

        }

        public LessonNotFoundException(int lessonId) : base($"The Lesson with ID {lessonId} could not be found.") {

        }

        public LessonNotFoundException(string message)
            : base(message) {

        }

        public LessonNotFoundException(string message, Exception innerException)
            : base(message, innerException) {

        }
    }
}
