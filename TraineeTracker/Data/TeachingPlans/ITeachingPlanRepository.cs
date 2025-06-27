using System.Collections.Generic;
using System.Threading.Tasks;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.TeachingPlans
{
    public interface ITeachingPlanRepository
    {
        bool Exists(int id);

        bool Exists(TeachingPlan teachingPlan);

        void Create(TeachingPlan teachingPlan);

        void Update(TeachingPlan teachingPlan);

        void Delete(TeachingPlan teachingPlan);

        TeachingPlan? GetTeachingPlanByIdWithLessonAndTrainees(int id);

        TeachingPlan? GetTeachingPlanById(int id);

        IEnumerable<TeachingPlan> GetAllTeachingPlans();
    }
}
