using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Data.Lessons;
using TraineeTracker.Data.TraineeLessons;
using TraineeTracker.Data.TeachingPlans;
using TraineeTracker.Models.Domain;
using TraineeTracker.Models.Dtos;

namespace TraineeTracker.Services {
    public class TeachingPlanService {
        private readonly ITeachingPlanRepository _teachingPlanRepo;
        private readonly ILessonRepository _lessonRepo;
        private readonly ITraineeLessonRepository _traineeLessonRepo;
        private readonly IApplicationUserRepository _applicationUserRepo;

        public TeachingPlanService(
            ITeachingPlanRepository teachingPlanRepo,
            ILessonRepository lessonRepo,
            ITraineeLessonRepository traineeLessonRepo,
            IApplicationUserRepository applicationUserRepo) {
            _teachingPlanRepo = teachingPlanRepo;
            _lessonRepo = lessonRepo;
            _traineeLessonRepo = traineeLessonRepo;
            _applicationUserRepo = applicationUserRepo;
        }

        public async Task ImportNewTeachingPlan(IFormFile file, string name) {
            ValidateFile(file);
            ValidateName(name);

            var json = await ReadJsonAsync(file);
            var dtos = DeserializeLessonDtos(json);
            ValidateDtos(dtos);

            var lessons = MapDtosToLessons(dtos);
            var teachingPlan = new TeachingPlan {
                Name = name,
                LastUpdated = DateTime.UtcNow,
                Lessons = lessons
            };

            await _teachingPlanRepo.CreateAsync(teachingPlan);
        }

        public async Task UpdateTeachingPlan(IFormFile file, int teachingPlanId) {
            ValidateFile(file);

            var json = await ReadJsonAsync(file);
            var dtos = DeserializeLessonDtos(json);
            ValidateDtos(dtos);

            var teachingPlan = await GetExistingPlanWithDetails(teachingPlanId);
            var dtoIds = dtos.Select(d => d.Id).ToHashSet();

            await RemoveDeprecatedLessonsAsync(teachingPlan, dtoIds);
            await UpsertLessonsAsync(teachingPlan, dtos);

            teachingPlan.LastUpdated = DateTime.UtcNow;
            await _teachingPlanRepo.UpdateAsync(teachingPlan);
        }

        public async Task DeleteTeachingPlan(int id) {
            var plan = await _teachingPlanRepo.GetTeachingPlanByIdAsync(id)
                       ?? throw new InvalidOperationException("TeachingPlan nicht gefunden.");

            if (plan.Trainees != null && plan.Trainees.Any())
                throw new InvalidOperationException("Dieser TeachingPlan wird noch verwendet!");

            await RemovePlanFromLessonsAsync(plan);
            await _teachingPlanRepo.DeleteAsync(plan);
        }

        public async Task AssignTeachingPlanToTraineeAsync(ApplicationUser trainee, int teachingPlanId) {
            var plan = await _teachingPlanRepo.GetTeachingPlanByIdWithLessonsAndTraineesAsync(teachingPlanId)
                       ?? throw new InvalidOperationException("TeachingPlan nicht gefunden.");

            trainee.TeachingPlan = plan;
            await CreateTraineeLessonsAsync(trainee, plan.Lessons);

            plan.Trainees.Add(trainee);
            await _applicationUserRepo.UpdateAsync(trainee);
            await _teachingPlanRepo.UpdateAsync(plan);
        }

        public async Task UnassignTeachingPlanFromTraineeAsync(ApplicationUser trainee) {
            var traineeLessons = await _traineeLessonRepo.GetAllTraineeLessonsOfTraineeWithLessonAsync(trainee.Id);
            foreach (var traineeLesson in traineeLessons) {
                await _traineeLessonRepo.DeleteAsync(traineeLesson.TraineeLessonId);
            }

            var teachingPlan = trainee.TeachingPlan;
            if (teachingPlan == null)
                throw new InvalidOperationException("TeachingPlan nicht gefunden.");
            teachingPlan.Trainees.Remove(trainee);
            await _teachingPlanRepo.UpdateAsync(teachingPlan);

            trainee.TeachingPlan = null;
            await _applicationUserRepo.UpdateAsync(trainee);
        }

        public Task<IEnumerable<TeachingPlan>> GetAllTeachingPlansAsync()
            => _teachingPlanRepo.GetAllTeachingPlansAsync();

        // Helpers

        private void ValidateFile(IFormFile file) {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Die Datei ist leer!");
        }

        private void ValidateName(string name) {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Ungültiger Name!");
        }

