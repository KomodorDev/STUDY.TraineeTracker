using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System;
using Xunit;
using TraineeTracker.Services;
using TraineeTracker.Models.Domain;
using TraineeTracker.Data.TraineeStatistics;
using TraineeTracker.Data.TraineeLessons;
using TraineeTracker.Data.ProcessingPauses;
using System.Threading;
using System.Net.Http.Headers;

public class TraineeStatisticsServiceTests {

    // --------------------------------------------------
    [Fact]
    public async Task GetPresentDaysAsyncTest() {
        var httpClient = new HttpClient();
        var pauseRepo = new FakeProcessingPauseRepository();
        var service = new TraineeStatisticsService(
            traineeStatisticsRepository: null!,
            traineeLessonRepository: null!,
            httpClient: httpClient,
            userManager: null!,
            processingPauseRepository: pauseRepo
        );

        var start = new DateOnly(2021, 9, 1);
        var end = new DateOnly(2021, 10, 21);
        var email = "vanessa.vital@makandra.de";

        var presentDays = await service.GetPresentDaysAsync(start, end, email);

        Console.WriteLine($"Present days: {presentDays}");


        // Test 1
        Assert.Equal(37, presentDays);
    }

    // --------------------------------------------------
    [Fact]
    public async Task BuildLatestTraineeStatisticsSnapshotAsync_ShouldCalculateCorrectPresentDays() {
        var traineeId = "ursula-1";
        var email = "ursula.urlaub@makandra.de";

        var trainee = new ApplicationUser {
            Id = traineeId,
            Email = email,
            EmailNotificationSetting = new EmailNotificationSetting(),
            TraineeStartDate = new DateOnly(2025, 6, 1),
            TraineeEndDate = new DateOnly(2025, 6, 28),
            ProcessingPauses = new List<ProcessingPause>()
        };

        foreach (var pause in new[]
        {
            new ProcessingPause
            {
                StartDate = new DateOnly(2025, 6, 9),
                EndDate = new DateOnly(2025, 6, 15),
                TraineeId = traineeId,
                Trainee = trainee
            },
            new ProcessingPause
            {
                StartDate = new DateOnly(2025, 6, 23),
                EndDate = new DateOnly(2025, 6, 29),
                TraineeId = traineeId,
                Trainee = trainee
            }
        }) {
            trainee.ProcessingPauses.Add(pause);
        }


        var handler = new FakeHttpMessageHandler((request) => {
            var url = request.RequestUri!.ToString();
            double days = url.Contains("2025-06-09") ? 5 :
                          url.Contains("2025-06-23") ? 5 : 20;

            var content = $"{{ \"present_days\": {days} }}";
            return new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            };
        });

        var httpClient = new HttpClient(handler);
        var userManager = new FakeUserManager(trainee);
        var statsRepo = new FakeTraineeStatisticsRepository(trainee);
        var lessonRepo = new FakeTraineeLessonRepository();
        var pauseRepo = new FakeProcessingPauseRepository();

        var service = new TraineeStatisticsService(statsRepo, lessonRepo, httpClient, userManager, pauseRepo);

        var snapshot = await service.BuildLatestTraineeStatisticsSnapshotAsync(traineeId);

