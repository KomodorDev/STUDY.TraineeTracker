using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Text;
using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Data.ProcessingPauses;
using TraineeTracker.Data.Feedbacks;
using TraineeTracker.Data.TeachingPlans;
using TraineeTracker.Data.TraineeStatistics;
using TraineeTracker.Data.UnitOfWork;

using TraineeTracker.Models;
using TraineeTracker.Models.Domain;
using TraineeTracker.Models.Dtos;
using TraineeTracker.Models.ViewModels.Admin;
using TraineeTracker.Services.Email;

namespace TraineeTracker.Services.Admin {
    /// <summary>
    /// Provides administrative services for managing users, roles, processing pauses, teaching plans, feedbacks, and trainee statistics.
    /// </summary>
    /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
    public class AdminService {

        private const int _pageSize = 20;
        private readonly IUnitOfWork _unitOfWork;

        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly IProcessingPauseRepository _processingPauseRepository;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly EmailNotificationService _emailNotificationService;
        private readonly TeachingPlanService _teachingPlanService;

        private readonly IFeedbackRepository _feedbackRepository;
        private readonly ITeachingPlanRepository _teachingPlanRepository;
        private readonly ITraineeStatisticsRepository _traineeStatisticsRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminService"/> class with required dependencies.
        /// </summary>
        /// <param name="unitOfWork">Unit of work for transaction management.</param>
        /// <param name="applicationUserRepository">Repository for application users.</param>
        /// <param name="processingPauseRepository">Repository for processing pauses.</param>
        /// <param name="roleManager">Role manager for identity roles.</param>
        /// <param name="emailNotificationService">Service for sending email notifications.</param>
        /// <param name="teachingPlanService">Service for managing teaching plans.</param>
        /// <param name="feedbackRepository">Repository for feedbacks.</param>
        /// <param name="teachingPlanRepository">Repository for teaching plans.</param>
        /// <param name="traineeStatisticsRepository">Repository for trainee statistics.</param>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public AdminService(IUnitOfWork unitOfWork,
                            IApplicationUserRepository applicationUserRepository,
                            IProcessingPauseRepository processingPauseRepository,
                            RoleManager<IdentityRole> roleManager,
                            EmailNotificationService emailNotificationService,
                            TeachingPlanService teachingPlanService,
                            IFeedbackRepository feedbackRepository,
                            ITeachingPlanRepository teachingPlanRepository,
                            ITraineeStatisticsRepository traineeStatisticsRepository) {
            _unitOfWork = unitOfWork;
            _applicationUserRepository = applicationUserRepository;
            _processingPauseRepository = processingPauseRepository;
            _roleManager = roleManager;
            _emailNotificationService = emailNotificationService;
            _teachingPlanService = teachingPlanService;
            _feedbackRepository = feedbackRepository;
            _teachingPlanRepository = teachingPlanRepository;
            _traineeStatisticsRepository = traineeStatisticsRepository;
        }

