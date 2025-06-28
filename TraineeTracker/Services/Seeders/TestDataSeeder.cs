using Microsoft.AspNetCore.Identity;
using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Data.Feedbacks;
using TraineeTracker.Data.Lessons;
using TraineeTracker.Data.TeachingPlans;
using TraineeTracker.Data.TraineeLessonLog;
using TraineeTracker.Data.TraineeLessons;
using TraineeTracker.Models.Domain;
using TraineeTracker.Models.Dtos;
using TraineeTracker.Services.Admin;

namespace TraineeTracker.Services.Seeders {
    public class TestDataSeeder {

        private readonly AdminService _adminService;
        private readonly IApplicationUserRepository _databaseApplicationUserRepository;
        private readonly ILessonRepository _databaseLessonRepository;

        private readonly ITraineeLessonRepository _databaseTraineeLessonRepository;
        private readonly ITeachingPlanRepository _databaseTeachingPlanRepository;

        private readonly IFeedbackRepository _databaseFeedbackRepository;
        private readonly ITraineeLessonLogEntryRepository _databaseTraineeLessonLogEntryRepository;

        // ---------------------------------------------------
        public TestDataSeeder(IApplicationUserRepository databaseApplicationUserRepository, ILessonRepository lessonRepo, ITraineeLessonRepository traineeLessonRepo, ITeachingPlanRepository teachingPlanRepo, AdminService adminService, IFeedbackRepository feedbackRepo, ITraineeLessonLogEntryRepository logEntryRepo) {
            _databaseApplicationUserRepository = databaseApplicationUserRepository;
            _databaseLessonRepository = lessonRepo;
            _databaseTeachingPlanRepository = teachingPlanRepo;
            _adminService = adminService;
            _databaseTraineeLessonRepository = traineeLessonRepo;
            _databaseFeedbackRepository = feedbackRepo;
            _databaseTraineeLessonLogEntryRepository = logEntryRepo;
        }


        // ---------------------------------------------------
        public async Task SeedLessonsAsync() {
            var lessons = new[] {
            new Lesson { LessonId = 1, Title = "Einführung in C# (Only WebDev)", EstimatedEffort = 3.0, LinkUrl = "https://learn.microsoft.com/de-de/dotnet/csharp/" },
            new Lesson { LessonId = 2, Title = "Objektorientierung (Only WebDev)", EstimatedEffort = 2.5, LinkUrl = "https://learn.microsoft.com/de-de/dotnet/csharp/fundamentals/object-oriented/" },
            new Lesson { LessonId = 3, Title = "Entity Framework Core (Both TeachingPlans) ", EstimatedEffort = 4.0, LinkUrl = "https://learn.microsoft.com/de-de/ef/core/" },
            new Lesson { LessonId = 4, Title = "ASP.NET Core MVC (Only DevOps)", EstimatedEffort = 5.0, LinkUrl = "https://learn.microsoft.com/de-de/aspnet/core/mvc/" }
            };

            foreach (var lesson in lessons) {
                var existing = await _databaseLessonRepository.GetLessonByIdAsync(lesson.LessonId);

                // Only add lesson if not existing yet
                if (existing == null) {
                    await _databaseLessonRepository.CreateAsync(lesson);
                }
            }
        }

        // ---------------------------------------------------
        public async Task SeedTeachingPlansAsync() {
            var allLessons = (await _databaseLessonRepository.GetAllLessonsAsync()).ToList();

            var plans = new[] {
                new {
                    Id = 1,
                    Name = "WebDevelopment",
                    LessonIds = new[] { 1, 2, 3 }
                },
                new {
                    Id = 2,
                    Name = "DevOps",
                    LessonIds = new[] { 3, 4 }
                }
            };


            foreach (var plan in plans) {
                var existing = await _databaseTeachingPlanRepository.GetTeachingPlanByIdWithLessonsAndTraineesAsync(plan.Id);

                // Only add teachingPlan if not existing yet
                if (existing != null)
                    continue;

                var lessons = allLessons
                    .Where(l => plan.LessonIds.Contains(l.LessonId))
                    .ToList();

                var teachingPlan = new TeachingPlan {
                    TeachingPlanId = plan.Id,
                    Name = plan.Name,
                    LastUpdated = DateTime.UtcNow,
                    Lessons = lessons
                };

                await _databaseTeachingPlanRepository.CreateAsync(teachingPlan);
            }
        }

