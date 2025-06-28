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

namespace TraineeTracker.Services {
    public class TraineeStatisticsService {
        private readonly ITraineeStatisticsRepository _traineeStatisticsRepository;
        private readonly ITraineeLessonRepository _traineeLessonRepository;
        private readonly HttpClient _httpClient;
        private readonly UserManager<ApplicationUser> _userManager;

        // --------------------------------------------------
        public TraineeStatisticsService(ITraineeStatisticsRepository traineeStatisticsRepository, ITraineeLessonRepository traineeLessonRepository, HttpClient httpClient, UserManager<ApplicationUser> userManager) {
            _traineeStatisticsRepository = traineeStatisticsRepository;
            _traineeLessonRepository = traineeLessonRepository;
            _httpClient = httpClient;
            _userManager = userManager;
        }

        // --------------------------------------------------
        public TraineeStatisticsViewModel BuildTraineeStatisticsViewModel(string traineeId) {
            var snapshot = _traineeStatisticsRepository.GetTraineeStatisticsSnapshot(traineeId);

            return new TraineeStatisticsViewModel {
                SnapshotDate = snapshot.SnapshotDate,
                DaysPresent = snapshot.DaysPresent,
                LessonDaysCompleted = snapshot.LessonDaysCompleted,
                LessonDaysOpen = snapshot.LessonDaysOpen,
                LessonDaysBuffer = snapshot.LessonDaysBuffer,
                Speed = snapshot.Speed,
                DaysBufferPredicted = snapshot.DaysBufferPredicted
            };
        }

        // --------------------------------------------------
        public bool CheckHasAccess(ClaimsPrincipal user, string traineeId) {
            var currentUserId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentUserId == traineeId) {
                return true;
            }

            if (user.IsInRole("Admin") || user.IsInRole("Mentor")) {
                return true;
            }

            return false;
        }

        // --------------------------------------------------
        public async Task<TraineeStatisticsSnapshot> BuildLatestTraineeStatisticsSnapshotAsync(string traineeId) {
            var trainee = await _userManager.FindByIdAsync(traineeId);
            if (trainee == null || trainee.TraineeStartDate == null) {
                throw new Exception("Trainee not found or start date is missing.");
            }

            var startDate = trainee.TraineeStartDate.Value;
            var endDate = trainee.TraineeEndDate ?? DateTime.Today;
            var email = trainee.Email ?? throw new Exception("Trainee has no email.");

            double daysPresent = await GetEffectivePresentDaysAsync(trainee);
            if (daysPresent < 0) {
                Console.WriteLine("⚠️ API-Error – use latest snapshot.");
                return _traineeStatisticsRepository.GetTraineeStatisticsSnapshot(traineeId);
            }
            double lessonDaysCompleted = await CalculateLessonDaysCompletedAsync(traineeId);
            double lessonDaysOpen = await CalculateLessonDaysOpenAsync(traineeId);
            double lessonDaysBuffer = CalculateLessonDaysBuffer(daysPresent, lessonDaysCompleted);
            double speed = CalculateSpeed(daysPresent, lessonDaysCompleted);
            double daysBufferPredicted = await CalculateDaysBufferPredictionAsync(traineeId, daysPresent, lessonDaysOpen, speed);

            var snapshot = new TraineeStatisticsSnapshot {
                Trainee = trainee,
                TraineeId = traineeId,
                SnapshotDate = DateTime.Now,
                DaysPresent = daysPresent,
                LessonDaysCompleted = lessonDaysCompleted,
                LessonDaysOpen = lessonDaysOpen,
                LessonDaysBuffer = lessonDaysBuffer,
                Speed = speed,
                DaysBufferPredicted = daysBufferPredicted,
            };

            _traineeStatisticsRepository.Create(snapshot);

            return snapshot;
        }

        // --------------------------------------------------
        public async Task<double> GetPresentDaysAsync(DateTime startDate, DateTime endDate, string email) {
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
        private async Task<double> GetEffectivePresentDaysAsync(ApplicationUser trainee) {
            var startDate = trainee.TraineeStartDate ?? throw new Exception("Startdatum fehlt");
            var endDate = trainee.TraineeEndDate ?? DateTime.Today;
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
                        TraineeLessonState.Accepted => effort,
                        TraineeLessonState.Rejected => effort * 0.8,
                        TraineeLessonState.Rated => effort,
                        _ => 0
                    };
                });
        }

        // --------------------------------------------------
        public async Task<double> CalculateLessonDaysOpenAsync(string traineeId) {
            var lessons = await _traineeLessonRepository.GetAllTraineeLessonsOfTraineeWithLessonAsync(traineeId);

            var relevantLessons = lessons
                .Where(tl => tl.State != TraineeLessonState.Skipped &&
                            !(tl.Lesson.IsInactive && tl.State == TraineeLessonState.Open));

            double totalEffort = relevantLessons.Sum(tl => tl.Lesson.EstimatedEffort);

            double completedEffort = await CalculateLessonDaysCompletedAsync(traineeId);

            return totalEffort - completedEffort;
        }

        // --------------------------------------------------
        public double CalculateLessonDaysBuffer(double daysPresent, double lessonDaysCompleted)
        {
            return lessonDaysCompleted - daysPresent;
        }
        
        // --------------------------------------------------
        public double CalculateSpeed(double daysPresent, double lessonDaysCompleted)
        {
            return daysPresent > 0 ? lessonDaysCompleted / daysPresent : 0;
        }

        // --------------------------------------------------
        public async Task<double> CalculateDaysBufferPredictionAsync(string traineeId, double daysPresent, double lessonDaysOpen, double speed)
        {
            var traineeLessons = await _traineeLessonRepository.GetAllTraineeLessonsOfTraineeWithLessonAsync(traineeId);

            double targetEffortInDays = traineeLessons
                .Where(tl => tl.State != TraineeLessonState.Skipped &&
                            !(tl.Lesson.IsInactive && tl.State == TraineeLessonState.Open)
                )
                .Select(tl => tl.Lesson.EstimatedEffort)
                .Sum();

            if (speed <= 0)
                return -1;

            double daysLeft = targetEffortInDays - daysPresent;
            double daysNeeded = lessonDaysOpen / speed;

            return daysLeft - daysNeeded;
        }
    }
}