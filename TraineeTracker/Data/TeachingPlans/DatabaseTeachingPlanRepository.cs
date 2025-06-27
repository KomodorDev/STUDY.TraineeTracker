using System.Collections.Generic;
using System.Linq;
using TraineeTracker.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace TraineeTracker.Data.TeachingPlans
{
    public class DatabaseTeachingPlanRepository : ITeachingPlanRepository
    {
        private readonly ApplicationDbContext _context;

        public DatabaseTeachingPlanRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Exists(int id)
        {
            return _context.TeachingPlans.Any(tp => tp.TeachingPlanId == id);
        }

        public bool Exists(TeachingPlan teachingPlan)
        {
            return _context.TeachingPlans.Any(tp =>
            tp.Name == teachingPlan.Name &&
            tp.LastUpdated == teachingPlan.LastUpdated);
        }

        public void Create(TeachingPlan teachingPlan)
        {
            _context.TeachingPlans.Add(teachingPlan);
            _context.SaveChanges();
        }

        public void Update(TeachingPlan teachingPlan)
        {
            _context.TeachingPlans.Update(teachingPlan);
            _context.SaveChanges();
        }

        public void Delete(TeachingPlan teachingPlan)
        {
            _context.TeachingPlans.Remove(teachingPlan);
            _context.SaveChanges();
        }

        public TeachingPlan? GetTeachingPlanByIdWithLessonAndTrainees(int id)
        {
            return _context.TeachingPlans
                .Include(tp => tp.Trainees)
                .Include(tp => tp.Lessons)
                .FirstOrDefault(tp => tp.TeachingPlanId == id);

        }

        public TeachingPlan? GetTeachingPlanById(int id)
        {
            return _context.TeachingPlans.FirstOrDefault(tp => tp.TeachingPlanId == id);
        }

        public IEnumerable<TeachingPlan> GetAllTeachingPlans()
        {
            return _context.TeachingPlans
            .Include(tp => tp.Lessons)
            .Include(tp => tp.Trainees)
            .ToList();
        }
    }
}