        // ---------------------------------------------------
        public async Task SeedUsersAsync() {

            var userData = new List<ApplicationUserDto> {
                new ApplicationUserDto {
                    Email = "simon.hinterreiter@uni-a.de",
                    Role = "Admin",
                    Password = "Sopro.2025"
                },
                new ApplicationUserDto {
                    Email = "paul.schweizer@uni-a.de",
                    Role = "Mentor",
                    Password = "Sopro.2025"
                },
                new ApplicationUserDto {
                    Email = "alexander.schlemmer@uni-a.de",
                    Role = "Mentor",
                    Password = "Sopro.2025"
                },
                new ApplicationUserDto {
                    Email = "alexandros.blask@uni-a.de",
                    Role = "Trainee",
                    TeachingPlanId = 1,
                    Password = "Sopro.2025",
                    TraineeStartDate = new DateOnly(2025, 4, 1),
                    TraineeEndDate = new DateOnly(2026, 7, 1)
                },
                new ApplicationUserDto {
                    Email = "nikita.stefan@uni-a.de",
                    Role = "Trainee",
                    TeachingPlanId = 2,
                    Password = "Sopro.2025",
                    TraineeStartDate = new DateOnly(2025, 3, 15),
                    TraineeEndDate = new DateOnly(2026, 6, 30)
                },
                new ApplicationUserDto {
                    Email = "stefan.schnupfen@makandra.de",
                    Role = "Trainee",
                    TeachingPlanId = 2,
                    Password = "Sopro.2025",
                    TraineeStartDate = new DateOnly(2025, 3, 15),
                    TraineeEndDate = new DateOnly(2026, 6, 30)
                },
                new ApplicationUserDto {
                    Email = "ursula.urlaub@makandra.de",
                    Role = "Trainee",
                    TeachingPlanId = 2,
                    Password = "Sopro.2025",
                    TraineeStartDate = new DateOnly(2025, 3, 15),
                    TraineeEndDate = new DateOnly(2026, 6, 30)
                },

                new ApplicationUserDto {
                    Email = "closed.trainee@uni-a.de",
                    Role = "Trainee",
                    TeachingPlanId = 2,
                    Password = "Sopro.2025",
                    TraineeStartDate = new DateOnly(2024, 1, 1),
                    TraineeEndDate = new DateOnly(2025, 4, 1)
                }
            };

            foreach (var dto in userData) {
                await _adminService.CreateUserAsync(dto);

                if (dto.Email == "closed.trainee@uni-a.de") {
                    var user = await _databaseApplicationUserRepository.FindByEmailAsync(dto.Email);
                    if (user != null) {
                        await _adminService.SetIsClosedAsync(user.Id, true);
                    }
                }
            }
        }

        // ---------------------------------------------------
        public async Task SeedProcessingPausesAsync() {
            var alex = await _databaseApplicationUserRepository.FindByEmailAsync("alexandros.blask@uni-a.de");
            var nikita = await _databaseApplicationUserRepository.FindByEmailAsync("nikita.stefan@uni-a.de");
            var stefan = await _databaseApplicationUserRepository.FindByEmailAsync("stefan.schnupfen@makandra.de");
            var ursula = await _databaseApplicationUserRepository.FindByEmailAsync("ursula.urlaub@makandra.de");

            var pauseDtos = new List<ProcessingPauseDto>();

            if (alex != null) {
                pauseDtos.Add(new ProcessingPauseDto {
                    TraineeId = alex.Id,
                    StartDate = new DateOnly(2025, 5, 5),
                    EndDate = new DateOnly(2025, 5, 10)
                });

                pauseDtos.Add(new ProcessingPauseDto {
                    TraineeId = alex.Id,
                    StartDate = new DateOnly(2025, 6, 1),
                    EndDate = new DateOnly(2025, 6, 3)
                });
            }

            if (nikita != null) {
                pauseDtos.Add(new ProcessingPauseDto {
                    TraineeId = nikita.Id,
                    StartDate = new DateOnly(2025, 5, 20),
                    EndDate = new DateOnly(2025, 5, 25)
                });
            }
            if (stefan != null) {
                pauseDtos.Add(new ProcessingPauseDto {
                    TraineeId = stefan.Id,
                    StartDate = new DateOnly(2025, 5, 20),
                    EndDate = new DateOnly(2025, 5, 25)
                });
            }
            if (ursula != null) {
                pauseDtos.Add(new ProcessingPauseDto {
                    TraineeId = ursula.Id,
                    StartDate = new DateOnly(2025, 5, 20),
                    EndDate = new DateOnly(2025, 5, 25)
                });
            }
            foreach (var dto in pauseDtos) {
                await _adminService.CreateProcessingPauseAsync(dto);
            }
            // ---------------------------------------------------
        }

