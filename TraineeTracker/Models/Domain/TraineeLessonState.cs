namespace TraineeTracker.Models.Domain {

    /// <summary>
    /// Represents the possible processing states of a trainee's lesson within the Trainee Tracker system.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Simon Hinterreiter (hintsimo)
    /// </remarks>
    public enum TraineeLessonState {
        Open,
        Started,
        Finished,
        Accepted,
        Rejected,
        Skipped,
        Rated
    }
}