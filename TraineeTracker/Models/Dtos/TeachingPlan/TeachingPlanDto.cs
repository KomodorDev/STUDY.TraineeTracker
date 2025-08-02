using System.ComponentModel.DataAnnotations;

namespace TraineeTracker.Models.Dtos {

    // ------------------------------------------------------
    /// <summary>
    /// Provides Dto for the TeachingPlan
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexandros Blask
    /// </remarks>
    public class TeachingPlanDto {

        /// <summary>
        /// IFormFile object for the TeachingPlan
        /// </summary>
        public IFormFile? NewPlanFile { get; set; }

        /// <summary>
        /// The Id of the existing TeachingPlan
        /// </summary>
        public int? ExistingTeachingPlanId { get; set; }

        /// <summary>
        /// The new Name for the TeachingPlan
        /// </summary>
        public string? NewPlanName { get; set; }

        /// <summary>
        /// The temporary filename
        /// </summary>
        public string? TempFileName { get; set; }
    }
}
