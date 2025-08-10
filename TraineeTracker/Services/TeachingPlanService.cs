using Newtonsoft.Json;
using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Data.Lessons;
using TraineeTracker.Data.TraineeLessons;
using TraineeTracker.Data.TeachingPlans;
using TraineeTracker.Models.Domain;
using TraineeTracker.Models.Dtos;
using TraineeTracker.Models.ViewModels.TeachingPlanViewModels;
using TraineeTracker.Services.Email;

namespace TraineeTracker.Services {

    // ---------------------------------------------------
    /// <summary>
    /// Provides business logic for managing teaching plans, including import preview, persistence,
    /// update, deletion, assignment to trainees, and change notifications.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexandros Blask, Simon Hinterreiter
    /// </remarks>
    public class TeachingPlanService {

        /// <summary>
        /// Provides Access to the TeachingPlanRepo
        /// </summary>
        private readonly ITeachingPlanRepository _databaseTeachingPlanRepository;

        /// <summary>
        /// Provides Access to the LessonRepo
        /// </summary>
        private readonly ILessonRepository _databaseLessonRepository;

        /// <summary>
        /// Provides Access to the TraineeLessonRepo
        /// </summary>
        private readonly ITraineeLessonRepository _databaseTraineeLessonRepository;

        /// <summary>
        /// Provides Access to the ApplicationUserRepo
        /// </summary>
        private readonly IApplicationUserRepository _databaseApplicationUserRepository;

        /// <summary>
        /// Provides Access to the NotificationService
        /// </summary>
        private readonly EmailNotificationService _emailNotificationService;

        // ---------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="TeachingPlanService"/> class.
        /// </summary>
        /// <param name="teachingPlanRepo">Repository for teaching plan data access.</param>
        /// <param name="databaseLessonRepository">Repository for lesson data access.</param>
        /// <param name="databaseTraineeLessonRepository">Repository for trainee lesson data access.</param>
        /// <param name="applicationUserRepo">Repository for application user data access.</param>
        /// <param name="emailNotificationService">Service for sending email notifications.</param>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
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
        /// <summary>
        /// Builds the import dashboard view model containing all existing teaching plans.
        /// </summary>
        /// <returns>
        /// A task that returns an <see cref="ImportDashboardViewModel"/> with existing teaching plans.
        /// </returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask, Simon Hinterreiter
        /// </remarks>
        public async Task<ImportDashboardViewModel> BuildImportDashboardViewModelAsync() {
            // a) Get all Teachingplans
            var allPlans = await _databaseTeachingPlanRepository.GetAllTeachingPlansWithLessonsAndTraineesAsync();

            // b) Map to dashboard
            var plans = allPlans.Select(tp => {

                // Get ActiveLessons
                var activeLessons = tp.Lessons?
                    .Where(l => !l.IsInactive)
                    .ToList() ?? new List<Lesson>();

                // Get ActiveTrainees (not-closed Trainees)
                var activeTrainees = tp.Trainees?
                    .Where(t => !t.IsClosed) // Redundant, since Closing also Unassigns a Trainee from a TeachingPlan
                    .ToList() ?? new List<ApplicationUser>();

                return new ExistingTeachingPlanViewModel {
                    TeachingPlanId = tp.TeachingPlanId,
                    Name = tp.Name,
                    LastUpdated = tp.LastUpdated,
                    LessonCount = activeLessons.Count,
                    TraineeCount = activeTrainees.Count,
                    ActiveLessons = activeLessons,
                    ActiveTrainees = activeTrainees
                };
            }).ToList();

            return new ImportDashboardViewModel {
                ExistingTeachingPlans = plans
            };
        }


        // ---------------------------------------------------
        /// <summary>
        /// Saves an uploaded JSON file temporarily and returns the generated file name.
        /// </summary>
        /// <param name="file">The IFormFile representing the uploaded JSON.</param>
        /// <returns>
        /// A task that returns the temporary file name.
        /// </returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        public async Task<string> SaveTempJsonFileAsync(IFormFile file) {
            var fileName = $"{Guid.NewGuid()}.json";
            var fullPath = Path.Combine(Path.GetTempPath(), fileName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return fileName;
        }

