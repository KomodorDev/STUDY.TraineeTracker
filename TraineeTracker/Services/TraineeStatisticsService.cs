using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TraineeTracker.Data.TraineeStatistics;
using TraineeTracker.Models.ViewModels;
using TraineeTracker.Models.Domain;
using TraineeTracker.Data.TraineeLessons;
using Microsoft.AspNetCore.Identity;
using TraineeTracker.Exceptions;
using TraineeTracker.Data.ProcessingPauses;

namespace TraineeTracker.Services {
    public class TraineeStatisticsService {
        private readonly ITraineeStatisticsRepository _traineeStatisticsRepository;
        private readonly ITraineeLessonRepository _traineeLessonRepository;
        private readonly HttpClient _httpClient;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IProcessingPauseRepository _processingPauseRepository;

        // --------------------------------------------------
        public TraineeStatisticsService(ITraineeStatisticsRepository traineeStatisticsRepository, ITraineeLessonRepository traineeLessonRepository, HttpClient httpClient, UserManager<ApplicationUser> userManager, IProcessingPauseRepository processingPauseRepository)
        {
            _traineeStatisticsRepository = traineeStatisticsRepository;
            _traineeLessonRepository = traineeLessonRepository;
            _httpClient = httpClient;
            _userManager = userManager;
            _processingPauseRepository = processingPauseRepository;
        }

        // --------------------------------------------------
        public async Task<TraineeStatisticsViewModel> BuildTraineeStatisticsViewModel(string traineeId, ClaimsPrincipal user) {
            CheckHasAccess(user, traineeId);

            var snapshot = await BuildLatestTraineeStatisticsSnapshotAsync(traineeId);
            var lessons = await _traineeLessonRepository.GetAllTraineeLessonsOfTraineeWithLessonAsync(traineeId);
            var processingPauses = (await _processingPauseRepository.GetAllPausesAsync(traineeId)).ToList();


            return new TraineeStatisticsViewModel
            {
                SnapshotDateTime = snapshot.SnapshotDateTime,
                DaysPresentTotal = snapshot.DaysPresentTotal,
                DaysPresentTillToday = snapshot.DaysPresentTillToday,
                LessonDaysCompleted = snapshot.LessonDaysCompleted,
                LessonDaysOpen = snapshot.LessonDaysOpen,
                LessonDaysBuffer = snapshot.LessonDaysBuffer,
                Speed = snapshot.Speed,
                PredictedMissingEstimatedEffortAtEnd = snapshot.PredictedMissingEstimatedEffortAtEnd,
                PredictedMissingActualDays = snapshot.PredictedMissingActualDays,
                IsUpToDate = snapshot.IsUpToDate,

                //++++++++++++++++

                FinishedLessons = lessons
                    .Where(l => l.State == TraineeLessonState.Finished)
                    .Select(l => new TraineeLessonViewModel
                    {
                        Title = l.Lesson.Title,
                        EstimatedEffort = l.Lesson.EstimatedEffort,
                        WeightedEffort = l.Lesson.EstimatedEffort * 0.7,
                        State = l.State
                    }).ToList(),
                AcceptedAndRatedLessons = lessons
                    .Where(l => l.State == TraineeLessonState.Accepted || l.State == TraineeLessonState.Rated)
                    .Select(l => new TraineeLessonViewModel
                    {
                        Title = l.Lesson.Title,
                        EstimatedEffort = l.Lesson.EstimatedEffort,
                        WeightedEffort = l.Lesson.EstimatedEffort * 1.0,
                        State = l.State
                    }).ToList(),
                RejectedLessons = lessons
                    .Where(l => l.State == TraineeLessonState.Rejected)
                    .Select(l => new TraineeLessonViewModel
                    {
                        Title = l.Lesson.Title,
                        EstimatedEffort = l.Lesson.EstimatedEffort,
                        WeightedEffort = l.Lesson.EstimatedEffort * 0.8,
                        State = l.State
                    }).ToList(),
                OpenLessons = lessons
                    .Where(l => l.State == TraineeLessonState.Open)
                    .Select(l => new TraineeLessonViewModel
                    {
                        Title = l.Lesson.Title,
                        EstimatedEffort = l.Lesson.EstimatedEffort,
                        WeightedEffort = 0,
                        State = l.State
                    }).ToList(),
                StartedLessons = lessons
                    .Where(l => l.State == TraineeLessonState.Started)
                    .Select(l => new TraineeLessonViewModel
                    {
                        Title = l.Lesson.Title,
                        EstimatedEffort = l.Lesson.EstimatedEffort,
                        WeightedEffort = 0,
                        State = l.State
                    }).ToList(),

                //++++++++++++++++

                ProcessingPauses = processingPauses
            };
        }

