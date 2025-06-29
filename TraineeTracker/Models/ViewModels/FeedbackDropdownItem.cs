namespace TraineeTracker.Models.ViewModels
{
    public class FeedbackDropdownItem
    {
        public int FeedbackId { get; set; }
        public string AuthorUserName { get; set; }
        public DateTime CreateTime { get; set; }
        public string Comment { get; set; }
    }
}