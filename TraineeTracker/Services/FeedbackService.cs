using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TraineeTracker.Data.Feedbacks;
using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Models;

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

        public async Task<Page<FeedbackDto>> GetAllFeedbacksAsync(int pageNumber)
        {
            if (pageNumber < 1)
                throw new ArgumentOutOfRangeException(nameof(pageNumber));

            // Alle Feedbacks laden und sortieren
            var all = await _feedbackRepo
            .GetAllFeedbacksWithLessonAndAuthorAndReadByUsers()
            .OrderByDescending(f => f.CreateTime)
            .ToListAsync();

            return CreatePagedResult(all, pageNumber);
        }

        public async Task<Page<FeedbackDto>> GetUnreadFeedbacksAsync(
            ClaimsPrincipal userPrincipal, int pageNumber)
        {
            if (pageNumber < 1)
                throw new ArgumentOutOfRangeException(nameof(pageNumber));

            var appUser = await GetUserFromPrincipalAsync(userPrincipal);

            // Nur unge­lesene Feedbacks für diesen User
            var unread = await _feedbackRepo
            .GetUnreadForUserAsync(appUser.Id)
            .OrderByDescending(f => f.CreateTime)
            .ToListAsync();

            return CreatePagedResult(unread, pageNumber);
        }

        private Page<FeedbackDto> CreatePagedResult(
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
                .Select(f => new FeedbackDto {
                    AuthorName = f.Author.UserName,
                    SendDate   = f.CreateTime,
                    Comment    = f.Comment
                })
                .ToList();

                return new Page<FeedbackDto> {
                    Items      = items,
                    PageNumber = pageNumber,
                    PageSize   = PageSize,
                    TotalItems = totalItems
                };
        }

        public async Task MarkFeedbackAsReadAsync(
            int feedbackId, ClaimsPrincipal userPrincipal)
        {
            var appUser = await GetUserFromPrincipalAsync(userPrincipal);

            // Feedback aus Repo holen
            var feedback = await _feedbackRepo
            .GetUnreadForUserAsync(appUser.Id)
            .FirstOrDefaultAsync(f => f.FeedbackId == feedbackId)
            ?? throw new InvalidOperationException("Feedback nicht gefunden oder bereits gelesen.");

            feedback.ReadByUsers.Add(appUser);
            await _feedbackRepo.UpdateAsync(feedback);
        }
    }
}
