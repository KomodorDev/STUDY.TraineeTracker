using System.Data;
using System.Security.Claims;
using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Exceptions;
using TraineeTracker.Models.Domain;
using TraineeTracker.Models.ViewModels;

namespace TraineeTracker.Services {

    /// <summary>
    /// Service responsible for building the dashboard (main page) view model,
    /// and hence also handling trainee selection and lesson filtering logic.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexander Schlemmer (schleale)
    /// </remarks>
    public class TraineeLessonDashboardService {

        /// <summary>
        /// Service for handling the statistics of a trainee.
        /// </summary>
        private readonly TraineeStatisticsService _traineeStatisticsService;

        /// <summary>
        /// Repository for retrieving and updating application user data, including roles and email addresses.
        /// </summary>
        private readonly IApplicationUserRepository _databaseApplicationUserRepository;

        // ------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="TraineeLessonDashboardService"/> class.
        /// </summary>
        /// <param name="traineeStatisticsService">Service used to send emails.</param>
        /// <param name="databaseApplicationUserRepository">Repository to access application user data.</param>
        /// <remarks>
        /// Code Ownership: Alexander Schlemmer (schleale)
        /// </remarks>
        public TraineeLessonDashboardService(
            TraineeStatisticsService traineeStatisticsService,
            IApplicationUserRepository databaseApplicationUserRepository) {
            _traineeStatisticsService = traineeStatisticsService;
            _databaseApplicationUserRepository = databaseApplicationUserRepository;
        }

        // ------------------------------------------------------
        /// <summary>
        /// Verifies whether the given user has access to view or  modifying the specified trainee's dashboard.
        /// Access is granted to the trainee themselves, as well as all 'Mentor's or 'Admin's.
        /// </summary>
        /// <param name="user">The authenticated user attempting to access the dashboard.</param>
        /// <param name="traineeId">The ID of the trainee whose dashboard is being accessed.</param>
        /// <exception cref="UserNotFoundException">Thrown when the user or trainee ID is invalid.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when access is denied due to insufficient permissions.</exception>
        /// <remarks>
        /// Code Ownership: Alexander Schlemmer (schleale)
        /// </remarks>
        private static void CheckHasAccess(ClaimsPrincipal user, string traineeId) {
            if (user == null || String.IsNullOrWhiteSpace(traineeId))
                throw new UserNotFoundException();

            // Looks through ClaimsPrincipal of user for a claim with the type ClaimTypes.NameIdentifier, which should be the UserId
            var userId = (user.FindFirst(ClaimTypes.NameIdentifier)?.Value) ?? throw new UserNotFoundException("ClaimTypes.NameIdentifier of user not found.");

            // Allows access, if Role is Admin or Mentor
            if (user.IsInRole("Admin") || user.IsInRole("Mentor"))
                return;

            // Allows access, if the correct Trainee tries to access
            if (!(userId == traineeId))
                throw new UnauthorizedAccessException("You can only access your own TraineeLessons.");
        }

