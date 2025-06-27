using Newtonsoft.Json;
using TraineeTracker.Data.TeachingPlans;
using TraineeTracker.Data.TraineeLessons;
using TraineeTracker.Models.Domain;
using TraineeTracker.Models.Dto;

namespace TraineeTracker.Services {

    public class TeachingPlanService {

        private readonly ITeachingPlanRepository _teachingPlanRepo;
        private readonly ILessonRepository _lessonRepo;
        private readonly ITraineeLessonRepository _traineeLessonRepo;

        public TeachingPlanService(ITeachingPlanRepository teachingPlanRepo, ILessonRepo lessonRepo) {
            _teachingPlanRepo = teachingPlanRepo;
            _lessonRepo = lessonRepo;
        }

        public async Task ImportNewTeachingPlan(IFormFile file, String name) {

            if (file == null || file.Length == 0)
                throw new ArgumentException("Die Datei ist leer!");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Ungültiger Name!");

            using var stream = new StreamReader(file.OpenReadStream());
            var jsonContent = await stream.ReadToEndAsync();

            var lessonsDto = JsonConvert.DeserializeObject<List<LessonDto>>(jsonContent);

            if (lessonsDto == null || lessonsDto.Count == 0)
                throw new InvalidOperationException("Keine gültigen Lektionen im JSON gefunden!");

            var lessons = lessonsDto.Select(dto => new Lesson
            {
                LessonId = dto.Id,
                Title = dto.Title,
                LinkUrl = dto.Url,
                EstimatedEffort = dto.Estimate ?? 0,
                IsInactive = dto.Deprecated
            }).ToList();

            var teachingPlan = new TeachingPlan
            {
                Name = name,
                LastUpdated = DateTime.UtcNow,
                Lessons = lessons
            };

            await _teachingPlanRepo.Create(teachingPlan);
        }

        public async Task UpdateTeachingPlan(IFormFile file) {

            if (file == null || file.Length == 0)
                throw new ArgumentException("Die Datei ist leer!");

            using var stream = new StreamReader(file.OpenReadStream());
            var jsonContent = await stream.ReadToEndAsync();

            var lessonsDto = JsonConvert.DeserializeObject<List<LessonDto>>(jsonContent);

            if (lessonsDto == null || lessonsDto.Count == 0)
                throw new InvalidOperationException("Keine gültigen Lektionen im JSON gefunden.");

            var lessons = lessonsDto.Select(dto => new Lesson
            {
                LessonId = dto.Id,
                Title = dto.Title,
                LinkUrl = dto.Url,
                EstimatedEffort = dto.Estimate ?? 0,
                IsInactive = dto.Deprecated
            }).ToList();

            foreach(var lesson in lessons){

                var traineeLesson = _traineeLessonRepo.GetTraineeLessonById(lesson.LessonId);

                if(_lessonRepo.GetLessonById(lesson.LessonId) != null){
                    if(lesson.IsInactive == true){
                        if(traineeLesson.TraineeLessonState == "open"){
                            await _traineeLessonRepo.Delete(lesson.LessonId);
                        }
                    }else{
                        await _lessonRepo.Update(lesson);
                    }
                }
            }

            await _teachingPlanRepo.Update(teachingPlan);
        }

        public async Task DeleteTeachingPlan(int id) {
            TeachingPlan teachingPlan = await _teachingPlanRepo.GetTeachingPlanById(id);

            if (teachingPlan == null)
                throw new InvalidOperationException("TeachingPlan nicht gefunden.");

            //Hier muss noch checkst für affected users gemacht werden

            await _teachingPlanRepo.Delete(teachingPlan);
        }
    }
}

