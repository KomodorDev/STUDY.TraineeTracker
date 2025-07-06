using Newtonsoft.Json;
using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Data.Lessons;
using TraineeTracker.Data.TraineeLessons;
using TraineeTracker.Data.TeachingPlans;
using TraineeTracker.Models.Domain;
using TraineeTracker.Models.Dtos;
using TraineeTracker.Models.ViewModels.TeachingPlan;
using TraineeTracker.Services.Email;

namespace TraineeTracker.Services {
    public class TeachingPlanService {
        private readonly ITeachingPlanRepository _databaseTeachingPlanRepository;
        private readonly ILessonRepository _databaseLessonRepository;
        private readonly ITraineeLessonRepository _databaseTraineeLessonRepository;
        private readonly IApplicationUserRepository _databaseApplicationUserRepository;
        private readonly EmailNotificationService _emailNotificationService;

        // ---------------------------------------------------
        public TeachingPlanService(
            ITeachingPlanRepository teachingPlanRepo,
            ILessonRepository databaseLessonRepository,
            ITraineeLessonRepository databaseTraineeLessonRepository,
            IApplicationUserRepository applicationUserRepo,
            EmailNotificationService emailNotificationService) {
            _databaseTeachingPlanRepository = teachingPlanRepo;
            _databaseLessonRepository = databaseLessonRepository;
            _databaseTraineeLessonRepository = databaseTraineeLessonRepository;
            _databaseApplicationUserRepository = applicationUserRepo;
            _emailNotificationService = emailNotificationService;
        }

        // ---------------------------------------------------
        public async Task<ImportDashboardViewModel> BuildImportDashboardViewModelAsync() {
            var allPlans = await _databaseTeachingPlanRepository.GetAllTeachingPlansWithLessonsAndTraineesAsync();
            return new ImportDashboardViewModel {
                ExistingTeachingPlans = allPlans.Select(tp => new ExistingTeachingPlanViewModel {
                    TeachingPlanId = tp.TeachingPlanId,
                    Name = tp.Name,
                    LastUpdated = tp.LastUpdated,
                    LessonCount = tp.Lessons?.Count(l => !l.IsInactive) ?? 0,
                    TraineeCount = tp.Trainees?.Count ?? 0
                }).ToList()
            };
        }

        // ---------------------------------------------------
        public async Task ImportNewTeachingPlan(TeachingPlanDto dto) {
            ValidateFile(dto.NewPlanFile);
            ValidateName(dto.NewPlanName!);

            var json = await ReadJsonAsync(dto.NewPlanFile);
            var lessonDtos = DeserializeLessonDtos(json);
            ValidateLessonDtos(lessonDtos);

            // Create TeachingPlan:
            var teachingPlan = new TeachingPlan {
                Name = dto.NewPlanName!,
                LastUpdated = DateTime.UtcNow,
            };

            await _databaseTeachingPlanRepository.CreateAsync(teachingPlan);

            // Create Lessons for TeachingPlan:
            var lessons = CreateLessons(lessonDtos, teachingPlan.TeachingPlanId);
            foreach (var lesson in lessons) {
                await _databaseLessonRepository.CreateAsync(lesson);
            }
        }


