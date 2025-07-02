using TraineeTracker.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace TraineeTracker.Data.Lessons {
    public class DatabaseLessonRepository : ILessonRepository {
        private readonly ApplicationDbContext _context;

        public DatabaseLessonRepository(ApplicationDbContext context) {
            _context = context;
        }

        public async Task<bool> ExistsAsync(int id) {
            return await _context.Lessons.AnyAsync(l => l.LessonId == id);
        }

        public async Task<bool> ExistsAsync(Lesson lesson) {
            return await _context.Lessons.AnyAsync(l =>
            l.Title == lesson.Title &&
            l.LinkUrl == lesson.LinkUrl &&
            l.EstimatedEffort == lesson.EstimatedEffort);
        }

        public async Task CreateAsync(Lesson lesson) {
            await _context.Lessons.AddAsync(lesson);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Lesson lesson) {
            _context.Lessons.Update(lesson);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Lesson lesson) {
            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();
        }

        public async Task<Lesson?> GetLessonByIdAsync(int id) {
            return await _context.Lessons
            .Include(l => l.Feedbacks)
            .FirstOrDefaultAsync(l => l.LessonId == id);
        }

        public async Task<Lesson?> GetLessonByTitleAsync(string title) {
            return await _context.Lessons
            .FirstOrDefaultAsync(l => l.Title == title);
        }

        public async Task<IEnumerable<Lesson>> GetAllLessonsAsync() {
            return await _context.Lessons
            .Include(l => l.Feedbacks)
            .ToListAsync();
        }
    }
}
