namespace TraineeTracker.Models.Domain {
    public class TraineeStatisticsSnapshot
    {
        public int TraineeStatisticsSnapshotId { get; set; }

        public required string TraineeId { get; set; }
        public required ApplicationUser Trainee { get; set; }

        // das Datum von dem Snapshot zu den einzelnen Kennzahlen
        public DateTime SnapshotDateTime { get; set; }

        //entspricht der Kennzahl "Anwesend"
        public double? DaysPresentTotal { get; set; }

        public double? DaysPresentTillToday { get; set; }

        //entspricht der Kennzahl "Geschafft"
        public double? LessonDaysCompleted { get; set; }

        //entspricht der Kennzahl "Offen"
        public double? LessonDaysOpen { get; set; }

        //entspricht der Kennzahl "Puffer"
        public double? LessonDaysBuffer { get; set; }

        //entspricht der Kennzahl "Geschwindigkeit"
        public double? Speed { get; set; }

        //entspricht der Kennzahl "Puffer Vorhersage"
        public double? PredictedMissingEstimatedEffortAtEnd { get; set; }

        public double? PredictedMissingActualDays { get; set; }

        public double? PredictedEffortEndX { get; set; }
        
        public bool IsUpToDate { get; set; }
    }
}