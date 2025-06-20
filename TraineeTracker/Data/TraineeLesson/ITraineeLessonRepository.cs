using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TraineeTracker.Data.TraineeLesson {
    public interface ITraineeLessonRepository {
        public bool Exists(int traineeLessonId);
        public bool Exists(TraineeLesson traineeLesson);
        public void Create(TraineeLesson traineeLesson);
        public void CreateRange(IEnumerable<TraineeLesson> traineeLessons);
        public void Update(TraineeLesson traineeLesson);
        public TraineeLesson GetTraineeLessonById(int traineeLessonId);
        public IEnumerable<TraineeLesson> GetAllTraineeLessonsOfTrainee(string traineeId);
        public IEnumerable<TraineeLesson> GetAllTraineeLessonsOfLesson(int lessonId);
    }
}