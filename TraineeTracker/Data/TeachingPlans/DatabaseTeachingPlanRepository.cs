using TraineeTracker.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace TraineeTracker.Data.TeachingPlans {

    public class DatabaseTeachingPlanRepository : ITeachingPlanRepository {

        private readonly ApplicationDbContext _context;

        public DatabaseTeachingPlanRepository(ApplicationDbContext context) {
            _context = context;
        }

        public async Task<bool> ExistsAsync(int id) {
            return await _context.TeachingPlans.AnyAsync(tp => tp.TeachingPlanId == id);
        }

        public async Task<bool> ExistsAsync(TeachingPlan teachingPlan) {
            return await _context.TeachingPlans.AnyAsync(tp =>
            tp.Name == teachingPlan.Name &&
            tp.LastUpdated == teachingPlan.LastUpdated);
        }

        public async Task CreateAsync(TeachingPlan teachingPlan) {
            await _context.TeachingPlans.AddAsync(teachingPlan);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TeachingPlan teachingPlan) {
            _context.TeachingPlans.Update(teachingPlan);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TeachingPlan teachingPlan) {
            _context.TeachingPlans.Remove(teachingPlan);
            await _context.SaveChangesAsync();
        }

        public async Task<TeachingPlan?> GetTeachingPlanByIdAsync(int id) {
            return await _context.TeachingPlans
            .FirstOrDefaultAsync(tp => tp.TeachingPlanId == id);

        }

        public async Task<TeachingPlan?> GetTeachingPlanByIdWithLessonsAndTraineesAsync(int id) {
            return await _context.TeachingPlans
                .Include(tp => tp.Trainees)
                .Include(tp => tp.Lessons)
                .FirstOrDefaultAsync(tp => tp.TeachingPlanId == id);

        }

        public async Task<IEnumerable<TeachingPlan>> GetAllTeachingPlansWithLessonsAndTraineesAsync() {
            return await _context.TeachingPlans
            .Include(tp => tp.Lessons)
            .Include(tp => tp.Trainees)
            .ToListAsync();
        }
    }
}