        // ---------------------------------------------------
        // ---------------------------------------------------
        // ---------------------------------------------------
        // ---------------------------------------------------
        // ---------------------------------------------------
        public async Task UpdateTeachingPlan(TeachingPlanDto teachingPlanDto) {
            /* 
            Console.WriteLine($"[DEBUG] Service: Called UpdateTeachingPlan");
            */

            int existingTeachingPlanId = teachingPlanDto.ExistingTeachingPlanId ?? throw new Exception("ExistingTeachingPlanId missing!");

            // +++++++++++++++
            // 1) Datei einlesen, DTOs validieren
            var json = await ReadJsonAsync(teachingPlanDto.NewPlanFile);
            var lessonDtos = DeserializeLessonDtos(json);
            ValidateLessonDtos(lessonDtos);

            // +++++++++++++++
            // 2) Bestehenden TeachingPlan inkl. Lessons & Trainees laden
            if (teachingPlanDto.ExistingTeachingPlanId == null)
                throw new ArgumentException("TeachingPlanId is required for Update.");

            var existingTeachingPlan = await _databaseTeachingPlanRepository
                .GetTeachingPlanByIdWithLessonsAndTraineesAsync(teachingPlanDto.ExistingTeachingPlanId.Value);

            // +++++++++++++++
            // Hilfslisten für diff
            var existingLessons = existingTeachingPlan!.Lessons.ToList();
            var dtoMakandraIds = lessonDtos.Select(d => d.Id).ToHashSet();
            var lessonsMarkedAsInactive = new List<Lesson>();
            var addedLessons = new List<Lesson>();

            int sortingIndex = 1;

            // +++++++++++++++
            // 3) DTOs upserten und zusätzlich:
            //    - Deprecated = true → nur Open-TraineeLessons sollen gelöscht werden ("removed")
            //    - ganz neue Lessons → in teachingPlan einfügen und als "added" markieren
            foreach (var lessonDto in lessonDtos) {
                // Find Lesson by lessonDto.MakandraId in existingLessons (MakandraId is at least unique within TeachingPlan)
                var lesson = existingLessons.FirstOrDefault(l => l.MakandraId == lessonDto.Id);

                if (lesson == null) {
                    // a) Lesson does not exist yet in existingTeachingPlan:
                    var newLesson = CreateLesson(lessonDto, existingTeachingPlanId, sortingIndex++);
                    await _databaseLessonRepository.CreateAsync(newLesson);
                    existingTeachingPlan.Lessons.Add(newLesson);
                    addedLessons.Add(newLesson);
                } else {
                    if (lessonDto.Deprecated) {
                        // Lesson is deprecated and we leave the index unchanged (we change it later)
                        UpdateLesson(lessonDto, lesson, lesson.SortingIndex); 
                        lessonsMarkedAsInactive.Add(lesson);
                    } else {
                        // Lesson is not-depreacted and we increment the index afterwards:
                        UpdateLesson(lessonDto, lesson, sortingIndex++);
                    }

                    // Update Database:
                    await _databaseLessonRepository.UpdateAsync(lesson);
                }

            }

            // +++++++++++++++
            // 4) Mark Lessons, that are missing in dto, as inactive
            lessonsMarkedAsInactive = existingLessons
             .Where(l => !dtoMakandraIds.Contains(l.MakandraId))
             .OrderBy(l => l.SortingIndex)
             .ToList();

            foreach (var lesson in lessonsMarkedAsInactive) {
                lesson.IsInactive = true;
                lesson.SortingIndex = sortingIndex++;

                await _databaseLessonRepository.UpdateAsync(lesson);
            }

            // +++++++++++++++
            // Update existingTeachingPlan -> New lessons are now in Db. Inactive Lessons are marked
            existingTeachingPlan.LastUpdated = DateTime.UtcNow;
            await _databaseTeachingPlanRepository.UpdateAsync(existingTeachingPlan);

            // +++++++++++++++
            // 5) Für jeden zugewiesenen Trainee:
            //    - Open-TraineeLessons der lessonsMarkedAsInactive löschen und sammeln
            //    - für addedLessons neue TraineeLesson anlegen und sammeln
            //    - NotifyAboutImportChangeAsync aufrufen
            foreach (var trainee in existingTeachingPlan.Trainees) {

                // +++++++++++++++
                // Get all current TraineeLessons of Trainee
                var traineeLessonsOfTrainee = (await _databaseTraineeLessonRepository
                    .GetAllTraineeLessonsOfTraineeWithLessonAsync(trainee.Id)).ToList();

                // +++++++++++++++
                // a) Entfernen
                var removedTraineeLessons = new List<TraineeLesson>();
                foreach (var lesson in lessonsMarkedAsInactive) {

                    // Collect traineeLessons that will be deleted
                    var toDelete = traineeLessonsOfTrainee
                        .Where(tl => tl.Lesson.LessonId == lesson.LessonId
                                  && tl.State == TraineeLessonState.Open)
                        .ToList();

                    // Delete TraineeLessons:
                    foreach (var tl in toDelete) {
                        await _databaseTraineeLessonRepository.DeleteAsync(tl.TraineeLessonId);
                        Console.WriteLine($"[TraineeLesson] Entfernt: {tl.Lesson.Title} (LessonID: {tl.Lesson.LessonId}, TraineeID: {tl.TraineeId})");
                    }

                    // Add to removedTraineeLessons for Notification
                    removedTraineeLessons.AddRange(toDelete);
                }

                // +++++++++++++++
                // b) Hinzufügen
                var addedTraineeLessons = new List<TraineeLesson>();
                foreach (var lesson in addedLessons) {
                    var tl = new TraineeLesson {
                        TraineeId = trainee.Id,
                        Trainee = trainee,
                        LessonId = lesson.LessonId,
                        Lesson = lesson
                    };
                    await _databaseTraineeLessonRepository.CreateAsync(tl);
                    addedTraineeLessons.Add(tl);
                }

                // +++++++++++++++
                // Debug-Ausgabe
                Console.WriteLine($"[Import] Trainee {trainee.UserName} – Removed Lessons:");
                foreach (var tl in removedTraineeLessons) {
                    Console.WriteLine($"  - {tl.Lesson.Title} (ID: {tl.Lesson.LessonId})");
                }

                Console.WriteLine($"[Import] Trainee {trainee.UserName} – Added Lessons:");
                foreach (var tl in addedTraineeLessons) {
                    Console.WriteLine($"  + {tl.Lesson.Title} (ID: {tl.Lesson.LessonId})");
                }

                // +++++++++++++++
                // c) Benachrichtigung
                await _emailNotificationService
                    .NotifyAboutImportChangeAsync(trainee, removedTraineeLessons, addedTraineeLessons);
            }
        }

