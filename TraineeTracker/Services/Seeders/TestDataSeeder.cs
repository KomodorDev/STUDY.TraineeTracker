using Microsoft.AspNetCore.Identity;
using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Data.Lessons;
using TraineeTracker.Data.TeachingPlans;
using TraineeTracker.Models.Domain;
using TraineeTracker.Models.Dtos;
using TraineeTracker.Services.Admin;

namespace TraineeTracker.Services.Seeders {
    public class TestDataSeeder {

        private readonly AdminService _adminService;
        private readonly IApplicationUserRepository _databaseApplicationUserRepository;
        private readonly ILessonRepository _databaseLessonRepository;
        private readonly ITeachingPlanRepository _databaseTeachingPlanRepository;

        // ---------------------------------------------------
        public TestDataSeeder(IApplicationUserRepository databaseApplicationUserRepository, ILessonRepository lessonRepo, ITeachingPlanRepository teachingPlanRepo, AdminService adminService) {
            _databaseApplicationUserRepository = databaseApplicationUserRepository;
            _databaseLessonRepository = lessonRepo;
            _databaseTeachingPlanRepository = teachingPlanRepo;
            _adminService = adminService;
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
                    LessonIds = new[] { 1, 2 }
                },
                new {
                    Id = 2,
                    Name = "DevOps",
                    LessonIds = new[] { 1, 2, 3, 4 }
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
                    Password = "Sopro.2025"
                },
                new ApplicationUserDto {
                    Email = "nikita.stefan@uni-a.de",
                    Role = "Trainee",
                    TeachingPlanId = 2,
                    Password = "Sopro.2025"
                }
                new ApplicationUserDto {
                    Email = "closed.trainee@uni-a.de",
                    Role = "Trainee",
                    TeachingPlanId = 2,
                    Password = "Sopro.2025"
                }
            };

            foreach (var dto in userData) {
                await _adminService.CreateUserAsync(dto);
            }
        }
    }

}