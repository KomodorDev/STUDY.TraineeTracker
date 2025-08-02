using System.ComponentModel.DataAnnotations;

namespace TraineeTracker.Models.Domain {

    /// <summary>
    /// Represents the possible difficulties, that a user can select during feedback creation/editing in the corresponding dropdown menu.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Simon Hinterreiter (hintsimo)
    /// </remarks>
    public enum LessonDifficulty {
        [Display(Name = "Very Easy")]
        VeryEasy,

        [Display(Name = "Easy")]
        Easy,

        [Display(Name = "Medium")]
        Medium,

        [Display(Name = "Hard")]
        Hard,

        [Display(Name = "Very Hard")]
        VeryHard,

        [Display(Name = "Insane")]
        Insane
    }
}