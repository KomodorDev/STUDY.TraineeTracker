namespace TraineeTracker.Models.Dtos{
    public class LessonDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public double? Estimate { get; set; }
        public bool Deprecated { get; set; }
    }
}
