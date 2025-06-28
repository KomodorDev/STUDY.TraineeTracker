using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using TraineeTracker.Data.TeachingPlans;
using TraineeTracker.Data.Lessons;
using TraineeTracker.Data.TraineeLessons;
using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Models.Domain;
using TraineeTracker.Models.Dtos;
using TraineeTracker.Services.Email;

namespace TraineeTracker.Services {
    public class TeachingPlanService {
        private readonly ITeachingPlanRepository _teachingPlanRepo;
        private readonly ILessonRepository _lessonRepo;
        private readonly ITraineeLessonRepository _traineeLessonRepo;
        private readonly IApplicationUserRepository _applicationUserRepo;
        private readonly EmailNotificationService _emailNotificationService;

        public TeachingPlanService(
            ITeachingPlanRepository teachingPlanRepo,
            ILessonRepository lessonRepo,
            ITraineeLessonRepository traineeLessonRepo,
            IApplicationUserRepository applicationUserRepo,
            EmailNotificationService emailNotificationService)
        {
            _teachingPlanRepo = teachingPlanRepo;
            _lessonRepo = lessonRepo;
            _traineeLessonRepo = traineeLessonRepo;
            _applicationUserRepo = applicationUserRepo;
            _emailNotificationService = emailNotificationService;
        }

        public async Task ImportNewTeachingPlan(IFormFile file, string name) {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Die Datei ist leer!");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Ungültiger Name!");

            using var stream = new StreamReader(file.OpenReadStream());
            var jsonContent = await stream.ReadToEndAsync();
            var lessonsDto = JsonConvert.DeserializeObject<List<LessonDto>>(jsonContent)
                             ?? throw new InvalidOperationException("Keine gültigen Lektionen im JSON gefunden!");

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

            // Prepare trainee lists for notifications
            var trainees = teachingPlan.Trainees.ToList();
            var removedByTrainee = trainees.ToDictionary(t => t.Id, t => new List<TraineeLesson>());
            var addedByTrainee   = trainees.ToDictionary(t => t.Id, t => new List<TraineeLesson>());

            var dtoIds = lessonsDto.Select(dto => dto.Id).ToHashSet();
            var existingLessons = teachingPlan.Lessons.ToList();

            // 1) Remove lessons no longer in DTO
            foreach (var oldLesson in existingLessons) {
                if (!dtoIds.Contains(oldLesson.LessonId)) {
                    // Delete open TraineeLessons per trainee
                    var allTles = await _traineeLessonRepo.GetAllTraineeLessonsOfLessonWithLessonAsync(oldLesson.LessonId);
                    foreach (var tl in allTles.Where(t => t.State == TraineeLessonState.Open)) {
                        await _traineeLessonRepo.DeleteAsync(tl.TraineeLessonId);
                        removedByTrainee[tl.TraineeId].Add(tl);
                    }

                    teachingPlan.Lessons.Remove(oldLesson);
                    await _lessonRepo.DeleteAsync(oldLesson);
                }
            }

            // 2) Update existing or create new lessons
            foreach (var dto in lessonsDto) {
                var lesson = teachingPlan.Lessons.FirstOrDefault(l => l.LessonId == dto.Id);
                if (lesson != null) {
                    // Update fields
                    lesson.Title = dto.Title;
                    lesson.LinkUrl = dto.Url;
                    lesson.EstimatedEffort = dto.Estimate ?? 0;
                    lesson.IsInactive = dto.Deprecated;

                    if (dto.Deprecated) {
                        var tles = await _traineeLessonRepo.GetAllTraineeLessonsOfLessonWithLessonAsync(lesson.LessonId);
                        foreach (var tl in tles.Where(t => t.State == TraineeLessonState.Open)) {
                            await _traineeLessonRepo.DeleteAsync(tl.TraineeLessonId);
                            removedByTrainee[tl.TraineeId].Add(tl);
                        }
                    }

                    await _lessonRepo.UpdateAsync(lesson);
                } else {
                    // Create new lesson
                    var newLesson = new Lesson {
                        LessonId = dto.Id,
                        Title = dto.Title,
                        LinkUrl = dto.Url,
                        EstimatedEffort = dto.Estimate ?? 0,
                        IsInactive = dto.Deprecated
                    };
                    await _lessonRepo.CreateAsync(newLesson);
                    teachingPlan.Lessons.Add(newLesson);

                    // Create TraineeLessons for each trainee
                    foreach (var trainee in trainees) {
                        var tl = new TraineeLesson {
                            TraineeId = trainee.Id,
                            Trainee = trainee,
                            LessonId = newLesson.LessonId,
                            Lesson = newLesson
                        };
                        await _traineeLessonRepo.CreateAsync(tl);
                        addedByTrainee[trainee.Id].Add(tl);
                    }
                }
            }

            // Finalize update
            teachingPlan.LastUpdated = DateTime.UtcNow;
            await _teachingPlanRepo.UpdateAsync(teachingPlan);

            // Notify trainees about import changes
            foreach (var trainee in trainees) {
                var removedList = removedByTrainee[trainee.Id];
                var addedList   = addedByTrainee[trainee.Id];
                if (removedList.Any() || addedList.Any()) {
                    await _emailNotificationService.NotifyAboutImportChangeAsync(
                        trainee,
                        removedList,
                        addedList);
                }
            }
        }

        public async Task DeleteTeachingPlan(int id) {
            var teachingPlan = await _teachingPlanRepo.GetTeachingPlanByIdAsync(id)
                               ?? throw new InvalidOperationException("TeachingPlan nicht gefunden.");
            if (teachingPlan.Trainees.Any())
                throw new InvalidOperationException("Dieser TeachingPlan wird noch verwendet!");

            var lessons = await _lessonRepo.GetAllLessonsAsync();
            foreach (var lesson in lessons.Where(l => l.TeachingPlans.Contains(teachingPlan))) {
                if (lesson.TeachingPlans.Count == 1)
                    await _lessonRepo.DeleteAsync(lesson);
                else {
                    lesson.TeachingPlans.Remove(teachingPlan);
                    await _lessonRepo.UpdateAsync(lesson);
                }
            }
            await _teachingPlanRepo.DeleteAsync(teachingPlan);
        }

        public async Task AssignTeachingPlanToTraineeAsync(ApplicationUser trainee, int teachingPlanId){
            var teachingPlan = await _teachingPlanRepo.GetTeachingPlanByIdWithLessonsAndTraineesAsync(teachingPlanId)
                               ?? throw new InvalidOperationException("TeachingPlan nicht gefunden.");

            trainee.TeachingPlan = teachingPlan;
            foreach (var lesson in teachingPlan.Lessons.Where(l => !l.IsInactive)) {
                var tl = new TraineeLesson {
                    TraineeId = trainee.Id,
                    Trainee = trainee,
                    LessonId = lesson.LessonId,
                    Lesson = lesson
                };
                trainee.TraineeLessons.Add(tl);
                await _traineeLessonRepo.CreateAsync(tl);
            }
            teachingPlan.Trainees.Add(trainee);
            await _applicationUserRepo.UpdateAsync(trainee);
            await _teachingPlanRepo.UpdateAsync(teachingPlan);
        }

        public async Task<IEnumerable<TeachingPlan>> GetAllTeachingPlansAsync() {
            return await _teachingPlanRepo.GetAllTeachingPlansAsync();
        }
    }
}
