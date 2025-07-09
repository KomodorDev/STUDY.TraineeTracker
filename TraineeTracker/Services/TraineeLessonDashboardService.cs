// class by schleale

using System.Security.Claims;
using TraineeTracker.Data.ApplicationUsers;

using TraineeTracker.Exceptions;
using TraineeTracker.Models.Domain;
using TraineeTracker.Models.ViewModels;

namespace TraineeTracker.Services {
    public class TraineeLessonDashboardService {
        private TraineeStatisticsService _traineeStatisticsService;
        private IApplicationUserRepository _applicationUserRepository;

        // ------------------------------------------------------

        public TraineeLessonDashboardService(TraineeStatisticsService traineeStatisticsService,
                                                IApplicationUserRepository applicationUserRepository) {
            _traineeStatisticsService = traineeStatisticsService;
            _applicationUserRepository = applicationUserRepository;
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

        public async Task<TraineeLessonDashboardViewModel> BuildTraineeLessonDashboardViewModel(ClaimsPrincipal user, String? traineeId) {
            var mentorId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new Exception("User ID not found");

            // set requested id to user id, so that trainees cannot access any other page other than their own
            if (user.IsInRole("Trainee"))
                traineeId = mentorId;

            var trainees = await _applicationUserRepository.GetOpenUsersInRoleAsync("Trainee");

            if (traineeId == null) {
                // no trainee selected

                ApplicationUser? mostRecentTrainee = await GetLastSelectedTrainee(mentorId);

                if (mostRecentTrainee != null) {
                    // select most recent trainee

                    CheckHasAccess(user, mostRecentTrainee.Id);

                    return new TraineeLessonDashboardViewModel {
                        SelectedTrainee = mostRecentTrainee,
                        TraineeStatisticsSnapshot = await _traineeStatisticsService.BuildLatestTraineeStatisticsSnapshotAsync(mostRecentTrainee.Id),
                        TraineeLessons = mostRecentTrainee.TraineeLessons,
                        Trainees = trainees
                    };
                } else {
                    // no recent trainee -> alphabetically first trainee

                    ApplicationUser? firstTrainee = trainees.FirstOrDefault();

                    if (firstTrainee == null) {
                        // no first trainee present -> empty

                        return new TraineeLessonDashboardViewModel { };

                    } else {
                        // return first trainee

                        CheckHasAccess(user, firstTrainee.Id);

                        // update recent trainees
                        await AddLastSelectedTraineeAsync(mentorId, firstTrainee);

                        return new TraineeLessonDashboardViewModel {
                            SelectedTrainee = firstTrainee,
                            TraineeStatisticsSnapshot = await _traineeStatisticsService.BuildLatestTraineeStatisticsSnapshotAsync(firstTrainee.Id),
                            TraineeLessons = firstTrainee.TraineeLessons,
                            Trainees = trainees
                        };
                    }
                }

            } else {
                // return requested trainee

                CheckHasAccess(user, traineeId);

                // update recent trainees
                var trainee = await _applicationUserRepository.FindByIdWithTeachingPlanAndTraineeLessonsWithLessonsAsync(traineeId) ?? throw new UserNotFoundException();
                await AddLastSelectedTraineeAsync(mentorId, trainee);

                return new TraineeLessonDashboardViewModel {
                    SelectedTrainee = trainee,
                    TraineeStatisticsSnapshot = await _traineeStatisticsService.BuildLatestTraineeStatisticsSnapshotAsync(traineeId),
                    TraineeLessons = trainee.TraineeLessons,
                    Trainees = trainees
                };
            }
        }

        // methods by schwepau
        // ------------------------------------------------------
        private async Task AddLastSelectedTraineeAsync(string mentorId, ApplicationUser trainee) {
            var mentor = await _applicationUserRepository.FindByIdAsync(mentorId);
            if (mentor == null) {
                throw new Exception("Mentor not found.");
            }
            mentor.LastSelectedTrainees.Remove(trainee);
            mentor.LastSelectedTrainees.Add(trainee);
            if (mentor.LastSelectedTrainees.Count > 3) { // max 3 entries in list
                mentor.LastSelectedTrainees.RemoveAt(0);
            }
            var result = await _applicationUserRepository.UpdateAsync(mentor);
            if (!result.Succeeded) {
                throw new Exception($"Update of mentor failed.");
            }
        }

        // ------------------------------------------------------
        private async Task<ApplicationUser?> GetLastSelectedTrainee(string mentorId) {
            var mentor = await _applicationUserRepository.FindByIdWithLastSelectedTraineesAsync(mentorId);
            if (mentor == null) {
                throw new Exception("Mentor not found.");
            }
            return mentor.LastSelectedTrainees.LastOrDefault();
        }
    }
}