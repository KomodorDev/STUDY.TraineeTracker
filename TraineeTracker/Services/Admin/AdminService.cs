using Microsoft.AspNetCore.Identity;
using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Data.ProcessingPauses;
using TraineeTracker.Models.Domain;
using TraineeTracker.Models.Dtos;

namespace TraineeTracker.Services.Admin {
    public class AdminService {
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly IProcessingPauseRepository _processingPauseRepository;

        public AdminService(IApplicationUserRepository applicationUserRepository, IProcessingPauseRepository processingPauseRepository) {
            _applicationUserRepository = applicationUserRepository;
            _processingPauseRepository = processingPauseRepository;
        }

        // Hier fehlen die TraineeLessons und die NotificationSettings
        public async Task<IdentityResult> CreateUserAsync(ApplicationUserDto dto) {
            var user = new ApplicationUser {
                UserName = dto.Email,
                Email = dto.Email,
                EmailConfirmed = true
            };
            var result = await _applicationUserRepository.CreateAsync(user, dto.Password);
            if (result.Succeeded) {
                await _applicationUserRepository.AddToRoleAsync(user, dto.Role);
            }
            return result;
        }

        public async Task<bool> SetIsClosedAsync(string userId, bool isClosed) {
            var user = await _applicationUserRepository.GetByIdAsync(userId);
            if (user == null) {
                return false;
            }
            user.IsClosed = isClosed;
            _applicationUserRepository.Update(user);
            return true;
        }

        public async Task<ServiceResult> CreateProcessingPauseAsync(ProcessingPauseDto dto) {
            var user = await _applicationUserRepository.GetByIdAsync(dto.TraineeId);
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
                await _processingPauseRepository.CreateAsync(processingPause);
                return ServiceResult.Failed("A break already exists for this user for this period.");
            }
            return ServiceResult.Success();
        }
    }
}