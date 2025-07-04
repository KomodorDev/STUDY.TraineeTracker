using Newtonsoft.Json;
using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Data.Lessons;
using TraineeTracker.Data.TraineeLessons;
using TraineeTracker.Data.TeachingPlans;
using TraineeTracker.Models.Domain;
using TraineeTracker.Models.Dtos;
using TraineeTracker.Models.ViewModels;
using TraineeTracker.Services.Email;

namespace TraineeTracker.Services {
    public class TeachingPlanService {
        private readonly ITeachingPlanRepository _teachingPlanRepo;
        private readonly ILessonRepository _lessonRepo;
        private readonly ITraineeLessonRepository _traineeLessonRepo;
        private readonly IApplicationUserRepository _applicationUserRepo;

        private readonly EmailNotificationService _emailNotificationService;

        // ---------------------------------------------------
        public TeachingPlanService(
            ITeachingPlanRepository teachingPlanRepo,
            ILessonRepository lessonRepo,
            ITraineeLessonRepository traineeLessonRepo,
            IApplicationUserRepository applicationUserRepo,
            EmailNotificationService emailNotificationService) {
            _teachingPlanRepo = teachingPlanRepo;
            _lessonRepo = lessonRepo;
            _traineeLessonRepo = traineeLessonRepo;
            _applicationUserRepo = applicationUserRepo;
            _emailNotificationService = emailNotificationService;
        }

        // ---------------------------------------------------
        public async Task<ImportDashboardViewModel> BuildImportDashboardAsync()
        {
            var allPlans = await _teachingPlanRepo.GetAllTeachingPlansAsync();
            var vm = new ImportDashboardViewModel
            {
            ExistingTeachingPlans = allPlans.Select(tp => new ImportDashboardViewModel.ExistingPlan
            {
                TeachingPlanId = tp.TeachingPlanId,
                Name           = tp.Name,
                LastUpdated    = tp.LastUpdated,
                LessonCount    = tp.Lessons?.Count ?? 0,
                TraineeCount   = tp.Trainees?.Count ?? 0
            }).ToList()
            };
            return vm;
        }
        
        // ---------------------------------------------------
        public async Task ImportNewTeachingPlan(IFormFile file, string name) {
            ValidateFile(file);
            ValidateName(name);

            var json = await ReadJsonAsync(file);
            var dtos = DeserializeLessonDtos(json);
            ValidateDtos(dtos);

            var lessons = MapDtosToLessons(dtos);

            foreach(var lesson in lessons) {
                await ValidateLesson(lesson);
                await _lessonRepo.CreateAsync(lesson);
            }

            var teachingPlan = new TeachingPlan {
                Name = name,
                LastUpdated = DateTime.UtcNow,
                Lessons = lessons
            };

            await _teachingPlanRepo.CreateAsync(teachingPlan);
        }

        // ---------------------------------------------------
        public async Task UpdateTeachingPlan(IFormFile file, int teachingPlanId)
        {
            // 1) Datei einlesen, DTOs validieren
            ValidateFile(file);
            var json = await ReadJsonAsync(file);
            var dtos = DeserializeLessonDtos(json);
            ValidateDtos(dtos);

            // 2) Bestehenden TeachingPlan inkl. Lessons & Trainees laden
            var teachingPlan = await GetExistingPlanWithDetails(teachingPlanId);

            // Hilfslisten für diff
            var existingLessons   = teachingPlan.Lessons.ToList();
            var dtoIds            = dtos.Select(d => d.Id).ToHashSet();
            var removedLessons    = new List<Lesson>();
            var addedLessons      = new List<Lesson>();

            // 3) Lessons entfernen, die im DTO fehlen → "removed"
            var toRemove = existingLessons
                .Where(l => !dtoIds.Contains(l.LessonId))
                .ToList();
            removedLessons.AddRange(toRemove);
            foreach (var lesson in toRemove)
            {
                teachingPlan.Lessons.Remove(lesson);
                await _lessonRepo.DeleteAsync(lesson);
            }

            // 4) DTOs upserten und zusätzlich:
            //    - Deprecated = true → nur Open-TraineeLessons sollen gelöscht werden ("removed")
            //    - ganz neue Lessons → in teachingPlan einfügen und als "added" markieren
            foreach (var dto in dtos)
            {
                var lesson = teachingPlan.Lessons.FirstOrDefault(l => l.LessonId == dto.Id);
                if (lesson != null)
                {
                    ApplyDtoToLesson(dto, lesson);

                    if (dto.Deprecated)
                    {
                        // nur Open-TraineeLessons davon als removed zählen
                        removedLessons.Add(lesson);
                    }

                    await _lessonRepo.UpdateAsync(lesson);
                }
                else
                {
                    var newLesson = MapDtoToLesson(dto);
                    await ValidateLesson(newLesson);
                    await _lessonRepo.CreateAsync(newLesson);

                    teachingPlan.Lessons.Add(newLesson);
                    addedLessons.Add(newLesson);
                }
            }

            teachingPlan.LastUpdated = DateTime.UtcNow;
            await _teachingPlanRepo.UpdateAsync(teachingPlan);

            // 5) Für jeden zugewiesenen Trainee:
            //    - Open-TraineeLessons der removedLessons löschen und sammeln
            //    - für addedLessons neue TraineeLesson anlegen und sammeln
            //    - NotifyAboutImportChangeAsync aufrufen
            foreach (var trainee in teachingPlan.Trainees)
            {
                var removedTraineeLessons = new List<TraineeLesson>();
                var addedTraineeLessons   = new List<TraineeLesson>();

                // a) Entfernen
                foreach (var lesson in removedLessons)
                {
                    var tls = await _traineeLessonRepo
                        .GetAllTraineeLessonsOfLessonWithLessonAsync(lesson.LessonId);

                    var toDelete = tls
                        .Where(tl => tl.TraineeId == trainee.Id
                                && tl.State     == TraineeLessonState.Open)
                        .ToList();

                    foreach (var tl in toDelete)
                    {
                        await _traineeLessonRepo.DeleteAsync(tl.TraineeLessonId);
                    }

                    removedTraineeLessons.AddRange(toDelete);
                }

                // b) Hinzufügen
                foreach (var lesson in addedLessons)
                {
                    var tl = new TraineeLesson
                    {
                        TraineeId = trainee.Id,
                        Trainee   = trainee,
                        LessonId  = lesson.LessonId,
                        Lesson    = lesson
                    };
                    await _traineeLessonRepo.CreateAsync(tl);
                    addedTraineeLessons.Add(tl);
                }

                // c) Benachrichtigung
                await _emailNotificationService
                    .NotifyAboutImportChangeAsync(trainee, removedTraineeLessons, addedTraineeLessons);
            }
        }