        public async Task SeedFeedbackAsync() {

            // Get Trainees
            var alex = await _databaseApplicationUserRepository.FindByEmailAsync("alexandros.blask@uni-a.de");
            var nikita = await _databaseApplicationUserRepository.FindByEmailAsync("nikita.stefan@uni-a.de");

            // Get Mentor and Admin who read it
            var simon = await _databaseApplicationUserRepository.FindByEmailAsync("simon.hinterreiter@uni-a.de"); // Admin
            var paul = await _databaseApplicationUserRepository.FindByEmailAsync("paul.schweizer@uni-a.de");     // Mentor

            // Get Lesson
            var lesson1 = await _databaseLessonRepository.GetLessonByIdAsync(1);
            var lesson3 = await _databaseLessonRepository.GetLessonByIdAsync(3);


            // FeedbackSeeds
            var feedbackSeeds = new List<(ApplicationUser? user, Lesson? lesson, int difficulty, float effort, string previousKnowledge, string? comment)> { (alex, lesson1, 7, 2.5f, "Some Java experience", "Good lesson, but needed more time for the exercises."),
            (nikita, lesson3, 4, 1.2f, "Already knew most of it", "Quick to go through, well explained.")
            };

            var readers = new List<ApplicationUser?> { simon, paul };
            int readerIndex = 0;

            foreach (var (user, lesson, difficulty, effort, previousKnowledge, comment) in feedbackSeeds) {
                if (user == null || lesson == null)
                    continue;

                var reader = readers[readerIndex % readers.Count];
                readerIndex++;

                if (reader == null)
                    continue;

                // Get the corret TraineeLesson
                var traineeLessons = await _databaseTraineeLessonRepository.GetAllTraineeLessonsOfTraineeWithLessonAsync(user.Id);
                var traineeLesson = traineeLessons.FirstOrDefault(tl => tl.Lesson.LessonId == lesson.LessonId);
                if (traineeLesson == null)
                    continue;


                // Change the State to Rated
                traineeLesson.State = TraineeLessonState.Rated;
                await _databaseTraineeLessonRepository.UpdateAsync(traineeLesson);

                // Check if Feedback already exists
                var existingFeedbacks = await _databaseFeedbackRepository
                    .GetAllFeedbacksWrittenByUserWithLessonAndAuthorAndReadByUsersAsync(user);

                if (existingFeedbacks.Any(f => f.LessonId == lesson.LessonId))
                    continue;

                // Create Feedback Objekt
                var feedback = new Feedback {
                    Difficulty = difficulty,
                    HoursOfEffort = effort,
                    PreviousKnowledge = previousKnowledge,
                    Comment = comment,
                    LessonId = lesson.LessonId,
                    Lesson = lesson,
                    AuthorId = user.Id,
                    Author = user,
                    ReadByUsers = new List<ApplicationUser> { reader }
                };

                // Store in DB
                await _databaseFeedbackRepository.CreateAsync(feedback);


                var logEntry = new TraineeLessonLogEntry {
                    TraineeLessonId = traineeLesson.TraineeLessonId,
                    LessonName = lesson.Title,
                    UserId = user.Id,
                    UserName = user.Email!, 
                    OldState = "Accepted",
                    NewState = "Rated",
                    Timestamp = DateTime.UtcNow
                };

                _databaseTraineeLessonLogEntryRepository.Create(logEntry);
            }
        }

    }

}