        // ------------------------------------------------------
        /// <summary>
        /// Builds the <see cref="TraineeLessonDashboardViewModel"/> for the current user,
        /// selecting a trainee and filtering/sorting their lessons based on input and role-based access.
        /// </summary>
        /// <param name="user">The authenticated user requesting the dashboard view.</param>
        /// <param name="traineeId">
        /// Optional. The ID of the trainee whose data should be displayed. If null, the method will resolve the trainee
        /// based on recent selections or alphabetical order (for mentors/admins).
        /// </param>
        /// <param name="filter">
        /// Optional. The lesson status filter to apply. Accepted values: "all", "open", "started", "finished",
        /// "skipped", "accepted", "rejected", "rated". Defaults to "all".
        /// </param>
        /// <param name="sortBy">
        /// Optional. The field to sort lessons by. Accepted values: "SortingIndex_asc", "title_asc", "state_desc", etc.
        /// Defaults to "SortingIndex_asc".
        /// </param>
        /// <returns>
        /// A fully populated <see cref="TraineeLessonDashboardViewModel"/> including selected trainee data,
        /// lesson statistics, and filtered/sorted lesson items.
        /// </returns>
        /// <exception cref="UserNotFoundException">
        /// Thrown when the user or trainee could not be found in the database.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// Thrown when the user attempts to access a trainee's data without proper permissions.
        /// </exception>
        /// <remarks>
        /// Code Ownership: Alexander Schlemmer (schleale) & Simon Hinterreiter (hintsimo)
        /// </remarks>
        public async Task<TraineeLessonDashboardViewModel> BuildTraineeLessonDashboardViewModel(
            ClaimsPrincipal user,
            string? traineeId,
            string filter = "all",
            string sortBy = "state_custom") {

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UserNotFoundException("User ID not found");

            IEnumerable<ApplicationUser> selectableTrainees;

            ApplicationUser selectedTrainee;

            // +++++++++++++++
            // A. If user is Trainee:
            if (user.IsInRole("Trainee")) {
                selectedTrainee = await _databaseApplicationUserRepository.FindByIdWithTeachingPlanAndTraineeLessonsWithLessonsAsync(userId) ?? throw new UserNotFoundException("Trainee not found");

                // Only current Trainee in Dropdown
                selectableTrainees = [selectedTrainee];
            }

            // +++++++++++++++
            // B. If user IS NOT Trainee:
            else if (user.IsInRole("Mentor") || user.IsInRole("Admin")) {
                // All active Trainees in Dropdown
                selectableTrainees = (await _databaseApplicationUserRepository
                    .GetOpenUsersInRoleAsync("Trainee"))
                    .OrderBy(user => user.UserName)
                    .ToList();

                // +++++++++++++++
                // a. No trainee selected:
                if (traineeId == null) {

                    // Get mostRecentlyViewedTrainee:
                    ApplicationUser? mostRecentTrainee = await GetLastSelectedTrainee(userId);

                    // If we have a mostRecentTrainee:
                    if (mostRecentTrainee != null) {
                        CheckHasAccess(user, mostRecentTrainee.Id);

                        // Set mostRecentlyViewedTrainee as selectedTrainee:
                        selectedTrainee = await _databaseApplicationUserRepository.FindByIdWithTeachingPlanAndTraineeLessonsWithLessonsAsync(mostRecentTrainee.Id) ?? throw new UserNotFoundException("Trainee not found");
                    }

                    // If we have NO mostRecentTrainee:
                    else {
                        // No recent trainee -> alphabetically first active trainee:
                        ApplicationUser? firstTrainee = selectableTrainees
                            .OrderBy(t => t.UserName)
                            .FirstOrDefault();

                        // Edge Case - No trainee exists yet:
                        if (firstTrainee == null) {

                            return new TraineeLessonDashboardViewModel { };

                        }

                        // A trainee exists:
                        else {

                            // Get firstTrainee with Lessons
                            selectedTrainee = await _databaseApplicationUserRepository.FindByIdWithTeachingPlanAndTraineeLessonsWithLessonsAsync(firstTrainee.Id) ?? throw new UserNotFoundException("Trainee not found");
                        }
                    }
                }
                // +++++++++++++++
                // b. A trainee was selected:
                else {
                    CheckHasAccess(user, traineeId);

                    // Set request Trainee as selectedTrainee
                    selectedTrainee = await _databaseApplicationUserRepository.FindByIdWithTeachingPlanAndTraineeLessonsWithLessonsAsync(traineeId) ?? throw new UserNotFoundException();
                }
            } else {
                throw new UnauthorizedAccessException("Unauthorized access: user is neither Trainee, Mentor, nor Admin.");
            }


            // +++++++++++++++
            // Building TraineeLessonDashboardViewModel with selected Trainee:

            // update recent trainees
            await AddLastSelectedTraineeAsync(userId, selectedTrainee);
            var traineeLessons = selectedTrainee.TraineeLessons ?? Enumerable.Empty<TraineeLesson>();



            return new TraineeLessonDashboardViewModel {
                SelectedTrainee = selectedTrainee,
                TraineeStatisticsSnapshot = await _traineeStatisticsService.BuildLatestTraineeStatisticsSnapshotAsync(selectedTrainee.Id),
                TraineeLessons = GetFilteredAndSortedTraineeLessonsForTrainee(selectedTrainee, filter, sortBy),
                Trainees = selectableTrainees,

                // Zähler pro Filterstatus
                CountAll = traineeLessons.Count(l => l.State != TraineeLessonState.Skipped),
                CountOpen = traineeLessons.Count(l => l.State == TraineeLessonState.Open),
                CountStarted = traineeLessons.Count(l => l.State == TraineeLessonState.Started),
                CountFinished = traineeLessons.Count(l => l.State == TraineeLessonState.Finished),
                CountAccepted = traineeLessons.Count(l => l.State == TraineeLessonState.Accepted),
                CountRejected = traineeLessons.Count(l => l.State == TraineeLessonState.Rejected),
                CountRated = traineeLessons.Count(l => l.State == TraineeLessonState.Rated),
                CountSkipped = traineeLessons.Count(l => l.State == TraineeLessonState.Skipped),

                ActiveFilter = filter,
                SortBy = sortBy
            };
        }

