using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using TraineeTracker.Data.Lessons;
using TraineeTracker.Data.TeachingPlans;
using TraineeTracker.Models.Domain;
using TraineeTracker.Models.Dtos;
using TraineeTracker.Services;
using Xunit;

namespace TraineeTracker.UnitTests.TeachingPlanTest
{
    public class TeachingPlanServiceTests
    {
        // Fake Repository für TeachingPlan
        private class FakeTeachingPlanRepository : ITeachingPlanRepository
        {
            public TeachingPlan? CreatedTeachingPlan;
            public bool CreateCalled = false;

            public Task CreateAsync(TeachingPlan teachingPlan)
            {
                CreatedTeachingPlan = teachingPlan;
                CreateCalled = true;
                return Task.CompletedTask;
            }

            // alle übrigen Methoden stubben
            public Task<bool> ExistsAsync(int id) => throw new NotImplementedException();
            public Task<bool> ExistsAsync(TeachingPlan teachingPlan) => throw new NotImplementedException();
            public Task UpdateAsync(TeachingPlan teachingPlan) => throw new NotImplementedException();
            public Task DeleteAsync(TeachingPlan teachingPlan) => throw new NotImplementedException();
            public Task<TeachingPlan?> GetTeachingPlanByIdAsync(int id) => throw new NotImplementedException();
            public Task<TeachingPlan?> GetTeachingPlanByIdWithLessonsAndTraineesAsync(int id) => throw new NotImplementedException();
            public Task<IEnumerable<TeachingPlan>> GetAllTeachingPlansAsync() => throw new NotImplementedException();
            public Task<IEnumerable<TeachingPlan>> GetAllTeachingPlansWithLessonsAndTraineesAsync() => throw new NotImplementedException();
        }

        // Fake Repository für Lesson
        private class FakeLessonRepository : ILessonRepository
        {
            public List<Lesson> CreatedLessons { get; } = new List<Lesson>();

            public Task CreateAsync(Lesson lesson)
            {
                CreatedLessons.Add(lesson);
                return Task.CompletedTask;
            }

            // alle übrigen Methoden stubben
            public Task<bool> ExistsAsync(int id) => throw new NotImplementedException();
            public Task<bool> ExistsAsync(string makandraId, int teachingPlanId) => throw new NotImplementedException();
            public Task UpdateAsync(Lesson lesson) => throw new NotImplementedException();
            public Task DeleteAsync(Lesson lesson) => throw new NotImplementedException();
            public Task<Lesson?> GetLessonByIdAsync(int id) => throw new NotImplementedException();
            public Task<IEnumerable<Lesson>> GetAllLessonsAsync() => throw new NotImplementedException();
            public Task<IEnumerable<Lesson>> GetAllLessonsWithFeedbacksAsync() => throw new NotImplementedException();
        }

        [Fact]
        public async Task ImportNewTeachingPlan_ShouldCreateTeachingPlanAndLessons()
        {
            // --- Arrange ---

            // 1) Erzeuge ein einzelnes LessonDto und serialisiere es
            var lessonDtos = new List<LessonDto>
            {
                new LessonDto
                {
                    Id = "L1",
                    Title = "Test Lesson",
                    Url = "http://example.com",
                    Estimate = 2.5,
                    Deprecated = false
                }
            };
            var json = JsonConvert.SerializeObject(lessonDtos);

            // 2) Packe das JSON in ein IFormFile
            var bytes = Encoding.UTF8.GetBytes(json);
            var stream = new MemoryStream(bytes);
            var formFile = new FormFile(stream, 0, bytes.Length, "file", "lessons.json");

            // 3) Baue das DTO für den Import
            var dto = new TeachingPlanDto
            {
                NewPlanFile = formFile,
                NewPlanName = "Mein neuer Plan"
            };

            // 4) Ersetze die Repos durch Fakes
            var teachingPlanRepo = new FakeTeachingPlanRepository();
            var lessonRepo = new FakeLessonRepository();

            // 5) Service instanziieren (die anderen Dependencies werden nicht in ImportNew genutzt)
            var service = new TeachingPlanService(
                teachingPlanRepo,
                lessonRepo,
                null!,      // ITraineeLessonRepository
                null!,      // IApplicationUserRepository
                null!       // EmailNotificationService
            );

            // --- Act ---
            await service.ImportNewTeachingPlan(dto);

            // --- Assert ---

            // 1) TeachingPlan wurde angelegt
            Assert.True(teachingPlanRepo.CreateCalled);
            Assert.NotNull(teachingPlanRepo.CreatedTeachingPlan);
            Assert.Equal("Mein neuer Plan", teachingPlanRepo.CreatedTeachingPlan!.Name);

            // 2) Genau ein Lesson wurde angelegt
            Assert.Single(lessonRepo.CreatedLessons);
            var created = lessonRepo.CreatedLessons[0];

            // Mapping-Checks
            Assert.Equal("L1", created.MakandraId);
            Assert.Equal("Test Lesson", created.Title);
            Assert.Equal("http://example.com", created.LinkUrl);
            Assert.Equal(2.5, created.EstimatedEffort, 3);
            Assert.False(created.IsInactive);
            Assert.Equal(1, created.SortingIndex);

            // TeachingPlanId wurde aus dem neu erstellten Plan übernommen (Default 0)
            Assert.Equal(teachingPlanRepo.CreatedTeachingPlan.TeachingPlanId, created.TeachingPlanId);
        }
    }
}
