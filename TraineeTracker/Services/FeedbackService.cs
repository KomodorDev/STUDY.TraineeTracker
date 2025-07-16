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
            string sortBy = "date_asc",
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
            // Get (all active lessons by trainee) OR (all lessons for all teachingPlans)
            List<Lesson> lessons;
            if (!string.IsNullOrEmpty(selectedTraineeId)) {

                /* 
                Console.WriteLine($"[DEBUG] selectedTraineeId: {selectedTraineeId}");
                Console.WriteLine($"[DEBUG] sortBy: {sortBy}");
                */

                // Get Trainee and the Lessons they wrote feedback for
                ApplicationUser? traineeNullable = await _databaseApplicaionUserRepository.FindByIdWithWrittenFeedbacksWithLessonAsync(selectedTraineeId!);

                ApplicationUser trainee = traineeNullable ?? throw new InvalidOperationException("User not found.");

                lessons = trainee.WrittenFeedbacks
                    .Select(f => f.Lesson)
                    .OrderBy(l => l.SortingIndex)
                    .ToList();
            } else {
                // Get all Lessons that have at least one feedback
                lessons = (await _databaseLessonRepository.GetAllLessonsWithFeedbacksAsync())
                    .Where(l => l.Feedbacks != null && l.Feedbacks.Any())
                    .OrderBy(l => l.TeachingPlanId)
                    .ThenBy(l => l.SortingIndex)
                    .ToList();
            }

            // +++++++++++++++
            // Get Pages for "Unread", "Read", and "All":
            Page<FeedbackDashboardDto> feedbackPage = filter switch {
                "unread" => await GetUnreadFeedbacksAsync(appUser, page, sortBy, selectedTraineeId, selectedLessonId),
                "read" => await GetReadFeedbacksAsync(appUser, page, sortBy, selectedTraineeId, selectedLessonId),
                _ => await GetAllFeedbacksAsync(appUser, page, sortBy, selectedTraineeId, selectedLessonId)
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
                SortBy = sortBy,

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
        private static IQueryable<Feedback> ApplySortingAndFiltering(
            IQueryable<Feedback> query,
            string sortBy,
            string? selectedTraineeId = null,
            int? selectedLessonId = null) {

            // Filter by Author (Trainee)
            if (!string.IsNullOrEmpty(selectedTraineeId))
                query = query.Where(f => f.Author.Id == selectedTraineeId);

            // +++++++++++++++
            // Filter by Lesson
            if (selectedLessonId.HasValue)
                query = query.Where(f => f.Lesson.LessonId == selectedLessonId);

            return sortBy.ToLower() switch {
                "author_asc" => query.OrderBy(f => f.Author.UserName),
                "author_desc" => query.OrderByDescending(f => f.Author.UserName),
                "lesson_asc" => query.OrderBy(f => f.Lesson.Title),
                "lesson_desc" => query.OrderByDescending(f => f.Lesson.Title),
                "date_asc" => query.OrderBy(f => f.CreateTime),
                _ => query.OrderByDescending(f => f.CreateTime) // default fallback
            };
        }

        // ------------------------------------------------------
        private async Task<Page<FeedbackDashboardDto>> GetAllFeedbacksAsync(
            ApplicationUser currentUser,
            int page,
            string sortBy,
            string? selectedTraineeId = null,
            int? selectedLessonId = null) {

            // +++++++++++++++
            // Get current userId
            var currentUserId = currentUser.Id;

            // +++++++++++++++
            // Buld query to get all Feedbacks
            var query = _databaseFeedbackRepository.GetAllFeedbacksWithLessonAndAuthorAndReadByUsers();

            /* 
            Console.WriteLine($"[DEBUG] GetAllFeedbacksAsync is called");
            Console.WriteLine($"[DEBUG] selectedTraineeId: {selectedTraineeId}");
            Console.WriteLine($"[DEBUG] selectedLessonId: {selectedLessonId}");

            */

            // +++++++++++++++
            // Apply Sorting and Filtering
            query = ApplySortingAndFiltering(query, sortBy, selectedTraineeId, selectedLessonId);

            // +++++++++++++++
            // Get Count of all Feedbacks in Query
            var totalItems = await query.CountAsync();

            // +++++++++++++++
            // Only get actual Feedbacks for current page
            var items = await query
                .Skip((page - 1) * _pageSize)
                .Take(_pageSize)
                .ToListAsync();

            // +++++++++++++++
            // Build dtoItems out of Feedbacks
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

            // +++++++++++++++
            // Build Page with dtoItems and return
            return new Page<FeedbackDashboardDto> {
                Items = dtoItems,
                PageNumber = page,
                PageSize = _pageSize,
                TotalItems = totalItems
            };
        }

        // ------------------------------------------------------
        public async Task<Page<FeedbackDashboardDto>> GetReadFeedbacksAsync(
            ApplicationUser currentUser,
            int page,
            string sortBy,
            string? selectedTraineeId = null,
            int? selectedLessonId = null) {

            // +++++++++++++++
            // Get all Feedbacks read by currentUser
            var query = _databaseFeedbackRepository.GetAllFeedbacksReadByUserWithLessonAndAuthor(currentUser);

            // +++++++++++++++
            // Apply Sorting and Filtering
            query = ApplySortingAndFiltering(query, sortBy, selectedTraineeId, selectedLessonId);

            // +++++++++++++++
            // Get Count of all Feedbacks in Query
            var totalItems = await query.CountAsync();

            // +++++++++++++++
            // Only get actual Feedbacks for current page
            var items = await query
                .Skip((page - 1) * _pageSize)
                .Take(_pageSize)
                .ToListAsync();

            // +++++++++++++++
            // Build dtoItems out of Feedbacks
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

            // +++++++++++++++
            // Build Page with dtoItems and return
            return new Page<FeedbackDashboardDto> {
                Items = dtoItems,
                PageNumber = page,
                PageSize = _pageSize,
                TotalItems = totalItems
            };
        }

        // ------------------------------------------------------
        public async Task<Page<FeedbackDashboardDto>> GetUnreadFeedbacksAsync(
            ApplicationUser currentUser,
            int page,
            string sortBy,
            string? selectedTraineeId = null,
            int? selectedLessonId = null) {

            // +++++++++++++++
            // Get all Feedbacks unread by currentUser
            var query = _databaseFeedbackRepository.GetAllFeedbacksUnreadByUserWithLessonAndAuthorAndReadByUsers(currentUser);

            // +++++++++++++++
            // Apply Sorting and Filtering
            query = ApplySortingAndFiltering(query, sortBy, selectedTraineeId, selectedLessonId);

            // +++++++++++++++
            // Get Count of all Feedbacks in Query
            var totalItems = await query.CountAsync();

            // +++++++++++++++
            // Only get actual Feedbacks for current page
            var items = await query
                .Skip((page - 1) * _pageSize)
                .Take(_pageSize)
                .ToListAsync();

            // +++++++++++++++
            // Build dtoItems out of Feedbacks
            var dtoItems = items.Select(f => new FeedbackDashboardDto {
                FeedbackId = f.FeedbackId,
                IsReadByCurrentUser = false, // All all unread
                AuthorName = f.Author.UserName!,
                LessonTitle = f.Lesson.Title,
                Comment = f.Comment,
                Difficulty = f.Difficulty,
                PreviousKnowledge = f.PreviousKnowledge,
                HoursOfEffort = f.HoursOfEffort,
                CreateTime = f.CreateTime
            }).ToList();

            // +++++++++++++++
            // Build Page with dtoItems and return
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

            // +++++++++++++++
            // 1. Get currentUser
            var currentUser = await _databaseApplicaionUserRepository.GetUserAsync(userPrincipal);

            // +++++++++++++++
            // 2. Get Feedback with ReadByUsers
            var feedback = await _databaseFeedbackRepository
            .GetFeedbackByIDWithLessonAndAuthorAndReadByUsersAsync(feedbackId)
                ?? throw new KeyNotFoundException($"Feedback with ID {feedbackId} not found.");

            // +++++++++++++++
            // 3. Check if currentUser has already read the Feedback
            if (!feedback.ReadByUsers.Any(u => u.Id == currentUser!.Id)) {
                // If not, add the currentUser and save
                feedback.ReadByUsers.Add(currentUser!);
                await _databaseFeedbackRepository.UpdateAsync(feedback);
            }
        }

        // ------------------------------------------------------
        public async Task MarkFeedbackAsUnreadAsync(ClaimsPrincipal userPrincipal, int feedbackId) {

            // +++++++++++++++
            // 1. Get currentUser
            var currentUser = await _databaseApplicaionUserRepository.GetUserAsync(userPrincipal);

            // +++++++++++++++
            // 2. Get Feedback with ReadByUsers
            var feedback = await _databaseFeedbackRepository
                .GetFeedbackByIDWithLessonAndAuthorAndReadByUsersAsync(feedbackId)
                ?? throw new KeyNotFoundException($"Feedback with ID {feedbackId} not found.");

            // +++++++++++++++
            // 3. Check if currentUser has already read the Feedback
            var userToRemove = feedback.ReadByUsers.FirstOrDefault(u => u.Id == currentUser!.Id);
            if (userToRemove != null) {
                // If currentUser has read the Feedback, remove him
                feedback.ReadByUsers.Remove(userToRemove);
                await _databaseFeedbackRepository.UpdateAsync(feedback);
            }
        }

        // ------------------------------------------------------
        public async Task MarkFeedbackAsUnreadForEveryoneAsync(int feedbackId) {

            // +++++++++++++++
            // 1. Load Feedback with ReadByUsers
            var feedback = await _databaseFeedbackRepository
                .GetFeedbackByIDWithLessonAndAuthorAndReadByUsersAsync(feedbackId)
                ?? throw new KeyNotFoundException($"Feedback with ID {feedbackId} not found.");

            // +++++++++++++++
            // 2. Empty ReadByUsers
            feedback.ReadByUsers.Clear();

            // 3. Save
            await _databaseFeedbackRepository.UpdateAsync(feedback);
        }

        // ------------------------------------------------------

    }
}