        // ---------------------------------------------------
        // ---------------------------------------------------
        // ---------------------------------------------------
        // PASST
        public async Task DeleteTeachingPlan(int existingTeachingPlanId) {

            Console.WriteLine($"ID used for delete: {existingTeachingPlanId}");
            var plan = await _databaseTeachingPlanRepository.GetTeachingPlanByIdWithLessonsAndTraineesAsync(existingTeachingPlanId)
                       ?? throw new InvalidOperationException("TeachingPlan not found.");

            if (plan.Trainees != null && plan.Trainees.Any())
                throw new InvalidOperationException("This TeachingPlan is still in use.");

            var lessonsCopy = plan.Lessons.ToList();

            foreach (var lesson in lessonsCopy) {
                await _databaseLessonRepository.DeleteAsync(lesson);
            }

            await _databaseTeachingPlanRepository.DeleteAsync(plan);
        }

        // ---------------------------------------------------
        // ---------------------------------------------------
        // ---------------------------------------------------
        public async Task AssignTeachingPlanToTraineeAsync(ApplicationUser trainee, int teachingPlanId) {
            var plan = await _databaseTeachingPlanRepository.GetTeachingPlanByIdWithLessonsAndTraineesAsync(teachingPlanId)
                       ?? throw new InvalidOperationException("TeachingPlan nicht gefunden.");

            trainee.TeachingPlan = plan;
            await CreateTraineeLessonsAsync(trainee, plan.Lessons);

            plan.Trainees.Add(trainee);
            await _databaseApplicationUserRepository.UpdateAsync(trainee);
            await _databaseTeachingPlanRepository.UpdateAsync(plan);
        }

        // ---------------------------------------------------
        public async Task UnassignTeachingPlanFromTraineeAsync(ApplicationUser trainee) {
            var traineeLessons = await _databaseTraineeLessonRepository.GetAllTraineeLessonsOfTraineeWithLessonAsync(trainee.Id);
            foreach (var traineeLesson in traineeLessons) {
                await _databaseTraineeLessonRepository.DeleteAsync(traineeLesson.TraineeLessonId);
            }

            var teachingPlan = trainee.TeachingPlan;
            if (teachingPlan == null)
                throw new InvalidOperationException("TeachingPlan nicht gefunden.");
            teachingPlan.Trainees.Remove(trainee);
            await _databaseTeachingPlanRepository.UpdateAsync(teachingPlan);

            trainee.TeachingPlan = null;
            await _databaseApplicationUserRepository.UpdateAsync(trainee);
        }


