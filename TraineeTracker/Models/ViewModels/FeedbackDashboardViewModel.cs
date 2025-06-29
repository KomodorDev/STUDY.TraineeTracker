using TraineeTracker.Models;
using TraineeTracker.Models.Dtos;

namespace TraineeTracker.Models.ViewModels
{
    public class FeedbackDashboardViewModel
    {
        public Page<FeedbackDashboardDto> AllFeedbacks    { get; set; } = null!;
        public Page<FeedbackDashboardDto> UnreadFeedbacks { get; set; } = null!;
        public Page<FeedbackDashboardDto> ReadFeedbacks   { get; set; } = null!;

        // Für schnelle Kennzahlen
        public int TotalFeedbackCount  => AllFeedbacks.TotalItems;
        public int UnreadFeedbackCount => UnreadFeedbacks.TotalItems;
        public int ReadFeedbackCount   => ReadFeedbacks.TotalItems;
    }
}

