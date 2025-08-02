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

namespace TraineeTracker.UnitTests.TeachingPlanTest{

    // ------------------------------------------------------
    /// <remarks>
    /// Code Ownership: Alexandros Blask
    /// </remarks>
    public class TeachingPlanServiceTests{

        // ------------------------------------------------------
        // Fake Repository for TeachingPlan
        private class FakeTeachingPlanRepository : ITeachingPlanRepository{

            public TeachingPlan? CreatedTeachingPlan;
            public bool CreateCalled = false;

            // ------------------------------------------------------
            public Task CreateAsync(TeachingPlan teachingPlan){
                CreatedTeachingPlan = teachingPlan;
                CreateCalled = true;
                return Task.CompletedTask;
            }

            // ------------------------------------------------------
            // Stubb all other Methods
            public Task<bool> ExistsAsync(int id) => throw new NotImplementedException();
            public Task<bool> ExistsAsync(TeachingPlan teachingPlan) => throw new NotImplementedException();
            public Task UpdateAsync(TeachingPlan teachingPlan) => throw new NotImplementedException();
            public Task DeleteAsync(TeachingPlan teachingPlan) => throw new NotImplementedException();
            public Task<TeachingPlan?> GetTeachingPlanByIdAsync(int id) => throw new NotImplementedException();
            public Task<TeachingPlan?> GetTeachingPlanByIdWithLessonsAndTraineesAsync(int id) => throw new NotImplementedException();
            public Task<IEnumerable<TeachingPlan>> GetAllTeachingPlansAsync() => throw new NotImplementedException();
            public Task<IEnumerable<TeachingPlan>> GetAllTeachingPlansWithLessonsAndTraineesAsync() => throw new NotImplementedException();
        }

        // ------------------------------------------------------
        // Fake Repository for Lesson
        private class FakeLessonRepository : ILessonRepository{
            
            public List<Lesson> CreatedLessons { get; } = new List<Lesson>();

            // ------------------------------------------------------
            public Task CreateAsync(Lesson lesson){
                CreatedLessons.Add(lesson);
                return Task.CompletedTask;
            }

            // ------------------------------------------------------
            // Stubb all other Methods
            public Task<bool> ExistsAsync(int id) => throw new NotImplementedException();
            public Task<bool> ExistsAsync(string makandraId, int teachingPlanId) => throw new NotImplementedException();
            public Task UpdateAsync(Lesson lesson) => throw new NotImplementedException();
            public Task DeleteAsync(Lesson lesson) => throw new NotImplementedException();
            public Task<Lesson?> GetLessonByIdAsync(int id) => throw new NotImplementedException();
            public Task<IEnumerable<Lesson>> GetAllLessonsAsync() => throw new NotImplementedException();
            public Task<IEnumerable<Lesson>> GetAllLessonsWithFeedbacksAsync() => throw new NotImplementedException();
        }

        // ------------------------------------------------------
        [Fact]
        public async Task ImportNewTeachingPlan_ShouldCreateTeachingPlanAndLessons()
        {
            // --- Arrange ---

            // 1) Create a single LessonDto and seriaize it
            var lessonDtos = new List<LessonDto>{
                new LessonDto{
                    Id = "L1",
                    Title = "Test Lesson",
                    Url = "http://example.com",
                    Estimate = 2.5,
                    Deprecated = false
                }
            };
            var json = JsonConvert.SerializeObject(lessonDtos);

            // 2) Put the JSON in a IFormFile
            var bytes = Encoding.UTF8.GetBytes(json);
            var stream = new MemoryStream(bytes);
            var formFile = new FormFile(stream, 0, bytes.Length, "file", "lessons.json");

            // 3) Create the DTO for the Import
            var dto = new TeachingPlanDto{
                NewPlanFile = formFile,
                NewPlanName = "Mein neuer Plan"
            };

            // 4) Replace the Repos with Fakes
            var teachingPlanRepo = new FakeTeachingPlanRepository();
            var lessonRepo = new FakeLessonRepository();

            // 5) Service
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

            // 1) TeachingPlan was created
            Assert.True(teachingPlanRepo.CreateCalled);
            Assert.NotNull(teachingPlanRepo.CreatedTeachingPlan);
            Assert.Equal("Mein neuer Plan", teachingPlanRepo.CreatedTeachingPlan!.Name);

            // 2) Exactly one Lesson was created
            Assert.Single(lessonRepo.CreatedLessons);
            var created = lessonRepo.CreatedLessons[0];

            // Mapping-Checks
            Assert.Equal("L1", created.MakandraId);
            Assert.Equal("Test Lesson", created.Title);
            Assert.Equal("http://example.com", created.LinkUrl);
            Assert.Equal(2.5, created.EstimatedEffort, 3);
            Assert.False(created.IsInactive);
            Assert.Equal(1, created.SortingIndex);

            // TeachingPlanId is taken from the new created plan (Default 0)
            Assert.Equal(teachingPlanRepo.CreatedTeachingPlan.TeachingPlanId, created.TeachingPlanId);
        }
    }
}
