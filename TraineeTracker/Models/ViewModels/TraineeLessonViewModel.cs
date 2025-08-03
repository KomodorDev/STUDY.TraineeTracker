using TraineeTracker.Models.Domain;

namespace TraineeTracker.Models.ViewModels {

    /// <summary>
    /// ViewModel that represents a single trainee lesson with state and effort details.
    /// Used in statistics views for display and analysis.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Nikita Stefan (stefanni)
    /// </remarks>
    public class TraineeLessonViewModel {

        /// <summary>
        /// The title of the lesson.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// The estimated effort for the lesson (in days).
        /// </summary>
        public double EstimatedEffort { get; set; }

        /// <summary>
        /// The weighted effort based on the lesson state (e.g. 0.7, 1.0, 0.8).
        /// </summary>
        public double WeightedEffort { get; set; }

        /// <summary>
        /// The current state of the trainee's lesson (e.g. Finished, Open, Accepted).
        /// </summary>
        public TraineeLessonState State { get; set; }

        /// <summary>
        /// Optional display string used to group or label the lesson in charts or lists.
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }
}
