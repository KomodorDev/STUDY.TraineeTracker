using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TraineeTracker.Data.Feedbacks;
using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Models;
using TraineeTracker.Models.Domain;
using TraineeTracker.Models.Dtos;
using TraineeTracker.Models.ViewModels;
using TraineeTracker.Data.Lessons;
using TraineeTracker.Data.TeachingPlans;


namespace TraineeTracker.Services {

    /// <summary>
    /// Provides business logic for managing feedback: building dashboard view models,
    /// filtering and sorting feedback, and marking feedback as read or unread.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexandros Blask, Simon Hinterreiter
    /// </remarks>
    public class FeedbackService {
        private const int _pageSize = 20;
        private readonly IFeedbackRepository _databaseFeedbackRepository;
        private readonly IApplicationUserRepository _databaseApplicaionUserRepository;
        private readonly ILessonRepository _databaseLessonRepository;
        private readonly ITeachingPlanRepository _databaseTeachingPlanRepository;


        // ---------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="FeedbackService"/> class.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="feedbackRepo">Repository for feedback persistence operations.</param>
        /// <param name="userRepo">Repository for application user data access.</param>
        /// <param name="lessonRepo">Repository for lesson data retrieval.</param>
        /// <param name="teachingPlanRepo">Repository for teaching plan data retrieval.</param>
        public FeedbackService(IFeedbackRepository feedbackRepo, IApplicationUserRepository userRepo, ILessonRepository lessonRepo, ITeachingPlanRepository teachingPlanRepo) {
            _databaseFeedbackRepository = feedbackRepo;
            _databaseApplicaionUserRepository = userRepo;
            _databaseLessonRepository = lessonRepo;
            _databaseTeachingPlanRepository = teachingPlanRepo;
        }

        /// <summary>
        /// Builds a <see cref="FeedbackDashboardViewModel"/> based on the current user and filter criteria.
        /// Code Ownership: Simon Hinterreiter
        /// </summary>
        /// <param name="user">The claims principal representing the current user.</param>
        /// <param name="filter">Filter type: "all", "read", or "unread".</param>
        /// <param name="page">Page number for pagination.</param>
        /// <param name="sortBy">Sort criteria, e.g. "date_asc".</param>
        /// <param name="selectedTraineeId">Optional filter by trainee ID.</param>
        /// <param name="selectedLessonId">Optional filter by lesson ID.</param>
        /// <param name="selectedTeachingPlanId">Optional filter by teaching plan ID.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the populated <see cref="FeedbackDashboardViewModel"/>.
        /// </returns>
        public async Task<FeedbackDashboardViewModel> BuildFeedbackDashboardViewModelAsync(
            ClaimsPrincipal user,
            string filter = "all",
            int page = 1,
            string sortBy = "date_asc",
            string? selectedTraineeId = null,
            int? selectedLessonId = null,
            int? selectedTeachingPlanId = null) {

            // +++++++++++++++
            // Get current user
            ApplicationUser? appUserNullable = await _databaseApplicaionUserRepository.GetUserAsync(user);
            ApplicationUser appUser = appUserNullable ?? throw new InvalidOperationException("User not found.");

            // +++++++++++++++
            // For Dropdown - get all activeTrainees
            var activeTrainees = await _databaseApplicaionUserRepository.GetOpenUsersInRoleAsync("Trainee");

            // +++++++++++++++
            // For Dropdown: Get (all active lessons by trainee) OR [(all lessons for all teachingPlans) OR (all lessons for selected teachingPlan)]
            List<Lesson> lessons;
            List<TeachingPlan> teachingPlans;

            if (!string.IsNullOrEmpty(selectedTraineeId)) {

                /* 
                Console.WriteLine($"[DEBUG] selectedTraineeId: {selectedTraineeId}");
                Console.WriteLine($"[DEBUG] sortBy: {sortBy}");
                */

                // +++++++++++++++
                // Get Trainee
                ApplicationUser? traineeNullable = await _databaseApplicaionUserRepository.FindByIdWithWrittenFeedbacksWithLessonAsync(selectedTraineeId!);

                ApplicationUser trainee = traineeNullable ?? throw new InvalidOperationException("User not found.");

                // +++++++++++++++
                // Reset LessonId if Trainee has no Feedback for that lesson
                if (!trainee.WrittenFeedbacks.Any(f => f.Lesson.LessonId == selectedLessonId)) {
                    selectedLessonId = null;
                }

                // +++++++++++++++
                // For Dropdown - Get Lessons of selected Trainee with Feedbacks:
                lessons = trainee.WrittenFeedbacks
                    .Select(f => f.Lesson)
                    .OrderBy(l => l.SortingIndex)
                    .ToList();

                // +++++++++++++++
                // Set selected TeachingPlan (a trainee only has one teachingPlan)
                selectedTeachingPlanId = trainee.TeachingPlanId;

                // +++++++++++++++
                // For Dropdown - Reduce Teachingplan Dropdown when Trainee is selected
                var teachingPlanNullable = await _databaseTeachingPlanRepository
                    .GetTeachingPlanByIdAsync(trainee.TeachingPlanId!.Value);

                var teachingPlan = teachingPlanNullable ?? throw new InvalidOperationException("TeachingPlan not found.");

                teachingPlans = new List<TeachingPlan> { teachingPlan };

            } else {
                // For Dropdown: Get all Lessons that have at least one feedback and match the selected teachingPlanId
                lessons = (await _databaseLessonRepository.GetAllLessonsWithFeedbacksAsync())
                    .Where(l =>
                        l.Feedbacks != null && l.Feedbacks.Any() &&
                        (!selectedTeachingPlanId.HasValue || l.TeachingPlanId == selectedTeachingPlanId.Value))
                    .OrderBy(l => l.TeachingPlanId)
                    .ThenBy(l => l.SortingIndex)
                    .ToList();

                // For Dorpdown: Get all TeachingPlans
                teachingPlans = (await _databaseTeachingPlanRepository.GetAllTeachingPlansAsync())
                    .OrderBy(tp => tp.TeachingPlanId)
                    .ToList();
            }

            // +++++++++++++++
            // Get Pages for "Unread", "Read", and "All":
            Page<FeedbackDashboardDto> feedbackPage = filter switch {
                "unread" => await GetUnreadFeedbacksAsync(appUser, page, sortBy, selectedTraineeId, selectedLessonId, selectedTeachingPlanId),
                "read" => await GetReadFeedbacksAsync(appUser, page, sortBy, selectedTraineeId, selectedLessonId, selectedTeachingPlanId),
                _ => await GetAllFeedbacksAsync(appUser, page, sortBy, selectedTraineeId, selectedLessonId, selectedTeachingPlanId)
            };

            // +++++++++++++++
            // For Filter Tabs - Get Count Numbers:
            var query = _databaseFeedbackRepository.GetAllFeedbacksWithLessonAndAuthorAndReadByUsers();

            if (!string.IsNullOrEmpty(selectedTraineeId)) {
                query = query.Where(f => f.Author.Id == selectedTraineeId);
            }

            if (selectedTeachingPlanId.HasValue) {
                query = query.Where(f => f.Lesson.TeachingPlanId == selectedTeachingPlanId.Value);
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
                TeachingPlans = teachingPlans,
                SelectedTraineeId = selectedTraineeId,
                SelectedLessonId = selectedLessonId,
                SelectedTeachingPlanId = selectedTeachingPlanId,

                TotalFeedbackCount = totalCount,
                ReadFeedbackCount = readCount,
                UnreadFeedbackCount = unreadCount
            };
        }

