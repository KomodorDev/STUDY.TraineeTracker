using Newtonsoft.Json;
using TraineeTracker.Data.TeachingPlans;
using TraineeTracker.Data.Lessons;
using TraineeTracker.Data.TraineeLessons;
using TraineeTracker.Data.ApplicationUsers;
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
            var lessonsDto = JsonConvert.DeserializeObject<List<LessonDto>>(jsonContent)
                ?? throw new InvalidOperationException("Keine gültigen Lektionen im JSON gefunden.");

            var teachingPlan = await _teachingPlanRepo.GetTeachingPlanByIdWithLessonsAndTraineesAsync(teachingPlanId)
                ?? throw new InvalidOperationException("TeachingPlan nicht gefunden.");

            var dtoLessonIds = lessonsDto.Select(dto => dto.Id).ToHashSet();
            var existingLessons = teachingPlan.Lessons.ToList();

            // Entferne Lessons, die nicht mehr im DTO enthalten sind
            foreach (var oldLesson in existingLessons) {
                if (!dtoLessonIds.Contains(oldLesson.LessonId)) {
                    var traineeLessons = await _traineeLessonRepo.GetAllTraineeLessonsOfLessonWithLessonAsync(oldLesson.LessonId);
                    foreach (var tl in traineeLessons.Where(t => t.State == TraineeLessonState.Open)) {
                        await _traineeLessonRepo.DeleteAsync(tl.TraineeLessonId);
                    }

                    teachingPlan.Lessons.Remove(oldLesson);
                    await _lessonRepo.DeleteAsync(oldLesson);
                }
            }

            // Update oder Create Lessons
            foreach (var dto in lessonsDto) {
                var existingLesson = teachingPlan.Lessons.FirstOrDefault(l => l.LessonId == dto.Id);

                if (existingLesson != null) {
                    existingLesson.Title = dto.Title;
                    existingLesson.LinkUrl = dto.Url;
                    existingLesson.EstimatedEffort = dto.Estimate ?? 0;
                    existingLesson.IsInactive = dto.Deprecated;

                    if (dto.Deprecated) {
                        var traineeLessons = await _traineeLessonRepo.GetAllTraineeLessonsOfLessonWithLessonAsync(existingLesson.LessonId);
                        foreach (var tl in traineeLessons.Where(t => t.State == TraineeLessonState.Open)) {
                            await _traineeLessonRepo.DeleteAsync(tl.TraineeLessonId);
                        }
                    }

                    await _lessonRepo.UpdateAsync(existingLesson);
                } else {
                    var newLesson = new Lesson {
                    LessonId = dto.Id,
                    Title = dto.Title,
                    LinkUrl = dto.Url,
                    EstimatedEffort = dto.Estimate ?? 0,
                    IsInactive = dto.Deprecated
                    };

                    await _lessonRepo.CreateAsync(newLesson);
                    teachingPlan.Lessons.Add(newLesson);
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

                    trainee.TraineeLessons.Add(traineeLesson);
                    await _traineeLessonRepo.CreateAsync(traineeLesson);
                }
            }

            teachingPlan.Trainees.Add(trainee);
            await _applicationUserRepo.UpdateAsync(trainee);
            await _teachingPlanRepo.UpdateAsync(teachingPlan);
        }
    }
}