        // ---------------------------------------------------
        private void ValidateFile(IFormFile file) {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Die Datei ist leer!");
        }

        // ---------------------------------------------------
        private void ValidateName(string name) {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Ungültiger Name!");
        }

        // ---------------------------------------------------
        /*         
        private async Task ValidateLesson(Lesson l) {
            bool exists = await _databaseLessonRepository.ExistsAsync(l);
            if (exists)
                throw new ArgumentException("Dieser Teachingplan existiert schon!");
        }
        */

        // ---------------------------------------------------
        private async Task<string> ReadJsonAsync(IFormFile file) {
            using var reader = new StreamReader(file.OpenReadStream());
            return await reader.ReadToEndAsync();
        }

        // ---------------------------------------------------
        private List<LessonDto> DeserializeLessonDtos(string json) {
            return JsonConvert.DeserializeObject<List<LessonDto>>(json)
                ?? throw new Exception("LessonDtos could not be deserialized.");
        }

        // ---------------------------------------------------
        private void ValidateLessonDtos(List<LessonDto> dtos) {
            if (dtos == null || !dtos.Any())
                throw new InvalidOperationException("Keine gültigen Lektionen im JSON gefunden!");
        }

        // ---------------------------------------------------
        private List<Lesson> CreateLessons(IEnumerable<LessonDto> dtos, int teachingPlanId) {

            int sortingIndex = 1;
            return dtos.Select(dto => new Lesson {
                MakandraId = dto.Id,
                Title = dto.Title,
                LinkUrl = dto.Url,
                EstimatedEffort = dto.Estimate ?? 0,
                IsInactive = dto.Deprecated,
                SortingIndex = sortingIndex++,
                TeachingPlanId = teachingPlanId
            }).ToList();
        }

        // ---------------------------------------------------
        private void UpdateLesson(LessonDto dto, Lesson lesson, int sortingIndex) {
            lesson.Title = dto.Title;
            lesson.LinkUrl = dto.Url;
            lesson.EstimatedEffort = dto.Estimate ?? 0;
            lesson.IsInactive = dto.Deprecated;
            lesson.SortingIndex = sortingIndex;
        }

        // ---------------------------------------------------
        private Lesson CreateLesson(LessonDto dto, int teachingPlanId, int sortingIndex) {
            return new Lesson {
                MakandraId = dto.Id,
                Title = dto.Title,
                LinkUrl = dto.Url,
                EstimatedEffort = dto.Estimate ?? 0,
                IsInactive = dto.Deprecated,
                TeachingPlanId = teachingPlanId,
                SortingIndex = sortingIndex
            };
        }

        // ---------------------------------------------------
        private async Task DeleteOpenTraineeLessonsAsync(int lessonId) {
            var traineeLessons = await _databaseTraineeLessonRepository
                .GetAllTraineeLessonsOfLessonWithLessonAsync(lessonId);

            foreach (var tl in traineeLessons.Where(t => t.State == TraineeLessonState.Open)) {
                await _databaseTraineeLessonRepository.DeleteAsync(tl.TraineeLessonId);
            }
        }


        // ---------------------------------------------------
        private async Task CreateTraineeLessonsAsync(ApplicationUser trainee, IEnumerable<Lesson> lessons) {
            foreach (var lesson in lessons.Where(l => !l.IsInactive)) {
                var tl = new TraineeLesson {
                    TraineeId = trainee.Id,
                    Trainee = trainee,
                    LessonId = lesson.LessonId,
                    Lesson = lesson
                };
                trainee.TraineeLessons.Add(tl);
                await _databaseTraineeLessonRepository.CreateAsync(tl);
            }
        }

        // ---------------------------------------------------

    }
}
