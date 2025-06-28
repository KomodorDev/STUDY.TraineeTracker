using System.Collections.Generic;
using System.Threading.Tasks;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.TeachingPlans {
    public interface ITeachingPlanRepository {

        Task<bool> ExistsAsync(int id);

        Task<bool> ExistsAsync(TeachingPlan teachingPlan);

        Task CreateAsync(TeachingPlan teachingPlan);

        Task UpdateAsync(TeachingPlan teachingPlan);

        Task DeleteAsync(TeachingPlan teachingPlan);

        Task AddTrainee(ApplicationUser trainee, TeachingPlan teachingPlan);

        Task<TeachingPlan?> GetTeachingPlanByIdAsync(int id);

        Task<TeachingPlan?> GetTeachingPlanByIdWithLessonsAndTraineesAsync(int id);

        Task<IEnumerable<TeachingPlan>> GetAllTeachingPlansAsync();
    }
}
