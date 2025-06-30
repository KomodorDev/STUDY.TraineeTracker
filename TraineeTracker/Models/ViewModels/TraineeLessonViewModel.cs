using TraineeTracker.Models.Domain;
using System;

namespace TraineeTracker.Models.ViewModels {
    public class TraineeLessonViewModel {
        public string Title { get; set; } = string.Empty;
        public double EstimatedEffort { get; set; }
        public double WeightedEffort { get; set; }
        public TraineeLessonState State { get; set; }
    }
}

