using System.Security.Claims;
using TraineeTracker.Data.Feedbacks;
using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Data.TraineeLessons;
using TraineeTracker.Data.TraineeStatistics;
using TraineeTracker.Models.Domain;
using TraineeTracker.Models.Dtos;

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
        public async Task<List<FeedbackViewModel>> BuildUnreadFeedbackViewModelsAsync(ClaimsPrincipal mentor) {

            var feedbacks = await GetUnreadFeedbacksForMentorAsync(mentor);

            // 2) in ViewModels umwandeln
            return feedbacks.Select(f => new FeedbackViewModel
            {
                SentAt     = f.CreateTime,
                AuthorName = f.Author.UserName,
                Comment    = f.Comment ?? string.Empty
            }).ToList();
        }
    }
}