        // --------------------------------------------------
        public void CheckHasAccess(ClaimsPrincipal user, string traineeId) {
            if (user == null)
                throw new UserNotFoundException();

            var currentUserId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (user.IsInRole("Admin") || user.IsInRole("Mentor")) {
                return;
            }

            if (!(currentUserId == traineeId)) {
                throw new UnauthorizedAccessException("You can only your own statistics.");
            }

        }

        // --------------------------------------------------
        public async Task<TraineeStatisticsSnapshot> BuildLatestTraineeStatisticsSnapshotAsync(string traineeId) {
            var trainee = await _userManager.FindByIdAsync(traineeId);
            if (trainee == null || trainee.TraineeStartDate == null) {
                throw new Exception("Trainee not found or start date is missing.");
            }

            double daysPresentTotal = await GetEffectivePresentDaysAsync(trainee, trainee.TraineeStartDate!.Value, trainee.TraineeEndDate!.Value);
            double daysPresentTillToday = await GetEffectivePresentDaysAsync(trainee, trainee.TraineeStartDate!.Value, DateOnly.FromDateTime(DateTime.Today));
            if (daysPresentTillToday < 0)
            {
                Console.WriteLine("⚠️ API-Error – use latest snapshot.");
                var fallbackSnapshot = await _traineeStatisticsRepository.GetTraineeStatisticsSnapshotAsync(traineeId);
                fallbackSnapshot.IsUpToDate = false;
                return fallbackSnapshot;
            }
            double lessonDaysCompleted = await CalculateLessonDaysCompletedAsync(traineeId);
            double lessonDaysOpen = await CalculateLessonDaysOpenAsync(traineeId);
            double lessonDaysBuffer = CalculateLessonDaysBuffer(daysPresentTillToday, lessonDaysCompleted);
            double speed = CalculateSpeed(daysPresentTillToday, lessonDaysCompleted);
            double predictedMissingEstimatedEffortAtEnd = CalculatePredictedMissingEstimatedEffortAtEnd(daysPresentTillToday, daysPresentTotal, lessonDaysOpen, speed);
            double predictedMissingActualDays = CalculatePredictedMissingActualDays(daysPresentTillToday, daysPresentTotal, lessonDaysOpen, speed);

            TraineeStatisticsSnapshot snapshot;

            try {
                snapshot = await _traineeStatisticsRepository.GetTraineeStatisticsSnapshotAsync(traineeId);

                // Fall: Snapshot existiert → wir aktualisieren ihn
                snapshot.SnapshotDateTime = DateTime.Now;
                snapshot.DaysPresentTotal = daysPresentTotal;
                snapshot.DaysPresentTillToday = daysPresentTillToday;
                snapshot.LessonDaysCompleted = lessonDaysCompleted;
                snapshot.LessonDaysOpen = lessonDaysOpen;
                snapshot.LessonDaysBuffer = lessonDaysBuffer;
                snapshot.Speed = speed;
                snapshot.PredictedMissingEstimatedEffortAtEnd = predictedMissingEstimatedEffortAtEnd;
                snapshot.PredictedMissingActualDays = predictedMissingActualDays;
                snapshot.IsUpToDate = true;

                await _traineeStatisticsRepository.UpdateAsync(snapshot);
            }
            catch (InvalidOperationException) {
                // Fall: Kein Snapshot vorhanden → wir erstellen einen neuen
                snapshot = new TraineeStatisticsSnapshot
                {
                    Trainee = trainee,
                    TraineeId = traineeId,
                    SnapshotDateTime = DateTime.Now,
                    DaysPresentTotal = daysPresentTotal,
                    DaysPresentTillToday = daysPresentTillToday,
                    LessonDaysCompleted = lessonDaysCompleted,
                    LessonDaysOpen = lessonDaysOpen,
                    LessonDaysBuffer = lessonDaysBuffer,
                    Speed = speed,
                    PredictedMissingEstimatedEffortAtEnd = predictedMissingEstimatedEffortAtEnd,
                    PredictedMissingActualDays = predictedMissingActualDays,
                    IsUpToDate = true
                };

                await _traineeStatisticsRepository.CreateAsync(snapshot);
            }


            return snapshot;
        }

        // --------------------------------------------------
        public async Task<double> GetPresentDaysAsync(DateOnly startDate, DateOnly endDate, string email) {
            var baseUrl = "https://api.sopro.makandra.de/api/v1/present_days";
            var url = $"{baseUrl}?email={Uri.EscapeDataString(email)}&start_date={startDate:yyyy-MM-dd}&end_date={endDate:yyyy-MM-dd}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var byteArray = System.Text.Encoding.ASCII.GetBytes("sopro:capybara");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode) {
                Console.WriteLine($"❌ API-Error: {response.StatusCode} - {response.ReasonPhrase}");
                return -1;
            }

            var content = await response.Content.ReadAsStringAsync();