        // Test 2
        Assert.Equal(10, snapshot.DaysPresentTotal); // 20 total - 5 - 5 pause = 10
    }

    // --------------------------------------------------
    private class FakeHttpMessageHandler : HttpMessageHandler {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

        public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) {
            _responder = responder;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(_responder(request));
    }

    // --------------------------------------------------
    private class FakeUserManager : UserManager<ApplicationUser> {
        private readonly ApplicationUser _user;

        public FakeUserManager(ApplicationUser user)
            : base(new FakeUserStore(), null!, null!, null!, null!, null!, null!, null!, null!) {
            _user = user;
        }

        public override Task<ApplicationUser?> FindByIdAsync(string userId)
            => Task.FromResult<ApplicationUser?>(_user);
    }

    // --------------------------------------------------
    private class FakeUserStore : IUserStore<ApplicationUser> {
        public Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Success);
        public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Success);
        public void Dispose() { }
        public Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken) => Task.FromResult<ApplicationUser?>(null);
        public Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) => Task.FromResult<ApplicationUser?>(null);
        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult("123");
        public Task<string?> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult<string?>("Name");
        public Task<string?> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult<string?>("Name");
        public Task SetNormalizedUserNameAsync(ApplicationUser user, string? normalizedName, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task SetUserNameAsync(ApplicationUser user, string? userName, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Success);
    }

    // --------------------------------------------------
    private class FakeTraineeStatisticsRepository : ITraineeStatisticsRepository {
        private readonly ApplicationUser _user;

        public FakeTraineeStatisticsRepository(ApplicationUser user) {
            _user = user;
        }


        public TraineeStatisticsSnapshot GetTraineeStatisticsSnapshot(string traineeId)
            => new TraineeStatisticsSnapshot {
                TraineeId = traineeId,
                Trainee = _user,
                SnapshotDateTime = DateTime.Today
            };

        Task<bool> ITraineeStatisticsRepository.ExistsAsync(int id) {
            throw new NotImplementedException();
        }

        Task<bool> ITraineeStatisticsRepository.ExistsAsync(TraineeStatisticsSnapshot snapshot) {
            throw new NotImplementedException();
        }

        Task ITraineeStatisticsRepository.CreateAsync(TraineeStatisticsSnapshot snapshot) {
            return Task.FromResult(snapshot); 
        }

        public Task UpdateAsync(TraineeStatisticsSnapshot snapshot)
        {
            return Task.CompletedTask;
        }

        Task ITraineeStatisticsRepository.DeleteAsync(TraineeStatisticsSnapshot snapshot) {
            throw new NotImplementedException();
        }

        public Task<TraineeStatisticsSnapshot> GetTraineeStatisticsSnapshotAsync(string traineeId)
        {
            var snapshot = new TraineeStatisticsSnapshot
            {
                TraineeId = traineeId,
                Trainee = _user,
                SnapshotDateTime = DateTime.Now,
                DaysPresentTotal = 10,
                DaysPresentTillToday = 10,
                LessonDaysCompleted = 0,
                LessonDaysOpen = 0,
                LessonDaysBuffer = 0,
                Speed = 0,
                PredictedMissingEstimatedEffortAtEnd = 0,
                PredictedMissingActualDays = 0,
                IsUpToDate = true
            };

            return Task.FromResult(snapshot);
        }
    }

    // --------------------------------------------------
    private class FakeTraineeLessonRepository : ITraineeLessonRepository {
        public Task<IEnumerable<TraineeLesson>> GetAllTraineeLessonsOfTraineeWithLessonAsync(string traineeId)
            => Task.FromResult<IEnumerable<TraineeLesson>>(new List<TraineeLesson>());

        public Task<IEnumerable<TraineeLesson>> GetAllTraineeLessonsOfLessonWithLessonAsync(int lessonId)
            => Task.FromResult<IEnumerable<TraineeLesson>>(new List<TraineeLesson>());

        public Task<TraineeLesson?> GetTraineeLessonByIdWithLessonAsync(int id)
            => Task.FromResult<TraineeLesson?>(null);

        public Task<bool> ExistsAsync(int id) => Task.FromResult(false);

        public Task<bool> ExistsAsync(TraineeLesson lesson) => Task.FromResult(false);

        public Task CreateAsync(TraineeLesson lesson) => Task.CompletedTask;

        public Task CreateRangeAsync(IEnumerable<TraineeLesson> lessons) => Task.CompletedTask;

        public Task UpdateAsync(TraineeLesson lesson) => Task.CompletedTask;

        public Task DeleteAsync(int id) => Task.CompletedTask;
    }

    // --------------------------------------------------
    private class FakeProcessingPauseRepository : IProcessingPauseRepository
    {
        private readonly List<ProcessingPause> _pauses = new();

        public Task CreateAsync(ProcessingPause processingPause)
        {
            _pauses.Add(processingPause);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(ProcessingPause processingPause)
        {
            var exists = _pauses.Contains(processingPause);
            return Task.FromResult(exists);
        }

        public Task<ProcessingPause?> FindByIdAsync(int processingPauseId)
        {
            return Task.FromResult<ProcessingPause?>(null);
        }

        public Task UpdateAsync(ProcessingPause processingPause)
        {
            return Task.CompletedTask;
        }

        public Task DeleteAsync(ProcessingPause processingPause)
        {
            _pauses.Remove(processingPause);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<ProcessingPause>> GetAllPausesAsync(string traineeId)
        {
            var result = _pauses.Where(p => p.TraineeId == traineeId);
            return Task.FromResult<IEnumerable<ProcessingPause>>(result);
        }
    }
}
