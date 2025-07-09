// class by schleale

using System.Data;
using System.Security.Claims;
using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Data.TraineeLessons;
using TraineeTracker.Exceptions;
using TraineeTracker.Models.Domain;
using TraineeTracker.Models.ViewModels;

namespace TraineeTracker.Services {
    public class TraineeLessonDashboardService {
        private readonly TraineeStatisticsService _traineeStatisticsService;
        private readonly IApplicationUserRepository _databaseApplicationUserRepository;

        // ------------------------------------------------------
        public TraineeLessonDashboardService(
            TraineeStatisticsService traineeStatisticsService,
            IApplicationUserRepository databaseApplicationUserRepository) {
            _traineeStatisticsService = traineeStatisticsService;
            _databaseApplicationUserRepository = databaseApplicationUserRepository;
        }

        // ------------------------------------------------------
        private static void CheckHasAccess(ClaimsPrincipal user, string traineeId) {
            if (user == null || String.IsNullOrWhiteSpace(traineeId))
                throw new UserNotFoundException();

            // looks through ClaimsPrincipal user for a claim with the type ClaimTypes.NameIdentifier, which should be the UserId
            var userId = (user.FindFirst(ClaimTypes.NameIdentifier)?.Value) ?? throw new Exception("ClaimTypes.NameIdentifier of user not found.");

            // allows access, if Role is Admin or Mentor
            if (user.IsInRole("Admin") || user.IsInRole("Mentor"))
                return;

            // allows access, if the correct Trainee tries to access
            if (!(userId == traineeId))
                throw new UnauthorizedAccessException("You can only access your own TraineeLessons.");
        }

        // ------------------------------------------------------
        public async Task<TraineeLessonDashboardViewModel> BuildTraineeLessonDashboardViewModel(
            ClaimsPrincipal user,
            string? traineeId,
            string filter = "all",
            string sortBy = "SortingIndex_asc") {

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new Exception("User ID not found");

            IEnumerable<ApplicationUser> selectableTrainees;

            ApplicationUser selectedTrainee;

            // +++++++++++++++
            // A. If user is Trainee:
            if (user.IsInRole("Trainee")) {
                selectedTrainee = await _databaseApplicationUserRepository.FindByIdWithTeachingPlanAndTraineeLessonsWithLessonsAsync(userId) ?? throw new Exception("Trainee not found");

                // Only current Trainee in Dropdown
                selectableTrainees = [selectedTrainee];
            }

            // +++++++++++++++
            // B. If user IS NOT Trainee:
            else if (user.IsInRole("Mentor") || user.IsInRole("Admin")) {
                // All active Trainees in Dropdown
                selectableTrainees = await _databaseApplicationUserRepository.GetOpenUsersInRoleAsync("Trainee");

                // +++++++++++++++
                // a. No trainee selected:
                if (traineeId == null) {

                    // Get mostRecentlyViewedTrainee:
                    ApplicationUser? mostRecentTrainee = await GetLastSelectedTrainee(userId);

                    // If we have a mostRecentTrainee:
                    if (mostRecentTrainee != null) {
                        CheckHasAccess(user, mostRecentTrainee.Id);

                        // Set mostRecentlyViewedTrainee as selectedTrainee:
                        selectedTrainee = await _databaseApplicationUserRepository.FindByIdWithTeachingPlanAndTraineeLessonsWithLessonsAsync(mostRecentTrainee.Id) ?? throw new Exception("Trainee not found");
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
                            selectedTrainee = await _databaseApplicationUserRepository.FindByIdWithTeachingPlanAndTraineeLessonsWithLessonsAsync(firstTrainee.Id) ?? throw new Exception("Trainee not found");
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
                throw new Exception("Unauthorized access: user is neither Trainee, Mentor, nor Admin.");
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

                ActiveFilter = filter
            };
        }

        // ------------------------------------------------------
        private IEnumerable<TraineeLesson> GetFilteredAndSortedTraineeLessonsForTrainee(
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
            return sortBy.ToLower() switch {
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


        // methods by schwepau
        // ------------------------------------------------------
        private async Task AddLastSelectedTraineeAsync(string mentorId, ApplicationUser trainee) {
            var mentor = await _databaseApplicationUserRepository.FindByIdAsync(mentorId);
            if (mentor == null) {
                throw new Exception("Mentor not found.");
            }
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
        private async Task<ApplicationUser?> GetLastSelectedTrainee(string mentorId) {
            var mentor = await _databaseApplicationUserRepository.FindByIdWithLastSelectedTraineesAsync(mentorId);
            if (mentor == null) {
                throw new Exception("Mentor not found.");
            }
            return mentor.LastSelectedTrainees.LastOrDefault();
        }

        // ------------------------------------------------------
    }
}