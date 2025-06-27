using Microsoft.AspNetCore.Identity;

namespace TraineeTracker.Models.Domain {
    public class ApplicationUser : IdentityUser {
        public bool IsClosed { get; set; } = false; // to be implemented
        public EmailNotificationSetting? EmailNotificationSetting { get; set; }
        
        public DateTime? TraineeStartDate { get; set; }
        public DateTime? TraineeEndDate { get; set; }
        public TeachingPlan? TeachingPlan { get; set; }
        public TraineeStatisticsSnapshot? TraineeStatisticsSnapshot { get; set; }

        public ICollection<Feedback> ReadFeedbacks { get; set; } = new List<Feedback>();
        public ICollection<Feedback> WrittenFeedbacks { get; set; } = new List<Feedback>();
        public ICollection<ProcessingPause> ProcessingPauses { get; set; } = new List<ProcessingPause>();
        public ICollection<TraineeLesson> TraineeLessons { get; set; } = new List<TraineeLesson>();
    }
}