        private async Task<string> ReadJsonAsync(IFormFile file) {
            using var reader = new StreamReader(file.OpenReadStream());
            return await reader.ReadToEndAsync();
        }

        private List<LessonDto> DeserializeLessonDtos(string json) {
            return JsonConvert.DeserializeObject<List<LessonDto>>(json) ?? throw new Exception("LessonDtos could not be deserialized.");
        }

        private void ValidateDtos(List<LessonDto> dtos) {
            if (dtos == null || !dtos.Any())
                throw new InvalidOperationException("Keine gültigen Lektionen im JSON gefunden!");
        }

        private List<Lesson> MapDtosToLessons(IEnumerable<LessonDto> dtos) {
            return dtos.Select(dto => new Lesson {
                LessonId = dto.Id,
                Title = dto.Title,
                LinkUrl = dto.Url,
                EstimatedEffort = dto.Estimate ?? 0,
                IsInactive = dto.Deprecated
            }).ToList();
        }

        private async Task<TeachingPlan> GetExistingPlanWithDetails(int id) {
            var plan = await _teachingPlanRepo
                .GetTeachingPlanByIdWithLessonsAndTraineesAsync(id);
            if (plan == null)
                throw new InvalidOperationException("TeachingPlan nicht gefunden.");
            return plan;
        }

        private async Task RemoveDeprecatedLessonsAsync(TeachingPlan plan, HashSet<int> dtoIds) {
            var toRemove = plan.Lessons
                .Where(l => !dtoIds.Contains(l.LessonId))
                .ToList();

            foreach (var lesson in toRemove) {
                await DeleteOpenTraineeLessonsAsync(lesson.LessonId);
                plan.Lessons.Remove(lesson);
                await _lessonRepo.DeleteAsync(lesson);
            }
        }

        private async Task UpsertLessonsAsync(TeachingPlan plan, IEnumerable<LessonDto> dtos) {
            foreach (var dto in dtos) {
                var lesson = plan.Lessons.FirstOrDefault(l => l.LessonId == dto.Id);
                if (lesson != null) {
                    ApplyDtoToLesson(dto, lesson);
                    if (dto.Deprecated)
                        await DeleteOpenTraineeLessonsAsync(lesson.LessonId);

                    await _lessonRepo.UpdateAsync(lesson);
                } else {
                    var newLesson = MapDtoToLesson(dto);
                    await _lessonRepo.CreateAsync(newLesson);
                    plan.Lessons.Add(newLesson);
                }
            }
        }

        private void ApplyDtoToLesson(LessonDto dto, Lesson lesson) {
            lesson.Title = dto.Title;
            lesson.LinkUrl = dto.Url;
            lesson.EstimatedEffort = dto.Estimate ?? 0;
            lesson.IsInactive = dto.Deprecated;
        }

        private Lesson MapDtoToLesson(LessonDto dto) {
            return new Lesson {
                LessonId = dto.Id,
                Title = dto.Title,
                LinkUrl = dto.Url,
                EstimatedEffort = dto.Estimate ?? 0,
                IsInactive = dto.Deprecated
            };
        }

        private async Task DeleteOpenTraineeLessonsAsync(int lessonId) {
            var traineeLessons = await _traineeLessonRepo
                .GetAllTraineeLessonsOfLessonWithLessonAsync(lessonId);

            foreach (var tl in traineeLessons.Where(t => t.State == TraineeLessonState.Open)) {
                await _traineeLessonRepo.DeleteAsync(tl.TraineeLessonId);
            }
        }

        private async Task RemovePlanFromLessonsAsync(TeachingPlan plan) {
            var allLessons = await _lessonRepo.GetAllLessonsAsync();
            foreach (var lesson in allLessons) {
                if (!lesson.TeachingPlans.Contains(plan))
                    continue;

                if (lesson.TeachingPlans.Count == 1) {
                    await _lessonRepo.DeleteAsync(lesson);
                } else {
                    lesson.TeachingPlans.Remove(plan);
                    await _lessonRepo.UpdateAsync(lesson);
                }
            }
        }

        private async Task CreateTraineeLessonsAsync(ApplicationUser trainee, IEnumerable<Lesson> lessons) {
            foreach (var lesson in lessons.Where(l => !l.IsInactive)) {
                var tl = new TraineeLesson {
                    TraineeId = trainee.Id,
                    Trainee = trainee,
                    LessonId = lesson.LessonId,
                    Lesson = lesson
                };
                trainee.TraineeLessons.Add(tl);
                await _traineeLessonRepo.CreateAsync(tl);
            }
        }


    }
}
