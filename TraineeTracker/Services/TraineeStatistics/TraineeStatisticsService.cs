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

namespace TraineeTracker.Services
{
    public class TraineeStatisticsService
    {
        private readonly ITraineeStatisticsRepository _traineeStatisticsRepository;
        private readonly HttpClient _httpClient;

        public TraineeStatisticsService(ITraineeStatisticsRepository traineeStatisticsRepository, HttpClient httpClient)
        {
            _traineeStatisticsRepository = traineeStatisticsRepository;
            _httpClient = httpClient;
        }

        public async Task<TraineeStatisticsViewModel> BuildTraineeStatisticsViewModel(string traineeId)
        {
            var snapshot = _traineeStatisticsRepository.GetTraineeStatisticsSnapshot(traineeId);

            return new TraineeStatisticsViewModel
            {
                SnapshotDate = snapshot.SnapshotDate,
                DaysPresent = snapshot.DaysPresent,
                LessonDaysCompleted = snapshot.LessonDaysCompleted,
                LessonDaysOpen = snapshot.LessonDaysOpen,
                LessonDaysBuffer = snapshot.LessonDaysBuffer,
                Speed = snapshot.Speed,
                DaysBufferPredicted = snapshot.DaysBufferPredicted
            };
        }

        public bool CheckHasAccess(ClaimsPrincipal user, string traineeId)
        {
            var currentUserId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentUserId == traineeId)
            {
                return true;
            }

            if (user.IsInRole("Admin") || user.IsInRole("Mentor"))
            {
                return true;
            }

            return false;
        }

        public async Task<TraineeStatisticsSnapshot> BuildLatestTraineeStatisticsSnapshot(string traineeId, DateTime startDate, DateTime endDate, string email)
        {
            double daysPresent = await GetPresentDays(startDate, endDate, email);
            if (daysPresent < 0)
            {
                Console.WriteLine("⚠️ API-Error – use latest snapshot.");
                return _traineeStatisticsRepository.GetTraineeStatisticsSnapshot(traineeId);
            }
            double lessonDaysCompleted = CalculateLessonDaysCompleted();
            double lessonDaysOpen = CalculateLessonDaysOpen();
            double lessonDaysBuffer = CalculateLessonDaysBuffer(daysPresent, lessonDaysCompleted);
            double speed = CalculateSpeed(daysPresent, lessonDaysCompleted);
            double daysBufferPredicted = CalculateDaysBufferPrediction(daysPresent, lessonDaysCompleted, speed, lessonDaysOpen);

            return new TraineeStatisticsSnapshot
            {
                TraineeId = traineeId,
                SnapshotDate = DateTime.Now,
                DaysPresent = daysPresent,
                LessonDaysCompleted = lessonDaysCompleted,
                LessonDaysOpen = lessonDaysOpen,
                LessonDaysBuffer = lessonDaysBuffer,
                Speed = speed,
                DaysBufferPredicted = daysBufferPredicted,
            };
        }

        private async Task<double> GetPresentDays(DateTime startDate, DateTime endDate, string email)
        {
            var baseUrl = "https://api.sopro.makandra.de/api/v1/present_days";
            var url = $"{baseUrl}?email={Uri.EscapeDataString(email)}&start_date={startDate:yyyy-MM-dd}&end_date={endDate:yyyy-MM-dd}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var byteArray = System.Text.Encoding.ASCII.GetBytes("sopro:capybara");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"❌ API-Error: {response.StatusCode} - {response.ReasonPhrase}");
                return -1;
            }

            var content = await response.Content.ReadAsStringAsync();

            try
            {
                using var json = System.Text.Json.JsonDocument.Parse(content);
                var root = json.RootElement;
                return root.GetProperty("present_days").GetDouble();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ JSON-Error: {ex.Message}");
                return -1;
            }
        }

        public double CalculateLessonDaysCompleted()
        {
            return 10; //Infos dazu müssen noch von woanders übergeben werden
        }

        public double CalculateLessonDaysOpen()
        {
            return 5; //Infos dazu müssen noch von woanders übergeben werden
        }

        public double CalculateLessonDaysBuffer(double daysPresent, double lessonDaysCompleted)
        {
            return lessonDaysCompleted - daysPresent;
        }

        public double CalculateSpeed(double daysPresent, double lessonDaysCompleted)
        {
            return daysPresent > 0 ? lessonDaysCompleted / daysPresent : 0;
        }

        public double CalculateDaysBufferPrediction(double daysPresent, double lessonDaysCompleted, double speed, double lessonDaysOpen)
        {
            double totalProgramDays = 100; ///bisher nur Beispiel für die Funktionsweise

            if (speed <= 0)
            {
                return -1;
            }
                
            double daysLeft = totalProgramDays - daysPresent; //totalProgramDays muss noch von woanders übergeben werden
            double requiredDays = lessonDaysOpen / speed;

            return daysLeft - requiredDays;
        }
    }
}