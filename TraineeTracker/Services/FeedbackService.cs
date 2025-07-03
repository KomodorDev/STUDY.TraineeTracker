using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TraineeTracker.Data.Feedbacks;
using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Models;
using TraineeTracker.Models.Domain;
using TraineeTracker.Models.Dtos;
using TraineeTracker.Models.ViewModels;
using TraineeTracker.Data.Lessons;


namespace TraineeTracker.Services {
    public class FeedbackService {
        private const int _pageSize = 20;
        private readonly IFeedbackRepository _databaseFeedbackRepository;
        private readonly IApplicationUserRepository _databaseApplicaionUserRepository;
        private readonly ILessonRepository _databaseLessonRepository;


        // ------------------------------------------------------
        public FeedbackService(IFeedbackRepository feedbackRepo, IApplicationUserRepository userRepo, ILessonRepository lessonRepo) {
            _databaseFeedbackRepository = feedbackRepo;
            _databaseApplicaionUserRepository = userRepo;
            _databaseLessonRepository = lessonRepo;
        }

        // ------------------------------------------------------
        public async Task<FeedbackDashboardViewModel> BuildFeedbackDashboardViewModelAsync(
            ClaimsPrincipal user,
            string filter = "all",
            int page = 1,
            string sortBy = "date",
            bool ascending = false,
            string? selectedTraineeId = null,
            int? selectedLessonId = null) {

            // +++++++++++++++
            // Get current user
            ApplicationUser? appUserNullable = await _databaseApplicaionUserRepository.GetUserAsync(user);
            ApplicationUser appUser = appUserNullable ?? throw new InvalidOperationException("User not found.");

            // +++++++++++++++
            // Get all activeTrainees
            var activeTrainees = await _databaseApplicaionUserRepository.GetOpenUsersInRoleAsync("Trainee");

            // +++++++++++++++
            // Get all active lessons by trainee or all lessons
            List<Lesson> lessons;
            if (!string.IsNullOrEmpty(selectedTraineeId)) {

                Console.WriteLine($"[DEBUG] selectedTraineeId: {selectedTraineeId}");

                // Get Trainee and its lessons
                ApplicationUser? traineeNullable = await _databaseApplicaionUserRepository.FindByIdWithTraineeLessonsWithLessonsAndTeachingPlanAsync(selectedTraineeId!);

                ApplicationUser trainee = traineeNullable ?? throw new InvalidOperationException("User not found.");


                lessons = trainee.TraineeLessons
                    .Where(tl => tl.Lesson is not null)
                    .Select(tl => tl.Lesson!)
                    .Distinct()
                    .ToList();
            } else {
                // Get all active Lessons
                lessons = (await _databaseLessonRepository.GetAllLessonsAsync())
                    .OrderBy(l => l.LessonId)
                    .ToList();

            }

            // +++++++++++++++
            // Get Pages
            Page<FeedbackDashboardDto> feedbackPage = filter switch {
                "unread" => await GetUnreadFeedbacksAsync(appUser, page, sortBy, ascending, selectedTraineeId, selectedLessonId),
                "read" => await GetReadFeedbacksAsync(appUser, page, sortBy, ascending, selectedTraineeId, selectedLessonId),
                _ => await GetAllFeedbacksAsync(appUser, page, sortBy, ascending, selectedTraineeId, selectedLessonId)
            };

            // +++++++++++++++
            // Get Count Numbers
            var query = _databaseFeedbackRepository.GetAllFeedbacksWithLessonAndAuthorAndReadByUsers();

            if (!string.IsNullOrEmpty(selectedTraineeId)) {
                query = query.Where(f => f.Author.Id == selectedTraineeId);
            }

            if (selectedLessonId.HasValue) {
                query = query.Where(f => f.Lesson.LessonId == selectedLessonId.Value);
            }

            var totalCount = await query.CountAsync();
            var readCount = await query.CountAsync(f => f.ReadByUsers.Contains(appUser));
            var unreadCount = totalCount - readCount;

            // +++++++++++++++
            // Return ViewModel
            return new FeedbackDashboardViewModel {
                Feedbacks = feedbackPage,
                ActiveFilter = filter,
                ActiveSortBy = sortBy,
                Ascending = ascending,

                Lessons = lessons,
                ActiveTrainees = activeTrainees,
                SelectedTraineeId = selectedTraineeId,
                SelectedLessonId = selectedLessonId,

                TotalFeedbackCount = totalCount,
                ReadFeedbackCount = readCount,
                UnreadFeedbackCount = unreadCount
            };
        }

