using Microsoft.EntityFrameworkCore;
using TraineeTracker.Exceptions;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.TraineeLessons {

    /// <summary>
    /// Provides CRUD operations for <see cref="TraineeLesson"/> entities using Entity Framework Core.
    /// Implements the <see cref="ITraineeLessonRepository"/> interface.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexander Schlemmer (schleale)
    /// </remarks>
    public class DatabaseTraineeLessonRepository : ITraineeLessonRepository {

        /// <summary>
        /// The Entity Framework Core database context used for data operations.
        /// </summary>
        private ApplicationDbContext _context;

        // ------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseTraineeLessonRepository"/> class
        /// with a provided <see cref="ApplicationDbContext"/>.
        /// </summary>
        /// <param name="context">The database context used to access the TraineeLessons table.</param>
        /// <remarks>
        /// Code Ownership: Alexander Schlemmer (schleale)
        /// </remarks>
        public DatabaseTraineeLessonRepository(ApplicationDbContext context) {
            _context = context;
        }

        // ------------------------------------------------------
        /// <summary>
        /// Adds a new <see cref="TraineeLesson"/> to the database.
        /// </summary>
        /// <param name="traineeLesson">The trainee lesson entity to be added.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <remarks>
        /// Code Ownership: Alexander Schlemmer (schleale)
        /// </remarks>
        public async Task CreateAsync(TraineeLesson traineeLesson) {
            await _context.TraineeLessons.AddAsync(traineeLesson);
            await _context.SaveChangesAsync();
        }

        // ------------------------------------------------------
        /// <summary>
        /// Adds multiple <see cref="TraineeLesson"/> entries to the database.
        /// </summary>
        /// <param name="traineeLessons">The collection of trainee lessons to be added.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// /// <remarks>
        /// Code Ownership: Alexander Schlemmer (schleale)
        /// </remarks>
        public async Task CreateRangeAsync(IEnumerable<TraineeLesson> traineeLessons) {
            await _context.TraineeLessons.AddRangeAsync(traineeLessons);
            await _context.SaveChangesAsync();
        }

         // ------------------------------------------------------
        /// <summary>
        /// Checks whether a trainee lesson exists based on its ID.
        /// </summary>
        /// <param name="traineeLessonId">The unique ID of the trainee lesson.</param>
        /// <returns>A Task representing the asynchronous operation. The result is true if the lesson exists; otherwise, false.</returns>
        /// <remarks>
        /// Code Ownership: Alexander Schlemmer (schleale)
        /// </remarks>
        public async Task<bool> ExistsAsync(int traineeLessonId) {
            return await _context.TraineeLessons.AnyAsync(tl => tl.TraineeLessonId == traineeLessonId);
        }

        // ------------------------------------------------------
        /// <summary>
        /// Checks whether a specific <see cref="TraineeLesson"/> instance exists in the database.
        /// </summary>
        /// <param name="traineeLesson">The trainee lesson entity to check.</param>
        /// <returns>A Task representing the asynchronous operation. The result is true if the lesson exists; otherwise, false.</returns>
        /// <remarks>
        /// Code Ownership: Alexander Schlemmer (schleale)
        /// </remarks>
        public async Task<bool> ExistsAsync(TraineeLesson traineeLesson) {
            return await _context.TraineeLessons.AnyAsync(tl => tl.TraineeLessonId == traineeLesson.TraineeLessonId);
        }


        // ------------------------------------------------------
        /// <summary>
        /// Updates an existing <see cref="TraineeLesson"/> entry in the database.
        /// </summary>
        /// <param name="traineeLesson">The trainee lesson with updated values.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <remarks>
        /// Code Ownership: Alexander Schlemmer (schleale)
        /// </remarks>
        public async Task UpdateAsync(TraineeLesson traineeLesson) {
            _context.TraineeLessons.Update(traineeLesson);      // Update is not async
            await _context.SaveChangesAsync();
        }


        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++
        /// <summary>
        /// Retrieves all <see cref="TraineeLesson"/> entries for a specific lesson, including the associated <see cref="Lesson"/>.
        /// </summary>
        /// <param name="lessonId">The ID of the lesson to filter trainee lessons by.</param>
        /// <returns>A Task representing the asynchronous operation. The result contains a collection of trainee lessons.</returns>
        /// <remarks>
        /// Code Ownership: Alexander Schlemmer (schleale)
        /// </remarks>
        public async Task<IEnumerable<TraineeLesson>> GetAllTraineeLessonsOfLessonWithLessonAsync(int lessonId) {
            return await _context.TraineeLessons
                .Include(t => t.Lesson)     // eager loads the Lesson for easier access to properties of the fitting lesson
                .Where(tl => tl.LessonId == lessonId)
                .ToListAsync();                  // "give me all lessons now" -> loaded into memory; if lots of further sorting is required, remove.
        }

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves all <see cref="TraineeLesson"/> entries for a specific trainee, including the associated <see cref="Lesson"/>.
        /// </summary>
        /// <param name="traineeId">The ID of the trainee to filter lessons by.</param>
        /// <returns>A Task representing the asynchronous operation. The result contains a collection of trainee lessons.</returns>
        /// <remarks>
        /// Code Ownership: Alexander Schlemmer (schleale)
        /// </remarks>
        public async Task<IEnumerable<TraineeLesson>> GetAllTraineeLessonsOfTraineeWithLessonAsync(string traineeId) {
            return await _context.TraineeLessons
                .Include(t => t.Lesson)     // eager loads the Lesson for easier access to properties of the fitting lesson
                .Where(tl => tl.TraineeId == traineeId)
                .ToListAsync();                  // "give me all lessons now" -> loaded into memory; if lots of further sorting is required, remove.
        }

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves a specific <see cref="TraineeLesson"/> by its ID, including the associated <see cref="Lesson"/>.
        /// </summary>
        /// <param name="traineeLessonId">The ID of the trainee lesson to retrieve.</param>
        /// <returns>
        /// A Task representing the asynchronous operation. The result contains the <see cref="TraineeLesson"/> if found; otherwise, null.
        /// </returns>
        /// <remarks>
        /// Code Ownership: Alexander Schlemmer (schleale)
        /// </remarks>
        public async Task<TraineeLesson?> GetTraineeLessonByIdWithLessonAsync(int traineeLessonId) {
            return await _context.TraineeLessons
                .Include(t => t.Lesson)     // eager loads the Lesson for easier access to properties of the fitting lesson
                .FirstOrDefaultAsync(tl => tl.TraineeLessonId == traineeLessonId);
        }

         // ------------------------------------------------------
        /// <summary>
        /// Deletes a <see cref="TraineeLesson"/> from the database based on its ID.
        /// Throws a <see cref="TraineeLessonNotFoundException"/> if no entry is found.
        /// </summary>
        /// <param name="traineeLessonId">The ID of the trainee lesson to delete.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <exception cref="TraineeLessonNotFoundException">Thrown when the trainee lesson does not exist.</exception>
        /// <remarks>
        /// Code Ownership: Alexander Schlemmer (schleale)
        /// </remarks>
        public async Task DeleteAsync(int traineeLessonId) {
            var traineeLesson = await _context.TraineeLessons
                                    .FirstOrDefaultAsync(l => l.TraineeLessonId == traineeLessonId)
                                        ?? throw new TraineeLessonNotFoundException(traineeLessonId);

            _context.TraineeLessons.Remove(traineeLesson);
            await _context.SaveChangesAsync();
        }
    }
}