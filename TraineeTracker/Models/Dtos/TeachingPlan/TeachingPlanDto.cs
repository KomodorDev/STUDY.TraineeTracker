using System.ComponentModel.DataAnnotations;

namespace TraineeTracker.Models.Dtos {

    // ------------------------------------------------------
    /// <summary>
    /// Provides Dto for the TeachingPlan
    /// Code Ownership: Alexandros Blask
    /// </summary>
    public class TeachingPlanDto {

        // ------------------------------------------------------
        /// <summary>
        /// IFormFile object for the TeachingPlan
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public IFormFile? NewPlanFile { get; set; }

        // ------------------------------------------------------
        /// <summary>
        /// The Id of the existing TeachingPlan
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public int? ExistingTeachingPlanId { get; set; }

        // ------------------------------------------------------
        /// <summary>
        /// The new Name for the TeachingPlan
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public string? NewPlanName { get; set; }

        // ------------------------------------------------------
        /// <summary>
        /// The temporary filename
        /// Code Ownership: Alexandros Blask
        /// </summary>
        public string? TempFileName { get; set; }
    }
}