        // ------------------------------------------------------
        private static IQueryable<Feedback> ApplySorting(IQueryable<Feedback> query, string sortBy, bool ascending) {
            return sortBy switch {
                "author" => ascending
                    ? query.OrderBy(f => f.Author.UserName)
                    : query.OrderByDescending(f => f.Author.UserName),
                "lesson" => ascending
                    ? query.OrderBy(f => f.Lesson.Title)
                    : query.OrderByDescending(f => f.Lesson.Title),
                _ => ascending
                    ? query.OrderBy(f => f.CreateTime)
                    : query.OrderByDescending(f => f.CreateTime)
            };
        }

        // ------------------------------------------------------
        private async Task<Page<FeedbackDashboardDto>> GetAllFeedbacksAsync(
            ApplicationUser currentUser,
            int page,
            string sortBy,
            bool ascending,
            string? selectedTraineeId = null,
            int? selectedLessonId = null) {
            var currentUserId = currentUser.Id;
            var query = _databaseFeedbackRepository.GetAllFeedbacksWithLessonAndAuthorAndReadByUsers();

            Console.WriteLine($"[DEBUG] GetAllFeedbacksAsync is called");
            Console.WriteLine($"[DEBUG] selectedTraineeId: {selectedTraineeId}");
            Console.WriteLine($"[DEBUG] selectedLessonId: {selectedLessonId}");
            // Filter nach Trainee (Author)
            if (!string.IsNullOrEmpty(selectedTraineeId))
                query = query.Where(f => f.Author.Id == selectedTraineeId);

            // Filter nach Lesson
            if (selectedLessonId.HasValue)
                query = query.Where(f => f.Lesson.LessonId == selectedLessonId);

            query = ApplySorting(query, sortBy, ascending);

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * _pageSize)
                .Take(_pageSize)
                .ToListAsync();

            var dtoItems = items.Select(f => new FeedbackDashboardDto {
                FeedbackId = f.FeedbackId,
                AuthorName = f.Author.UserName!,
                LessonTitle = f.Lesson.Title,
                Comment = f.Comment,
                Difficulty = f.Difficulty,
                PreviousKnowledge = f.PreviousKnowledge,
                HoursOfEffort = f.HoursOfEffort,
                CreateTime = f.CreateTime,
                IsReadByCurrentUser = f.ReadByUsers != null && f.ReadByUsers.Any(u => u.Id == currentUserId)
            }).ToList();

            return new Page<FeedbackDashboardDto> {
                Items = dtoItems,
                PageNumber = page,
                PageSize = _pageSize,
                TotalItems = totalItems
            };
        }


        // ------------------------------------------------------
        public async Task<Page<FeedbackDashboardDto>> GetReadFeedbacksAsync(
            ApplicationUser user, int page, string sortBy, bool ascending, string? selectedTraineeId = null,
            int? selectedLessonId = null) {
            var query = _databaseFeedbackRepository.GetAllFeedbacksReadByUserWithLessonAndAuthor(user);


            // Filter nach Trainee (Author)
            if (!string.IsNullOrEmpty(selectedTraineeId))
                query = query.Where(f => f.Author.Id == selectedTraineeId);

            // Filter nach Lesson
            if (selectedLessonId.HasValue)
                query = query.Where(f => f.Lesson.LessonId == selectedLessonId);

            query = ApplySorting(query, sortBy, ascending);

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * _pageSize)
                .Take(_pageSize)
                .ToListAsync();

            var dtoItems = items.Select(f => new FeedbackDashboardDto {
                FeedbackId = f.FeedbackId,
                IsReadByCurrentUser = true,  // Alle sind gelesen
                AuthorName = f.Author.UserName!,
                LessonTitle = f.Lesson.Title,
                Comment = f.Comment,
                Difficulty = f.Difficulty,
                PreviousKnowledge = f.PreviousKnowledge,
                HoursOfEffort = f.HoursOfEffort,
                CreateTime = f.CreateTime
            }).ToList();

