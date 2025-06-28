using System.Security.Claims;
using TraineeTracker.Data.Feedbacks;
using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Data.TraineeLessons;
using TraineeTracker.Data.TraineeStatistics;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Services
{
    public class FeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepo;
        private readonly IApplicationUserRepository _userRepo;
        private readonly ITraineeLessonRepository _traineeLessonRepo;
        private readonly ITraineeStatisticsRepository _statsRepo;

        public FeedbackService(
            IFeedbackRepository feedbackRepo,
            IApplicationUserRepository userRepo,
            ITraineeLessonRepository traineeLessonRepo,
            ITraineeStatisticsRepository statsRepo)
        {
            _feedbackRepo = feedbackRepo;
            _userRepo = userRepo;
            _traineeLessonRepo = traineeLessonRepo;
            _statsRepo = statsRepo;
        }

        // Holt alle Feedbacks, die der aktuell eingeloggte Mentor/Admin noch nicht als gelesen markiert hat. Kurwa Bober
        public async Task<List<Feedback>> GetUnreadFeedbacksForMentorAsync(ClaimsPrincipal mentor)
        {
            var userId = mentor.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? throw new InvalidOperationException("Unbekannter Benutzer.");
            var appUser = await _userRepo.FindByIdWithProcessingPausesAndTraineeLessonsAsync(userId)
                          ?? throw new InvalidOperationException("Mentor nicht gefunden.");

            // Nur Mentoren oder Admins dürfen hier anfragen
            var isMentor = await _userRepo.IsInRoleAsync(appUser, "Mentor");
            var isAdmin  = await _userRepo.IsInRoleAsync(appUser, "Admin");
            if (!isMentor && !isAdmin)
                throw new UnauthorizedAccessException("Zugriff nur für Mentor/Admin.");

            // Unread = alle Feedbacks, bei denen der User noch nicht in ReadByUsers steht
            var unread = await _feedbackRepo
                .GetAllFeedbacksUnreadByUserWithLessonAndAuthorAndReadByUsersAsync(appUser);
            return unread.ToList();
        }

        
        // Markiert ein Feedback als gelesen für den aktuell eingeloggten Mentor/Admin. Jairdo Kurwe
        public async Task MarkFeedbackAsReadAsync(int feedbackId, ClaimsPrincipal mentor)
        {
            var userId = mentor.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? throw new InvalidOperationException("Unbekannter Benutzer.");
            var appUser = await _userRepo.FindByIdWithProcessingPausesAndTraineeLessonsAsync(userId)
                          ?? throw new InvalidOperationException("Benutzer nicht gefunden.");

            // Lade das Feedback speedy Gonzales
            var feedback = await _feedbackRepo.GetAllFeedbacksUnreadByUserWithLessonAndAuthorAndReadByUsersAsync(appUser)
                                .ContinueWith(t => t.Result.FirstOrDefault(f => f.FeedbackId == feedbackId));
            if (feedback == null)
                throw new InvalidOperationException("Feedback nicht gefunden oder bereits gelesen.");

            // Als gelesen markieren
            if (!feedback.ReadByUsers.Any(u => u.Id == userId))
            {
                var userEntity = await _userRepo.FindByIdAsync(userId)
                                 ?? throw new InvalidOperationException("Benutzer nicht gefunden.");
                feedback.ReadByUsers.Add(userEntity);
                await _feedbackRepo.UpdateAsync(feedback);
            }
        }

        /// Baut ein Dashboard-Modell für einen Trainee, basierend auf seinem Snapshot und den aktuellen TraineeLesson-Daten.
        public TraineeStatisticsSnapshot BuildFeedBackDashboardViewModel(string traineeId, DateTime snapshotDate)
        {
            // 1. Existierenden Snapshot laden oder neu anlegen
            var snapshot = _statsRepo.Exists(0) 
                ? _statsRepo.GetTraineeStatisticsSnapshot(traineeId)
                : new TraineeStatisticsSnapshot
                {
                    TraineeId = traineeId,
                    SnapshotDate = snapshotDate
                };

            // 2. Alle TraineeLessons dieses Trainees laden
            var lessons = _traineeLessonRepo
                .GetAllTraineeLessonsOfTraineeWithLessonAsync(traineeId)
                .Result
                .ToList();

            // 3. Kennzahlen berechnen
            var user = _userRepo.FindByIdWithProcessingPausesAndTraineeLessonsAsync(traineeId).Result;
            var start = user?.TraineeStartDate ?? DateOnly.FromDateTime(snapshotDate);
            var totalDays = (snapshotDate.Date - start.ToDateTime(TimeOnly.MinValue).Date).TotalDays;
            snapshot.DaysPresent = totalDays; 

            var written = _feedbackRepo
                .GetAllFeedbacksWrittenByUserWithLessonAndAuthorAndReadByUsersAsync(user!)
                .Result;
            snapshot.LessonDaysCompleted = written.Sum(f => f.HoursOfEffort);

            var openLessons = lessons.Count(tl => tl.State == TraineeLessonState.Open);
            snapshot.LessonDaysOpen = openLessons;

            var estimated = lessons.Sum(tl => tl.Lesson.EstimatedEffort);
            snapshot.LessonDaysBuffer = estimated - snapshot.LessonDaysCompleted;

            snapshot.Speed =
                snapshot.LessonDaysCompleted / (totalDays > 0 ? totalDays : 1);

            snapshot.DaysBufferPredicted =
                snapshot.LessonDaysBuffer / (snapshot.Speed > 0 ? snapshot.Speed : 1);

            return snapshot;
        }
    }
}
