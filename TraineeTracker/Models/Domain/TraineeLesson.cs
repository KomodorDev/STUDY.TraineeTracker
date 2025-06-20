using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TraineeTracker.Models.Domain
{
    public class TraineeLesson {

        // Key
        public int TraineeLessonId { get; }

        public string UserId { get; }

        public TraineeLessonState State { get; set; }

        // only required if state is rejected
        public string? RejectionReason { get; set; }

        public DateOnly? DayStarted { get; set; }

        public DateOnly? DayFinished { get; set; }
        
        public int LessonId { get; }
 
        // ---------- Constructors
        public TraineeLesson() { }

        public TraineeLesson(int lessonId, int traineeLessonId, string userId) {
            LessonId = lessonId;
            TraineeLessonId = traineeLessonId;
            UserId = userId;
        }
    }
}