            return new Page<FeedbackDashboardDto> {
                Items = dtoItems,
                PageNumber = page,
                PageSize = _pageSize,
                TotalItems = totalItems
            };
        }

        // ------------------------------------------------------
        public async Task<Page<FeedbackDashboardDto>> GetUnreadFeedbacksAsync(
            ApplicationUser user, int page, string sortBy, bool ascending, string? selectedTraineeId = null,
            int? selectedLessonId = null) {
            var query = _databaseFeedbackRepository.GetAllFeedbacksUnreadByUserWithLessonAndAuthorAndReadByUsers(user);

            // Filter nach Trainee (Author)
            if (!string.IsNullOrEmpty(selectedTraineeId))
                query = query.Where(f => f.Author.Id == selectedTraineeId);

            // Filter nach Lesson
            if (selectedLessonId.HasValue)
                query = query.Where(f => f.Lesson.LessonId == selectedLessonId);

            query = ApplySorting(query, sortBy, ascending);

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * _pageSize)
                .Take(_pageSize)
                .ToListAsync();

            var dtoItems = items.Select(f => new FeedbackDashboardDto {
                FeedbackId = f.FeedbackId,
                IsReadByCurrentUser = false, // Alle sind ungelesen
                AuthorName = f.Author.UserName!,
                LessonTitle = f.Lesson.Title,
                Comment = f.Comment,
                Difficulty = f.Difficulty,
                PreviousKnowledge = f.PreviousKnowledge,
                HoursOfEffort = f.HoursOfEffort,
                CreateTime = f.CreateTime
            }).ToList();

            return new Page<FeedbackDashboardDto> {
                Items = dtoItems,
                PageNumber = page,
                PageSize = _pageSize,
                TotalItems = totalItems
            };
        }

        // ------------------------------------------------------
        // ------------------------------------------------------
        // ------------------------------------------------------
        // ------------------------------------------------------
        // ------------------------------------------------------                         
        // ------------------------------------------------------
        public async Task MarkFeedbackAsReadAsync(ClaimsPrincipal userPrincipal, int feedbackId) {
            // 1) Aktuellen User holen
            var appUser = await _databaseApplicaionUserRepository.GetUserAsync(userPrincipal);

            // 2) Feedback mit ReadByUsers laden
            var feedback = await _databaseFeedbackRepository
            .GetFeedbackByIDWithLessonAndAuthorAndReadByUsersAsync(feedbackId)
                ?? throw new KeyNotFoundException($"Feedback mit ID {feedbackId} nicht gefunden.");

            // 3) Prüfen, ob er es schon gelesen hat
            if (!feedback.ReadByUsers.Any(u => u.Id == appUser!.Id)) {
                // 4) Wenn nicht, zur Liste hinzufügen und speichern
                feedback.ReadByUsers.Add(appUser!);
                await _databaseFeedbackRepository.UpdateAsync(feedback);
            }
        }

        // ------------------------------------------------------
        public async Task MarkFeedbackAsUnreadAsync(ClaimsPrincipal userPrincipal, int feedbackId) {
            // 1) Aktuellen User holen
            var appUser = await _databaseApplicaionUserRepository.GetUserAsync(userPrincipal);

            // 2) Feedback mit ReadByUsers laden
            var feedback = await _databaseFeedbackRepository
                .GetFeedbackByIDWithLessonAndAuthorAndReadByUsersAsync(feedbackId)
                ?? throw new KeyNotFoundException($"Feedback mit ID {feedbackId} nicht gefunden.");

            // 3) Prüfen, ob der User in der Liste ist
            var userToRemove = feedback.ReadByUsers.FirstOrDefault(u => u.Id == appUser!.Id);
            if (userToRemove != null) {
                feedback.ReadByUsers.Remove(userToRemove);
                await _databaseFeedbackRepository.UpdateAsync(feedback);
            }
        }
        // ------------------------------------------------------
    }
}
