using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Data.Feedbacks;
using TraineeTracker.Data.Lessons;
using TraineeTracker.Data.TeachingPlans;
using TraineeTracker.Models.Domain;
using TraineeTracker.Services;
using Xunit;
using Microsoft.AspNetCore.Identity;

namespace TraineeTracker.UnitTests.FeedbackTest 
{
    public class FeedbackServiceTests 
    {
        // Fake Feedback Repository
        private class FakeFeedbackRepository : IFeedbackRepository {
            public Feedback? StoredFeedback;
            public bool UpdateCalled = false;

            public Task<Feedback?> GetFeedbackByIDWithLessonAndAuthorAndReadByUsersAsync(int feedbackId)
                => Task.FromResult(StoredFeedback);
            public Task UpdateAsync(Feedback feedback) {
                StoredFeedback = feedback;
                UpdateCalled = true;
                return Task.CompletedTask;
            }

            // Stubb all other Methods:
            public Task<bool> ExistsAsync(int id) => throw new NotImplementedException();
            public Task<bool> ExistsAsync(Feedback feedback) => throw new NotImplementedException();
            public Task CreateAsync(Feedback feedback) => throw new NotImplementedException();
            public Task DeleteAsync(Feedback feedback) => throw new NotImplementedException();
            public Task DeleteAsync(int feedbackId) => throw new NotImplementedException();
            public Task<List<Feedback>> GetAllFeedbacksWithLessonAndAuthorAndReadByUsersAsync() 
                => throw new NotImplementedException();
            public Task<IEnumerable<Feedback>> GetAllFeedbacksForLessonWithLessonAndAuthorAndReadByUsersAsync(Lesson lesson) 
                => throw new NotImplementedException();
            public Task<IEnumerable<Feedback>> GetAllFeedbacksWrittenByUserWithLessonAndAuthorAndReadByUsersAsync(ApplicationUser user) 
                => throw new NotImplementedException();
            public Task<List<Feedback>> GetAllFeedbacksReadByUserWithLessonAndAuthorAndReadByUsersAsync(ApplicationUser user)
                => throw new NotImplementedException();
            public Task<List<Feedback>> GetAllFeedbacksUnreadByUserWithLessonAndAuthorAndReadByUsersAsync(ApplicationUser user)
                => throw new NotImplementedException();
            public Task<Feedback?> GetFeedbackOfTraineeLessonWithLessonAndAuthorAndReadByUsersAsync(TraineeLesson traineeLesson)
                => throw new NotImplementedException();
            public IQueryable<Feedback> GetAllFeedbacksWithLessonAndAuthor() => throw new NotImplementedException();
            public IQueryable<Feedback> GetAllFeedbacksReadByUserWithLessonAndAuthor(ApplicationUser user)
                => throw new NotImplementedException();
            public IQueryable<Feedback> GetAllFeedbacksUnreadByUserWithLessonAndAuthor(ApplicationUser user)
                => throw new NotImplementedException();
            public IQueryable<Feedback> GetAllFeedbacksUnreadByUserWithLessonAndAuthorAndReadByUsers(ApplicationUser user)
                => throw new NotImplementedException();
            public IQueryable<Feedback> GetAllFeedbacksWithLessonAndAuthorAndReadByUsers()
                => throw new NotImplementedException();
        }

        // Fake User Repository with all Methods stubbed
        private class FakeUserRepository : IApplicationUserRepository {
            private readonly ApplicationUser _user;
            public FakeUserRepository(ApplicationUser user) { _user = user; }

            public Task<ApplicationUser?> GetUserAsync(ClaimsPrincipal user) => Task.FromResult(_user);

