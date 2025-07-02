using System.ComponentModel.DataAnnotations;

namespace TraineeTracker.Models.ViewModels
{
    public class ImportDashboardViewModel
    {
        [Required] public string NewPlanName   { get; set; }
        [Required] public IFormFile NewPlanFile { get; set; }
        public List<ExistingPlan> ExistingTeachingPlans { get; set; }
            = new();

        public class ExistingPlan
        {
            public int    TeachingPlanId { get; set; }
            public string Name           { get; set; }
            public DateTime LastUpdated  { get; set; }
            public int    LessonCount    { get; set; }
            public int    TraineeCount   { get; set; }
        }
    }
}

