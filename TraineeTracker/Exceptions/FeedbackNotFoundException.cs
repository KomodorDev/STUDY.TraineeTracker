namespace TraineeTracker.Exceptions
{
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