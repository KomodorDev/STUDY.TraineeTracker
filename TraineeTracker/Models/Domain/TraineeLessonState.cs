namespace TraineeTracker.Models.Domain {
    
    // Enum for TraineeLessonStates to be used in TraineeLessonDetail Lane
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