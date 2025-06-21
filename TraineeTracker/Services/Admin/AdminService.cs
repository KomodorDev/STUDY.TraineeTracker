using Microsoft.AspNetCore.Identity;
using TraineeTracker.Models.Domain;
using TraineeTracker.Models.Dtos;

namespace TraineeTracker.Services.Admin {
    public class AdminService {
        private readonly IApplicationUserRepository _applicationUserRepository;

        public AdminService(IApplicationUserRepository applicationUserRepository) {
            _applicationUserRepository = applicationUserRepository;
        }
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
    }
}