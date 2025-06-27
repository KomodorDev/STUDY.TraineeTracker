using Newtonsoft.Json;
using TraineeTracker.Data.TeachingPlans;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Services {

    public class TeachingPlanService {
        private readonly ITeachingPlanRepository _teachingPlanRepo;

        public TeachingPlanService(ITeachingPlanRepository teachingPlanRepo) {
            _teachingPlanRepo = teachingPlanRepo;
        }

        public async Task ImportNewTeachingPlan(IFormFile file) {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Die Datei ist leer!");

            using var stream = new StreamReader(file.OpenReadStream());
            var jsonContent = await stream.ReadToEndAsync();

            TeachingPlan teachingPlan = JsonConvert.DeserializeObject<TeachingPlan>(jsonContent);

            if (teachingPlan == null)
                throw new InvalidOperationException("Ungültiges JSON: TeachingPlan konnte nicht deserialisiert werden.");


            await _teachingPlanRepo.Create(teachingPlan);
        }

        public async Task UpdateTeachingPlan(IFormFile file) {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Die Datei ist leer!");

            using var stream = new StreamReader(file.OpenReadStream());
            var jsonContent = await stream.ReadToEndAsync();

            TeachingPlan teachingPlan = JsonConvert.DeserializeObject<TeachingPlan>(jsonContent);

            if (teachingPlan == null)
                throw new InvalidOperationException("Ungültiges JSON: TeachingPlan konnte nicht deserialisiert werden.");

            await _teachingPlanRepo.Update(teachingPlan);
        }

        public async Task DeleteTeachingPlan(int id) {
            TeachingPlan teachingPlan = await _teachingPlanRepo.GetTeachingPlanById(id);

            if (teachingPlan == null)
                throw new InvalidOperationException("TeachingPlan nicht gefunden.");

            await _teachingPlanRepo.Delete(teachingPlan);
        }
    }
}

