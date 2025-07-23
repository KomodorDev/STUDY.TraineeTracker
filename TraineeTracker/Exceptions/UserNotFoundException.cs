namespace TraineeTracker.Exceptions {

    /// <summary>
    /// A custom exception class, that should be used when a user was not found.
    /// Can be used for both ApplicationUser and ClaimsPrincipal.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexander Schlemmer (schleale)
    /// </remarks>
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