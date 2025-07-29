using TraineeTracker.Models.Dtos;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Models.ViewModels.Admin {
    /// <summary>
    /// ViewModel for managing processing pauses for a trainee in the admin area.
    /// </summary>
    /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
    public class ManageProcessingPausesViewModel {
        /// <summary>
        /// Gets or sets the username of the trainee.
        /// </summary>
        public string UserName { get; set; } = "";

        /// <summary>
        /// Gets or sets the unique identifier of the trainee.
        /// </summary>
        public string TraineeId { get; set; } = "";

        /// <summary>
        /// Gets or sets the list of existing processing pauses for the trainee.
        /// </summary>
        public List<ProcessingPause> ProcessingPauses { get; set; } = new();

        /// <summary>
        /// Gets or sets the new processing pause to be added for the trainee.
        /// </summary>
        public ProcessingPauseDto NewProcessingPause { get; set; } = new();
    }
}