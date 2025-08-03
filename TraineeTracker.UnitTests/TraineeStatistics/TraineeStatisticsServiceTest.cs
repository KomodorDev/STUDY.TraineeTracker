using System.Net;
using System.Text;
using Microsoft.AspNetCore.Identity;
using TraineeTracker.Services;
using TraineeTracker.Models.Domain;
using TraineeTracker.Data.TraineeStatistics;
using TraineeTracker.Data.TraineeLessons;
using TraineeTracker.Data.ProcessingPauses;
using TraineeTracker.UnitTests.TraineeStatistics.TestDataFactory;


/// <summary>
/// Contains unit tests for <see cref="TraineeStatisticsService"/>, verifying correctness of calculated statistics.
/// </summary>
/// <remarks>
/// Code Ownership: Nikita Stefan (stefanni)
/// </remarks>
public class TraineeStatisticsServiceTests {

    // --------------------------------------------------
    /// <summary>
    /// Verifies correct number of present days returned by <see cref="TraineeStatisticsService.GetPresentDaysAsync"/>.
    /// </summary>
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
    /// <summary>
    /// Ensures <see cref="TraineeStatisticsService.BuildLatestTraineeStatisticsSnapshotAsync"/> calculates present days correctly,
    /// factoring in processing pauses.
    /// </summary>
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
    /// <summary>
    /// Verifies correct calculation of completed lesson days based on weighted effort and states.
    /// </summary>
    [Fact]
    public async Task CalculateLessonDaysCompletedAsyncTest() {
        var trainee = TestDataFactory.CreateTestTrainee();
        var lessons = TestDataFactory.CreateTestTraineeLessons(trainee);
        var lessonRepo = new FakeTraineeLessonRepository(lessons);
        var service = new TraineeStatisticsService(
            traineeStatisticsRepository: null!,
            traineeLessonRepository: lessonRepo,
            httpClient: null!,
            userManager: null!,
            processingPauseRepository: null!
        );

        var result = await service.CalculateLessonDaysCompletedAsync(trainee.Id);

        // Test 3
        Assert.Equal(11.9, result, precision: 1);   //(Finished = 5*0.7, Accepted = 4, Rejected = 3*0.8, Rated = 2 → Sum = 11.9)
    }

    // --------------------------------------------------
    /// <summary>
    /// Ensures correct calculation of open lesson days by subtracting completed from total effort.
    /// </summary>
    [Fact]
    public async Task CalculateLessonDaysOpenAsyncTest() {
        var trainee = TestDataFactory.CreateTestTrainee();
        var lessons = TestDataFactory.CreateTestTraineeLessons(trainee);

        var lessonRepo = new FakeTraineeLessonRepository(lessons);
        var service = new TraineeStatisticsService(
            traineeStatisticsRepository: null!,
            traineeLessonRepository: lessonRepo,
            httpClient: null!,
            userManager: null!,
            processingPauseRepository: null!
        );

        var result = await service.CalculateLessonDaysOpenAsync(trainee.Id);

        // Test 4
        Assert.Equal(12.1, result, precision: 1);   //(Total effort = 5+4+3+2+10 = 24, Skipped ignored. Completed = 11.9 → Open = 12.1)
    }

    // --------------------------------------------------
    /// <summary>
    /// Verifies that the buffer is calculated as the difference between completed lesson days and present days.
    /// </summary>
    [Fact]
    public void CalculateLessonDaysBufferTest() {
        var service = new TraineeStatisticsService(
            traineeStatisticsRepository: null!,
            traineeLessonRepository: null!,
            httpClient: null!,
            userManager: null!,
            processingPauseRepository: null!
        );

        double daysPresentTillToday = 10.0;
        double lessonDaysCompleted = 12.5;

        var result = service.CalculateLessonDaysBuffer(daysPresentTillToday, lessonDaysCompleted);

        // Test 5
        Assert.Equal(2.5, result, precision: 1);    // 12.5 - 10 = 2.5
    }