            try {
                using var json = System.Text.Json.JsonDocument.Parse(content);
                var root = json.RootElement;
                return root.GetProperty("present_days").GetDouble();
            }
            catch (Exception ex) {
                Console.WriteLine($"❌ JSON-Error: {ex.Message}");
                return -1;
            }
        }

        // --------------------------------------------------
        private async Task<double> GetEffectivePresentDaysAsync(ApplicationUser trainee, DateOnly startDate, DateOnly endDate) {

            if (trainee.TraineeStartDate is null || trainee.TraineeEndDate is null)
                throw new Exception("TraineeStartDate or EndDate is missing");

            var email = trainee.Email ?? throw new Exception("E-Mail fehlt");

            double totalDays = await GetPresentDaysAsync(startDate, endDate, email);

            if (totalDays < 0)
                return -1;

            double pauseDaysTotal = 0;

            foreach (var pause in trainee.ProcessingPauses) {
                var pauseStart = pause.StartDate;
                var pauseEnd = pause.EndDate;

                if (pauseEnd < startDate || pauseStart > endDate)
                    continue;

                var effectivePauseStart = pauseStart < startDate ? startDate : pauseStart;
                var effectivePauseEnd = pauseEnd > endDate ? endDate : pauseEnd;

                double pauseDays = await GetPresentDaysAsync(effectivePauseStart, effectivePauseEnd, email);

                if (pauseDays > 0) {
                    pauseDaysTotal += pauseDays;
                }
            }

            return totalDays - pauseDaysTotal;
        }

        // --------------------------------------------------
        public async Task<double> CalculateLessonDaysCompletedAsync(string traineeId) {
            var lessons = await _traineeLessonRepository.GetAllTraineeLessonsOfTraineeWithLessonAsync(traineeId);

            return lessons
                .Where(tl => tl.State != TraineeLessonState.Skipped &&
                            !(tl.Lesson.IsInactive && tl.State == TraineeLessonState.Open))
                .Sum(tl => {
                    var effort = tl.Lesson.EstimatedEffort;

                    return tl.State switch {
                        TraineeLessonState.Finished => effort * 0.7,
                        TraineeLessonState.Accepted => effort * 1.0,
                        TraineeLessonState.Rejected => effort * 0.8,
                        TraineeLessonState.Rated => effort * 1.0,
                        _ => 0
                    };
                });
        }

        // --------------------------------------------------
        public async Task<double> CalculateLessonDaysOpenAsync(string traineeId) {
            double totalEffort = await CalculateTotalEffort(traineeId);

            double completedEffort = await CalculateLessonDaysCompletedAsync(traineeId);

            return totalEffort - completedEffort;
        }

        // --------------------------------------------------
        public double CalculateLessonDaysBuffer(double daysPresentTillToday, double lessonDaysCompleted) {
            return lessonDaysCompleted - daysPresentTillToday;
        }

        // --------------------------------------------------
        public double CalculateSpeed(double daysPresentTillToday, double lessonDaysCompleted) {
            return daysPresentTillToday > 0 ? lessonDaysCompleted / daysPresentTillToday : 0;
        }

        // --------------------------------------------------
        public double CalculatePredictedMissingEstimatedEffortAtEnd(double daysPresentTillToday, double daysPresentTotal, double estimatedEffortOpen, double speed) {
            if (speed <= 0)
                return -1;

            // Days from today till EndDate:
            double daysPresentInFuture = daysPresentTotal - daysPresentTillToday;

            // Likey EstimatedEffort completed from today till EndDate:
            double predictedEstimatedEffortDoneInFuture = daysPresentInFuture * speed;

            // predicted Buffer in EstimatedEffort: estimatedEffort remaining at EndDate
            double predictedMissingEstimatedEffortAtEnd = estimatedEffortOpen - predictedEstimatedEffortDoneInFuture;

            return predictedMissingEstimatedEffortAtEnd;
        }

        // --------------------------------------------------
        public double CalculatePredictedMissingActualDays(double daysPresentTillToday, double daysPresentTotal, double estimatedEffortOpen, double speed)
        {
            if (speed <= 0)
                return -1;

            double predictedMissingEstimatedEffortAtEnd = CalculatePredictedMissingEstimatedEffortAtEnd(daysPresentTillToday, daysPresentTotal, estimatedEffortOpen, speed);

            // predicted Buffer in actual Days:
            return predictedMissingEstimatedEffortAtEnd / speed;
        }

        // --------------------------------------------------
        private async Task<double> CalculateTotalEffort(string traineeId)
        {
            var lessons = await _traineeLessonRepository.GetAllTraineeLessonsOfTraineeWithLessonAsync(traineeId);

            return lessons
                .Where(tl => tl.State != TraineeLessonState.Skipped &&
                            !(tl.Lesson.IsInactive && tl.State == TraineeLessonState.Open))
                .Sum(tl => tl.Lesson.EstimatedEffort);
        }

        // --------------------------------------------------
    }
}