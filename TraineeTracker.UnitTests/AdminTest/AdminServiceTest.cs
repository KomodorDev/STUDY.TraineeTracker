using Moq;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Data.Feedbacks;
using TraineeTracker.Data.ProcessingPauses;
using TraineeTracker.Data.TeachingPlans;
using TraineeTracker.Data.TraineeStatistics;
using TraineeTracker.Data.UnitOfWork;
using TraineeTracker.Models.Domain;
using TraineeTracker.Models.Dtos;
using TraineeTracker.Services;
using TraineeTracker.Services.Admin;
using TraineeTracker.Services.Email;
using TraineeTracker.Data.EmailNotificationSettings;
using TraineeTracker.Data.Lessons;
using TraineeTracker.Data.TraineeLessons;


namespace TraineeTracker.UnitTests.AdminTest {

    /// <summary>
    /// Contains unit tests for the <see cref="AdminService"/> class, verifying user creation, role assignment,
    /// email notification, and user closure functionalities.
    /// </summary>
    /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
    public class AdminServiceTest {

        // ------------------------------------------------------
        /// <summary>
        /// Tests that <see cref="AdminService.CreateUserAsync"/> returns success when provided with valid user data.
        /// Verifies that the user creation method is called once and the result is successful.
        /// </summary>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        [Fact]
        public async Task CreateUserAsync_WithValidData_ReturnsSuccess() {
            // Arrange
            var dto = new ApplicationUserDto { Email = "test@example.com", Password = "Test123!", Role = "Admin" };

            var userRepoMock = new Mock<IApplicationUserRepository>();
            userRepoMock.Setup(r => r.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password)).ReturnsAsync(IdentityResult.Success);
            userRepoMock.Setup(r => r.AddToRoleAsync(It.IsAny<ApplicationUser>(), dto.Role)).ReturnsAsync(IdentityResult.Success);

            var roleManagerMock = new RoleManager<IdentityRole>(Mock.Of<IRoleStore<IdentityRole>>(),
                                                            new List<IRoleValidator<IdentityRole>>(),
                                                            Mock.Of<ILookupNormalizer>(),
                                                            Mock.Of<IdentityErrorDescriber>(),
                                                            Mock.Of<ILogger<RoleManager<IdentityRole>>>());
            var emailServiceMock = new EmailNotificationService(Mock.Of<IEmailSender>(),
                                                                Mock.Of<IApplicationUserRepository>(),
                                                                Mock.Of<IEmailNotificationSettingRepository>(),
                                                                Mock.Of<ILessonRepository>());
            var teachingPlanServiceMock = new TeachingPlanService(Mock.Of<ITeachingPlanRepository>(),
                                                                  Mock.Of<ILessonRepository>(),
                                                                  Mock.Of<ITraineeLessonRepository>(),
                                                                  Mock.Of<IApplicationUserRepository>(),
                                                                  emailServiceMock);

            var service = new AdminService(Mock.Of<IUnitOfWork>(),
                                           userRepoMock.Object,
                                           Mock.Of<IProcessingPauseRepository>(),
                                           roleManagerMock,
                                           emailServiceMock,
                                           teachingPlanServiceMock,
                                           Mock.Of<IFeedbackRepository>(),
                                           Mock.Of<ITeachingPlanRepository>(),
                                           Mock.Of<ITraineeStatisticsRepository>());

            // Act
            var result = await service.CreateUserAsync(dto, true, null);

            // Assert
            userRepoMock.Verify(r => r.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            Assert.True(result.Succeeded);
        }

        // ------------------------------------------------------
        /// <summary>
        /// Tests that <see cref="AdminService.CreateUserAsync"/> returns a failed result when adding the user to a role fails.
        /// Verifies that the error message contains the expected role error description.
        /// </summary>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        [Fact]
        public async Task CreateUserAsync_AddToRoleFails_ReturnsFailed() {
            // Arrange
            var dto = new ApplicationUserDto { Email = "test@example.com", Password = "Test123!", Role = "Admin" };

            var userRepoMock = new Mock<IApplicationUserRepository>();
            userRepoMock.Setup(r => r.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password)).ReturnsAsync(IdentityResult.Success);
            userRepoMock.Setup(r => r.AddToRoleAsync(It.IsAny<ApplicationUser>(), dto.Role)).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Role error" }));
            userRepoMock.Setup(r => r.DeleteAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);

            var roleManagerMock = new RoleManager<IdentityRole>(Mock.Of<IRoleStore<IdentityRole>>(),
                                                            new List<IRoleValidator<IdentityRole>>(),
                                                            Mock.Of<ILookupNormalizer>(),
                                                            Mock.Of<IdentityErrorDescriber>(),
                                                            Mock.Of<ILogger<RoleManager<IdentityRole>>>());
            var emailServiceMock = new EmailNotificationService(Mock.Of<IEmailSender>(),
                                                                Mock.Of<IApplicationUserRepository>(),
                                                                Mock.Of<IEmailNotificationSettingRepository>(),
                                                                Mock.Of<ILessonRepository>());
            var teachingPlanServiceMock = new TeachingPlanService(Mock.Of<ITeachingPlanRepository>(),
                                                                  Mock.Of<ILessonRepository>(),
                                                                  Mock.Of<ITraineeLessonRepository>(),
                                                                  Mock.Of<IApplicationUserRepository>(),
                                                                  emailServiceMock);

