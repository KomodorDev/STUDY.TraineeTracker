using Microsoft.AspNetCore.Identity;
using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Data.ProcessingPauses;
using TraineeTracker.Data.TeachingPlans;
using TraineeTracker.Models.Domain;
using TraineeTracker.Models.Dtos;

namespace TraineeTracker.Services.Admin {
    public class AdminService {
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly IProcessingPauseRepository _processingPauseRepository;
        private readonly ITeachingPlanRepository _teachingPlanRepository;

        public AdminService(IApplicationUserRepository applicationUserRepository, IProcessingPauseRepository processingPauseRepository, ITeachingPlanRepository teachingPlanRepository) {
            _applicationUserRepository = applicationUserRepository;
            _processingPauseRepository = processingPauseRepository;
            _teachingPlanRepository = teachingPlanRepository;
        }

        // Hier fehlen die TraineeLessons und die NotificationSettings
        public async Task<ServiceResult> CreateUserAsync(ApplicationUserDto dto) {
            var user = new ApplicationUser {
                UserName = dto.Email,
                Email = dto.Email,
                EmailConfirmed = true
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