        // ------------------------------------------------------
        /// <summary>
        /// Filters and sorts a trainee's lessons based on the specified filter and sorting criteria.
        /// </summary>
        /// <param name="trainee">The trainee whose lessons will be processed.</param>
        /// <param name="filter">
        /// The filter to apply to lesson states. Accepted values: "all", "open", "started", "finished",
        /// "skipped", "accepted", "rejected", "rated".
        /// </param>
        /// <param name="sortBy">
        /// The field and direction by which to sort lessons. Accepted values: "title_asc", "title_desc",
        /// "estimatedeffort_asc", "estimatedeffort_desc", "sortingindex_asc", "sortingindex_desc",
        /// "state_asc", "state_desc".
        /// </param>
        /// <returns>A filtered and sorted <see cref="IEnumerable{T}"/> of <see cref="TraineeLesson"/> entries.</returns>
        /// <remarks>
        /// Code Ownership: Simon Hinterreiter (hintsimo)
        /// </remarks>
        private static IEnumerable<TraineeLesson> GetFilteredAndSortedTraineeLessonsForTrainee(
            ApplicationUser trainee,
            string filter,
            string sortBy) {
            var allLessons = trainee.TraineeLessons ?? new List<TraineeLesson>();

            // Apply Filter
            var filtered = filter.ToLower() switch {
                "open" => allLessons.Where(l => l.State == TraineeLessonState.Open),
                "started" => allLessons.Where(l => l.State == TraineeLessonState.Started),
                "finished" => allLessons.Where(l => l.State == TraineeLessonState.Finished),
                "skipped" => allLessons.Where(l => l.State == TraineeLessonState.Skipped),
                "rejected" => allLessons.Where(l => l.State == TraineeLessonState.Rejected),
                "accepted" => allLessons.Where(l => l.State == TraineeLessonState.Accepted),
                "rated" => allLessons.Where(l => l.State == TraineeLessonState.Rated),
                _ => allLessons.Where(l => l.State != TraineeLessonState.Skipped) // "all" = excluding skipped
            };

            // Apply Sorting:
            int GetCustomStateOrder(TraineeLessonState state) {
                return state switch {
                    TraineeLessonState.Rejected => 0,
                    TraineeLessonState.Finished => 1,
                    TraineeLessonState.Started => 2,
                    TraineeLessonState.Open => 3,
                    TraineeLessonState.Accepted => 4,
                    TraineeLessonState.Rated => 5,
                    _ => 6
                };
            }
            return sortBy.ToLower() switch {

                "state_custom" => filtered
                    .OrderBy(l => GetCustomStateOrder(l.State))
                    .ThenBy(l => l.Lesson?.SortingIndex),

                "title_asc" => filtered.OrderBy(l => l.Lesson?.Title),
                "title_desc" => filtered.OrderByDescending(l => l.Lesson?.Title),

                "estimatedeffort_asc" => filtered.OrderBy(l => l.Lesson?.EstimatedEffort),
                "estimatedeffort_desc" => filtered.OrderByDescending(l => l.Lesson?.EstimatedEffort),

                "sortingindex_asc" => filtered.OrderBy(l => l.Lesson?.SortingIndex),
                "sortingindex_desc" => filtered.OrderByDescending(l => l.Lesson?.SortingIndex),

                "state_asc" => filtered.OrderBy(l => l.State),
                "state_desc" => filtered.OrderByDescending(l => l.State),

                _ => filtered.OrderBy(l => l.Lesson?.SortingIndex)// Fallback
            };
        }

        // ------------------------------------------------------
        /// <summary>
        /// Updates the mentor's list of recently selected trainees by adding the specified trainee.
        /// Ensures the list does not exceed the maximum allowed entries.
        /// </summary>
        /// <param name="mentorId">The ID of the mentor whose recent trainee list should be updated.</param>
        /// <param name="trainee">The trainee to be added to the mentor's recent selection list.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="UserNotFoundException">
        /// Thrown when the mentor could not be found in the database.
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown if the updated last selected trainee of the mentor fails.
        /// </exception>
        /// <remarks>
        /// Code Ownership: Paul Schweizer (schwepau)
        /// </remarks>
        private async Task AddLastSelectedTraineeAsync(string mentorId, ApplicationUser trainee) {
            var mentor = await _databaseApplicationUserRepository.FindByIdAsync(mentorId) ?? throw new UserNotFoundException("Mentor not found.");
            mentor.LastSelectedTrainees.Remove(trainee);
            mentor.LastSelectedTrainees.Add(trainee);
            if (mentor.LastSelectedTrainees.Count > 1) { // max 3 entries in list
                mentor.LastSelectedTrainees.RemoveAt(0);
            }
            var result = await _databaseApplicationUserRepository.UpdateAsync(mentor);
            if (!result.Succeeded) {
                throw new Exception($"Update of mentor failed.");
            }
        }

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves the most recently selected trainee for the specified mentor.
        /// </summary>
        /// <param name="mentorId">The ID of the mentor whose last selected trainee is requested.</param>
        /// <returns>
        /// The most recently selected <see cref="ApplicationUser"/> representing the trainee,
        /// or <c>null</c> if no trainees have been selected yet.
        /// </returns>
        /// <exception cref="UserNotFoundException">
        /// Thrown when the mentor could not be found in the database.
        /// </exception>
        /// <remarks>
        /// Code Ownership: Paul Schweizer (schwepau)
        /// </remarks>
        private async Task<ApplicationUser?> GetLastSelectedTrainee(string mentorId) {
            var mentor = await _databaseApplicationUserRepository.FindByIdWithLastSelectedTraineesAsync(mentorId);
            if (mentor == null) {
                throw new UserNotFoundException("Mentor not found.");
            }
            return mentor.LastSelectedTrainees.LastOrDefault();
        }

        // ------------------------------------------------------
    }
}