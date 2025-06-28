using System.Security.Claims;
using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Data.Lessons;
using TraineeTracker.Data.TeachingPlans;
using TraineeTracker.Data.TraineeLessons;

using TraineeTracker.Exceptions;
using TraineeTracker.Models.ViewModels;

namespace TraineeTracker.Services {
    public class TraineeLessonDashboardService {
        private TraineeStatisticsService _traineeStatisticsService;
        private IApplicationUserRepository _databaseApplicationUserRepository;

        public TraineeLessonDashboardService(TraineeStatisticsService traineeStatisticsService,
                                                IApplicationUserRepository databaseApplicationUserRepository) {
            _traineeStatisticsService = traineeStatisticsService;
            _databaseApplicationUserRepository = databaseApplicationUserRepository;
        }

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

        public async Task<TraineeLessonDashboardViewModel> BuildTraineeLessonDashboardViewModel(ClaimsPrincipal user, String? traineeId) {
            if (traineeId == null) {
                // no trainee selected -> return empty page
                return new TraineeLessonDashboardViewModel {

                };
            } else {
                CheckHasAccess(user, traineeId);

                var trainee = await _databaseApplicationUserRepository.FindByIdWithTraineeLessonsWithLessonsAndTeachingPlanAsync(traineeId) ?? throw new UserNotFoundException();

                return new TraineeLessonDashboardViewModel {
                    TraineeStatisticsSnapshot = await _traineeStatisticsService.BuildLatestTraineeStatisticsSnapshotAsync(traineeId),
                    Lessons = trainee.TraineeLessons.Lessons,
                    TraineeLessons = trainee.TraineeLessons,
                    TeachingPlan = trainee.TeachingPlan
                };
            }
        }
    }
}