        // ---------------------------------------------------
        public async Task DeleteTeachingPlan(int id) {
            var plan = await _teachingPlanRepo.GetTeachingPlanByIdWithLessonsAndTraineesAsync(id)
                       ?? throw new InvalidOperationException("TeachingPlan nicht gefunden.");

            if (plan.Trainees != null && plan.Trainees.Any())
                throw new InvalidOperationException("Dieser TeachingPlan wird noch verwendet!");

            var lessonsCopy = plan.Lessons.ToList();

            foreach(var lesson in lessonsCopy){
                await _lessonRepo.DeleteAsync(lesson);
            }

            await _teachingPlanRepo.DeleteAsync(plan);
        }

        // ---------------------------------------------------
        public async Task AssignTeachingPlanToTraineeAsync(ApplicationUser trainee, int teachingPlanId) {
            var plan = await _teachingPlanRepo.GetTeachingPlanByIdWithLessonsAndTraineesAsync(teachingPlanId)
                       ?? throw new InvalidOperationException("TeachingPlan nicht gefunden.");

            trainee.TeachingPlan = plan;
            await CreateTraineeLessonsAsync(trainee, plan.Lessons);

            plan.Trainees.Add(trainee);
            await _applicationUserRepo.UpdateAsync(trainee);
            await _teachingPlanRepo.UpdateAsync(plan);
        }

        // ---------------------------------------------------
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

        // ---------------------------------------------------
        public Task<IEnumerable<TeachingPlan>> GetAllTeachingPlansAsync()
            => _teachingPlanRepo.GetAllTeachingPlansAsync();

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
        private async Task ValidateLesson(Lesson l) {
            bool exists = await _lessonRepo.ExistsAsync(l);
            if(exists)
                throw new ArgumentException("Dieser Teachingplan existiert schon!");
        }

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
        private void ValidateDtos(List<LessonDto> dtos) {
            if (dtos == null || !dtos.Any())
                throw new InvalidOperationException("Keine gültigen Lektionen im JSON gefunden!");
        }

        // ---------------------------------------------------
        private List<Lesson> MapDtosToLessons(IEnumerable<LessonDto> dtos) {
            return dtos.Select(dto => new Lesson {
                LessonId = dto.Id,
                Title = dto.Title,
                LinkUrl = dto.Url,
                EstimatedEffort = dto.Estimate ?? 0,
                IsInactive = dto.Deprecated
            }).ToList();
        }

        // ---------------------------------------------------
        private async Task<TeachingPlan> GetExistingPlanWithDetails(int id) {
            var plan = await _teachingPlanRepo
                .GetTeachingPlanByIdWithLessonsAndTraineesAsync(id);
            if (plan == null)
                throw new InvalidOperationException("TeachingPlan nicht gefunden.");
            return plan;
        }

        // ---------------------------------------------------
        private void ApplyDtoToLesson(LessonDto dto, Lesson lesson) {
            lesson.Title = dto.Title;
            lesson.LinkUrl = dto.Url;
            lesson.EstimatedEffort = dto.Estimate ?? 0;
            lesson.IsInactive = dto.Deprecated;
        }

        // ---------------------------------------------------
        private Lesson MapDtoToLesson(LessonDto dto) {
            return new Lesson {
                LessonId = dto.Id,
                Title = dto.Title,
                LinkUrl = dto.Url,
                EstimatedEffort = dto.Estimate ?? 0,
                IsInactive = dto.Deprecated
            };
        }

        // ---------------------------------------------------
        private async Task DeleteOpenTraineeLessonsAsync(int lessonId) {
            var traineeLessons = await _traineeLessonRepo
                .GetAllTraineeLessonsOfLessonWithLessonAsync(lessonId);

            foreach (var tl in traineeLessons.Where(t => t.State == TraineeLessonState.Open)) {
                await _traineeLessonRepo.DeleteAsync(tl.TraineeLessonId);
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
                await _traineeLessonRepo.CreateAsync(tl);
            }
        }


    }
}
