using Microsoft.AspNetCore.Identity;

namespace TraineeTracker.Models.Domain {
    public class ApplicationUser : IdentityUser {
        public bool IsClosed { get; set; } = false; // to be implemented
        public required EmailNotificationSetting EmailNotificationSetting { get; set; }


        // Trainee
        public DateOnly? TraineeStartDate { get; set; }
        public DateOnly? TraineeEndDate { get; set; }
        public TeachingPlan? TeachingPlan { get; set; }
        public TraineeStatisticsSnapshot? TraineeStatisticsSnapshot { get; set; }

        public ICollection<Feedback> WrittenFeedbacks { get; set; } = new List<Feedback>();
        public ICollection<ProcessingPause> ProcessingPauses { get; set; } = new List<ProcessingPause>();
        public ICollection<TraineeLesson> TraineeLessons { get; set; } = new List<TraineeLesson>();


        // Mentor
        public ICollection<Feedback> ReadFeedbacks { get; set; } = new List<Feedback>();
        public List<ApplicationUser> LastSelectedTrainees { get; set; } = new List<ApplicationUser>();
    }
}