        // ---------------------------------------------------
        /// <summary>
        /// Builds the import preview view model for a given teaching plan DTO,
        /// showing lists of new, reactivated, and deactivated lessons.
        /// </summary>
        /// <param name="teachingPlanDto">The DTO containing import parameters and file.</param>
        /// <returns>
        /// A task that returns a <see cref="TeachingPlanImportPreviewViewModel"/>.
        /// </returns>
        /// <remarks>
        /// Code Ownership: Simon Hinterreiter
        /// </remarks>
        public async Task<TeachingPlanImportPreviewViewModel> BuildImportPreviewViewModelAsync(TeachingPlanDto teachingPlanDto) {

            // +++++++++++++++
            // a. Modal was just opened:
            if (teachingPlanDto.NewPlanFile is null) {

                var plan = await _databaseTeachingPlanRepository
                    .GetTeachingPlanByIdAsync(teachingPlanDto.ExistingTeachingPlanId!.Value);

                return new TeachingPlanImportPreviewViewModel {
                    TeachingPlanId = plan!.TeachingPlanId,
                    TeachingPlanName = plan.Name
                };
            }

            // +++++++++++++++
            // b. Modal already open and NewPlanFile was uploaded:

            // Save the file temporarily and give the dto the TempFileName
            var tempFileName = await SaveTempJsonFileAsync(teachingPlanDto.NewPlanFile);
            teachingPlanDto.TempFileName = tempFileName;

            // 1) Load imported Dtos and validate
            var json = await ReadJsonAsync(teachingPlanDto.NewPlanFile);
            var importedDtos = DeserializeLessonDtos(json);
            ValidateLessonDtos(importedDtos);

            // 2) Get existing teachingPlan
            int teachingPlanId = teachingPlanDto.ExistingTeachingPlanId!.Value;
            var existingPlan = await _databaseTeachingPlanRepository
                .GetTeachingPlanByIdWithLessonsAndTraineesAsync(teachingPlanId);

            // 3) Get existing Lessons:
            var existingLessons = existingPlan!.Lessons.ToList();
            var existingLessonsByMakandraId = existingLessons.ToDictionary(l => l.MakandraId);

            // Prepare lists for ViewModel:
            var newActive = new List<LessonDto>();
            var newInactive = new List<LessonDto>();
            var existingReactivated = new List<LessonDto>();
            var existingDeactivated = new List<LessonDto>();
            var allImported = new List<LessonDto>();

            // 4) Loop through imported lessons:
            var processedMakandraIds = new HashSet<string>(); // Used to ignore duplicate MakandraIDs: Only the first lesson with a specific MakandraID in the JSON is processed
            foreach (var dto in importedDtos) {

                // Skip if we've already processed this MakandraId in this loop
                if (!processedMakandraIds.Add(dto.Id))
                    continue;

                // Keep de-duped import order list for the UI
                allImported.Add(dto);

                // Check if that MakandraId already exists in existingLessons of that teachingPlan:
                var exists = existingLessonsByMakandraId.TryGetValue(dto.Id, out var existingLesson);

                // Lesson is new:
                if (!exists) {
                    if (dto.Deprecated)
                        // 2. new but Inative:
                        newInactive.Add(dto);
                    else
                        // 1. New and Active:
                        newActive.Add(dto);
                }
                // Lesson already exists:
                else if (!dto.Deprecated && existingLesson!.IsInactive) {
                    // existing and reactivated:
                    existingReactivated.Add(MapLessonToLessonDto(existingLesson));
                }
            }

            // 5) Determine deactivated Lessons:
            var importedDeprecatedIds = importedDtos
                .Where(dto => dto.Deprecated)
                .Select(dto => dto.Id)
                .ToHashSet();

            var importedMakandraIds = importedDtos
                .Select(dto => dto.Id)
                .ToHashSet();

            foreach (var lesson in existingLessons) {
                // Only look at newly deprecated:
                if (lesson.IsInactive)
                    continue;

                bool isNowDeprecated = importedDeprecatedIds.Contains(lesson.MakandraId);
                bool wasRemoved = !importedMakandraIds.Contains(lesson.MakandraId);

                if (wasRemoved || isNowDeprecated) {
                    existingDeactivated.Add(MapLessonToLessonDto(lesson));
                }
            }

            // 6) Fill ViewModel
            var vm = new TeachingPlanImportPreviewViewModel {
                TeachingPlanId = teachingPlanId,
                TeachingPlanName = existingPlan.Name,
                TempFileName = teachingPlanDto.TempFileName,
                NewActiveLessons = newActive,
                NewInactiveLessons = newInactive,
                ExistingReactivatedLessons = existingReactivated,
                ExistingDeactivatedLessons = existingDeactivated,
                AllImportedLessons = allImported
            };

            return vm;
        }