        // ---------------------------------------------------
        /// <summary>
        /// Applies filtering and sorting to an <see cref="IQueryable{Feedback}"/> sequence.
        /// Code Ownership: Simon Hinterreiter
        /// </summary>
        /// <param name="query">The base feedback query.</param>
        /// <param name="sortBy">Sort criteria string.</param>
        /// <param name="selectedTraineeId">Optional trainee ID filter.</param>
        /// <param name="selectedLessonId">Optional lesson ID filter.</param>
        /// <param name="selectedTeachingPlanId">Optional teaching plan ID filter.</param>
        /// <returns>The modified query with filtering and sorting applied.</returns>
        private static IQueryable<Feedback> ApplySortingAndFiltering(
            IQueryable<Feedback> query,
            string sortBy,
            string? selectedTraineeId = null,
            int? selectedLessonId = null,
            int? selectedTeachingPlanId = null) {

            // +++++++++++++++
            // Filter by Author (Trainee)
            if (!string.IsNullOrEmpty(selectedTraineeId))
                query = query.Where(f => f.Author.Id == selectedTraineeId);

            // +++++++++++++++
            // Filter by Lesson
            if (selectedLessonId.HasValue)
                query = query.Where(f => f.Lesson.LessonId == selectedLessonId);

            // +++++++++++++++
            // Filter by TeachingPlan
            if (selectedTeachingPlanId.HasValue)
                query = query.Where(f => f.Lesson.TeachingPlanId == selectedTeachingPlanId.Value);

            // +++++++++++++++
            // Sort:
            return sortBy.ToLower() switch {
                "author_asc" => query.OrderBy(f => f.Author.UserName),
                "author_desc" => query.OrderByDescending(f => f.Author.UserName),
                "lesson_asc" => query.OrderBy(f => f.Lesson.Title),
                "lesson_desc" => query.OrderByDescending(f => f.Lesson.Title),
                "date_asc" => query.OrderBy(f => f.CreateTime),
                _ => query.OrderByDescending(f => f.CreateTime) // default fallback
            };
        }

