using Microsoft.EntityFrameworkCore;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.TraineeLessons {
    public class DatabaseTraineeLessonRepository : ITraineeLessonRepository {

        private ApplicationDbContext _context;

        public DatabaseTraineeLessonRepository(ApplicationDbContext context) {
            _context = context;
        }

        public void Create(TraineeLesson traineeLesson) {
            _context.TraineeLessons.Add(traineeLesson);
            _context.SaveChanges();
        }

        public void CreateRange(IEnumerable<TraineeLesson> traineeLessons) {
            _context.AddRange(traineeLessons);
            _context.SaveChanges();
        }

        public bool Exists(int traineeLessonId) {
            return _context.TraineeLessons.Any(tl => tl.TraineeLessonId == traineeLessonId);
        }

        public bool Exists(TraineeLesson traineeLesson) {
            return _context.TraineeLessons.Any(tl => tl.TraineeLessonId == traineeLesson.TraineeLessonId);
        }

        public void Update(TraineeLesson traineeLesson) {
            _context.TraineeLessons.Update(traineeLesson);
            _context.SaveChanges();
        }

        public IEnumerable<TraineeLesson> GetAllTraineeLessonsOfLesson(int lessonId) {
            return _context.TraineeLessons
                .Include(t => t.Lesson)     // eager loads the Lesson for easier access to properties of the fitting lesson
                .Where(tl => tl.LessonId == lessonId)
                .ToList();                  // "give me all lessons now" -> loaded into memory; if lots of further sorting is required, remove.
        }

        public IEnumerable<TraineeLesson> GetAllTraineeLessonsOfTrainee(string traineeId) {
            return _context.TraineeLessons
                .Include(t => t.Lesson)     // eager loads the Lesson for easier access to properties of the fitting lesson
                .Where(tl => tl.UserId == traineeId)
                .ToList();                  // "give me all lessons now" -> loaded into memory; if lots of further sorting is required, remove.
        }

        public TraineeLesson? GetTraineeLessonById(int traineeLessonId) {
            return _context.TraineeLessons
                .Include(t => t.Lesson)     // eager loads the Lesson for easier access to properties of the fitting lesson
                .FirstOrDefault(tl => tl.TraineeLessonId == traineeLessonId);
        }
    }
}