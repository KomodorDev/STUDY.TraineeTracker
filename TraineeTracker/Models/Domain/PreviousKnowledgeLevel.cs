using System.ComponentModel.DataAnnotations;

namespace TraineeTracker.Models.Domain {

    /// <summary>
    /// Represents the possible selectable previous knowledge levels, that a trainee can select during feedback creation/editing in the corresponding dropdown menu.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Simon Hinterreiter (hintsimo)
    /// </remarks>
    public enum PreviousKnowledgeLevel {
        [Display(Name = "No Prior Knowledge")]
        None,

        [Display(Name = "Basic Awareness")]
        Aware,

        [Display(Name = "Foundational Knowledge")]
        Basic,

        [Display(Name = "Practical Experience")]
        Intermediate,

        [Display(Name = "Extensive Experience")]
        Advanced,

        [Display(Name = "Expert-Level Mastery")]
        Expert
    }
}