        // ---------------------------------------------------
        /// <summary>
        /// Imports a new teaching plan and its lessons from the provided DTO.
        /// </summary>
        /// <param name="dto">The DTO containing new teaching plan data and file.</param>
        /// <returns>A task representing the asynchronous import operation.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        public async Task ImportNewTeachingPlan(TeachingPlanDto dto) {
            // a) Validate Dto attributes
            ValidateFile(dto.NewPlanFile!);
            ValidateName(dto.NewPlanName!);

            // b) Get jsonstring out of file, deserialize json string and validate lessonDto
            var json = await ReadJsonAsync(dto.NewPlanFile!);
            var lessonDtos = DeserializeLessonDtos(json);
            ValidateLessonDtos(lessonDtos);

            // c) Create TeachingPlan:
            var teachingPlan = new TeachingPlan {
                Name = dto.NewPlanName!,
                LastUpdated = DateTime.UtcNow,
            };

            await _databaseTeachingPlanRepository.CreateAsync(teachingPlan);

            // d) Create Lessons for TeachingPlan:
            var lessons = CreateLessons(lessonDtos, teachingPlan.TeachingPlanId);
            foreach (var lesson in lessons) {
                await _databaseLessonRepository.CreateAsync(lesson);
            }
        }

        // ---------------------------------------------------
        /// <summary>
        /// Updates an existing teaching plan and synchronizes lessons based on the provided DTO.
        /// </summary>
        /// <param name="teachingPlanDto">The DTO containing update information and temp file.</param>
        /// <returns>A task representing the asynchronous update operation.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask, Simon Hinterreiter
        /// </remarks>
        public async Task UpdateTeachingPlan(TeachingPlanDto teachingPlanDto) {
            /* 
            Console.WriteLine($"[DEBUG] Service: Called UpdateTeachingPlan");
            */

            int existingTeachingPlanId = teachingPlanDto.ExistingTeachingPlanId ?? throw new Exception("ExistingTeachingPlanId missing!");

            // +++++++++++++++
            // 1) Get File from temp Location:
            if (string.IsNullOrWhiteSpace(teachingPlanDto.TempFileName))
                throw new InvalidOperationException("Temp file name is missing for import.");

            var fullPath = Path.Combine(Path.GetTempPath(), teachingPlanDto.TempFileName);
            var json = await File.ReadAllTextAsync(fullPath);

            if (File.Exists(fullPath))
                File.Delete(fullPath);

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

            int sortingIndex = 0;

            // +++++++++++++++
            // 3) Upsert DTOs and in addition:
            //    - Deprecated = true → only Open-TraineeLessons should be deleted ("removed")
            //    - totally new Lessons → insert into teachingPlan and mark as "added"

            var processedMakandraIds = new HashSet<string>();

            foreach (var lessonDto in lessonDtos) {

                // Skip if we've already processed this MakandraId
                if (!processedMakandraIds.Add(lessonDto.Id))
                    continue;

                // Find Lesson by lessonDto.MakandraId in existingLessons (MakandraId is at least unique within TeachingPlan)
                var lesson = existingLessons.FirstOrDefault(l => l.MakandraId == lessonDto.Id);

                if (lesson == null) {
                    // Lesson does not exist yet in existingTeachingPlan:

                    // Create new lesson object:
                    var newLesson = CreateLesson(lessonDto, existingTeachingPlanId, sortingIndex++);

                    // Store new lesson in DB:
                    await _databaseLessonRepository.CreateAsync(newLesson);
                    /*  
                                        // Add Lesson to teachingPlan:
                                        existingTeachingPlan.Lessons.Add(newLesson);
                    */
                    // Append addedLessons
                    addedLessons.Add(newLesson);

                } else if (lessonDto.Deprecated) {
                    // Existing lesson gets deprecated:

                    // Update the lesson (do not increment sorting index, because we set it later):
                    UpdateLesson(lessonDto, lesson, lesson.SortingIndex);

                    // Update lesson in DB:
                    await _databaseLessonRepository.UpdateAsync(lesson);

                    // Append lessonsMarkedAsInactive:
                    lessonsMarkedAsInactive.Add(lesson);

                } else {
                    // Lesson exisits already and is not deprecated in import:

                    // If it was previously inactive, we add it to addedLessons
                    if (lesson.IsInactive) {
                        addedLessons.Add(lesson);
                    }

                    // Update the lesson:
                    UpdateLesson(lessonDto, lesson, sortingIndex++); // Setzt IsInactive = false

                    // Update lesson in DB:
                    await _databaseLessonRepository.UpdateAsync(lesson);
                }
            }

            // +++++++++++++++
            // 4) Mark Lessons, that are missing in dto, as inactive
            var missingLessons = existingLessons
                .Where(l => !dtoMakandraIds.Contains(l.MakandraId))
                .OrderBy(l => l.SortingIndex)
                .ToList();

            lessonsMarkedAsInactive.AddRange(missingLessons);


            foreach (var lesson in lessonsMarkedAsInactive) {
                lesson.IsInactive = true;
                lesson.SortingIndex = sortingIndex++;

                await _databaseLessonRepository.UpdateAsync(lesson);
            }

            // +++++++++++++++
            // Update existingTeachingPlan -> New lessons are now in Db. Inactive Lessons are marked
            existingTeachingPlan.LastUpdated = DateTime.UtcNow;
            await _databaseTeachingPlanRepository.UpdateAsync(existingTeachingPlan);

            // DEBUG:
            Console.WriteLine("[Import] Final sorting indices:");
            foreach (var l in existingTeachingPlan.Lessons.OrderBy(l => l.SortingIndex)) {
                var status = l.IsInactive ? "inactive" : "active";
                Console.WriteLine($"  - {l.SortingIndex}: [{l.LessonId}] {l.Title} | MakandraId: {l.MakandraId} ({status})");
            }

            // +++++++++++++++
            // 5) For every added Trainee:
            //    - Delete Open-TraineeLessons of lessonsMarkedAsInactive and gather
            //    - for addedLessons create new TraineeLesson and gather
            //    - call NotifyAboutImportChangeAsync
            foreach (var trainee in existingTeachingPlan.Trainees) {

                // +++++++++++++++
                // Get all current TraineeLessons of Trainee
                var traineeLessonsOfTrainee = (await _databaseTraineeLessonRepository
                    .GetAllTraineeLessonsOfTraineeWithLessonAsync(trainee.Id)).ToList();

                // +++++++++++++++
                // a) Remove
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
                // b) Add
                var addedTraineeLessons = new List<TraineeLesson>();
                foreach (var lesson in addedLessons.Where(l => !l.IsInactive)) {

                    // Check if Trainee already has a TraineeLesson with fitting Lesson.MakandraId:
                    bool alreadyExists = traineeLessonsOfTrainee
                        .Any(tl => tl.Lesson.MakandraId == lesson.MakandraId);

                    if (alreadyExists)
                        continue;

                    // Trainee does no have a TraineeLesson with fitting Lesson.MakandraId:
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
                // Debug-Output
                Console.WriteLine($"[Import] Trainee {trainee.UserName} – Removed Lessons:");
                foreach (var tl in removedTraineeLessons) {
                    Console.WriteLine($"  - {tl.Lesson.Title} (ID: {tl.Lesson.LessonId})");
                }

                Console.WriteLine($"[Import] Trainee {trainee.UserName} – Added Lessons:");
                foreach (var tl in addedTraineeLessons) {
                    Console.WriteLine($"  + {tl.Lesson.Title} (ID: {tl.Lesson.LessonId})");
                }

                // +++++++++++++++
                // c) Notifications
                _ = _emailNotificationService
                    .NotifyAboutImportChangeAsync(trainee, removedTraineeLessons, addedTraineeLessons);
            }
        }

        // ---------------------------------------------------
        /// <summary>
        /// Deletes a teaching plan and its associated lessons if no trainees are assigned.
        /// </summary>
        /// <param name="existingTeachingPlanId">The ID of the teaching plan to delete.</param>
        /// <returns>A task representing the asynchronous delete operation.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        public async Task DeleteTeachingPlan(int existingTeachingPlanId) {

            // a) Debug-Output
            Console.WriteLine($"ID used for delete: {existingTeachingPlanId}");

            // b) Get teachingplan with requested ID
            var plan = await _databaseTeachingPlanRepository.GetTeachingPlanByIdWithLessonsAndTraineesAsync(existingTeachingPlanId)
                       ?? throw new InvalidOperationException("TeachingPlan not found.");

            // c) Check if Trainees are assigned to teachingplan and if not delete 
            if (plan.Trainees != null && plan.Trainees.Any())
                throw new InvalidOperationException("This TeachingPlan is still in use.");

            var lessonsCopy = plan.Lessons.ToList();

            foreach (var lesson in lessonsCopy) {
                await _databaseLessonRepository.DeleteAsync(lesson);
            }

            await _databaseTeachingPlanRepository.DeleteAsync(plan);
        }

