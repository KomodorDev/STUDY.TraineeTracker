public class FeedbackDto
{
    public int FeedbackId { get; set; }
    public string AuthorName { get; set; } = null!;
    public int Difficulty { get; set; }
    public string PreviousKnowledge { get; set; }
    public float HoursOfEffort { get; set; }
    public DateTime SendDate   { get; set; }
    public string? Comment     { get; set; }
}