    // --------------------------------------------------
    /// <summary>
    /// Tests the logic for speed calculation (effort per present day).
    /// </summary>
    [Fact]
    public void CalculateSpeedTest() {
        var service = new TraineeStatisticsService(
            traineeStatisticsRepository: null!,
            traineeLessonRepository: null!,
            httpClient: null!,
            userManager: null!,
            processingPauseRepository: null!
        );

        // Test 6
        Assert.Equal(1.5, service.CalculateSpeed(10, 15), precision: 2);  // 15 / 10 = 1.5
        Assert.Equal(0.5, service.CalculateSpeed(20, 10), precision: 2);  // 10 / 20 = 0.5
        Assert.Equal(0, service.CalculateSpeed(0, 10), precision: 2);     // Division by zero
        Assert.Equal(0, service.CalculateSpeed(5, 0), precision: 2);      // 0 lessonDaysCompleted
    }

    // --------------------------------------------------
    /// <summary>
    /// Verifies prediction of remaining effort buffer at end of training, based on current speed.
    /// </summary>
    [Fact]
    public void CalculatePredictedMissingEstimatedEffortAtEndTest() {
        var service = new TraineeStatisticsService(
            traineeStatisticsRepository: null!,
            traineeLessonRepository: null!,
            httpClient: null!,
            userManager: null!,
            processingPauseRepository: null!
        );

        // Test 7
        // 0 Effort Buffer
        double? result1 = service.CalculatePredictedMissingEstimatedEffortAtEnd(
            daysPresentTillToday: 10,
            daysPresentTotal: 20,
            estimatedEffortOpen: 10,
            speed: 1.0
        );
        Assert.Equal(0, result1); // 10 remaining days * 1.0 = 10 => 10 - 10 = 0

        // negative Effort Buffer
        double? result2 = service.CalculatePredictedMissingEstimatedEffortAtEnd(
            daysPresentTillToday: 10,
            daysPresentTotal: 20,
            estimatedEffortOpen: 13,
            speed: 0.7
        );
        Assert.Equal(-6, result2); // 10 * 0.7 = 7 => 7 - 13 = -6

        // positive Effort Buffer
        double? result3 = service.CalculatePredictedMissingEstimatedEffortAtEnd(
            daysPresentTillToday: 5,
            daysPresentTotal: 15,
            estimatedEffortOpen: 5,
            speed: 1.5
        );
        Assert.Equal(10, result3); // 10 days * 1.5 = 15 => 15 - 5 = 10

        // speed = 0 => return null
        double? result4 = service.CalculatePredictedMissingEstimatedEffortAtEnd(
            daysPresentTillToday: 10,
            daysPresentTotal: 20,
            estimatedEffortOpen: 10,
            speed: 0
        );
        Assert.Null(result4);
    }

    // --------------------------------------------------
    /// <summary>
    /// Ensures accurate conversion of estimated effort buffer into actual days (based on speed).
    /// </summary>
    [Fact]
    public void CalculatePredictedMissingActualDaysAtEndTest() {
        var service = new TraineeStatisticsService(
            traineeStatisticsRepository: null!,
            traineeLessonRepository: null!,
            httpClient: null!,
            userManager: null!,
            processingPauseRepository: null!
        );

        // Test 8
        // 0 Actual Buffer
        double? result1 = service.CalculatePredictedMissingActualDaysAtEnd(
            daysPresentTillToday: 10,
            daysPresentTotal: 20,
            estimatedEffortOpen: 10,
            speed: 1.0
        );
        Assert.Equal(0, result1); // missingEffort = 0 → 0 / 1 = 0

        // negative Actual Buffer
        double? result2 = service.CalculatePredictedMissingActualDaysAtEnd(
            daysPresentTillToday: 10,
            daysPresentTotal: 20,
            estimatedEffortOpen: 15,
            speed: 0.5
        );
        Assert.Equal(-20, result2); // 10 * 0.5 = 5 => 5 - 15 = -10 => -10 / 0.5 = -20

        // positive Actual Buffer
        double? result3 = service.CalculatePredictedMissingActualDaysAtEnd(
            daysPresentTillToday: 10,
            daysPresentTotal: 20,
            estimatedEffortOpen: 5,
            speed: 1.25
        );
        Assert.Equal(6, result3); // 10 * 1.25 = 12.5 => 12.5 - 5 = 7.5 => 7.5 / 1.25 = 6

        // speed = 0 => return null
        double? result4 = service.CalculatePredictedMissingActualDaysAtEnd(
            daysPresentTillToday: 10,
            daysPresentTotal: 20,
            estimatedEffortOpen: 10,
            speed: 0
        );
        Assert.Null(result4);
    }

