using System.Runtime.CompilerServices;
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
        public List<ProcessingPause> ProcessingPauses;
        public List<TraineeLesson> TraineeLessons;
    }
}