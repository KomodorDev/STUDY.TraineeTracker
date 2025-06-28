using Newtonsoft.Json;
using TraineeTracker.Data.TeachingPlans;
using TraineeTracker.Data.Lessons;
using TraineeTracker.Data.TraineeLessons;
using TraineeTracker.Models.Domain;
using TraineeTracker.Models.Dtos;

namespace TraineeTracker.Services {

    public class TeachingPlanService {

        private readonly ITeachingPlanRepository _teachingPlanRepo;
        private readonly ILessonRepository _lessonRepo;
        private readonly ITraineeLessonRepository _traineeLessonRepo;
        private readonly IApplicationUserRepository _applicationUserRepo;

        public TeachingPlanService(ITeachingPlanRepository teachingPlanRepo, ILessonRepository lessonRepo, ITraineeLessonRepository traineeLessonRepo, IApplicationUserRepository applicationUserRepo) {
            _teachingPlanRepo = teachingPlanRepo;
            _lessonRepo = lessonRepo;
            _traineeLessonRepo = traineeLessonRepo;
            _applicationUserRepo = applicationUserRepo;
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

            var lessons = lessonsDto.Select(dto => new Lesson {
                LessonId = dto.Id,
                Title = dto.Title,
                LinkUrl = dto.Url,
                EstimatedEffort = dto.Estimate ?? 0,
                IsInactive = dto.Deprecated
            }).ToList();

            var teachingPlan = new TeachingPlan {
                Name = name,
                LastUpdated = DateTime.UtcNow,
                Lessons = lessons
            };

            await _teachingPlanRepo.CreateAsync(teachingPlan);
        }

        public async Task UpdateTeachingPlan(IFormFile file, int teachingPlanId) {

            if (file == null || file.Length == 0)
                throw new ArgumentException("Die Datei ist leer!");

            using var stream = new StreamReader(file.OpenReadStream());
            var jsonContent = await stream.ReadToEndAsync();

            var lessonsDto = JsonConvert.DeserializeObject<List<LessonDto>>(jsonContent);

            if (lessonsDto == null || lessonsDto.Count == 0)
                throw new InvalidOperationException("Keine gültigen Lektionen im JSON gefunden.");

            var teachingPlan = await _teachingPlanRepo.GetTeachingPlanByIdAsync(teachingPlanId);

            if (teachingPlan == null)
                throw new InvalidOperationException("TeachingPlan nicht gefunden.");

            var oldLessons = await _lessonRepo.GetAllLessonsAsync();

            foreach (var oldLesson in oldLessons) {

                bool stillExists = lessonsDto.Any(dto => dto.Id == oldLesson.LessonId);

                if (!stillExists) {
                    var traineeLesson = await _traineeLessonRepo.GetTraineeLessonByIdWithLessonAsync(oldLesson.LessonId);

                    if (traineeLesson != null && traineeLesson.State == TraineeLessonState.Open) {
                        await _traineeLessonRepo.DeleteAsync(oldLesson.LessonId);
                    }
                    await _lessonRepo.DeleteAsync(oldLesson);
                }
            }

            foreach (var dto in lessonsDto) {

                var lesson = new Lesson {
                    LessonId = dto.Id,
                    Title = dto.Title,
                    LinkUrl = dto.Url,
                    EstimatedEffort = dto.Estimate ?? 0,
                    IsInactive = dto.Deprecated
                };

                var traineeLesson = await _traineeLessonRepo.GetTraineeLessonByIdWithLessonAsync(lesson.LessonId);
                var existingLesson = await _lessonRepo.GetLessonByIdAsync(lesson.LessonId);

                if (existingLesson != null) {

                    if (lesson.IsInactive) {

                        if (traineeLesson != null && traineeLesson.State == TraineeLessonState.Open) {

                            await _traineeLessonRepo.DeleteAsync(lesson.LessonId);
                        }

                        await _lessonRepo.UpdateAsync(lesson);
                    } else {

                        await _lessonRepo.UpdateAsync(lesson);
                    }
                } else {

                    await _lessonRepo.CreateAsync(lesson);
                }
            }

            teachingPlan.LastUpdated = DateTime.UtcNow;
            await _teachingPlanRepo.UpdateAsync(teachingPlan);
        }

        public async Task DeleteTeachingPlan(int id) {

            TeachingPlan teachingPlan = await _teachingPlanRepo.GetTeachingPlanByIdAsync(id);

            if (teachingPlan == null)
                throw new InvalidOperationException("TeachingPlan nicht gefunden.");

            if (teachingPlan.Trainees != null)
                throw new InvalidOperationException("Dieser TeachingPlan wird noch verwendet!");

            var lessons = await _lessonRepo.GetAllLessonsAsync();

            foreach (var lesson in lessons) {
                if (lesson.TeachingPlans.Contains(teachingPlan)) {
                    if (lesson.TeachingPlans.Count == 1) {
                        await _lessonRepo.DeleteAsync(lesson);
                    } else {
                        lesson.TeachingPlans.Remove(teachingPlan);
                        await _lessonRepo.UpdateAsync(lesson);
                    }
                }
            }

            await _teachingPlanRepo.DeleteAsync(teachingPlan);
        }

        public async Task AssignTeachingPlanToTraineeAsync(ApplicationUser trainee, int teachingPlanId){

            var teachingPlan = await _teachingPlanRepo.GetTeachingPlanByIdWithLessonsAndTraineesAsync(teachingPlanId);

            if (teachingPlan == null)
                throw new InvalidOperationException("TeachingPlan nicht gefunden.");

            trainee.TeachingPlan = teachingPlan;

            foreach (var lesson in teachingPlan.Lessons)
            {
                if(lesson.IsInactive == false) {

                    var traineeLesson = new TraineeLesson{
                        TraineeId = trainee.Id,
                        Trainee = trainee,
                        LessonId = lesson.LessonId,
                        Lesson = lesson
                    };
                }

                trainee.TraineeLessons.Add(traineeLesson);
                await _traineeLessonRepo.CreateAsync(traineeLesson);
            }

            teachingPlan.Trainees.Add(trainee);
            await _applicationUserRepo.UpdateAsync(trainee);
            await _teachingPlanRepo.UpdateAsync(teachingPlan);
        }
    }
}

