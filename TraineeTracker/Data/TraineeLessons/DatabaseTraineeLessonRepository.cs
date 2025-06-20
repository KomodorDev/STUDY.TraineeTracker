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
            foreach (var traineeLesson in traineeLessons)
                _context.TraineeLessons.Add(traineeLesson);
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
                .Include(t => t.Lesson)
                .Where(tl => tl.LessonId == lessonId)
                .ToList();
        }

        public IEnumerable<TraineeLesson> GetAllTraineeLessonsOfTrainee(string traineeId) {
            return _context.TraineeLessons
                .Include(t => t.Lesson)
                .Where(tl => tl.UserId == traineeId)
                .ToList();
        }

        public TraineeLesson? GetTraineeLessonById(int traineeLessonId) {
            return _context.TraineeLessons
                .Include(t => t.Lesson)
                .FirstOrDefault(tl => tl.TraineeLessonId == traineeLessonId);
        }
    }
}