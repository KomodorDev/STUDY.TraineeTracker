public class FeedbackDto
{
    public int FeedbackId { get; set; }
    public string AuthorName { get; set; }
    public int Difficulty { get; set; }
    public string PreviousKnowledge { get; set; }
    public float HoursOfEffort { get; set; }
    public DateTime CreateTime   { get; set; }
    public string? Comment     { get; set; }
}
