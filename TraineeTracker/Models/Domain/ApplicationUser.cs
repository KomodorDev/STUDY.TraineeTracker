using Microsoft.AspNetCore.Identity;

namespace TraineeTracker.Models.Domain {
    public class ApplicationUser : IdentityUser {
        public bool IsClosed {
            get;
            set;
        } = false; // to be implemented

        public DateTime? TraineeStartDate {
            get;
            set;
        }
        public DateTime? TraineeEndDate {
            get;
            set;
        }
        public ICollection<ProcessingPause> ProcessingPauses { get; set; } = new List<ProcessingPause>();
        public List<TraineeLesson> TraineeLessons { get; set; } = new List<TraineeLesson>();

    }
}