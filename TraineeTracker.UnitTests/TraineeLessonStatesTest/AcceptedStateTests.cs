using System.Security.Claims;
using TraineeTracker.Models.Domain;
using TraineeTracker.Services.TraineeLessonStates.States;

namespace TraineeTracker.UnitTests.TraineeLessonStates {

    /// <summary>
    /// Unit tests for the AcceptedState class.
    /// Verifies allowed transitions for different user roles.
    /// </summary>
    /// <remarks>Code Ownership: Simon Hinterreiter (hintsimo)</remarks>
    public class AcceptedStateTests {

        private readonly AcceptedState _state = new();

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
        [InlineData("Admin", TraineeLessonState.Rated)]
        [InlineData("Trainee", TraineeLessonState.Rated)]
        public void TransitionTo_AllowedRoleAndTarget_DoesNotThrow(string role, TraineeLessonState targetState) {
            // Arrange
            var user = CreateUserWithRole(role);

            // Act
            var result = _state.TransitionTo(targetState, user);

            // Assert - Test 1 to 4
            Assert.Equal(targetState, result);
        }

        // ------------------------------------------------------
        [Fact]
        public void TransitionTo_DisallowedTransition_ThrowsException() {
            // Arrange
            var user = CreateUserWithRole("Trainee");
            var disallowedTarget = TraineeLessonState.Finished;

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() =>
                _state.TransitionTo(disallowedTarget, user));

            // Assert - Test 5
            Assert.Contains("not allowed", ex.Message);
        }

        // ------------------------------------------------------
    }
}
