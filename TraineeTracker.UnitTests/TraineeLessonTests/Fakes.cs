using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Data.Feedbacks;
using TraineeTracker.Data.Lessons;
using TraineeTracker.Data.TeachingPlans;
using TraineeTracker.Data.TraineeLessonLog;
using TraineeTracker.Data.TraineeLessons;
using TraineeTracker.Models.Domain;
using TraineeTracker.Services;

namespace TraineeTracker.UnitTests.TraineeLessonTests
{
    /// <summary>
    /// Fake trainee lesson repository for unit tests
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexander Schlemmer (schleale)
    /// </remarks>
    public class FakeTraineeLessonRepository : ITraineeLessonRepository {
        private readonly Dictionary<int, TraineeLesson> _lessons = new();

        public FakeTraineeLessonRepository(params TraineeLesson[] lessons) {
            foreach (var l in lessons)
                _lessons[l.TraineeLessonId] = l;
        }

        public Task<TraineeLesson?> GetTraineeLessonByIdWithLessonAsync(int id) {
            _lessons.TryGetValue(id, out var result);
            return Task.FromResult(result);
        }

        public Task<IEnumerable<TraineeLesson>> GetAllTraineeLessonsOfTraineeWithLessonAsync(string traineeId) {
            var result = _lessons.Values.Where(l => l.TraineeId == traineeId);
            return Task.FromResult(result);
        }

        public Task<IEnumerable<TraineeLesson>> GetAllTraineeLessonsOfLessonWithLessonAsync(int lessonId) {
            var result = _lessons.Values.Where(l => l.LessonId == lessonId);
            return Task.FromResult(result);
        }

        public Task<bool> ExistsAsync(int id) => Task.FromResult(_lessons.ContainsKey(id));

        public Task<bool> ExistsAsync(TraineeLesson lesson) => Task.FromResult(
            _lessons.TryGetValue(lesson.TraineeLessonId, out var existing) &&
            existing == lesson
        );

        public Task CreateAsync(TraineeLesson lesson) {
            _lessons[lesson.TraineeLessonId] = lesson;
            return Task.CompletedTask;
        }

        public Task CreateRangeAsync(IEnumerable<TraineeLesson> lessons) {
            foreach (var lesson in lessons)
                _lessons[lesson.TraineeLessonId] = lesson;
            return Task.CompletedTask;
        }

