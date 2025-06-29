using TraineeTracker.Models;

namespace TraineeTracker.Models.ViewModels
{
    public class FeedbackDashboardViewModel
    {
        public Page<FeedbackDto> AllFeedbacks    { get; set; } = null!;
        public Page<FeedbackDto> UnreadFeedbacks { get; set; } = null!;
        public Page<FeedbackDto> ReadFeedbacks   { get; set; } = null!;

        // Für schnelle Kennzahlen
        public int TotalFeedbackCount  => AllFeedbacks.TotalItems;
        public int UnreadFeedbackCount => UnreadFeedbacks.TotalItems;
        public int ReadFeedbackCount   => ReadFeedbacks.TotalItems;
    }
}

