using TraineeTracker.Models.Domain;
using TraineeTracker.Services.TraineeLessonStates;
using TraineeTracker.Services.TraineeLessonStates.States;

namespace TraineeTracker.UnitTests.TraineeLessonStatesTest {

    /// <summary>
    /// Unit tests for the TraineeLessonStateFactory class.
    /// These tests verify that the correct ITraineeLessonState implementation
    /// is returned for each valid TraineeLessonState enum value,
    /// and that an unsupported value throws an exception.
    /// </summary>
    /// <remarks>Code Ownnership: Simon Hinterreiter (hintsimo)</remarks>
    public class TraineeLessonStateFactoryTest {

        // Create the factory instance
        private readonly TraineeLessonStateFactory _factory = new();

        // ------------------------------------------------------
        // This test runs multiple times with different inputs
        [Theory]
        [InlineData(TraineeLessonState.Open, typeof(OpenState))]
        [InlineData(TraineeLessonState.Started, typeof(StartedState))]
        [InlineData(TraineeLessonState.Finished, typeof(FinishedState))]
        [InlineData(TraineeLessonState.Accepted, typeof(AcceptedState))]
        [InlineData(TraineeLessonState.Rejected, typeof(RejectedState))]
        [InlineData(TraineeLessonState.Skipped, typeof(SkippedState))]
        [InlineData(TraineeLessonState.Rated, typeof(RatedState))]
        public void Create_ValidState_ReturnsCorrectStateInstance(TraineeLessonState state, Type expectedType) {

            // Arrange & Act - Call the Create method with a specific state
            var result = _factory.Create(state);

            // Assert - Test 1 to 7
            Assert.IsType(expectedType, result);
        }

        // ------------------------------------------------------
        [Fact]
        public void Create_InvalidState_ThrowsNotSupportedException() {
            // Arrange - Create an invalid enum value (not defined in the TraineeLessonState enum)
            var invalidValue = (TraineeLessonState)999;

            // Act - Check if the factory throws a NotSupportedException when using an invalid value
            var exception = Assert.Throws<NotSupportedException>(() => _factory.Create(invalidValue));

            // Assert - Test 8:
            Assert.Contains("State '999' is not supported", exception.Message);
        }

        // ------------------------------------------------------
    }
}
