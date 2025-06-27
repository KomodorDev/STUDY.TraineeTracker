using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Data.ProcessingPauses;
using TraineeTracker.Models.Domain;
using TraineeTracker.Models.Dtos;

namespace TraineeTracker.Services.Admin {
    public class AdminService {
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly IProcessingPauseRepository _processingPauseRepository;

        private readonly EmailNotificationSettingService _emailNotificationSettingService;
        private readonly TeachingPlanService _teachingPlanService;

        public AdminService(IApplicationUserRepository applicationUserRepository, IProcessingPauseRepository processingPauseRepository, IEmailNotificationSettingService emailNotificationSettingService, ITeachingPlanService teachingPlanService) {
            _applicationUserRepository = applicationUserRepository;
            _processingPauseRepository = processingPauseRepository;
            _emailNotificationSettingService = emailNotificationSettingService;
            _teachingPlanService = teachingPlanService;
        }

        public async Task<ServiceResult> CreateUserAsync(ApplicationUserDto dto) {
            var user = new ApplicationUser {
                UserName = dto.Email,
                Email = dto.Email,
                EmailConfirmed = true,
                EmailNotificationSetting = _emailNotificationSettingService.CreateDefaultSettings(dto.Role)
            };

            if (dto.Role == "Trainee") {
                if (dto.TeachingPlanId == null) {
                    return ServiceResult.Failed("Teachingplan required for Trainee.");
                }
                var teachingPlan = _teachingPlanService.GetTeachingPlanByIdAsync(dto.TeachingPlanId);
                if (teachingPlan == null) {
                    return ServiceResult.Failed("Teachingplan not found.");
                }
                user.TeachingPlan = teachingPlan;
                
                var traineeLessons = await _teachingPlanService.CreateTraineeLessonsAsync(user, dto.TeachingPlanId);
                user.TraineeLessons = traineeLessons;
            }

            var result = await _applicationUserRepository.CreateAsync(user, dto.Password);
            if (!result.Succeeded) {
                return ServiceResult.Failed(result.Errors.Select(e => e.Description).ToArray());
            }

            var roleResult = await _applicationUserRepository.AddToRoleAsync(user, dto.Role);
            if (!roleResult.Succeeded) {
                var errors = result.Errors.Concat(roleResult.Errors);
                return ServiceResult.Failed(errors.Select(e => e.Description).ToArray());
            }

            return ServiceResult.Success();
        }

        public async Task<bool> SetIsClosedAsync(string userId, bool isClosed) {
            var user = await _applicationUserRepository.FindByIdAsync(userId);
            if (user == null) {
                return false;
            }
            user.IsClosed = isClosed;
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
            if (_processingPauseRepository.Exists(processingPause)) {
                _processingPauseRepository.Create(processingPause);
                return ServiceResult.Failed("A break already exists for this user for this period.");
            }
            return ServiceResult.Success();
        }
    }
}