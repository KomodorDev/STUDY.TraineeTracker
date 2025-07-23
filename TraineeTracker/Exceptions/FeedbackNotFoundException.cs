namespace TraineeTracker.Exceptions
{
    /// <summary>
    /// A custom exception class, that should be used when a feedback was not found.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexander Schlemmer (schleale)
    /// </remarks>
    public class FeedbackNotFoundException : Exception {
        // : base() passes parameters upward to Exception class
        public FeedbackNotFoundException()
            : base("The specified Feedback could not be found.") {

        }

        public FeedbackNotFoundException(int feedbackId)
            : base($"The Feedback with ID {feedbackId} could not be found.") {

        }

        public FeedbackNotFoundException(string message)
            : base(message) {

        }

        public FeedbackNotFoundException(string message, Exception innerException)
            : base(message, innerException) {

        }
    }
}