        // ---------------------------------------------------
        /// <summary>
        /// Assigns a teaching plan to a trainee, creating corresponding trainee lessons and sending notifications.
        /// </summary>
        /// <param name="trainee">The trainee to assign the plan to.</param>
        /// <param name="teachingPlanId">The ID of the teaching plan to assign.</param>
        /// <returns>A task representing the asynchronous assignment operation.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        public async Task AssignTeachingPlanToTraineeAsync(ApplicationUser trainee, int teachingPlanId) {
            // a) Get teachingplan with requested ID
            var plan = await _databaseTeachingPlanRepository.GetTeachingPlanByIdWithLessonsAndTraineesAsync(teachingPlanId)
                       ?? throw new InvalidOperationException("TeachingPlan not found.");

            // b) Set teachingplan of trainee to teachingplan which is requested
            trainee.TeachingPlanId = teachingPlanId;
            trainee.TeachingPlan = plan;
            await CreateTraineeLessonsAsync(trainee, plan.Lessons);

            // c) Add trainee to teachingplan
            plan.Trainees.Add(trainee);
            await _databaseApplicationUserRepository.UpdateAsync(trainee);
            await _databaseTeachingPlanRepository.UpdateAsync(plan);
        }

        // ---------------------------------------------------
        /// <summary>
        /// Unassigns a teaching plan from a trainee by removing all associated trainee lessons,
        /// detaching the trainee from the teaching plan, and clearing the foreign key reference.
        /// </summary>
        /// <param name="trainee">The trainee whose teaching plan should be unassigned.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the trainee has no assigned teaching plan (<c>TeachingPlanId</c> is null).
        /// </exception>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        public async Task UnassignTeachingPlanFromTraineeAsync(ApplicationUser trainee) {
            // Trainee - Get all TraineeLessons
            var traineeLessons = await _databaseTraineeLessonRepository.GetAllTraineeLessonsOfTraineeWithLessonAsync(trainee.Id);

            // Trainee - Delete all TraineeLessons
            foreach (var traineeLesson in traineeLessons) {
                await _databaseTraineeLessonRepository.DeleteAsync(traineeLesson.TraineeLessonId);
            }

            // Get TeachingPlan
            var teachingPlan = await _databaseTeachingPlanRepository
                .GetTeachingPlanByIdWithLessonsAndTraineesAsync(trainee.TeachingPlanId ?? throw new InvalidOperationException("Trainee hat keinen TeachingPlan."));

            // TeachingPlan - Remove Trainee
            teachingPlan!.Trainees.Remove(trainee);
            await _databaseTeachingPlanRepository.UpdateAsync(teachingPlan);
            await _databaseApplicationUserRepository.UpdateAsync(trainee);

            // Trainee - Remove Foreign Key
            trainee.TeachingPlanId = null;
        }

        // ---------------------------------------------------
        /// <summary>
        /// Validates that the given file is not null or empty.
        /// </summary>
        /// <param name="file">The uploaded file to validate.</param>
        /// <exception cref="ArgumentException">
        /// Thrown if the file is null or has a length of 0.
        /// </exception>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        private void ValidateFile(IFormFile file) {
            // Validate File
            if (file == null || file.Length == 0)
                throw new ArgumentException("The file is empty!");
        }

        // ---------------------------------------------------
        /// <summary>
        /// Validates that the given name is not null, empty, or whitespace-only.
        /// </summary>
        /// <param name="name">The name string to validate.</param>
        /// <exception cref="ArgumentException">
        /// Thrown if the name is null, empty, or consists only of whitespace.
        /// </exception>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        private void ValidateName(string name) {
            // Validate Name
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Invalid name!");
        }

        // ---------------------------------------------------
        /// <summary>
        /// Reads the content of the given file and returns it as a JSON string.
        /// </summary>
        /// <param name="file">The uploaded file to read from.</param>
        /// <returns>A task representing the asynchronous operation, containing the file content as a string.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        private async Task<string> ReadJsonAsync(IFormFile file) {
            // Read Json which is used for Import and Update
            using var reader = new StreamReader(file.OpenReadStream());
            return await reader.ReadToEndAsync();
        }

        // ---------------------------------------------------
        /// <summary>
        /// Deserializes the given JSON string into a list of <see cref="LessonDto"/> objects.
        /// </summary>
        /// <param name="json">The JSON string representing a list of lessons.</param>
        /// <returns>A list of deserialized <see cref="LessonDto"/> instances.</returns>
        /// <exception cref="Exception">
        /// Thrown if the JSON string could not be deserialized into a valid lesson list.
        /// </exception>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        private List<LessonDto> DeserializeLessonDtos(string json) {
            // Use json string from ReadJson to convert to deserialized object
            return JsonConvert.DeserializeObject<List<LessonDto>>(json)
                ?? throw new Exception("LessonDtos could not be deserialized.");
        }

        // ---------------------------------------------------
        /// <summary>
        /// Validates that the given list of lesson DTOs is not null or empty.
        /// </summary>
        /// <param name="dtos">The list of <see cref="LessonDto"/> objects to validate.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the list is null or contains no elements.
        /// </exception>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        private void ValidateLessonDtos(List<LessonDto> dtos) {
            // Validate Dtos
            if (dtos == null || !dtos.Any())
                throw new InvalidOperationException("No valid lessons found in JSON!");
        }

        // ---------------------------------------------------
        /// <summary>
        /// Maps a collection of <see cref="LessonDto"/> objects to a list of <see cref="Lesson"/> entities
        /// and assigns them to the specified teaching plan with sequential sorting indices.
        /// </summary>
        /// <param name="dtos">The lesson DTOs to convert.</param>
        /// <param name="teachingPlanId">The ID of the teaching plan to associate with the created lessons.</param>
        /// <returns>A list of <see cref="Lesson"/> entities ready for persistence.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        private List<Lesson> CreateLessons(IEnumerable<LessonDto> dtos, int teachingPlanId) {
            var processedMakandraIds = new HashSet<string>();
            var lessons = new List<Lesson>();
            int sortingIndex = 1;

            foreach (var dto in dtos) {
                
                // Skip duplicates — only first occurrence counts
                if (!processedMakandraIds.Add(dto.Id))
                    continue;

                lessons.Add(new Lesson {
                    MakandraId = dto.Id,
                    Title = dto.Title,
                    LinkUrl = dto.Url,
                    EstimatedEffort = dto.Estimate ?? 0,
                    IsInactive = dto.Deprecated,
                    SortingIndex = sortingIndex++,
                    TeachingPlanId = teachingPlanId
                });
            }

            return lessons;
        }

        // ---------------------------------------------------
        /// <summary>
        /// Maps a <see cref="Lesson"/> entity to a corresponding <see cref="LessonDto"/>.
        /// </summary>
        /// <param name="lesson">The <see cref="Lesson"/> instance to map.</param>
        /// <returns>The mapped <see cref="LessonDto"/>.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        public LessonDto MapLessonToLessonDto(Lesson lesson) {
            return new LessonDto {
                Id = lesson.MakandraId,
                Title = lesson.Title,
                Url = lesson.LinkUrl,
                Estimate = lesson.EstimatedEffort,
                Deprecated = lesson.IsInactive
            };
        }

        // ---------------------------------------------------
        /// <summary>
        /// Updates an existing <see cref="Lesson"/> entity with data from a <see cref="LessonDto"/> 
        /// and assigns the given sorting index.
        /// </summary>
        /// <param name="dto">The DTO containing the updated lesson data.</param>
        /// <param name="lesson">The existing <see cref="Lesson"/> entity to update.</param>
        /// <param name="sortingIndex">The new sorting index to assign to the lesson.</param>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        private void UpdateLesson(LessonDto dto, Lesson lesson, int sortingIndex) {

            // Update Lesson object with lesson Dto
            lesson.Title = dto.Title;
            lesson.LinkUrl = dto.Url;
            lesson.EstimatedEffort = dto.Estimate ?? 0;
            lesson.IsInactive = dto.Deprecated;
            lesson.SortingIndex = sortingIndex;
        }

        // ---------------------------------------------------
        /// <summary>
        /// Creates a new <see cref="Lesson"/> entity from a <see cref="LessonDto"/> and assigns it
        /// to the specified teaching plan with the given sorting index.
        /// </summary>
        /// <param name="dto">The DTO containing the lesson data.</param>
        /// <param name="teachingPlanId">The ID of the teaching plan to associate the lesson with.</param>
        /// <param name="sortingIndex">The sorting index to assign to the lesson.</param>
        /// <returns>The newly created <see cref="Lesson"/> entity.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        private Lesson CreateLesson(LessonDto dto, int teachingPlanId, int sortingIndex) {

            // Create Lesson object with lesson Dto
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
        /// <summary>
        /// Creates <see cref="TraineeLesson"/> entries for all active lessons and associates them with the given trainee.
        /// Inactive lessons are skipped.
        /// </summary>
        /// <param name="trainee">The trainee to assign the lessons to.</param>
        /// <param name="lessons">The collection of <see cref="Lesson"/> entities to process.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        private async Task CreateTraineeLessonsAsync(ApplicationUser trainee, IEnumerable<Lesson> lessons) {

            // Create Lessons which are inactive
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
