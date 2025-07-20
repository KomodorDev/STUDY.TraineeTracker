using System.ComponentModel.DataAnnotations;

namespace TraineeTracker.Models.Domain {

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