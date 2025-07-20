using Microsoft.AspNetCore.Identity;
using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Data.Feedbacks;
using TraineeTracker.Data.Lessons;
using TraineeTracker.Data.TeachingPlans;
using TraineeTracker.Data.TraineeLessonLog;
using TraineeTracker.Data.TraineeLessons;
using TraineeTracker.Data.TraineeStatistics;
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

        private readonly TraineeStatisticsService _traineeStatisticsService;

        // ---------------------------------------------------
        public TestDataSeeder(IApplicationUserRepository databaseApplicationUserRepository, ILessonRepository lessonRepo, ITraineeLessonRepository traineeLessonRepo, ITeachingPlanRepository teachingPlanRepo, AdminService adminService, IFeedbackRepository feedbackRepo, ITraineeLessonLogEntryRepository logEntryRepo, TraineeStatisticsService statisticsService) {
            _databaseApplicationUserRepository = databaseApplicationUserRepository;
            _databaseLessonRepository = lessonRepo;
            _databaseTeachingPlanRepository = teachingPlanRepo;
            _adminService = adminService;
            _databaseTraineeLessonRepository = traineeLessonRepo;
            _databaseFeedbackRepository = feedbackRepo;
            _databaseTraineeLessonLogEntryRepository = logEntryRepo;
            _traineeStatisticsService = statisticsService;
        }


        // ---------------------------------------------------
        public async Task SeedLessonsAsync() {
            var lessons = new List<Lesson>();

            for (int i = 1; i <= 80; i++) {
                lessons.Add(new Lesson {
                    TeachingPlanId = i > 40 ? 2 : 1,
                    SortingIndex = i - 1, // 0-basiert
                    MakandraId = (200 + i).ToString(), // "201" bis "280"
                    Title = $"Lesson {i}: {(i <= 40 ? "WebDev" : "DevOps")} Topic {i} (Only {(i <= 40 ? "WebDev" : "DevOps")})",
                    EstimatedEffort = (i % 4) switch {
                        1 => 2.0,
                        2 => 2.5,
                        3 => 3.0,
                        _ => 1.5
                    },
                    LinkUrl = (i % 4) switch {
                        1 => "https://learn.microsoft.com/de-de/dotnet/csharp/fundamentals/object-oriented/",
                        2 => "https://learn.microsoft.com/de-de/ef/core/",
                        3 => "https://learn.microsoft.com/de-de/aspnet/core/mvc/",
                        _ => "https://learn.microsoft.com/de-de/dotnet/csharp/"
                    }
                });
            }


            foreach (var lesson in lessons) {
                bool alreadyExists = await _databaseLessonRepository
                    .ExistsAsync(lesson.MakandraId, lesson.TeachingPlanId);

                if (!alreadyExists) {
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
                    Name = "WebDevelopment"
                },
                new {
                    Id = 2,
                    Name = "DevOps"
                }
            };


            foreach (var plan in plans) {
                var existing = await _databaseTeachingPlanRepository.GetTeachingPlanByIdWithLessonsAndTraineesAsync(plan.Id);

                // Only add teachingPlan if not existing yet
                if (existing != null)
                    continue;

                var lessons = allLessons
                    .Where(l => l.TeachingPlanId == plan.Id)
                    .ToList();

                var teachingPlan = new TeachingPlan {
                    TeachingPlanId = plan.Id,
                    Name = plan.Name,
                    LastUpdated = DateTime.UtcNow,
                };

                await _databaseTeachingPlanRepository.CreateAsync(teachingPlan);
            }
        }

        // ---------------------------------------------------
        public async Task SeedUsersAsync() {

            var userData = new List<ApplicationUserDto> {

                // ++++++++++++++++
                // Admin
                new ApplicationUserDto {
                    Email = "simon.hinterreiter@uni-a.de",
                    Role = "Admin",
                    Password = "Sopro.2025"
                },

                // ++++++++++++++++
                // Mentor
                new ApplicationUserDto {
                    Email = "paul.schweizerTEST@uni-a.de",
                    Role = "Mentor",
                    Password = "Sopro.2025"
                },
                new ApplicationUserDto {
                    Email = "alexander.schlemmerTEST@uni-a.de",
                    Role = "Mentor",
                    Password = "Sopro.2025"
                },

                // ++++++++++++++++
                // Trainee
                new ApplicationUserDto {
                    Email = "alexandros.blaskTEST@uni-a.de",
                    Role = "Trainee",
                    TeachingPlanId = 1,
                    Password = "Sopro.2025",
                    TraineeStartDate = new DateOnly(2025, 4, 1),
                    TraineeEndDate = new DateOnly(2026, 7, 1)
                },

                new ApplicationUserDto {
                    Email = "nikita.stefanTEST@uni-a.de",
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
                    TraineeStartDate = new DateOnly(2025, 1, 15),
                    TraineeEndDate = new DateOnly(2025, 7, 30)
                },

                new ApplicationUserDto {
                    Email = "closed.traineeTEST@uni-a.de",
                    Role = "Trainee",
                    TeachingPlanId = 2,
                    Password = "Sopro.2025",
                    TraineeStartDate = new DateOnly(2024, 1, 1),
                    TraineeEndDate = new DateOnly(2025, 4, 1)
                }

                // ++++++++++++++++
            };

            foreach (var dto in userData) {
                await _adminService.CreateUserAsync(dto, true);

                if (dto.Email == "closed.traineeTEST@uni-a.de") {
                    var user = await _databaseApplicationUserRepository.FindByEmailAsync(dto.Email);
                    if (user != null) {
                        await _adminService.CloseUserAsync(user.Id);
                    }
                }
            }
        }

        // ---------------------------------------------------
        public async Task SeedProcessingPausesAsync() {
            var alex = await _databaseApplicationUserRepository.FindByEmailAsync("alexandros.blaskTEST@uni-a.de");
            var nikita = await _databaseApplicationUserRepository.FindByEmailAsync("nikita.stefanTEST@uni-a.de");
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

        }

        // ---------------------------------------------------
        public async Task SeedFeedbackAsync() {

            // Get Trainees
            var alex = await _databaseApplicationUserRepository.FindByEmailAsync("alexandros.blaskTEST@uni-a.de");
            var stefan = await _databaseApplicationUserRepository.FindByEmailAsync("stefan.schnupfen@makandra.de");
            var ursula = await _databaseApplicationUserRepository.FindByEmailAsync("ursula.urlaub@makandra.de");
            // Get Mentor and Admin who read it
            var simon = await _databaseApplicationUserRepository.FindByEmailAsync("simon.hinterreiter@uni-a.de"); // Admin
            var paul = await _databaseApplicationUserRepository.FindByEmailAsync("paul.schweizerTEST@uni-a.de");     // Mentor

            // FeedbackSeeds
            var feedbackSeeds = new List<(ApplicationUser? user, Lesson? lesson, LessonDifficulty difficulty, float effort, PreviousKnowledgeLevel previousKnowledge, string? comment)> {

                // Alex - WebDev
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(2), LessonDifficulty.VeryEasy, 2.2f, PreviousKnowledgeLevel.Advanced, "Clear material, just a bit verbose."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(3), LessonDifficulty.Easy, 2.0f, PreviousKnowledgeLevel.Basic, "Good structure, helpful hands-on tasks."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(4), LessonDifficulty.Hard, 2.5f, PreviousKnowledgeLevel.Intermediate, "Challenging parts popped up unexpectedly."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(5), LessonDifficulty.Medium, 2.2f, PreviousKnowledgeLevel.Intermediate, "Enjoyed the progression, though a bit long."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(6), LessonDifficulty.VeryEasy, 2.8f, PreviousKnowledgeLevel.Expert, "Still managed to learn some new tricks."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(7), LessonDifficulty.Medium, 1.8f, PreviousKnowledgeLevel.Basic, "Quick to implement, examples were useful."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(8), LessonDifficulty.Easy, 2.6f, PreviousKnowledgeLevel.Aware, "Tutorials helped clarify key ideas."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(9), LessonDifficulty.VeryEasy, 2.8f, PreviousKnowledgeLevel.Advanced, "Took some time but solidified fundamentals."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(10), LessonDifficulty.VeryHard, 2.6f, PreviousKnowledgeLevel.Basic, "Needed to look things up, but manageable."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(11), LessonDifficulty.Medium, 2.3f, PreviousKnowledgeLevel.Basic, "Exercises made the lesson click."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(12), LessonDifficulty.VeryHard, 2.9f, PreviousKnowledgeLevel.Intermediate, "Ran into minor issues, but figured it out."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(13), LessonDifficulty.Easy, 2.1f, PreviousKnowledgeLevel.Advanced, "Mostly reinforcing things I already knew."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(14), LessonDifficulty.Easy, 1.4f, PreviousKnowledgeLevel.Intermediate, "Breezed through most of it."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(15), LessonDifficulty.VeryHard, 2.0f, PreviousKnowledgeLevel.Basic, "Good recap, not too deep."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(16), LessonDifficulty.Easy, 2.9f, PreviousKnowledgeLevel.Intermediate, "Took longer than expected but worth it."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(17), LessonDifficulty.Hard, 1.6f, PreviousKnowledgeLevel.Advanced, "Smooth process, clear goals."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(18), LessonDifficulty.VeryEasy, 1.7f, PreviousKnowledgeLevel.Advanced, "Was already comfortable with most tasks."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(19), LessonDifficulty.VeryHard, 2.1f, PreviousKnowledgeLevel.Aware, "Could’ve skipped a few parts, but still helpful."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(20), LessonDifficulty.Medium, 1.6f, PreviousKnowledgeLevel.Basic, "No surprises here – steady pace."),

                // Stefan - DevOps
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(41), LessonDifficulty.VeryEasy, 2.5f, PreviousKnowledgeLevel.Advanced, "Most of the content was familiar, but the structure helped reinforce it."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(42), LessonDifficulty.VeryHard, 2.4f, PreviousKnowledgeLevel.Aware, "Nice and easy to go through, not much new for me."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(43), LessonDifficulty.Easy, 2.6f, PreviousKnowledgeLevel.Advanced, "I remembered a lot as I worked through it, still useful."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(44), LessonDifficulty.Hard, 2.1f, PreviousKnowledgeLevel.Intermediate, "It went quickly because I had done similar tasks before."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(45), LessonDifficulty.VeryHard, 2.0f, PreviousKnowledgeLevel.Expert, "Skimmed most of it, only reviewed one new concept."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(46), LessonDifficulty.VeryHard, 2.1f, PreviousKnowledgeLevel.Advanced, "Just revisited a few key ideas, rest was known."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(47), LessonDifficulty.VeryHard, 2.2f, PreviousKnowledgeLevel.Intermediate, "Efficient to work through, good confidence boost."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(48), LessonDifficulty.Medium, 2.5f, PreviousKnowledgeLevel.Basic, "Felt familiar, but a few details were nice to recall."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(49), LessonDifficulty.VeryHard, 2.2f, PreviousKnowledgeLevel.Aware, "I just brushed up on the terms."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(50), LessonDifficulty.VeryEasy, 1.3f, PreviousKnowledgeLevel.Expert, "Breezed through – I could do this lesson blind."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(51), LessonDifficulty.Easy, 2.8f, PreviousKnowledgeLevel.Intermediate, "No surprises, but well presented."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(52), LessonDifficulty.Easy, 1.4f, PreviousKnowledgeLevel.Intermediate, "Quick and efficient to complete."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(53), LessonDifficulty.VeryHard, 1.8f, PreviousKnowledgeLevel.Advanced, "I used it more as a self-check."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(54), LessonDifficulty.VeryEasy, 2.0f, PreviousKnowledgeLevel.Advanced, "I remembered most from previous projects."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(55), LessonDifficulty.Medium, 2.5f, PreviousKnowledgeLevel.Intermediate, "I went through it smoothly, good review."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(56), LessonDifficulty.VeryHard, 2.3f, PreviousKnowledgeLevel.Basic, "Just needed to remind myself of a few terms."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(57), LessonDifficulty.Easy, 2.1f, PreviousKnowledgeLevel.Intermediate, "The structure helped reinforce a few edge cases."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(58), LessonDifficulty.VeryEasy, 2.2f, PreviousKnowledgeLevel.Expert, "I’ve done this several times already."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(59), LessonDifficulty.Easy, 1.5f, PreviousKnowledgeLevel.Advanced, "Didn’t take much time – was confident going in."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(60), LessonDifficulty.Medium, 2.1f, PreviousKnowledgeLevel.Intermediate, "Some overlap with earlier"),

                // Ursula - DevOps
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(41), LessonDifficulty.Hard, 2.8f, PreviousKnowledgeLevel.Intermediate, "It required careful reading, but I got through it."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(42), LessonDifficulty.Medium, 3.1f, PreviousKnowledgeLevel.Basic, "A good refresher with some deeper insights."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(43), LessonDifficulty.Medium, 2.5f, PreviousKnowledgeLevel.Intermediate, "Easy to follow and very practical."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(44), LessonDifficulty.VeryHard, 2.0f, PreviousKnowledgeLevel.Basic, "I appreciated the examples throughout."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(45), LessonDifficulty.Hard, 2.7f, PreviousKnowledgeLevel.Intermediate, "Not too hard, not too easy."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(46), LessonDifficulty.Medium, 3.0f, PreviousKnowledgeLevel.Intermediate, "Some tricky parts, but doable."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(47), LessonDifficulty.Insane, 1.8f, PreviousKnowledgeLevel.Aware, "A smooth experience overall."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(48), LessonDifficulty.Medium, 2.9f, PreviousKnowledgeLevel.Intermediate, "The tasks matched the theory well."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(49), LessonDifficulty.Medium, 2.3f, PreviousKnowledgeLevel.Intermediate, "I liked how it built on earlier lessons."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(50), LessonDifficulty.Hard, 3.2f, PreviousKnowledgeLevel.Advanced, "I had to revisit parts to fully understand."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(51), LessonDifficulty.VeryHard, 3.5f, PreviousKnowledgeLevel.Advanced, "I learned a lot from this one."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(52), LessonDifficulty.VeryHard, 2.1f, PreviousKnowledgeLevel.Basic, "Mostly review, but still useful."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(53), LessonDifficulty.Hard, 2.6f, PreviousKnowledgeLevel.Intermediate, "Good level of depth for a single lesson."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(54), LessonDifficulty.Medium, 3.0f, PreviousKnowledgeLevel.Intermediate, "I enjoyed working through it."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(55), LessonDifficulty.Medium, 2.7f, PreviousKnowledgeLevel.Intermediate, "Real-world relevance made it easier."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(56), LessonDifficulty.Hard, 2.4f, PreviousKnowledgeLevel.Intermediate, "Challenging enough to stay focused."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(57), LessonDifficulty.Medium, 2.8f, PreviousKnowledgeLevel.Intermediate, "The exercises helped solidify things."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(58), LessonDifficulty.Medium, 3.1f, PreviousKnowledgeLevel.Intermediate, "I had to look up a few things."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(59), LessonDifficulty.Hard, 3.3f, PreviousKnowledgeLevel.Advanced, "Took longer than expected."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(60), LessonDifficulty.Medium, 2.5f, PreviousKnowledgeLevel.Intermediate, "Felt productive afterward.")
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

                // Get the correct TraineeLesson
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

        // ---------------------------------------------------
        public async Task SeedTraineeStatisticsSnapshotAsync() {
            var stefan = await _databaseApplicationUserRepository.FindByEmailAsync("stefan.schnupfen@makandra.de");
            var ursula = await _databaseApplicationUserRepository.FindByEmailAsync("ursula.urlaub@makandra.de");

            // Wenn garantiert nicht null, dann direkt:
            var trainees = new[] { stefan, ursula };

            foreach (var trainee in trainees) {

                await _traineeStatisticsService.BuildLatestTraineeStatisticsSnapshotAsync(trainee!.Id);
                Console.WriteLine($"✅ Snapshot created/updated for {trainee.Email}");
            }
        }

        // ---------------------------------------------------

    }

}