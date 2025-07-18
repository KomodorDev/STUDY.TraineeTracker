using System.ComponentModel.DataAnnotations;

namespace TraineeTracker.Models.Domain {
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