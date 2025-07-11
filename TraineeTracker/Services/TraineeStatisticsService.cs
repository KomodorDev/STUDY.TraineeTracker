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
        public TraineeStatisticsService(ITraineeStatisticsRepository traineeStatisticsRepository, ITraineeLessonRepository traineeLessonRepository, HttpClient httpClient, UserManager<ApplicationUser> userManager, IProcessingPauseRepository processingPauseRepository) {
            _traineeStatisticsRepository = traineeStatisticsRepository;
            _traineeLessonRepository = traineeLessonRepository;
            _httpClient = httpClient;
            _userManager = userManager;
            _processingPauseRepository = processingPauseRepository;
        }

        // --------------------------------------------------
        public async Task<TraineeStatisticsViewModel> BuildTraineeStatisticsViewModel(string traineeId, ClaimsPrincipal user) {
            CheckHasAccess(user, traineeId);

            // ++++++++++++++++
            // Get Data for ViewModel
            var snapshot = await BuildLatestTraineeStatisticsSnapshotAsync(traineeId);
            var lessons = await _traineeLessonRepository.GetAllTraineeLessonsOfTraineeWithLessonAsync(traineeId);
            var processingPauses = (await _processingPauseRepository.GetAllPausesAsync(traineeId)).ToList();

            // ++++++++++++++++
            // all lessons for chart
            var allLessons = new List<TraineeLessonViewModel>();

            void AddWithStatus(List<TraineeLessonViewModel>? lessons, string status) {
                if (lessons == null)
                    return;
                foreach (var lesson in lessons) {
                    lesson.Status = status;
                    allLessons.Add(lesson);
                }
            }

            // ++++++++++++++++
            // Build and return ViewModel
            var model = new TraineeStatisticsViewModel {
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

                // ++++++++++++++++
                // Lesson Lists
                FinishedLessons = lessons
                    .Where(l => l.State == TraineeLessonState.Finished)
                    .Select(l => new TraineeLessonViewModel {
                        Title = l.Lesson.Title,
                        EstimatedEffort = l.Lesson.EstimatedEffort,
                        WeightedEffort = l.Lesson.EstimatedEffort * 0.7,
                        State = l.State
                    }).ToList(),
                AcceptedLessons = lessons
                    .Where(l => l.State == TraineeLessonState.Accepted)
                    .Select(l => new TraineeLessonViewModel {
                        Title = l.Lesson.Title,
                        EstimatedEffort = l.Lesson.EstimatedEffort,
                        WeightedEffort = l.Lesson.EstimatedEffort * 1.0,
                        State = l.State
                    }).ToList(),
                RatedLessons = lessons
                    .Where(l => l.State == TraineeLessonState.Rated)
                    .Select(l => new TraineeLessonViewModel {
                        Title = l.Lesson.Title,
                        EstimatedEffort = l.Lesson.EstimatedEffort,
                        WeightedEffort = l.Lesson.EstimatedEffort * 1.0,
                        State = l.State
                    }).ToList(),
                RejectedLessons = lessons
                    .Where(l => l.State == TraineeLessonState.Rejected)
                    .Select(l => new TraineeLessonViewModel {
                        Title = l.Lesson.Title,
                        EstimatedEffort = l.Lesson.EstimatedEffort,
                        WeightedEffort = l.Lesson.EstimatedEffort * 0.8,
                        State = l.State
                    }).ToList(),
                OpenLessons = lessons
                    .Where(l => l.State == TraineeLessonState.Open)
                    .Select(l => new TraineeLessonViewModel {
                        Title = l.Lesson.Title,
                        EstimatedEffort = l.Lesson.EstimatedEffort,
                        WeightedEffort = 0,
                        State = l.State
                    }).ToList(),
                StartedLessons = lessons
                    .Where(l => l.State == TraineeLessonState.Started)
                    .Select(l => new TraineeLessonViewModel {
                        Title = l.Lesson.Title,
                        EstimatedEffort = l.Lesson.EstimatedEffort,
                        WeightedEffort = 0,
                        State = l.State
                    }).ToList(),

                // ++++++++++++++++
                // ProcessingPauses
                ProcessingPauses = processingPauses
            };

            AddWithStatus(model.RatedLessons, "rated");
            AddWithStatus(model.AcceptedLessons, "accepted");
            AddWithStatus(model.FinishedLessons, "finished");
            AddWithStatus(model.RejectedLessons, "rejected");
            AddWithStatus(model.StartedLessons, "started");
            AddWithStatus(model.OpenLessons, "open");

            model.AllLessons = allLessons;

            model.TodayPosition = (
                (model.FinishedLessons?.Sum(l => l.WeightedEffort) ?? 0) +
                (model.AcceptedLessons?.Sum(l => l.WeightedEffort) ?? 0) +
                (model.RatedLessons?.Sum(l => l.WeightedEffort) ?? 0) +
                (model.RejectedLessons?.Sum(l => l.WeightedEffort) ?? 0)
            );

            return model;
        }

        // --------------------------------------------------
        public void CheckHasAccess(ClaimsPrincipal user, string traineeId) {
            Console.WriteLine("TraineeStatisticsService - CheckHasAccess called");
            if (user == null)
                throw new UserNotFoundException();

            var currentUserId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (user.IsInRole("Admin") || user.IsInRole("Mentor")) {
                return;
            }

            if (!(currentUserId == traineeId)) {
                throw new UnauthorizedAccessException("You can only view your own statistics.");
            }

        }

        // --------------------------------------------------
        public async Task<TraineeStatisticsSnapshot> BuildLatestTraineeStatisticsSnapshotAsync(string traineeId) {

            // ++++++++++++++++
            // Get Trainee
            var trainee = await _userManager.FindByIdAsync(traineeId);
            if (trainee == null || trainee.TraineeStartDate == null) {
                throw new Exception("Trainee not found or start date is missing.");
            }

            // ++++++++++++++++
            // Calculate Data for Trainee
            double daysPresentTotal = await GetEffectivePresentDaysAsync(trainee, trainee.TraineeStartDate!.Value, trainee.TraineeEndDate!.Value);
            double daysPresentTillToday = await GetEffectivePresentDaysAsync(trainee, trainee.TraineeStartDate!.Value, DateOnly.FromDateTime(DateTime.Today));
            if (daysPresentTillToday < 0) {

                // Return Fallback Snapshot if no API Access
                Console.WriteLine("⚠️ API-Error – use latest snapshot.");
                try {
                    // Case 1: Snapshot in DB exists and is returned
                    var fallbackSnapshot = await _traineeStatisticsRepository.GetTraineeStatisticsSnapshotAsync(traineeId);
                    fallbackSnapshot.IsUpToDate = false;
                    return fallbackSnapshot;
                }
                catch (InvalidOperationException) {
                    // Case 2: Snapshot does not exist in DB. We create one, store it in DB, and return it
                    var newSnapshot = new TraineeStatisticsSnapshot {
                        Trainee = trainee,
                        TraineeId = traineeId,
                        SnapshotDateTime = DateTime.Now,
                        DaysPresentTotal = null,
                        DaysPresentTillToday = null,
                        LessonDaysCompleted = null,
                        LessonDaysOpen = null,
                        LessonDaysBuffer = null,
                        Speed = null,
                        PredictedMissingEstimatedEffortAtEnd = null,
                        PredictedMissingActualDays = null,
                        IsUpToDate = false
                    };

                    await _traineeStatisticsRepository.CreateAsync(newSnapshot);
                    return newSnapshot;
                }
            }
            double lessonDaysCompleted = await CalculateLessonDaysCompletedAsync(traineeId);
            double lessonDaysOpen = await CalculateLessonDaysOpenAsync(traineeId);
            double lessonDaysBuffer = CalculateLessonDaysBuffer(daysPresentTillToday, lessonDaysCompleted);
            double speed = CalculateSpeed(daysPresentTillToday, lessonDaysCompleted);
            double predictedMissingEstimatedEffortAtEnd = CalculatePredictedMissingEstimatedEffortAtEnd(daysPresentTillToday, daysPresentTotal, lessonDaysOpen, speed);
            double predictedMissingActualDays = CalculatePredictedMissingActualDaysAtEnd(daysPresentTillToday, daysPresentTotal, lessonDaysOpen, speed);

            TraineeStatisticsSnapshot snapshot;

            // ++++++++++++++++
            // Update or Create Snapshot
            try {

                // Case 1 - Update: Snapshot exisit and we update it
                snapshot = await _traineeStatisticsRepository.GetTraineeStatisticsSnapshotAsync(traineeId);

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

                // Case 2 - Create: Snapshot does not exist and we create it
                snapshot = new TraineeStatisticsSnapshot {
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

            // ++++++++++++++++
            // Return Snapshot
            return snapshot;
        }

        // --------------------------------------------------
        public async Task<double> GetPresentDaysAsync(DateOnly startDate, DateOnly endDate, string email) {

            double totalDays = endDate.DayNumber - startDate.DayNumber;
            Console.WriteLine("TotalDays:" + totalDays + " for " + email);
            return totalDays * 0.7;
            
            // ++++++++++++++++
            // Build Request
            var baseUrl = "https://api.sopro.makandra.de/api/v1/present_days";
            var url = $"{baseUrl}?email={Uri.EscapeDataString(email)}&start_date={startDate:yyyy-MM-dd}&end_date={endDate:yyyy-MM-dd}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var byteArray = System.Text.Encoding.ASCII.GetBytes("sopro:capybara");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            // ++++++++++++++++
            // Send Request and Wait for Response
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode) {
                Console.WriteLine($"❌ API-Error: {response.StatusCode} - {response.ReasonPhrase}");
                return -1;
            }

            var content = await response.Content.ReadAsStringAsync();

            // ++++++++++++++++
            // Try to extract and return presentDays from Response
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

            // ++++++++++++++++
            // Checks
            if (trainee.TraineeStartDate is null || trainee.TraineeEndDate is null)
                throw new Exception("StartDate or EndDate is missing");

            var email = trainee.Email ?? throw new Exception("E-Mail is missing");

            // ++++++++++++++++
            // Get totalDays
            double totalDays = await GetPresentDaysAsync(startDate, endDate, email);

            if (totalDays < 0)
                return -1;

            // ++++++++++++++++
            // Get pauseDaysTotal
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

            // ++++++++++++++++
            // Calculate effectivePresentDays and return
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
            double daysPresentDaysInFuture = daysPresentTotal - daysPresentTillToday;

            // Predicted EstimatedEffort completed from today till EndDate:
            double predictedEstimatedEffortDoneInFuture = daysPresentDaysInFuture * speed;

            // predicted Buffer in EstimatedEffort: estimatedEffort remaining at EndDate
            double predictedMissingEstimatedEffortAtEnd = predictedEstimatedEffortDoneInFuture - estimatedEffortOpen;

            return predictedMissingEstimatedEffortAtEnd;
        }

        // --------------------------------------------------
        public double CalculatePredictedMissingActualDaysAtEnd(double daysPresentTillToday, double daysPresentTotal, double estimatedEffortOpen, double speed) {
            if (speed <= 0)
                return -1;

            double predictedMissingEstimatedEffortAtEnd = CalculatePredictedMissingEstimatedEffortAtEnd(daysPresentTillToday, daysPresentTotal, estimatedEffortOpen, speed);

            // predicted Buffer in actual Days:
            return predictedMissingEstimatedEffortAtEnd / speed;
        }

        // --------------------------------------------------
        private async Task<double> CalculateTotalEffort(string traineeId) {
            var lessons = await _traineeLessonRepository.GetAllTraineeLessonsOfTraineeWithLessonAsync(traineeId);

            return lessons
                .Where(tl => tl.State != TraineeLessonState.Skipped &&
                            !(tl.Lesson.IsInactive && tl.State == TraineeLessonState.Open))
                .Sum(tl => tl.Lesson.EstimatedEffort);
        }

        // --------------------------------------------------
    }
}