        public Task UpdateAsync(TraineeLesson lesson) {
            _lessons[lesson.TraineeLessonId] = lesson;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id) {
            _lessons.Remove(id);
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Fake lesson repository for unit tests, only needed methods are implemented.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexander Schlemmer (schleale)
    /// </remarks>
    public class FakeLessonRepository : ILessonRepository {
        private Lesson _lessonInRepo;

        public FakeLessonRepository(Lesson lesson) {
            _lessonInRepo = lesson;
        }

        public Task CreateAsync(Lesson lesson) {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Lesson lesson) {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(int id) {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(string makandraId, int teachingPlanId) {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Lesson>> GetAllLessonsAsync() {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Lesson>> GetAllLessonsWithFeedbacksAsync() {
            throw new NotImplementedException();
        }

        public Task<Lesson?> GetLessonByIdAsync(int id) {
            if (_lessonInRepo.LessonId != id)
                throw new Exception("How did we get here?");

            return Task.FromResult<Lesson?>(_lessonInRepo);
        }

        public Task UpdateAsync(Lesson lesson) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Fake trainee lesson log repository for unit tests, only needed methods are implemented.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexander Schlemmer (schleale)
    /// </remarks>
    public class FakeTraineeLessonLogEntryRepository : ITraineeLessonLogEntryRepository {
        public void Create(TraineeLessonLogEntry log) {
            throw new NotImplementedException();
        }

        public IEnumerable<TraineeLessonLogEntry> GetAllLogs() {
            throw new NotImplementedException();
        }

        public IEnumerable<TraineeLessonLogEntry> GetAllLogsForTraineeLesson(int traineeLessonId) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Fake feedback repository for unit tests, only needed methods are implemented.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexander Schlemmer (schleale)
    /// </remarks>
    public class FakeFeedbackRepository : IFeedbackRepository {
        public Feedback? CreatedFeedback { get; private set; }

        public Task CreateAsync(Feedback feedback) {
            CreatedFeedback = feedback;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Feedback feedback) {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int feedbackId) {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(int feedbackId) {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Feedback feedback) {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Feedback>> GetAllFeedbacksForLessonWithLessonAndAuthorAndReadByUsersAsync(Lesson lesson) {
            throw new NotImplementedException();
        }

        public IQueryable<Feedback> GetAllFeedbacksReadByUserWithLessonAndAuthor(ApplicationUser user) {
            throw new NotImplementedException();
        }

        public Task<List<Feedback>> GetAllFeedbacksReadByUserWithLessonAndAuthorAndReadByUsersAsync(ApplicationUser user) {
            throw new NotImplementedException();
        }

        public IQueryable<Feedback> GetAllFeedbacksUnreadByUserWithLessonAndAuthor(ApplicationUser user) {
            throw new NotImplementedException();
        }

        public IQueryable<Feedback> GetAllFeedbacksUnreadByUserWithLessonAndAuthorAndReadByUsers(ApplicationUser user) {
            throw new NotImplementedException();
        }

        public Task<List<Feedback>> GetAllFeedbacksUnreadByUserWithLessonAndAuthorAndReadByUsersAsync(ApplicationUser user) {
            throw new NotImplementedException();
        }

        public IQueryable<Feedback> GetAllFeedbacksWithLessonAndAuthor() {
            throw new NotImplementedException();
        }

        public IQueryable<Feedback> GetAllFeedbacksWithLessonAndAuthorAndReadByUsers() {
            throw new NotImplementedException();
        }

        public Task<List<Feedback>> GetAllFeedbacksWithLessonAndAuthorAndReadByUsersAsync() {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Feedback>> GetAllFeedbacksWrittenByUserWithLessonAndAuthorAndReadByUsersAsync(ApplicationUser user) {
            throw new NotImplementedException();
        }

        public Task<Feedback?> GetFeedbackByIDWithLessonAndAuthorAndReadByUsersAsync(int feedbackId) {
            throw new NotImplementedException();
        }

        public Task<Feedback?> GetFeedbackOfTraineeLessonWithLessonAndAuthorAndReadByUsersAsync(TraineeLesson traineeLesson) {
            // In the test, we create a new feedback, and this method normally checks if it already exists
            return Task.FromResult<Feedback?>(null);
        }

        public Task UpdateAsync(Feedback feedback) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Fake application user repository for unit tests, only needed methods are implemented.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexander Schlemmer (schleale)
    /// </remarks>
    public class FakeApplicationUserRepository : IApplicationUserRepository {
        private ApplicationUser _trainee;

        public FakeApplicationUserRepository(ApplicationUser trainee) {
            _trainee = trainee;
        }

        public Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role) {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> CreateAsync(ApplicationUser user, string password) {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(ApplicationUser user) {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(string userId) {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(ApplicationUser user) {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser?> FindByEmailAsync(string email) {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser?> FindByEmailWithProcessingPausesAndTraineeLessonsAsync(string email) {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser?> FindByIdAsync(string userId) {
            if (_trainee.Id != userId)
                throw new Exception("The cake is a lie");
            return Task.FromResult<ApplicationUser?>(_trainee);
        }

        public Task<ApplicationUser?> FindByIdWithLastSelectedTraineesAsync(string userId) {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser?> FindByIdWithNotificationSettingAsync(string userId) {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser?> FindByIdWithProcessingPausesAsync(string userId) {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser?> FindByIdWithTeachingPlanAndTraineeLessonsWithLessonsAsync(string userId) {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser?> FindByIdWithTraineeLessonsAndProcessingPausesAsync(string userId) {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser?> FindByIdWithWrittenFeedbacksWithLessonAsync(string userId) {
            throw new NotImplementedException();
        }

        public Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user) {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ApplicationUser>> GetAllAsync() {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ApplicationUser>> GetAllAsync(bool isClosed) {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ApplicationUser>> GetClosedUsersInRoleAsync(string roleName) {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ApplicationUser>> GetOpenUsersInRoleAsync(string roleName) {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ApplicationUser>> GetOpenUsersInRoleWithEmailNotificationSettingAsync(string roleName) {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetRolesAsync(ApplicationUser user) {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser?> GetUserAsync(ClaimsPrincipal principal) {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ApplicationUser>> GetUsersInRoleAsync(string roleName) {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ApplicationUser>> GetUsersInRoleWithProcessingPausesAndTraineeLessonsAsync(string roleName) {
            throw new NotImplementedException();
        }

        public Task<bool> IsInRoleAsync(ApplicationUser user, string role) {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(ApplicationUser user) {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Fake feedback service for unit tests, only needed methods are implemented.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexander Schlemmer (schleale)
    /// </remarks>
    public class FakeFeedbackService : FeedbackService {
        public FakeFeedbackService(IFeedbackRepository feedbackRepo, IApplicationUserRepository userRepo, ILessonRepository lessonRepo, ITeachingPlanRepository teachingPlanRepo) : base(feedbackRepo, userRepo, lessonRepo, teachingPlanRepo) {
        }

        public new Task MarkFeedbackAsUnreadForEveryoneAsync(int feedbackId) {
            return Task.CompletedTask;
        }
    }

}
