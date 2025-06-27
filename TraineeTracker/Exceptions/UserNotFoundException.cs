// class by schleale

namespace TraineeTracker.Exceptions {
    public class UserNotFoundException : Exception {

        // : base() passes parameters upward to Exception class
        public UserNotFoundException()
            : base("The specified User could not be found.") {

        }

        public UserNotFoundException(string message)
            : base(message) {

        }

        public UserNotFoundException(string message, Exception innerException)
            : base(message, innerException) {

        }
    }
}