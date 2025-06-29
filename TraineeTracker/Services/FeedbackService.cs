using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TraineeTracker.Data.Feedbacks;
using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Models;
using TraineeTracker.Models.Domain;
using TraineeTracker.Models.Dtos;
using TraineeTracker.Models.ViewModels;


namespace TraineeTracker.Services
{
    public class FeedbackService
    {
        private const int PageSize = 20;
        private readonly IFeedbackRepository      _feedbackRepo;
        private readonly IApplicationUserRepository _userRepo;

        public FeedbackService(
            IFeedbackRepository feedbackRepo,
            IApplicationUserRepository userRepo)
        {
            _feedbackRepo = feedbackRepo;
            _userRepo     = userRepo;
        }

        private async Task<ApplicationUser> GetUserFromPrincipalAsync(ClaimsPrincipal userPrincipal)
        {
            var userId = userPrincipal
            .FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User nicht authentifiziert.");

            return await _userRepo
            .FindByIdAsync(userId)
            ?? throw new UnauthorizedAccessException("User nicht gefunden.");
        }

        public async Task<Page<FeedbackDashboardDto>> GetAllFeedbacksAsync(int pageNumber)
        {
            if (pageNumber < 1)
                throw new ArgumentOutOfRangeException(nameof(pageNumber));

            // Alle Feedbacks laden und sortieren
            var all = await _feedbackRepo.GetAllFeedbacksWithLessonAndAuthorAndReadByUsersAsync();


            return CreatePagedResult(all, pageNumber);
        }

        public async Task<Page<FeedbackDashboardDto>> GetUnreadFeedbacksAsync(
            ClaimsPrincipal userPrincipal, int pageNumber)
        {
            if (pageNumber < 1)
                throw new ArgumentOutOfRangeException(nameof(pageNumber));

            var appUser = await GetUserFromPrincipalAsync(userPrincipal);

            // Nur unge­lesene Feedbacks für diesen User
            var unread = await _feedbackRepo.GetAllFeedbacksUnreadByUserWithLessonAndAuthorAndReadByUsersAsync(appUser);

            return CreatePagedResult(unread, pageNumber);
        }

        public async Task<Page<FeedbackDashboardDto>> GetReadFeedbacksAsync(
            ClaimsPrincipal userPrincipal, int pageNumber)
        {
            if (pageNumber < 1)
                throw new ArgumentOutOfRangeException(nameof(pageNumber));

            // 1) Aktuellen User ermitteln
            var appUser = await GetUserFromPrincipalAsync(userPrincipal);

            // 2) Gelesene Feedbacks (ReadByUsers enthält den aktuellen User)
            var read = await _feedbackRepo.GetAllFeedbacksReadByUserWithLessonAndAuthorAndReadByUsersAsync(appUser);

            // 3) Ergebnis paginieren
            return CreatePagedResult(read, pageNumber);
        }

        private Page<FeedbackDashboardDto> CreatePagedResult(
            List<Feedback> source, int pageNumber)
        {
            int totalItems = source.Count;
            int totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            if (totalPages > 0 && pageNumber > totalPages)
                throw new ArgumentOutOfRangeException(nameof(pageNumber),
                                                      $"Maximal {totalPages} Seiten vorhanden.");

                var items = source
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .Select(f => new FeedbackDashboardDto {
                    FeedbackId = f.FeedbackId,
                    AuthorName = f.Author.UserName,
                    SendDate   = f.CreateTime,
                    Comment    = f.Comment
                })
                .ToList();

                return new Page<FeedbackDashboardDto> {
                    Items      = items,
                    PageNumber = pageNumber,
                    PageSize   = PageSize,
                    TotalItems = totalItems
                };
        }

        public async Task MarkFeedbackAsReadAsync(ClaimsPrincipal userPrincipal, int feedbackId)
        {
            // 1) Aktuellen User holen
            var appUser = await GetUserFromPrincipalAsync(userPrincipal);

            // 2) Feedback mit ReadByUsers laden
            var feedback = await _feedbackRepo
            .GetFeedbackByIDWithLessonAndAuthorAndReadByUsersAsync(feedbackId)
                ?? throw new KeyNotFoundException($"Feedback mit ID {feedbackId} nicht gefunden.");

            // 3) Prüfen, ob er es schon gelesen hat
            if (!feedback.ReadByUsers.Any(u => u.Id == appUser.Id))
            {
                // 4) Wenn nicht, zur Liste hinzufügen und speichern
                feedback.ReadByUsers.Add(appUser);
                await _feedbackRepo.UpdateAsync(feedback);
            }
        }


        public async Task<FeedbackDashboardViewModel> BuildFeedbackDashboardViewModelAsync(
            ClaimsPrincipal userPrincipal)
        {
            const int defaultPage = 1;

            // 1) Gesamte Feedback-Seite
            var allPage = await GetAllFeedbacksAsync(defaultPage);

            // 2) Ungelesene Feedback-Seite für den User
            var unreadPage = await GetUnreadFeedbacksAsync(userPrincipal, defaultPage);

            // 3) Gelesene Feedback-Seite für den User
            var readPage = await GetReadFeedbacksAsync(userPrincipal, defaultPage);

            // 4) ViewModel füllen und zurückgeben
            return new FeedbackDashboardViewModel
            {
                AllFeedbacks    = allPage,
                UnreadFeedbacks = unreadPage,
                ReadFeedbacks   = readPage
            };
        }
    }
}
