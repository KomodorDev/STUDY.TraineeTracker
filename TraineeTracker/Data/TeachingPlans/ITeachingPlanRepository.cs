using System.Collections.Generic;
using System.Threading.Tasks;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.TeachingPlans
{
    public interface ITeachingPlanRepository
    {
        bool Exists(int id);

        bool Exists(TeachingPlan teachingPlan);

        Task Create(TeachingPlan teachingPlan);

        Task Update(TeachingPlan teachingPlan);

        Task Delete(TeachingPlan teachingPlan);

        Task<TeachingPlan?> GetTeachingPlanById(int id);

        TeachingPlan? GetTeachingPlanByIdWithLessonsAndTrainees(int id);

        IEnumerable<TeachingPlan> GetAllTeachingPlans();
    }
}