        // ------------------------------------------------------
        /// <summary>
        /// Builds the admin dashboard view model with user filtering, sorting, and pagination.
        /// </summary>
        /// <param name="page">Page number for pagination.</param>
        /// <param name="filterRole">Role filter ("all", "Admin", "Mentor", "Trainee").</param>
        /// <param name="filterStatus">Status filter ("all", "open", "closed").</param>
        /// <param name="sortBy">Sort order ("role_asc", "username_asc", etc.).</param>
        /// <returns>The populated <see cref="AdminDashboardViewModel"/>.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<AdminDashboardViewModel> BuildAdminDashboardViewModelAsync(
                int page = 1,
                string? filterRole = "all",
                string? filterStatus = "open",
                string? sortBy = "role_asc") {

            // +++++++++++++++
            // 1. Load all users once
            var allUsers = await _applicationUserRepository.GetAllAsync();

            // +++++++++++++++
            // Initialize Counts
            int totalUserCount = 0;
            int openUserCount = 0;
            int closedUserCount = 0;

            int roleUserCount = 0;
            int adminCount = 0;
            int mentorCount = 0;
            int traineeCount = 0;

            // Dict userRoles
            var userRoles = new Dictionary<string, string>();

            var filteredUsers = new List<ApplicationUser>();

            // +++++++++++++++
            // Iterate through each user in allUsers:
            foreach (var user in allUsers) {
                var roles = await _applicationUserRepository.GetRolesAsync(user);
                userRoles[user.Id] = roles.First();

                // Count for role-tabs (Admin/Mentor/Trainee) - depending on Status
                if (filterStatus == "all"
                    || (filterStatus == "open" && !user.IsClosed)
                    || (filterStatus == "closed" && user.IsClosed)) {
                    switch (userRoles[user.Id]) {
                        case "Admin":
                            adminCount++;
                            break;
                        case "Mentor":
                            mentorCount++;
                            break;
                        case "Trainee":
                            traineeCount++;
                            break;
                    }
                }

                // Count for status-tabs (Total/Open/Closed) - depending on Role
                if (filterRole == "all" || userRoles[user.Id] == filterRole) {
                    totalUserCount++;
                    if (!user.IsClosed)
                        openUserCount++;
                }

                // Users for the current dashboard view
                if (
                    (filterRole == "all" || userRoles[user.Id] == filterRole) &&
                    (filterStatus == "all" ||
                     (filterStatus == "open" && !user.IsClosed) ||
                     (filterStatus == "closed" && user.IsClosed))
                ) {
                    filteredUsers.Add(user);
                }
            }
            roleUserCount = adminCount + mentorCount + traineeCount;
            closedUserCount = totalUserCount - openUserCount;

            // +++++++++++++++
            // Sort the users by sortBy
            var sortedAndFilteredUsers = sortBy!.ToLower() switch {
                "role_asc" => filteredUsers.OrderBy(u => userRoles[u.Id]),
                "role_desc" => filteredUsers.OrderByDescending(u => userRoles[u.Id]),

                "username_asc" => filteredUsers.OrderBy(u => u.UserName),
                "username_desc" => filteredUsers.OrderByDescending(u => u.UserName),

                "startdate_asc" => filteredUsers.OrderBy(u => u.TraineeStartDate),
                "startdate_desc" => filteredUsers.OrderByDescending(u => u.TraineeStartDate),

                "enddate_asc" => filteredUsers.OrderBy(u => u.TraineeEndDate),
                "enddate_desc" => filteredUsers.OrderByDescending(u => u.TraineeEndDate),

                _ => filteredUsers.OrderBy(u => userRoles[u.Id]) // fallback
            };

            // +++++++++++++++
            // Pagination
            int totalUsers = sortedAndFilteredUsers.Count();

            var pagedUsers = sortedAndFilteredUsers
                .Skip((page - 1) * _pageSize)
                .Take(_pageSize)
                .ToList();

            var pagedResult = new Page<ApplicationUser> {
                Items = pagedUsers,
                PageNumber = page,
                PageSize = _pageSize,
                TotalItems = totalUsers
            };

            return new AdminDashboardViewModel {
                Users = pagedResult,
                UserRoles = userRoles,
                FilterRole = filterRole!,
                FilterStatus = filterStatus!,
                SortBy = sortBy,

                TotalUserCount = totalUserCount,
                RoleUserCount = roleUserCount,
                OpenUserCount = openUserCount,
                ClosedUserCount = closedUserCount,
                AdminCount = adminCount,
                MentorCount = mentorCount,
                TraineeCount = traineeCount
            };
        }

        // ------------------------------------------------------
        /// <summary>
        /// Builds the view model for creating a new user, including dropdowns for roles and teaching plans.
        /// </summary>
        /// <returns>The populated <see cref="CreateUserViewModel"/>.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<CreateUserViewModel> BuildCreateUserViewModelAsync() {
            var viewModel = new CreateUserViewModel();
            return await FillCreateUserDropdownsAsync(viewModel);
        }

        // ------------------------------------------------------
        /// <summary>
        /// Fills the dropdown lists in the create user view model with available roles and teaching plans.
        /// </summary>
        /// <param name="viewModel">The view model to populate.</param>
        /// <returns>The updated <see cref="CreateUserViewModel"/>.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
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

        // ------------------------------------------------------
        /// <summary>
        /// Creates a new user with the specified data, assigns roles, and sends confirmation email if required.
        /// </summary>
        /// <param name="dto">User data transfer object.</param>
        /// <param name="isSeeder">Indicates if the user is created by a seeder (no email confirmation).</param>
        /// <param name="urlHelper">URL helper for generating confirmation links.</param>
        /// <returns>A <see cref="ServiceResult"/> indicating success or failure.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<ServiceResult> CreateUserAsync(ApplicationUserDto dto, bool isSeeder, IUrlHelper? urlHelper = null) {
            ArgumentNullException.ThrowIfNull(dto);
            if (!isSeeder && urlHelper == null) {
                throw new ArgumentNullException(nameof(urlHelper), "urlHelper must be provided if isSeeder is false");
            }
            var user = new ApplicationUser {
                UserName = dto.Email,
                Email = dto.Email,
                EmailConfirmed = isSeeder ? true : false,
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
                var deleteTask = _applicationUserRepository.DeleteAsync(user);
                var errors = result.Errors;
                var deleteResult = await deleteTask;
                if (!deleteResult.Succeeded) {
                    errors = result.Errors.Concat(deleteResult.Errors);
                }
                return ServiceResult.Failed(errors.Select(e => e.Description).ToArray());
            }

            if (dto.Role == "Trainee") {
                user.TraineeStartDate = dto.TraineeStartDate;
                user.TraineeEndDate = dto.TraineeEndDate;

                if (dto.TeachingPlanId == null) {
                    return ServiceResult.Failed("Trainee requires Teachingplan.");  // only for compiler
                }
                await _teachingPlanService.AssignTeachingPlanToTraineeAsync(user, dto.TeachingPlanId.Value);

                result = await _applicationUserRepository.UpdateAsync(user);
                if (!result.Succeeded) {
                    var deleteTask = _applicationUserRepository.DeleteAsync(user);
                    var errors = result.Errors;
                    var deleteResult = await deleteTask;
                    if (!deleteResult.Succeeded) {
                        errors = result.Errors.Concat(deleteResult.Errors);
                    }
                    return ServiceResult.Failed(errors.Select(e => e.Description).ToArray());
                }
            }

            if (!isSeeder) {
                ArgumentNullException.ThrowIfNull(urlHelper);
                var token = await _applicationUserRepository.GenerateEmailConfirmationTokenAsync(user);
                token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                var confirmationLink = urlHelper.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new {
                        area = "Identity",
                        userId = user.Id,
                        code = token
                    },
                    protocol: "https");
                try {
                    await _emailNotificationService.NotifyUserAsync(
                        user,
                        "Confirm your email to set your password",
                        $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.\nYou will be redirected to set your password after.");
                }
                catch (Exception ex) {
                    return ServiceResult.Failed(ex.Message);
                }
            }

            return ServiceResult.Success();
        }

        // ------------------------------------------------------
        /// <summary>
        /// Closes a user account, removes related references, and updates the user status.
        /// </summary>
        /// <param name="userId">The ID of the user to close.</param>
        /// <returns>A <see cref="ServiceResult"/> indicating success or failure.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<ServiceResult> CloseUserAsync(string userId) {
            await _unitOfWork.BeginTransactionAsync();

            var user = await _applicationUserRepository.FindByIdAsync(userId);
            if (user == null) {
                await _unitOfWork.RollbackAsync();
                throw new InvalidOperationException($"{nameof(user)} not found");
            }

            user.IsClosed = true;

            var referenceUpdateTasks = new List<Task>();

            if (await _applicationUserRepository.IsInRoleAsync(user, "Trainee")) {
                foreach (var pause in user.ProcessingPauses.ToList()) {
                    referenceUpdateTasks.Add(_processingPauseRepository.DeleteAsync(pause));
                }
                user.ProcessingPauses.Clear();

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

            try {
                await Task.WhenAll(referenceUpdateTasks);
            }
            catch (AggregateException aggEx) {
                await _unitOfWork.RollbackAsync();
                return ServiceResult.Failed(aggEx.InnerExceptions.Select(e => e.Message).ToArray());
            }
            catch (Exception ex) {
                await _unitOfWork.RollbackAsync();
                return ServiceResult.Failed(ex.Message);
            }

            var result = await _applicationUserRepository.UpdateAsync(user);
            if (!result.Succeeded) {
                await _unitOfWork.RollbackAsync();
                return ServiceResult.Failed(result.Errors.Select(e => e.Description).ToArray());
            }

            await _unitOfWork.CommitAsync();
            return ServiceResult.Success();
        }

        // ------------------------------------------------------
        /// <summary>
        /// Builds the view model for managing processing pauses for a specific trainee.
        /// </summary>
        /// <param name="traineeId">The ID of the trainee.</param>
        /// <returns>The populated <see cref="ManageProcessingPausesViewModel"/>, or null if not found.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<ManageProcessingPausesViewModel?> BuildManageProcessingPausesViewModelAsync(string traineeId) {
            var trainee = await FindByIdWithProcessingPausesAsync(traineeId);
            if (trainee == null)
                return null;

            return new ManageProcessingPausesViewModel {
                UserName = trainee.UserName!,
                TraineeId = trainee.Id,
                ProcessingPauses = trainee.ProcessingPauses.ToList(),
                NewProcessingPause = new ProcessingPauseDto {
                    TraineeId = trainee.Id
                }
            };
        }

        // ------------------------------------------------------
        /// <summary>
        /// Creates a new processing pause for a trainee.
        /// </summary>
        /// <param name="dto">Processing pause data transfer object.</param>
        /// <returns>A <see cref="ServiceResult"/> indicating success or failure.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<ServiceResult> CreateProcessingPauseAsync(ProcessingPauseDto dto) {
            var user = await _applicationUserRepository.FindByIdAsync(dto.TraineeId);
            if (user == null) {
                throw new InvalidOperationException($"{nameof(user)} not found.");
            }
            var processingPause = new ProcessingPause {
                TraineeId = dto.TraineeId,
                Trainee = user,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };

            var result = await ValidateProcessingPause(processingPause, true);
            if (!result.Succeeded) {
                return result;
            }
            await _processingPauseRepository.CreateAsync(processingPause);
            return ServiceResult.Success();
        }

        // ------------------------------------------------------
        /// <summary>
        /// Updates an existing processing pause.
        /// </summary>
        /// <param name="dto">Processing pause data transfer object.</param>
        /// <returns>A <see cref="ServiceResult"/> indicating success or failure.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<ServiceResult> UpdateProcessingPauseAsync(ProcessingPauseDto dto) {
            ArgumentNullException.ThrowIfNull(dto.ProcessingPauseId);
            var pause = await _processingPauseRepository.FindByIdAsync(dto.ProcessingPauseId.Value);
            if (pause == null) {
                return ServiceResult.Failed($"{nameof(dto)} not found.");
            }
            var trainee = await _applicationUserRepository.FindByIdAsync(dto.TraineeId);
            if (trainee == null) {
                throw new InvalidOperationException($"{nameof(trainee)} not found.");
            }
            pause.TraineeId = dto.TraineeId;
            pause.Trainee = trainee;
            pause.StartDate = dto.StartDate;
            pause.EndDate = dto.EndDate;
            var result = await ValidateProcessingPause(pause, false);
            if (!result.Succeeded) {
                return result;
            }
            await _processingPauseRepository.UpdateAsync(pause);
            return ServiceResult.Success();
        }

        // ------------------------------------------------------
        /// <summary>
        /// Validates a processing pause for date correctness and overlap with existing pauses.
        /// </summary>
        /// <param name="processingPause">The processing pause to validate.</param>
        /// <param name="newProcessingPause">Indicates if this is a new pause.</param>
        /// <returns>A <see cref="ServiceResult"/> indicating validation result.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        private async Task<ServiceResult> ValidateProcessingPause(ProcessingPause processingPause, bool newProcessingPause) {
            if (processingPause.StartDate > processingPause.EndDate) {
                return ServiceResult.Failed("Startdate after Enddate");
            }
            if (await _processingPauseRepository.OverlapsAsync(processingPause, newProcessingPause)) {
                return ServiceResult.Failed($"{nameof(processingPause)} overlaps with another.");
            }
            return ServiceResult.Success();
        }

        // ------------------------------------------------------
        /// <summary>
        /// Deletes a processing pause by its ID.
        /// </summary>
        /// <param name="processingPauseId">The ID of the processing pause to delete.</param>
        /// <returns>A <see cref="ServiceResult{ProcessingPause}"/> indicating success or failure.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<ServiceResult<ProcessingPause>> DeleteProcessingPauseAsync(int processingPauseId) {
            var pause = await _processingPauseRepository.FindByIdAsync(processingPauseId);
            if (pause == null) {
                return ServiceResult<ProcessingPause>.Failed($"{nameof(pause)} not found");
            }
            await _processingPauseRepository.DeleteAsync(pause);
            return ServiceResult<ProcessingPause>.Success(pause);
        }

        // ------------------------------------------------------
        /// <summary>
        /// Finds a user by ID and includes their processing pauses.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The <see cref="ApplicationUser"/> with processing pauses, or null if not found.</returns>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public async Task<ApplicationUser?> FindByIdWithProcessingPausesAsync(string userId) {
            return await _applicationUserRepository.FindByIdWithProcessingPausesAsync(userId);
        }

        // ------------------------------------------------------
    }
}