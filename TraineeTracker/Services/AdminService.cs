using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Data.ProcessingPauses;
using TraineeTracker.Services.Email;
using TraineeTracker.Models.Domain;
using TraineeTracker.Models.Dtos;
using TraineeTracker.Data.Feedbacks;

namespace TraineeTracker.Services.Admin {
    public class AdminService {
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly IProcessingPauseRepository _processingPauseRepository;

        private readonly EmailNotificationService _emailNotificationService;
        private readonly TeachingPlanService _teachingPlanService;

        private readonly IFeedbackRepository _feedbackRepository;

        public AdminService(IApplicationUserRepository applicationUserRepository,
                            IProcessingPauseRepository processingPauseRepository,
                            EmailNotificationService emailNotificationService,
                            TeachingPlanService teachingPlanService,
                            IFeedbackRepository feedbackRepository) {
            _applicationUserRepository = applicationUserRepository;
            _processingPauseRepository = processingPauseRepository;
            _emailNotificationService = emailNotificationService;
            _teachingPlanService = teachingPlanService;
            _feedbackRepository = feedbackRepository;
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

            var readFeedbacks = await _feedbackRepository.GetAllFeedbacksReadByUserAsync(user);
            foreach (var feedback in readFeedbacks) {
                feedback.ReadByUsers.Remove(user);
                await _feedbackRepository.UpdateAsync(feedback);
            }
            user.ReadFeedbacks.Clear();

            var processingPauses = await _processingPauseRepository.GetAllPausesAsync(user.Id);
            foreach (var pause in processingPauses) {
                await _processingPauseRepository.DeleteAsync(pause);
            }
            user.ProcessingPauses.Clear();

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
            if (_processingPauseRepository.Exists(processingPause)) {
                return ServiceResult.Failed("A break already exists for this user for this period.");
            }
            _processingPauseRepository.Create(processingPause);
            return ServiceResult.Success();
        }
    }
}