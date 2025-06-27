using Microsoft.EntityFrameworkCore;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.TraineeLessons {
    public class DatabaseTraineeLessonRepository : ITraineeLessonRepository {

        private ApplicationDbContext _context;

        public DatabaseTraineeLessonRepository(ApplicationDbContext context) {
            _context = context;
        }

        public async Task Create(TraineeLesson traineeLesson) {
            await _context.TraineeLessons.AddAsync(traineeLesson);
            await _context.SaveChangesAsync();
        }

        public async Task CreateRange(IEnumerable<TraineeLesson> traineeLessons) {
            await _context.AddRangeAsync(traineeLessons);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> Exists(int traineeLessonId) {
            return await _context.TraineeLessons.AnyAsync(tl => tl.TraineeLessonId == traineeLessonId);
        }

        public async Task<bool> Exists(TraineeLesson traineeLesson) {
            return await _context.TraineeLessons.AnyAsync(tl => tl.TraineeLessonId == traineeLesson.TraineeLessonId);
        }

        public async Task Update(TraineeLesson traineeLesson) {
            _context.TraineeLessons.Update(traineeLesson);      // Update is not async
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TraineeLesson>> GetAllTraineeLessonsOfLesson(int lessonId) {
            return await _context.TraineeLessons
                .Include(t => t.Lesson)     // eager loads the Lesson for easier access to properties of the fitting lesson
                .Where(tl => tl.LessonId == lessonId)
                .ToListAsync();                  // "give me all lessons now" -> loaded into memory; if lots of further sorting is required, remove.
        }

        public async Task<IEnumerable<TraineeLesson>> GetAllTraineeLessonsOfTrainee(string traineeId) {
            return await _context.TraineeLessons
                .Include(t => t.Lesson)     // eager loads the Lesson for easier access to properties of the fitting lesson
                .Where(tl => tl.UserId == traineeId)
                .ToListAsync();                  // "give me all lessons now" -> loaded into memory; if lots of further sorting is required, remove.
        }

        public async Task<TraineeLesson?> GetTraineeLessonById(int traineeLessonId) {
            return await _context.TraineeLessons
                .Include(t => t.Lesson)     // eager loads the Lesson for easier access to properties of the fitting lesson
                .FirstOrDefaultAsync(tl => tl.TraineeLessonId == traineeLessonId);
        }
    }
}