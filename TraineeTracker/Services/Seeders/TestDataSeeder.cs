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
            var lessons = new[] {
                new Lesson { LessonId = 1, Title = "Lesson 1: WebDev Topic 1 (Only WebDev)", EstimatedEffort = 2.0, LinkUrl = "https://learn.microsoft.com/de-de/dotnet/csharp/fundamentals/object-oriented/" },
                new Lesson { LessonId = 2, Title = "Lesson 2: WebDev Topic 2 (Only WebDev)", EstimatedEffort = 2.5, LinkUrl = "https://learn.microsoft.com/de-de/ef/core/" },
                new Lesson { LessonId = 3, Title = "Lesson 3: WebDev Topic 3 (Only WebDev)", EstimatedEffort = 3.0, LinkUrl = "https://learn.microsoft.com/de-de/aspnet/core/mvc/" },
                new Lesson { LessonId = 4, Title = "Lesson 4: WebDev Topic 4 (Only WebDev)", EstimatedEffort = 1.5, LinkUrl = "https://learn.microsoft.com/de-de/dotnet/csharp/" },
                new Lesson { LessonId = 5, Title = "Lesson 5: WebDev Topic 5 (Only WebDev)", EstimatedEffort = 2.0, LinkUrl = "https://learn.microsoft.com/de-de/dotnet/csharp/fundamentals/object-oriented/" },
                new Lesson { LessonId = 6, Title = "Lesson 6: WebDev Topic 6 (Only WebDev)", EstimatedEffort = 2.5, LinkUrl = "https://learn.microsoft.com/de-de/ef/core/" },
                new Lesson { LessonId = 7, Title = "Lesson 7: WebDev Topic 7 (Only WebDev)", EstimatedEffort = 3.0, LinkUrl = "https://learn.microsoft.com/de-de/aspnet/core/mvc/" },
                new Lesson { LessonId = 8, Title = "Lesson 8: WebDev Topic 8 (Only WebDev)", EstimatedEffort = 1.5, LinkUrl = "https://learn.microsoft.com/de-de/dotnet/csharp/" },
                new Lesson { LessonId = 9, Title = "Lesson 9: WebDev Topic 9 (Only WebDev)", EstimatedEffort = 2.0, LinkUrl = "https://learn.microsoft.com/de-de/dotnet/csharp/fundamentals/object-oriented/" },
                new Lesson { LessonId = 10, Title = "Lesson 10: WebDev Topic 10 (Only WebDev)", EstimatedEffort = 2.5, LinkUrl = "https://learn.microsoft.com/de-de/ef/core/" },
                new Lesson { LessonId = 11, Title = "Lesson 11: WebDev Topic 11 (Only WebDev)", EstimatedEffort = 3.0, LinkUrl = "https://learn.microsoft.com/de-de/aspnet/core/mvc/" },
                new Lesson { LessonId = 12, Title = "Lesson 12: WebDev Topic 12 (Only WebDev)", EstimatedEffort = 1.5, LinkUrl = "https://learn.microsoft.com/de-de/dotnet/csharp/" },
                new Lesson { LessonId = 13, Title = "Lesson 13: WebDev Topic 13 (Only WebDev)", EstimatedEffort = 2.0, LinkUrl = "https://learn.microsoft.com/de-de/dotnet/csharp/fundamentals/object-oriented/" },
                new Lesson { LessonId = 14, Title = "Lesson 14: WebDev Topic 14 (Only WebDev)", EstimatedEffort = 2.5, LinkUrl = "https://learn.microsoft.com/de-de/ef/core/" },
                new Lesson { LessonId = 15, Title = "Lesson 15: WebDev Topic 15 (Only WebDev)", EstimatedEffort = 3.0, LinkUrl = "https://learn.microsoft.com/de-de/aspnet/core/mvc/" },
                new Lesson { LessonId = 16, Title = "Lesson 16: WebDev Topic 16 (Only WebDev)", EstimatedEffort = 1.5, LinkUrl = "https://learn.microsoft.com/de-de/dotnet/csharp/" },
                new Lesson { LessonId = 17, Title = "Lesson 17: WebDev Topic 17 (Only WebDev)", EstimatedEffort = 2.0, LinkUrl = "https://learn.microsoft.com/de-de/dotnet/csharp/fundamentals/object-oriented/" },
                new Lesson { LessonId = 18, Title = "Lesson 18: WebDev Topic 18 (Only WebDev)", EstimatedEffort = 2.5, LinkUrl = "https://learn.microsoft.com/de-de/ef/core/" },
                new Lesson { LessonId = 19, Title = "Lesson 19: WebDev Topic 19 (Only WebDev)", EstimatedEffort = 3.0, LinkUrl = "https://learn.microsoft.com/de-de/aspnet/core/mvc/" },
                new Lesson { LessonId = 20, Title = "Lesson 20: WebDev Topic 20 (Only WebDev)", EstimatedEffort = 1.5, LinkUrl = "https://learn.microsoft.com/de-de/dotnet/csharp/" },
                new Lesson { LessonId = 21, Title = "Lesson 21: WebDev Topic 21 (Only WebDev)", EstimatedEffort = 2.0, LinkUrl = "https://learn.microsoft.com/de-de/dotnet/csharp/fundamentals/object-oriented/" },
                new Lesson { LessonId = 22, Title = "Lesson 22: WebDev Topic 22 (Only WebDev)", EstimatedEffort = 2.5, LinkUrl = "https://learn.microsoft.com/de-de/ef/core/" },
                new Lesson { LessonId = 23, Title = "Lesson 23: WebDev Topic 23 (Only WebDev)", EstimatedEffort = 3.0, LinkUrl = "https://learn.microsoft.com/de-de/aspnet/core/mvc/" },
                new Lesson { LessonId = 24, Title = "Lesson 24: WebDev Topic 24 (Only WebDev)", EstimatedEffort = 1.5, LinkUrl = "https://learn.microsoft.com/de-de/dotnet/csharp/" },
                new Lesson { LessonId = 25, Title = "Lesson 25: WebDev Topic 25 (Only WebDev)", EstimatedEffort = 2.0, LinkUrl = "https://learn.microsoft.com/de-de/dotnet/csharp/fundamentals/object-oriented/" },
                new Lesson { LessonId = 26, Title = "Lesson 26: WebDev Topic 26 (Only WebDev)", EstimatedEffort = 2.5, LinkUrl = "https://learn.microsoft.com/de-de/ef/core/" },
                new Lesson { LessonId = 27, Title = "Lesson 27: WebDev Topic 27 (Only WebDev)", EstimatedEffort = 3.0, LinkUrl = "https://learn.microsoft.com/de-de/aspnet/core/mvc/" },
                new Lesson { LessonId = 28, Title = "Lesson 28: WebDev Topic 28 (Only WebDev)", EstimatedEffort = 1.5, LinkUrl = "https://learn.microsoft.com/de-de/dotnet/csharp/" },
                new Lesson { LessonId = 29, Title = "Lesson 29: WebDev Topic 29 (Only WebDev)", EstimatedEffort = 2.0, LinkUrl = "https://learn.microsoft.com/de-de/dotnet/csharp/fundamentals/object-oriented/" },
                new Lesson { LessonId = 30, Title = "Lesson 30: WebDev Topic 30 (Only WebDev)", EstimatedEffort = 2.5, LinkUrl = "https://learn.microsoft.com/de-de/ef/core/" },
                new Lesson { LessonId = 31, Title = "Lesson 31: WebDev Topic 31 (Only WebDev)", EstimatedEffort = 3.0, LinkUrl = "https://learn.microsoft.com/de-de/aspnet/core/mvc/" },
                new Lesson { LessonId = 32, Title = "Lesson 32: WebDev Topic 32 (Only WebDev)", EstimatedEffort = 1.5, LinkUrl = "https://learn.microsoft.com/de-de/dotnet/csharp/" },
                new Lesson { LessonId = 33, Title = "Lesson 33: WebDev Topic 33 (Only WebDev)", EstimatedEffort = 2.0, LinkUrl = "https://learn.microsoft.com/de-de/dotnet/csharp/fundamentals/object-oriented/" },
                new Lesson { LessonId = 34, Title = "Lesson 34: WebDev Topic 34 (Only WebDev)", EstimatedEffort = 2.5, LinkUrl = "https://learn.microsoft.com/de-de/ef/core/" },
                new Lesson { LessonId = 35, Title = "Lesson 35: WebDev Topic 35 (Only WebDev)", EstimatedEffort = 3.0, LinkUrl = "https://learn.microsoft.com/de-de/aspnet/core/mvc/" },
                new Lesson { LessonId = 36, Title = "Lesson 36: WebDev Topic 36 (Only WebDev)", EstimatedEffort = 1.5, LinkUrl = "https://learn.microsoft.com/de-de/dotnet/csharp/" },
                new Lesson { LessonId = 37, Title = "Lesson 37: WebDev Topic 37 (Only WebDev)", EstimatedEffort = 2.0, LinkUrl = "https://learn.microsoft.com/de-de/dotnet/csharp/fundamentals/object-oriented/" },
                new Lesson { LessonId = 38, Title = "Lesson 38: WebDev Topic 38 (Only WebDev)", EstimatedEffort = 2.5, LinkUrl = "https://learn.microsoft.com/de-de/ef/core/" },
                new Lesson { LessonId = 39, Title = "Lesson 39: WebDev Topic 39 (Only WebDev)", EstimatedEffort = 3.0, LinkUrl = "https://learn.microsoft.com/de-de/aspnet/core/mvc/" },
                new Lesson { LessonId = 40, Title = "Lesson 40: WebDev Topic 40 (Only WebDev)", EstimatedEffort = 1.5, LinkUrl = "https://learn.microsoft.com/de-de/dotnet/csharp/" },
                new Lesson { LessonId = 41, Title = "Lesson 41: DevOps Topic 41 (Only DevOps)", EstimatedEffort = 2.5, LinkUrl = "https://learn.microsoft.com/de-de/powershell/" },
                new Lesson { LessonId = 42, Title = "Lesson 42: DevOps Topic 42 (Only DevOps)", EstimatedEffort = 3.0, LinkUrl = "https://learn.microsoft.com/de-de/windows-server/" },
                new Lesson { LessonId = 43, Title = "Lesson 43: DevOps Topic 43 (Only DevOps)", EstimatedEffort = 3.5, LinkUrl = "https://learn.microsoft.com/de-de/docker/" },
                new Lesson { LessonId = 44, Title = "Lesson 44: DevOps Topic 44 (Only DevOps)", EstimatedEffort = 4.0, LinkUrl = "https://learn.microsoft.com/de-de/azure/devops/" },
                new Lesson { LessonId = 45, Title = "Lesson 45: DevOps Topic 45 (Only DevOps)", EstimatedEffort = 2.0, LinkUrl = "https://learn.microsoft.com/de-de/powershell/" },
                new Lesson { LessonId = 46, Title = "Lesson 46: DevOps Topic 46 (Only DevOps)", EstimatedEffort = 2.5, LinkUrl = "https://learn.microsoft.com/de-de/windows-server/" },
                new Lesson { LessonId = 47, Title = "Lesson 47: DevOps Topic 47 (Only DevOps)", EstimatedEffort = 3.0, LinkUrl = "https://learn.microsoft.com/de-de/docker/" },
                new Lesson { LessonId = 48, Title = "Lesson 48: DevOps Topic 48 (Only DevOps)", EstimatedEffort = 3.5, LinkUrl = "https://learn.microsoft.com/de-de/azure/devops/" },
                new Lesson { LessonId = 49, Title = "Lesson 49: DevOps Topic 49 (Only DevOps)", EstimatedEffort = 4.0, LinkUrl = "https://learn.microsoft.com/de-de/powershell/" },
                new Lesson { LessonId = 50, Title = "Lesson 50: DevOps Topic 50 (Only DevOps)", EstimatedEffort = 2.0, LinkUrl = "https://learn.microsoft.com/de-de/windows-server/" },
                new Lesson { LessonId = 51, Title = "Lesson 51: DevOps Topic 51 (Only DevOps)", EstimatedEffort = 2.5, LinkUrl = "https://learn.microsoft.com/de-de/docker/" },
                new Lesson { LessonId = 52, Title = "Lesson 52: DevOps Topic 52 (Only DevOps)", EstimatedEffort = 3.0, LinkUrl = "https://learn.microsoft.com/de-de/azure/devops/" },
                new Lesson { LessonId = 53, Title = "Lesson 53: DevOps Topic 53 (Only DevOps)", EstimatedEffort = 3.5, LinkUrl = "https://learn.microsoft.com/de-de/powershell/" },
                new Lesson { LessonId = 54, Title = "Lesson 54: DevOps Topic 54 (Only DevOps)", EstimatedEffort = 4.0, LinkUrl = "https://learn.microsoft.com/de-de/windows-server/" },
                new Lesson { LessonId = 55, Title = "Lesson 55: DevOps Topic 55 (Only DevOps)", EstimatedEffort = 2.0, LinkUrl = "https://learn.microsoft.com/de-de/docker/" },
                new Lesson { LessonId = 56, Title = "Lesson 56: DevOps Topic 56 (Only DevOps)", EstimatedEffort = 2.5, LinkUrl = "https://learn.microsoft.com/de-de/azure/devops/" },
                new Lesson { LessonId = 57, Title = "Lesson 57: DevOps Topic 57 (Only DevOps)", EstimatedEffort = 3.0, LinkUrl = "https://learn.microsoft.com/de-de/powershell/" },
                new Lesson { LessonId = 58, Title = "Lesson 58: DevOps Topic 58 (Only DevOps)", EstimatedEffort = 3.5, LinkUrl = "https://learn.microsoft.com/de-de/windows-server/" },
                new Lesson { LessonId = 59, Title = "Lesson 59: DevOps Topic 59 (Only DevOps)", EstimatedEffort = 4.0, LinkUrl = "https://learn.microsoft.com/de-de/docker/" },
                new Lesson { LessonId = 60, Title = "Lesson 60: DevOps Topic 60 (Only DevOps)", EstimatedEffort = 2.0, LinkUrl = "https://learn.microsoft.com/de-de/azure/devops/" },
                new Lesson { LessonId = 61, Title = "Lesson 61: DevOps Topic 61 (Only DevOps)", EstimatedEffort = 2.5, LinkUrl = "https://learn.microsoft.com/de-de/powershell/" },
                new Lesson { LessonId = 62, Title = "Lesson 62: DevOps Topic 62 (Only DevOps)", EstimatedEffort = 3.0, LinkUrl = "https://learn.microsoft.com/de-de/windows-server/" },
                new Lesson { LessonId = 63, Title = "Lesson 63: DevOps Topic 63 (Only DevOps)", EstimatedEffort = 3.5, LinkUrl = "https://learn.microsoft.com/de-de/docker/" },
                new Lesson { LessonId = 64, Title = "Lesson 64: DevOps Topic 64 (Only DevOps)", EstimatedEffort = 4.0, LinkUrl = "https://learn.microsoft.com/de-de/azure/devops/" },
                new Lesson { LessonId = 65, Title = "Lesson 65: DevOps Topic 65 (Only DevOps)", EstimatedEffort = 2.0, LinkUrl = "https://learn.microsoft.com/de-de/powershell/" },
                new Lesson { LessonId = 66, Title = "Lesson 66: DevOps Topic 66 (Only DevOps)", EstimatedEffort = 2.5, LinkUrl = "https://learn.microsoft.com/de-de/windows-server/" },
                new Lesson { LessonId = 67, Title = "Lesson 67: DevOps Topic 67 (Only DevOps)", EstimatedEffort = 3.0, LinkUrl = "https://learn.microsoft.com/de-de/docker/" },
                new Lesson { LessonId = 68, Title = "Lesson 68: DevOps Topic 68 (Only DevOps)", EstimatedEffort = 3.5, LinkUrl = "https://learn.microsoft.com/de-de/azure/devops/" },
                new Lesson { LessonId = 69, Title = "Lesson 69: DevOps Topic 69 (Only DevOps)", EstimatedEffort = 4.0, LinkUrl = "https://learn.microsoft.com/de-de/powershell/" },
                new Lesson { LessonId = 70, Title = "Lesson 70: DevOps Topic 70 (Only DevOps)", EstimatedEffort = 2.0, LinkUrl = "https://learn.microsoft.com/de-de/windows-server/" },
                new Lesson { LessonId = 71, Title = "Lesson 71: DevOps Topic 71 (Only DevOps)", EstimatedEffort = 2.5, LinkUrl = "https://learn.microsoft.com/de-de/docker/" },
                new Lesson { LessonId = 72, Title = "Lesson 72: DevOps Topic 72 (Only DevOps)", EstimatedEffort = 3.0, LinkUrl = "https://learn.microsoft.com/de-de/azure/devops/" },
                new Lesson { LessonId = 73, Title = "Lesson 73: DevOps Topic 73 (Only DevOps)", EstimatedEffort = 3.5, LinkUrl = "https://learn.microsoft.com/de-de/powershell/" },
                new Lesson { LessonId = 74, Title = "Lesson 74: DevOps Topic 74 (Only DevOps)", EstimatedEffort = 4.0, LinkUrl = "https://learn.microsoft.com/de-de/windows-server/" },
                new Lesson { LessonId = 75, Title = "Lesson 75: DevOps Topic 75 (Only DevOps)", EstimatedEffort = 2.0, LinkUrl = "https://learn.microsoft.com/de-de/docker/" },
                new Lesson { LessonId = 76, Title = "Lesson 76: DevOps Topic 76 (Only DevOps)", EstimatedEffort = 2.5, LinkUrl = "https://learn.microsoft.com/de-de/azure/devops/" },
                new Lesson { LessonId = 77, Title = "Lesson 77: DevOps Topic 77 (Only DevOps)", EstimatedEffort = 3.0, LinkUrl = "https://learn.microsoft.com/de-de/powershell/" },
                new Lesson { LessonId = 78, Title = "Lesson 78: DevOps Topic 78 (Only DevOps)", EstimatedEffort = 3.5, LinkUrl = "https://learn.microsoft.com/de-de/windows-server/" },
                new Lesson { LessonId = 79, Title = "Lesson 79: DevOps Topic 79 (Only DevOps)", EstimatedEffort = 4.0, LinkUrl = "https://learn.microsoft.com/de-de/docker/" },
                new Lesson { LessonId = 80, Title = "Lesson 80: DevOps Topic 80 (Only DevOps)", EstimatedEffort = 2.0, LinkUrl = "https://learn.microsoft.com/de-de/azure/devops/" }
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
                    LessonIds = Enumerable.Range(1, 40).ToArray()
                },
                new {
                    Id = 2,
                    Name = "DevOps",
                    LessonIds = Enumerable.Range(41, 80).ToArray()
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
                    Email = "paul.schweizer@uni-a.de",
                    Role = "Mentor",
                    Password = "Sopro.2025"
                },
                new ApplicationUserDto {
                    Email = "alexander.schlemmer@uni-a.de",
                    Role = "Mentor",
                    Password = "Sopro.2025"
                },

                // ++++++++++++++++
                // Trainee
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

                // ++++++++++++++++
            };

            foreach (var dto in userData) {
                await _adminService.CreateUserAsync(dto);

                if (dto.Email == "closed.trainee@uni-a.de") {
                    var user = await _databaseApplicationUserRepository.FindByEmailAsync(dto.Email);
                    if (user != null) {
                        await _adminService.CloseUserAsync(user.Id);
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

        }

        // ---------------------------------------------------
        public async Task SeedFeedbackAsync() {

            // Get Trainees
            var alex = await _databaseApplicationUserRepository.FindByEmailAsync("alexandros.blask@uni-a.de");
            var stefan = await _databaseApplicationUserRepository.FindByEmailAsync("stefan.schnupfen@makandra.de");
            var ursula = await _databaseApplicationUserRepository.FindByEmailAsync("ursula.urlaub@makandra.de");
            // Get Mentor and Admin who read it
            var simon = await _databaseApplicationUserRepository.FindByEmailAsync("simon.hinterreiter@uni-a.de"); // Admin
            var paul = await _databaseApplicationUserRepository.FindByEmailAsync("paul.schweizer@uni-a.de");     // Mentor

            // FeedbackSeeds
            var feedbackSeeds = new List<(ApplicationUser? user, Lesson? lesson, int difficulty, float effort, string previousKnowledge, string? comment)> {
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