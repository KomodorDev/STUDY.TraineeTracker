using Microsoft.AspNetCore.Identity;

namespace TraineeTracker.Models.Domain {
    public class ProcessingPause {
        public int Id {
            get;
            private set;
        }

        public required int TraineeId {
            get;
            set;
        }

        public required DateTime StartDate {
            get;
            set;
        }

        public required DateTime EndDate {
            get;
            set;
        }
    }
}