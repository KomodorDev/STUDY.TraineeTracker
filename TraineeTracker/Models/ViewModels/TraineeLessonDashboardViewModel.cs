using TraineeTracker.Models.Domain;

namespace TraineeTracker.Models.ViewModels {

    /// <summary>
    /// A viewmodel containing all necessary information to display the main page
    /// (dashboard) of a specific trainee.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexander Schlemmer (schleale)
    /// </remarks>
    public class TraineeLessonDashboardViewModel {

        /// <summary>
        /// The current selected trainee, of which the information is displayed.
        /// </summary>
        public ApplicationUser? SelectedTrainee { get; set; }

        /// <summary>
        /// The latest statistics snapshot of the trainee, containing statistics like e.g. completion percentage
        /// </summary>
        public TraineeStatisticsSnapshot? TraineeStatisticsSnapshot { get; set; }

        /// <summary>
        /// The list of all trainee lessons the trainee can, is or has been working on.
        /// </summary>
        public IEnumerable<TraineeLesson>? TraineeLessons { get; set; }

        /// <summary>
        /// A list of all open trainees, allowing mentors to change the currently viewed trainee, and hidden for trainees themselves.
        /// </summary>
        public IEnumerable<ApplicationUser>? Trainees { get; set; }

        /// <summary>
        /// The current filtering of the trainee lessons. Lessons can be filtered by their state.
        /// </summary>
        public string ActiveFilter { get; set; } = "all";

        /// <summary>
        /// The current sorting of the trainee lessons. Currently, they are always sorted according to makandra's preferences.
        /// </summary>
        public string SortBy { get; set; } = "state_custom";

        /// <summary>
        /// A number for counting how many trainee lessons are in a specific state, in this case, all states excluding skipped.
        /// </summary>
        public int CountAll { get; set; }

        /// <summary>
        /// A number for counting how many trainee lessons are in a specific state, in this case, 'Open'.
        /// </summary>
        public int CountOpen { get; set; }

        /// <summary>
        /// A number for counting how many trainee lessons are in a specific state, in this case, 'Started'.
        /// </summary>
        public int CountStarted { get; set; }

        /// <summary>
        /// A number for counting how many trainee lessons are in a specific state, in this case, 'Finished'.
        /// </summary>
        public int CountFinished { get; set; }

        /// <summary>
        /// A number for counting how many trainee lessons are in a specific state, in this case, 'Accepted'.
        /// </summary>
        public int CountAccepted { get; set; }

        /// <summary>
        /// A number for counting how many trainee lessons are in a specific state, in this case, 'Rejected'.
        /// </summary>
        public int CountRejected { get; set; }
        /// <summary>
        /// A number for counting how many trainee lessons are in a specific state, in this case, 'Rated'.
        /// </summary>
        public int CountRated { get; set; }

        /// <summary>
        /// A number for counting how many trainee lessons are in a specific state, in this case, 'Skipped'.
        /// </summary>
        public int CountSkipped { get; set; }
    }
}