    // --------------------------------------------------
    /// <summary>
    /// Fake message handler used to simulate HTTP responses from external services.
    /// </summary>
    private class FakeHttpMessageHandler : HttpMessageHandler {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

        public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) {
            _responder = responder;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(_responder(request));
    }

    // --------------------------------------------------
    /// <summary>
    /// Fake user manager that returns a predefined <see cref="ApplicationUser"/>.
    /// </summary>
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
    /// <summary>
    /// Dummy implementation of user store for <see cref="FakeUserManager"/>.
    /// </summary>
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
    /// <summary>
    /// Fake repository that returns a fixed snapshot for a given trainee.
    /// </summary>
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

        public Task UpdateAsync(TraineeStatisticsSnapshot snapshot) {
            return Task.CompletedTask;
        }

        Task ITraineeStatisticsRepository.DeleteAsync(TraineeStatisticsSnapshot snapshot) {
            throw new NotImplementedException();
        }

        public Task<TraineeStatisticsSnapshot> GetTraineeStatisticsSnapshotAsync(string traineeId) {
            var snapshot = new TraineeStatisticsSnapshot {
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
    /// <summary>
    /// Fake lesson repository that returns a preconfigured set of lessons for a trainee.
    /// </summary>
    private class FakeTraineeLessonRepository : ITraineeLessonRepository {
        private readonly IEnumerable<TraineeLesson> _lessons;

        public FakeTraineeLessonRepository(IEnumerable<TraineeLesson>? lessons = null) {
            _lessons = lessons ?? new List<TraineeLesson>();
        }
        public Task<IEnumerable<TraineeLesson>> GetAllTraineeLessonsOfTraineeWithLessonAsync(string traineeId)
            => Task.FromResult(_lessons.Where(l => l.TraineeId == traineeId));

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
    /// <summary>
    /// In-memory repository to simulate pause records for testing pause-related statistics.
    /// </summary>
    private class FakeProcessingPauseRepository : IProcessingPauseRepository {
        private readonly List<ProcessingPause> _pauses = new();

        public Task CreateAsync(ProcessingPause processingPause) {
            _pauses.Add(processingPause);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(ProcessingPause processingPause) {
            var exists = _pauses.Contains(processingPause);
            return Task.FromResult(exists);
        }

        public Task<ProcessingPause?> FindByIdAsync(int processingPauseId) {
            return Task.FromResult<ProcessingPause?>(null);
        }

        public Task UpdateAsync(ProcessingPause processingPause) {
            return Task.CompletedTask;
        }

        public Task DeleteAsync(ProcessingPause processingPause) {
            _pauses.Remove(processingPause);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<ProcessingPause>> GetAllPausesAsync(string traineeId) {
            var result = _pauses.Where(p => p.TraineeId == traineeId);
            return Task.FromResult<IEnumerable<ProcessingPause>>(result);
        }

        public Task<bool> OverlapsAsync(ProcessingPause pause, bool excludeSelf) {
            return Task.FromResult(false);
        }
    }

    // --------------------------------------------------
}