// class by schleale

namespace TraineeTracker.Exceptions {
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