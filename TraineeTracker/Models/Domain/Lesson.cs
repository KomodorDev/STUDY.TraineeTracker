namespace TraineeTracker.Models.Domain
{
    public class Lesson {
        public required int LessonId {
            get; set;
        }

        public required string Title {
            get; set;
        }

        public required double EstimatedEffort {
            get; set;
        }

        public required string LinkUrl {
            get; set;
        }

        public bool IsInactive { get; set; } = false;

        public List<Feedback> Feedbacks { get; set; } = new();
    
        public ICollection<TeachingPlan> TeachingPlans { get; set; } = new List<TeachingPlan>();
    

    }
}
