public class FeedbackDashboardViewModel
{
    public Page<FeedbackDto> AllFeedbacks    { get; set; }
    public Page<FeedbackDto> UnreadFeedbacks { get; set; }
    public Page<FeedbackDto> ReadFeedbacks   { get; set; }

    // Für schnelle Kennzahlen
    public int TotalFeedbackCount  => AllFeedbacks.TotalItems;
    public int UnreadFeedbackCount => UnreadFeedbacks.TotalItems;
    public int ReadFeedbackCount   => ReadFeedbacks.TotalItems;
}
