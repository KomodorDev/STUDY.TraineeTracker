using System.Security.Claims;
using TraineeTracker.Models.Domain;
using TraineeTracker.Services.TraineeLessonStates.States;

namespace TraineeTracker.UnitTests.TraineeLessonStates {

    /// <summary>
    /// Unit tests for the RejectedState class.
    /// Verifies allowed transitions for different user roles.
    /// </summary>
    /// <remarks>Code Ownership: Simon Hinterreiter (hintsimo) </remarks>
    public class RejectedStateTests {

        private readonly RejectedState _state = new();

        // ------------------------------------------------------
        private ClaimsPrincipal CreateUserWithRole(string role) {
            var identity = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Role, role)
            }, "mock");

            return new ClaimsPrincipal(identity);
        }

        // ------------------------------------------------------
        [Theory]
        [InlineData("Mentor", TraineeLessonState.Finished)]
        [InlineData("Admin", TraineeLessonState.Finished)]
        [InlineData("Trainee", TraineeLessonState.Finished)]
        public void TransitionTo_AllowedRoleAndTarget_DoesNotThrow(string role, TraineeLessonState targetState) {
            // Arrange
            var user = CreateUserWithRole(role);

            // Act
            var result = _state.TransitionTo(targetState, user);

            // Assert - Test 1 to 3
            Assert.Equal(targetState, result);
        }

        // ------------------------------------------------------
        [Theory]
        [InlineData("Mentor", TraineeLessonState.Accepted)]
        [InlineData("Trainee", TraineeLessonState.Rated)]
        public void TransitionTo_DisallowedTransition_ThrowsException(string role, TraineeLessonState invalidTarget) {
            // Arrange
            var user = CreateUserWithRole(role);

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() =>
                _state.TransitionTo(invalidTarget, user));

            // Assert - Test 4 to 5
            Assert.Contains("not allowed", ex.Message);
        }

        // ------------------------------------------------------
    }
}
