using System.Collections.Generic;
using System.Linq;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.Lessons
{
    public class DatabaseLessonRepository : ILessonRepository
    {
        private readonly ApplicationDbContext _context;

        public DatabaseLessonRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Exists(int id)
        {
            return _context.Lessons.Any(l => l.LessonId == id);
        }

        public bool Exists(Lesson lesson)
        {
            return _context.Lessons.Any(l =>
            l.Title == lesson.Title &&
            l.LinkUrl == lesson.LinkUrl &&
            l.EstimatedEffort == lesson.EstimatedEffort);
        }

        public void Create(Lesson lesson)
        {
            _context.Lessons.Add(lesson);
            _context.SaveChanges();
        }

        public void Update(Lesson lesson)
        {
            _context.Lessons.Update(lesson);
            _context.SaveChanges();
        }

        public void Delete(Lesson lesson)
        {
            _context.Lessons.Remove(lesson);
            _context.SaveChanges();
        }

        public Lesson GetLessonById(int id)
        {
            return _context.Lessons.FirstOrDefault(l => l.LessonId == id);
        }

        public IEnumerable<Lesson> GetAllLessons()
        {
            return _context.Lessons.ToList();
        }
    }
}