            // Alle anderen Methoden:
            public Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role) => throw new NotImplementedException();
            public Task<IdentityResult> CreateAsync(ApplicationUser user, string password) => throw new NotImplementedException();
            public Task<IdentityResult> DeleteAsync(ApplicationUser user) => throw new NotImplementedException();
            public Task<bool> ExistsByIdAsync(string id) => throw new NotImplementedException();
            public Task<bool> ExistsAsync(ApplicationUser user) => throw new NotImplementedException();
            public Task<ApplicationUser?> FindByEmailAsync(string email) => throw new NotImplementedException();
            public Task<ApplicationUser?> FindByEmailWithProcessingPausesAndTraineeLessonsAsync(string email) 
                => throw new NotImplementedException();
            public Task<ApplicationUser?> FindByIdAsync(string id) => throw new NotImplementedException();
            public Task<ApplicationUser?> FindByIdWithLastSelectedTraineesAsync(string id) => throw new NotImplementedException();
            public Task<ApplicationUser?> FindByIdWithNotificationSettingAsync(string id) => throw new NotImplementedException();
            public Task<ApplicationUser?> FindByIdWithTeachingPlanAndTraineeLessonsWithLessonsAsync(string id)
                => throw new NotImplementedException();
            public Task<ApplicationUser?> FindByIdWithTraineeLessonsAndProcessingPausesAsync(string id)
                => throw new NotImplementedException();
            public Task<ApplicationUser?> FindByIdWithProcessingPausesAsync(string id) => throw new NotImplementedException();
            public Task<IEnumerable<ApplicationUser>> GetOpenUsersInRoleAsync(string role) => throw new NotImplementedException();
            public Task<IEnumerable<ApplicationUser>> GetClosedUsersInRoleAsync(string role) => throw new NotImplementedException();
            public Task<IEnumerable<ApplicationUser>> GetOpenUsersInRoleWithEmailNotificationSettingAsync(string role)
                => throw new NotImplementedException();
            public Task<IEnumerable<string>> GetRolesAsync(ApplicationUser user) => throw new NotImplementedException();
            public Task<IEnumerable<ApplicationUser>> GetUsersInRoleAsync(string role) => throw new NotImplementedException();
            public Task<IEnumerable<ApplicationUser>> GetUsersInRoleWithProcessingPausesAndTraineeLessonsAsync(string role)
                => throw new NotImplementedException();
            public Task<IEnumerable<ApplicationUser>> GetAllAsync()
                => Task.FromResult<IEnumerable<ApplicationUser>>(new List<ApplicationUser>());
            public Task<IEnumerable<ApplicationUser>> GetAllAsync(bool includeInactive)
                => Task.FromResult<IEnumerable<ApplicationUser>>(new List<ApplicationUser>());
            public Task<bool> IsInRoleAsync(ApplicationUser user, string role) => throw new NotImplementedException();
            public Task<IdentityResult> UpdateAsync(ApplicationUser user) => throw new NotImplementedException();
            public Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user) => throw new NotImplementedException();
            public Task<ApplicationUser?> FindByIdWithWrittenFeedbacksWithLessonAsync(string id)
                => Task.FromResult<ApplicationUser?>(_user);

        }

        [Fact]
        public async Task MarkFeedbackAsReadAsync_ShouldAddUser_WhenNotAlreadyRead() {
            var user = new ApplicationUser {
                Id = "test-user",
                EmailNotificationSetting = new EmailNotificationSetting() // oder ein valides Dummy-Objekt
            };

            var lesson = new Lesson {
                LessonId = 1,
                MakandraId = "123",
                TeachingPlanId = 456,
                SortingIndex = 1,
                Title = "Test Lesson",
                EstimatedEffort = 2.0f,
                LinkUrl = "http://example.com"
            };

            var feedback = new Feedback {
                FeedbackId = 1,
                Author = user,
                AuthorId = user.Id,
                Lesson = lesson,
                LessonId = 1,
                ReadByUsers = new List<ApplicationUser>(),
                Difficulty = LessonDifficulty.Medium, // Beispielwert
                PreviousKnowledge = PreviousKnowledgeLevel.Basic, // Beispielwert
                HoursOfEffort = 3.5f // Beispielwert
            };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            }));
            var feedbackRepo = new FakeFeedbackRepository { StoredFeedback = feedback };
            var userRepo = new FakeUserRepository(user);
            var service = new FeedbackService(feedbackRepo, userRepo, null!, null!);

            await service.MarkFeedbackAsReadAsync(claimsPrincipal, 1);

            Assert.Contains(user, feedback.ReadByUsers);
            Assert.True(feedbackRepo.UpdateCalled);
        }
    }
}