        // ---------------------------------------------------
        /// <summary>
        /// Retrieves a paged list of all feedback entries for display.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="currentUser">The current user performing the query.</param>
        /// <param name="page">Page number for pagination.</param>
        /// <param name="sortBy">Sort criteria string.</param>
        /// <param name="selectedTraineeId">Optional trainee ID filter.</param>
        /// <param name="selectedLessonId">Optional lesson ID filter.</param>
        /// <param name="selectedTeachingPlanId">Optional teaching plan ID filter.</param>
        /// <returns>A task with a <see cref="Page{FeedbackDashboardDto}"/> of all feedbacks.</returns>
        private async Task<Page<FeedbackDashboardDto>> GetAllFeedbacksAsync(
            ApplicationUser currentUser,
            int page,
            string sortBy,
            string? selectedTraineeId = null,
            int? selectedLessonId = null,
            int? selectedTeachingPlanId = null
            ) {

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
            query = ApplySortingAndFiltering(query, sortBy, selectedTraineeId, selectedLessonId, selectedTeachingPlanId);

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

        // ---------------------------------------------------
        /// <summary>
        /// Retrieves a paged list of feedback entries already read by the current user.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="currentUser">The current user performing the query.</param>
        /// <param name="page">Page number for pagination.</param>
        /// <param name="sortBy">Sort criteria string.</param>
        /// <param name="selectedTraineeId">Optional trainee ID filter.</param>
        /// <param name="selectedLessonId">Optional lesson ID filter.</param>
        /// <param name="selectedTeachingPlanId">Optional teaching plan ID filter.</param>
        /// <returns>A task with a <see cref="Page{FeedbackDashboardDto}"/> of read feedbacks.</returns>
        public async Task<Page<FeedbackDashboardDto>> GetReadFeedbacksAsync(
            ApplicationUser currentUser,
            int page,
            string sortBy,
            string? selectedTraineeId = null,
            int? selectedLessonId = null,
            int? selectedTeachingPlanId = null) {

            // +++++++++++++++
            // Get all Feedbacks read by currentUser
            var query = _databaseFeedbackRepository.GetAllFeedbacksReadByUserWithLessonAndAuthor(currentUser);

            // +++++++++++++++
            // Apply Sorting and Filtering
            query = ApplySortingAndFiltering(query, sortBy, selectedTraineeId, selectedLessonId, selectedTeachingPlanId);

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

        // ---------------------------------------------------
        /// <summary>
        /// Retrieves a paged list of feedback entries not yet read by the current user.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="currentUser">The current user performing the query.</param>
        /// <param name="page">Page number for pagination.</param>
        /// <param name="sortBy">Sort criteria string.</param>
        /// <param name="selectedTraineeId">Optional trainee ID filter.</param>
        /// <param name="selectedLessonId">Optional lesson ID filter.</param>
        /// <param name="selectedTeachingPlanId">Optional teaching plan ID filter.</param>
        /// <returns>A task with a <see cref="Page{FeedbackDashboardDto}"/> of unread feedbacks.</returns>
        public async Task<Page<FeedbackDashboardDto>> GetUnreadFeedbacksAsync(
            ApplicationUser currentUser,
            int page,
            string sortBy,
            string? selectedTraineeId = null,
            int? selectedLessonId = null,
            int? selectedTeachingPlanId = null) {

            // +++++++++++++++
            // Get all Feedbacks unread by currentUser
            var query = _databaseFeedbackRepository.GetAllFeedbacksUnreadByUserWithLessonAndAuthorAndReadByUsers(currentUser);

            // +++++++++++++++
            // Apply Sorting and Filtering
            query = ApplySortingAndFiltering(query, sortBy, selectedTraineeId, selectedLessonId, selectedTeachingPlanId);

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
        
        // ---------------------------------------------------
        /// <summary>
        /// Marks a specific feedback entry as read for the given user.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="userPrincipal">The claims principal of the user marking the feedback as read.</param>
        /// <param name="feedbackId">The ID of the feedback to mark as read.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
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

        // ---------------------------------------------------
        /// <summary>
        /// Marks a specific feedback entry as unread for the given user.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="userPrincipal">The claims principal of the user marking the feedback as unread.</param>
        /// <param name="feedbackId">The ID of the feedback to mark as unread.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
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

        // ---------------------------------------------------
        /// <summary>
        /// Clears the "read" status for all users on a specific feedback entry.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="feedbackId">The ID of the feedback to reset.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
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