            var service = new AdminService(Mock.Of<IUnitOfWork>(),
                                           userRepoMock.Object,
                                           Mock.Of<IProcessingPauseRepository>(),
                                           roleManagerMock,
                                           emailServiceMock,
                                           teachingPlanServiceMock,
                                           Mock.Of<IFeedbackRepository>(),
                                           Mock.Of<ITeachingPlanRepository>(),
                                           Mock.Of<ITraineeStatisticsRepository>());

            // Act
            var result = await service.CreateUserAsync(dto, true, null);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("Role error", result.ErrorMessages.FirstOrDefault() ?? "");
        }

        // ------------------------------------------------------
        /// <summary>
        /// Tests that <see cref="AdminService.CreateUserAsync"/> returns a failed result when email notification dispatch fails.
        /// Verifies that the error message contains the expected email dispatch failure description.
        /// </summary>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        [Fact]
        public async Task CreateUserAsync_NotifyUserFails_ReturnsFailed() {
            // Arrange
            var dto = new ApplicationUserDto { Email = "test@example.com", Password = "Test123!", Role = "Admin" };

            var userRepoMock = new Mock<IApplicationUserRepository>();
            userRepoMock.Setup(r => r.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password)).ReturnsAsync(IdentityResult.Success);
            userRepoMock.Setup(r => r.AddToRoleAsync(It.IsAny<ApplicationUser>(), dto.Role)).ReturnsAsync(IdentityResult.Success);
            userRepoMock.Setup(r => r.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>())).ReturnsAsync("token");

            var emailSenderMock = new Mock<IEmailSender>();
            emailSenderMock.Setup(s => s.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception("EmailDispatchFailed"));

            var emailServiceMock = new EmailNotificationService(emailSenderMock.Object,
                                                                Mock.Of<IApplicationUserRepository>(),
                                                                Mock.Of<IEmailNotificationSettingRepository>(),
                                                                Mock.Of<ILessonRepository>());

            var roleManagerMock = new RoleManager<IdentityRole>(Mock.Of<IRoleStore<IdentityRole>>(),
                                                            new List<IRoleValidator<IdentityRole>>(),
                                                            Mock.Of<ILookupNormalizer>(),
                                                            Mock.Of<IdentityErrorDescriber>(),
                                                            Mock.Of<ILogger<RoleManager<IdentityRole>>>());

            var teachingPlanServiceMock = new TeachingPlanService(Mock.Of<ITeachingPlanRepository>(),
                                                                  Mock.Of<ILessonRepository>(),
                                                                  Mock.Of<ITraineeLessonRepository>(),
                                                                  Mock.Of<IApplicationUserRepository>(),
                                                                  emailServiceMock);

            var service = new AdminService(Mock.Of<IUnitOfWork>(),
                                           userRepoMock.Object,
                                           Mock.Of<IProcessingPauseRepository>(),
                                           roleManagerMock,
                                           emailServiceMock,
                                           teachingPlanServiceMock,
                                           Mock.Of<IFeedbackRepository>(),
                                           Mock.Of<ITeachingPlanRepository>(),
                                           Mock.Of<ITraineeStatisticsRepository>());

            // Act
            var result = await service.CreateUserAsync(dto, false, new UrlHelperMock());

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("EmailDispatchFailed", result.ErrorMessages.FirstOrDefault() ?? "");
        }

        // ------------------------------------------------------
        /// <summary>
        /// Tests that <see cref="AdminService.CloseUserAsync"/> correctly sets the <c>IsClosed</c> property of a user to true.
        /// Verifies that the user is marked as closed after the operation.
        /// </summary>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        [Fact]
        public async Task CloseUserAsync_UserGetsClosed_IsClosedIsTrue() {
            // Arrange
            var userId = "test-id";
            var user = new ApplicationUser { Id = userId, IsClosed = false, EmailNotificationSetting = Mock.Of<EmailNotificationSetting>() };

            var userRepoMock = new Mock<IApplicationUserRepository>();
            userRepoMock.Setup(r => r.FindByIdAsync(userId)).ReturnsAsync(user);
            userRepoMock.Setup(r => r.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            var roleManagerMock = new RoleManager<IdentityRole>(Mock.Of<IRoleStore<IdentityRole>>(),
                                                            new List<IRoleValidator<IdentityRole>>(),
                                                            Mock.Of<ILookupNormalizer>(),
                                                            Mock.Of<IdentityErrorDescriber>(),
                                                            Mock.Of<ILogger<RoleManager<IdentityRole>>>());

            var emailServiceMock = new EmailNotificationService(Mock.Of<IEmailSender>(),
                                                                Mock.Of<IApplicationUserRepository>(),
                                                                Mock.Of<IEmailNotificationSettingRepository>(),
                                                                Mock.Of<ILessonRepository>());

            var teachingPlanServiceMock = new TeachingPlanService(Mock.Of<ITeachingPlanRepository>(),
                                                                  Mock.Of<ILessonRepository>(),
                                                                  Mock.Of<ITraineeLessonRepository>(),
                                                                  Mock.Of<IApplicationUserRepository>(),
                                                                  emailServiceMock);

            var service = new AdminService(Mock.Of<IUnitOfWork>(),
                                           userRepoMock.Object,
                                           Mock.Of<IProcessingPauseRepository>(),
                                           roleManagerMock,
                                           emailServiceMock,
                                           teachingPlanServiceMock,
                                           Mock.Of<IFeedbackRepository>(),
                                           Mock.Of<ITeachingPlanRepository>(),
                                           Mock.Of<ITraineeStatisticsRepository>());
            // Act
            await service.CloseUserAsync(userId);
            // Assert
            Assert.True(user.IsClosed);
        }
    }
}