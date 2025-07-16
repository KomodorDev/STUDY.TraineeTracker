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
                await _adminService.CreateUserAsync(dto);

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
            var feedbackSeeds = new List<(ApplicationUser? user, Lesson? lesson, int difficulty, float effort, string previousKnowledge, string? comment)> {

                // Alex - WebDev
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(1), 9, 1.7f, "Prior Java knowledge", "Felt mostly like review, but a few nuances were new."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(2), 9, 2.2f, "Comfortable with Java syntax", "Clear material, just a bit verbose."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(3), 8, 2.0f, "Familiar with core concepts", "Good structure, helpful hands-on tasks."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(4), 5, 2.5f, "Decent background", "Challenging parts popped up unexpectedly."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(5), 7, 2.2f, "Used Java in past projects", "Enjoyed the progression, though a bit long."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(6), 9, 2.8f, "Advanced Java knowledge", "Still managed to learn some new tricks."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(7), 7, 1.8f, "Basic backend experience", "Quick to implement, examples were useful."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(8), 8, 2.6f, "Some academic exposure", "Tutorials helped clarify key ideas."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(9), 9, 2.8f, "Built Java apps before", "Took some time but solidified fundamentals."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(10), 4, 2.6f, "Seen this in lectures", "Needed to look things up, but manageable."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(11), 7, 2.3f, "Basic Java background", "Exercises made the lesson click."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(12), 4, 2.9f, "Some Java experience", "Ran into minor issues, but figured it out."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(13), 8, 2.1f, "Comfortable with the topic", "Mostly reinforcing things I already knew."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(14), 8, 1.4f, "Worked with similar topics", "Breezed through most of it."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(15), 4, 2.0f, "Familiar from uni", "Good recap, not too deep."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(16), 8, 2.9f, "Practical knowledge", "Took longer than expected but worth it."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(17), 5, 1.6f, "Java is not new to me", "Smooth process, clear goals."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(18), 9, 1.7f, "Used this in side-projects", "Was already comfortable with most tasks."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(19), 4, 2.1f, "Attended similar lectures", "Could’ve skipped a few parts, but still helpful."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(20), 7, 1.6f, "Experienced with basics", "No surprises here – steady pace."),


                // Stefan - DevOps
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(41), 9, 2.5f, "Solid understanding beforehand", "Most of the content was familiar, but the structure helped reinforce it."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(42), 3, 2.4f, "Introductory level", "Nice and easy to go through, not much new for me."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(43), 8, 2.6f, "Advanced concepts reviewed", "I remembered a lot as I worked through it, still useful."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(44), 5, 2.1f, "Some parts were repetitive", "It went quickly because I had done similar tasks before."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(45), 3, 2.0f, "Very familiar topic", "Skimmed most of it, only reviewed one new concept."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(46), 3, 2.1f, "No major challenges", "Just revisited a few key ideas, rest was known."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(47), 4, 2.2f, "Refresher material", "Efficient to work through, good confidence boost."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(48), 6, 2.5f, "Light review", "Felt familiar, but a few details were nice to recall."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(49), 4, 2.2f, "Covered in uni already", "I just brushed up on the terms."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(50), 9, 1.3f, "Expert-level familiarity", "Breezed through – I could do this lesson blind."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(51), 8, 2.8f, "Expected difficulty", "No surprises, but well presented."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(52), 8, 1.4f, "Reinforced known material", "Quick and efficient to complete."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(53), 3, 1.8f, "All known content", "I used it more as a self-check."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(54), 9, 2.0f, "Already implemented similar things", "I remembered most from previous projects."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(55), 6, 2.5f, "Clear and familiar", "I went through it smoothly, good review."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(56), 4, 2.3f, "Mildly engaging", "Just needed to remind myself of a few terms."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(57), 8, 2.1f, "Well-known process", "The structure helped reinforce a few edge cases."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(58), 9, 2.2f, "Deeply familiar topic", "I’ve done this several times already."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(59), 8, 1.5f, "Previously practiced", "Didn’t take much time – was confident going in."),
                (stefan, await _databaseLessonRepository.GetLessonByIdAsync(60), 7, 2.1f, "Clear from prior work", "Some overlap with earlier"),

                // Ursula - DevOps
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(41), 5, 2.8f, "Some parts were challenging", "It required careful reading, but I got through it."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(42), 6, 3.1f, "Familiar with the basics", "A good refresher with some deeper insights."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(43), 7, 2.5f, "Well-structured content", "Easy to follow and very practical."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(44), 4, 2.0f, "Clear explanations", "I appreciated the examples throughout."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(45), 5, 2.7f, "Moderate difficulty", "Not too hard, not too easy."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(46), 6, 3.0f, "Required focus", "Some tricky parts, but doable."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(47), 3, 1.8f, "Pretty straightforward", "A smooth experience overall."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(48), 7, 2.9f, "Good balance", "The tasks matched the theory well."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(49), 6, 2.3f, "Solid content", "I liked how it built on earlier lessons."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(50), 8, 3.2f, "Quite demanding", "I had to revisit parts to fully understand."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(51), 9, 3.5f, "Challenging but rewarding", "I learned a lot from this one."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(52), 4, 2.1f, "Basic concepts", "Mostly review, but still useful."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(53), 5, 2.6f, "Well-paced", "Good level of depth for a single lesson."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(54), 6, 3.0f, "Engaging material", "I enjoyed working through it."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(55), 7, 2.7f, "Interesting topic", "Real-world relevance made it easier."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(56), 5, 2.4f, "Decent complexity", "Challenging enough to stay focused."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(57), 6, 2.8f, "Hands-on content", "The exercises helped solidify things."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(58), 7, 3.1f, "Required some research", "I had to look up a few things."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(59), 8, 3.3f, "Advanced content", "Took longer than expected."),
                (ursula, await _databaseLessonRepository.GetLessonByIdAsync(60), 6, 2.5f, "Good balance of theory and practice", "Felt productive afterward.")

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