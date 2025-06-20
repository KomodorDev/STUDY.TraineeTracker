using Microsoft.AspNetCore.Identity;

namespace TraineeTracker.Models.Domain {
    public class ProcessingPause {
        public int Id {
            get;
            set;
        }

        public DateTime StartDate {
            get;
            set;
        }

        public DateTime EndDate {
            get;
            set;
        }
    }
}