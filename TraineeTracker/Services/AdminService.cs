using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Data.ProcessingPauses;
using TraineeTracker.Services.Email;
using TraineeTracker.Models.Domain;
using TraineeTracker.Models.Dtos;
using TraineeTracker.Data.Feedbacks;
using TraineeTracker.Data.TeachingPlans;
using TraineeTracker.Data.TraineeStatistics;
using TraineeTracker.Models.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace TraineeTracker.Services.Admin {
    public class AdminService {
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly IProcessingPauseRepository _processingPauseRepository;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly EmailNotificationService _emailNotificationService;
        private readonly TeachingPlanService _teachingPlanService;

        private readonly IFeedbackRepository _feedbackRepository;
        private readonly ITeachingPlanRepository _teachingPlanRepository;
        private readonly ITraineeStatisticsRepository _traineeStatisticsRepository;

        public AdminService(IApplicationUserRepository applicationUserRepository,
                            IProcessingPauseRepository processingPauseRepository,
                            RoleManager<IdentityRole> roleManager,
                            EmailNotificationService emailNotificationService,
                            TeachingPlanService teachingPlanService,
                            IFeedbackRepository feedbackRepository,
                            ITeachingPlanRepository teachingPlanRepository,
                            ITraineeStatisticsRepository traineeStatisticsRepository) {
            _applicationUserRepository = applicationUserRepository;
            _processingPauseRepository = processingPauseRepository;
            _roleManager = roleManager;
            _emailNotificationService = emailNotificationService;
            _teachingPlanService = teachingPlanService;
            _feedbackRepository = feedbackRepository;
            _teachingPlanRepository = teachingPlanRepository;
            _traineeStatisticsRepository = traineeStatisticsRepository;
        }

        public async Task<AdminDashboardViewModel> BuildAdminDashboardViewModelAsync() {
            var users = await _applicationUserRepository.GetAllAsync();
            var userRoles = new Dictionary<string, string>();
            foreach (var user in users) {
                var roles = await _applicationUserRepository.GetRolesAsync(user);
                if (roles.Contains("Admin")) {
                    userRoles[user.Id] = "Admin";
                } else {
                    userRoles[user.Id] = roles.First();
                }
            }
            return new AdminDashboardViewModel {
                Users = users,
                UserRoles = userRoles
            };
        }

        public async Task<CreateUserViewModel> BuildCreateUserViewModelAsync() {
            var viewModel = new CreateUserViewModel();
            return await FillCreateUserDropdownsAsync(viewModel);
        }

        public async Task<CreateUserViewModel> FillCreateUserDropdownsAsync(CreateUserViewModel viewModel) {
            ArgumentNullException.ThrowIfNull(viewModel);
            var rolesTask = _roleManager.Roles.ToListAsync();
            var plansTask = _teachingPlanRepository.GetAllTeachingPlansAsync();
            var roles = await rolesTask;
            viewModel.Roles = roles.Select(r => new SelectListItem {
                Value = r.Name,
                Text = r.Name
            }).ToList();
            var plans = await plansTask;
            viewModel.TeachingPlans = plans.Select(p => new SelectListItem {
                Value = p.TeachingPlanId.ToString(),
                Text = p.Name
            }).ToList();
            return viewModel;
        }

        public async Task<ServiceResult> CreateUserAsync(ApplicationUserDto dto) {
            ArgumentNullException.ThrowIfNull(dto);
            var user = new ApplicationUser {
                UserName = dto.Email,
                Email = dto.Email,
                EmailConfirmed = true,
                EmailNotificationSetting = _emailNotificationService.CreateDefaultEmailNotificationSetting(dto.Role)
            };

            if (dto.Role == "Trainee") {
                if (dto.TraineeStartDate == null || dto.TraineeEndDate == null) {
                    return ServiceResult.Failed("Trainee requires start- and end-date.");
                }
                if (dto.TeachingPlanId == null) {
                    return ServiceResult.Failed("Trainee requires Teachingplan.");
                }
            }

            var result = await _applicationUserRepository.CreateAsync(user, dto.Password);
            if (!result.Succeeded) {
                return ServiceResult.Failed(result.Errors.Select(e => e.Description).ToArray());
            }

            result = await _applicationUserRepository.AddToRoleAsync(user, dto.Role);
            if (!result.Succeeded) {
                await _applicationUserRepository.DeleteAsync(user);
                return ServiceResult.Failed(result.Errors.Select(e => e.Description).ToArray());
            }

            if (dto.Role == "Trainee") {
                user.TraineeStartDate = dto.TraineeStartDate;
                user.TraineeEndDate = dto.TraineeEndDate;

                if (dto.TeachingPlanId == null) {
                    return ServiceResult.Failed("Trainee requires Teachingplan.");  // only for compiler
                }
                /* result = */
                await _teachingPlanService.AssignTeachingPlanToTraineeAsync(user, dto.TeachingPlanId.Value); // TODO: method should return a ServiceResult
                /* if (!result.Succeeded) {
                    await _applicationUserRepository.DeleteAsync(user);
                    return result;
                } */

                result = await _applicationUserRepository.UpdateAsync(user);
                if (!result.Succeeded) {
                    await _applicationUserRepository.DeleteAsync(user);
                    return ServiceResult.Failed(result.Errors.Select(e => e.Description).ToArray());
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

            if (await _applicationUserRepository.IsInRoleAsync(user, "Trainee")) {
                foreach (var pause in user.ProcessingPauses.ToList()) {
                    referenceUpdateTasks.Add(_processingPauseRepository.DeleteAsync(pause));
                }
                user.ProcessingPauses.Clear();

                if (user.TeachingPlanId == null) {
                    throw new Exception("Trainee requires Teachingplan.");
                }
                referenceUpdateTasks.Add(_teachingPlanService.UnassignTeachingPlanFromTraineeAsync(user));

                if (user.TraineeStatisticsSnapshot != null) {
                    referenceUpdateTasks.Add(_traineeStatisticsRepository.DeleteAsync(user.TraineeStatisticsSnapshot));
                    user.TraineeStatisticsSnapshot = null;
                }
            } else {
                foreach (var feedback in user.ReadFeedbacks.ToList()) {
                    feedback.ReadByUsers.Remove(user);
                    referenceUpdateTasks.Add(_feedbackRepository.UpdateAsync(feedback));
                }
                user.ReadFeedbacks.Clear();

                user.LastSelectedTrainees.Clear();
            }

            await Task.WhenAll(referenceUpdateTasks);
            await _applicationUserRepository.UpdateAsync(user);

            return true;
        }

        public async Task<CreateProcessingPauseViewModel> BuildCreateProcessingPauseViewModel(string traineeId) {
            var user = await _applicationUserRepository.FindByIdAsync(traineeId);
            return new CreateProcessingPauseViewModel {
                ProcessingPause = new ProcessingPauseDto {
                    TraineeId = traineeId
                },
                UserName = user.UserName
            };
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

            var result = await CheckProcessingPause(processingPause);
            if (!result.Succeeded) {
                return result;
            }
            await _processingPauseRepository.CreateAsync(processingPause);
            return ServiceResult.Success();
        }
        public async Task UpdateProcessingPauseAsync(ProcessingPauseDto dto) {
            if (!dto.ProcessingPauseId.HasValue) {
                throw new Exception($"Missing {nameof(dto.ProcessingPauseId)} in {nameof(dto)}");
            }
            var pause = await _processingPauseRepository.FindByIdAsync(dto.ProcessingPauseId.Value);
            if (pause == null) {
                throw new Exception($"{nameof(dto)} not found.");
            }
            var trainee = await _applicationUserRepository.FindByIdAsync(dto.TraineeId);
            if (trainee == null) {
                throw new Exception($"{nameof(trainee)} not found.");
            }
            pause.TraineeId = dto.TraineeId;
            pause.Trainee = trainee;
            pause.StartDate = dto.StartDate;
            pause.EndDate = dto.EndDate;
            var result = await CheckProcessingPause(pause);
            if (!result.Succeeded) {
                throw new Exception(result.ErrorMessages.First());
            }
            await _processingPauseRepository.UpdateAsync(pause);
        }

        private async Task<ServiceResult> CheckProcessingPause(ProcessingPause processingPause) {
            if (processingPause.StartDate > processingPause.EndDate) {
                return ServiceResult.Failed("Startdate after Enddate");
            }
            if (await _processingPauseRepository.OverlapsAsync(processingPause)) {
                return ServiceResult.Failed($"{nameof(processingPause)} overlaps with another.");
            }
            return ServiceResult.Success();
        }

        public async Task<ProcessingPause> DeleteProcessingPauseAsync(int processingPauseId) {
            var pause = await _processingPauseRepository.FindByIdAsync(processingPauseId);
            var traineeId = pause.TraineeId;
            await _processingPauseRepository.DeleteAsync(pause);
            return pause;
        }

        public async Task<ApplicationUser?> FindByIdWithProcessingPausesAsync(string userId) {
            return await _applicationUserRepository.FindByIdWithProcessingPausesAsync(userId);
        }

        public async Task<ProcessingPauseDto> GetProcessingPauseDtoAsync(int processingPauseId) {
            var pause = await _processingPauseRepository.FindByIdAsync(processingPauseId);
            if (pause == null) {
                throw new Exception($"{nameof(pause)} not found.");
            }
            return new ProcessingPauseDto {
                ProcessingPauseId = pause.ProcessingPauseId,
                TraineeId = pause.TraineeId,
                StartDate = pause.StartDate,
                EndDate = pause.EndDate
            };
        }
    }
}