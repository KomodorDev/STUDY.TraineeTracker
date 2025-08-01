using TraineeTracker.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace TraineeTracker.Data.TeachingPlans {

    // ------------------------------------------------------
    /// <summary>
    /// Provides CRUD operations for <see cref="TeachingPlan"/> entities using Entity Framework Core.
    /// Implements the <see cref="ITeachingPlanRepository"/> interface.
    /// Code Ownership: Alexandros Blask
    /// </summary>
    public class DatabaseTeachingPlanRepository : ITeachingPlanRepository {

        // ------------------------------------------------------
        /// <summary>
        /// The Entity Framework Core database context used for accessing and modifying teaching plans.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        private readonly ApplicationDbContext _context;

        // ------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseTeachingPlanRepository"/> class.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="context">The database context used for accessing the TeachingPlans table.</param>
        public DatabaseTeachingPlanRepository(ApplicationDbContext context) {
            _context = context;
        }

        // ------------------------------------------------------
        /// <summary>
        /// Checks whether a <see cref="TeachingPlan"/> with the specified ID exists.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="id">The ID of the teaching plan to check.</param>
        /// <returns>True if the teaching plan exists; otherwise, false.</returns>
        public async Task<bool> ExistsAsync(int id) {
            return await _context.TeachingPlans.AnyAsync(tp => tp.TeachingPlanId == id);
        }

        // ------------------------------------------------------
        /// <summary>
        /// Checks whether a given <see cref="TeachingPlan"/> entity already exists based on name and last updated date.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="teachingPlan">The teaching plan to check.</param>
        /// <returns>True if a matching teaching plan exists; otherwise, false.</returns>
        public async Task<bool> ExistsAsync(TeachingPlan teachingPlan) {
            return await _context.TeachingPlans.AnyAsync(tp =>
            tp.Name == teachingPlan.Name &&
            tp.LastUpdated == teachingPlan.LastUpdated);
        }

        // ------------------------------------------------------
        /// <summary>
        /// Adds a new <see cref="TeachingPlan"/> to the database.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="teachingPlan">The teaching plan to create.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task CreateAsync(TeachingPlan teachingPlan) {
            await _context.TeachingPlans.AddAsync(teachingPlan);
            await _context.SaveChangesAsync();
        }

        // ------------------------------------------------------
        /// <summary>
        /// Updates an existing <see cref="TeachingPlan"/> in the database.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="teachingPlan">The updated teaching plan entity.</param>
        /// <returns>A Task representing the asynchronous update operation.</returns>
        public async Task UpdateAsync(TeachingPlan teachingPlan) {
            _context.TeachingPlans.Update(teachingPlan);
            await _context.SaveChangesAsync();
        }

        // ------------------------------------------------------
        /// <summary>
        /// Deletes a specified <see cref="TeachingPlan"/> from the database.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="teachingPlan">The teaching plan to delete.</param>
        /// <returns>A Task representing the asynchronous delete operation.</returns>
        public async Task DeleteAsync(TeachingPlan teachingPlan) {
            _context.TeachingPlans.Remove(teachingPlan);
            await _context.SaveChangesAsync();
        }

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves a <see cref="TeachingPlan"/> by its ID.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="id">The ID of the teaching plan.</param>
        /// <returns>The matching teaching plan or null if not found.</returns>
        public async Task<TeachingPlan?> GetTeachingPlanByIdAsync(int id) {
            return await _context.TeachingPlans
            .FirstOrDefaultAsync(tp => tp.TeachingPlanId == id);

        }

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves a <see cref="TeachingPlan"/> by ID, including its associated lessons and trainees.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="id">The ID of the teaching plan.</param>
        /// <returns>The teaching plan with related entities or null if not found.</returns>
        public async Task<TeachingPlan?> GetTeachingPlanByIdWithLessonsAndTraineesAsync(int id) {
            return await _context.TeachingPlans
                .Include(tp => tp.Trainees)
                .Include(tp => tp.Lessons)
                .FirstOrDefaultAsync(tp => tp.TeachingPlanId == id);

        }

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves all <see cref="TeachingPlan"/> entities from the database.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <returns>A list of all teaching plans.</returns>
        public async Task<IEnumerable<TeachingPlan>> GetAllTeachingPlansAsync() {
            return await _context.TeachingPlans.ToListAsync();
        }

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves all <see cref="TeachingPlan"/> entities including their lessons and trainees.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <returns>A list of teaching plans with related entities.</returns>
        public async Task<IEnumerable<TeachingPlan>> GetAllTeachingPlansWithLessonsAndTraineesAsync() {
            return await _context.TeachingPlans
            .Include(tp => tp.Lessons)
            .Include(tp => tp.Trainees)
            .ToListAsync();
        }
    }
}
