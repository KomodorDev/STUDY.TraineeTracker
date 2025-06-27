using Microsoft.EntityFrameworkCore;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.TraineeLessons {
    public class DatabaseTraineeLessonRepository : ITraineeLessonRepository {

        private ApplicationDbContext _context;

        public DatabaseTraineeLessonRepository(ApplicationDbContext context) {
            _context = context;
        }

        public async Task CreateAsync(TraineeLesson traineeLesson) {
            await _context.TraineeLessons.AddAsync(traineeLesson);
            await _context.SaveChangesAsync();
        }

        public async Task CreateRangeAsync(IEnumerable<TraineeLesson> traineeLessons) {
            await _context.AddRangeAsync(traineeLessons);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int traineeLessonId) {
            return await _context.TraineeLessons.AnyAsync(tl => tl.TraineeLessonId == traineeLessonId);
        }

        public async Task<bool> ExistsAsync(TraineeLesson traineeLesson) {
            return await _context.TraineeLessons.AnyAsync(tl => tl.TraineeLessonId == traineeLesson.TraineeLessonId);
        }

        public async Task UpdateAsync(TraineeLesson traineeLesson) {
            _context.TraineeLessons.Update(traineeLesson);      // Update is not async
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TraineeLesson>> GetAllTraineeLessonsOfLessonAsync(int lessonId) {
            return await _context.TraineeLessons
                .Include(t => t.Lesson)     // eager loads the Lesson for easier access to properties of the fitting lesson
                .Where(tl => tl.LessonId == lessonId)
                .ToListAsync();                  // "give me all lessons now" -> loaded into memory; if lots of further sorting is required, remove.
        }

        public async Task<IEnumerable<TraineeLesson>> GetAllTraineeLessonsOfTraineeAsync(string traineeId) {
            return await _context.TraineeLessons
                .Include(t => t.Lesson)     // eager loads the Lesson for easier access to properties of the fitting lesson
                .Where(tl => tl.UserId == traineeId)
                .ToListAsync();                  // "give me all lessons now" -> loaded into memory; if lots of further sorting is required, remove.
        }

        public async Task<TraineeLesson?> GetTraineeLessonByIdAsync(int traineeLessonId) {
            return await _context.TraineeLessons
                .Include(t => t.Lesson)     // eager loads the Lesson for easier access to properties of the fitting lesson
                .FirstOrDefaultAsync(tl => tl.TraineeLessonId == traineeLessonId);
        }
    }
}