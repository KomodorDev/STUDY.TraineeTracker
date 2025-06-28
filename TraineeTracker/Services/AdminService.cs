using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Data.ProcessingPauses;
using TraineeTracker.Services.Email;
using TraineeTracker.Models.Domain;
using TraineeTracker.Models.Dtos;
using TraineeTracker.Data.Feedbacks;
using TraineeTracker.Data.TeachingPlans;
using TraineeTracker.Data.TraineeStatistics;

namespace TraineeTracker.Services.Admin {
    public class AdminService {
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly IProcessingPauseRepository _processingPauseRepository;

        private readonly EmailNotificationService _emailNotificationService;
        private readonly TeachingPlanService _teachingPlanService;

        private readonly IFeedbackRepository _feedbackRepository;
        private readonly ITeachingPlanRepository _teachingPlanRepository;
        private readonly ITraineeStatisticsRepository _traineeStatisticsRepository;

        public AdminService(IApplicationUserRepository applicationUserRepository,
                            IProcessingPauseRepository processingPauseRepository,
                            EmailNotificationService emailNotificationService,
                            TeachingPlanService teachingPlanService,
                            IFeedbackRepository feedbackRepository,
                            ITeachingPlanRepository teachingPlanRepository,
                            ITraineeStatisticsRepository traineeStatisticsRepository) {
            _applicationUserRepository = applicationUserRepository;
            _processingPauseRepository = processingPauseRepository;
            _emailNotificationService = emailNotificationService;
            _teachingPlanService = teachingPlanService;
            _feedbackRepository = feedbackRepository;
            _teachingPlanRepository = teachingPlanRepository;
            _traineeStatisticsRepository = traineeStatisticsRepository;
        }

        public async Task<ServiceResult> CreateUserAsync(ApplicationUserDto dto) {
            var user = new ApplicationUser {
                UserName = dto.Email,
                Email = dto.Email,
                EmailConfirmed = true,
                EmailNotificationSetting = _emailNotificationService.CreateDefaultEmailNotificationSetting(dto.Role)
            };

            var result = await _applicationUserRepository.CreateAsync(user, dto.Password);
            if (!result.Succeeded) {
                return ServiceResult.Failed(result.Errors.Select(e => e.Description).ToArray());
            }

            var roleResult = await _applicationUserRepository.AddToRoleAsync(user, dto.Role);
            if (!roleResult.Succeeded) {
                var errors = result.Errors.Concat(roleResult.Errors);
                return ServiceResult.Failed(errors.Select(e => e.Description).ToArray());
            }

            if (dto.Role == "Trainee") {
                if (dto.TraineeStartDate == null || dto.TraineeEndDate == null) {
                    return ServiceResult.Failed("Trainee requires start- and end-date.");
                }
                if (dto.TeachingPlanId == null) {
                    return ServiceResult.Failed("Trainee requires Teachingplan.");
                }

                user.TraineeStartDate = dto.TraineeStartDate;
                user.TraineeEndDate = dto.TraineeEndDate;

                /* var teachingPlanResult = */
                await _teachingPlanService.AssignTeachingPlanToTraineeAsync(user, dto.TeachingPlanId.Value); // TODO: method should return a ServiceResult
                /* if (!teachingPlanResult.Succeeded) {
                    return teachingPlanResult;
                } */

                var updateResult = await _applicationUserRepository.UpdateAsync(user);
                if (!updateResult.Succeeded) {
                    var errors = result.Errors.Concat(updateResult.Errors);
                    return ServiceResult.Failed(errors.Select(e => e.Description).ToArray());
                }
            }

            return ServiceResult.Success();
        }

        public async Task<bool> CloseUserAsync(string userId) {
            var user = await _applicationUserRepository.FindByIdAsync(userId);
            if (user == null) {
                return false;
            }

            user.IsClosed = true;

            var referenceUpdateTasks = new List<Task>();

            foreach (var feedback in user.ReadFeedbacks.ToList()) {
                feedback.ReadByUsers.Remove(user);
                referenceUpdateTasks.Add(_feedbackRepository.UpdateAsync(feedback));
            }
            user.ReadFeedbacks.Clear();

            foreach (var pause in user.ProcessingPauses.ToList()) {
                referenceUpdateTasks.Add(_processingPauseRepository.DeleteAsync(pause));
            }
            user.ProcessingPauses.Clear();

            if (await _applicationUserRepository.IsInRoleAsync(user, "Trainee")) {

                if (user.TeachingPlan == null) {
                    throw new Exception("Trainee requires Teachingplan.");
                }
                referenceUpdateTasks.Add(_teachingPlanService.UnassignTeachingPlanFromTraineeAsync(user, user.TeachingPlan.TeachingPlanId));

                if (user.TraineeStatisticsSnapshot != null) {
                    referenceUpdateTasks.Add(_traineeStatisticsRepository.DeleteAsync(user.TraineeStatisticsSnapshot));
                    user.TraineeStatisticsSnapshot = null;
                }
            }

            await Task.WhenAll(referenceUpdateTasks);
            await _applicationUserRepository.UpdateAsync(user);

            return true;
        }

        public async Task<ServiceResult> CreateProcessingPauseAsync(ProcessingPauseDto dto) {
            var user = await _applicationUserRepository.FindByIdAsync(dto.TraineeId);
            if (user == null) {
                return ServiceResult.Failed("User not found.");
            }
            var processingPause = new ProcessingPause {
                TraineeId = dto.TraineeId,
                Trainee = user,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };

            // Only create processingPause if non-existent
            if (await _processingPauseRepository.ExistsAsync(processingPause)) {
                return ServiceResult.Failed("A break already exists for this user for this period.");
            }
            await _processingPauseRepository.CreateAsync(processingPause);
            return ServiceResult.Success();
        }
    }
}