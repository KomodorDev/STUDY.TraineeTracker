using TraineeTracker.Models.Domain;
using TraineeTracker.Services.TraineeLessonStates.States;

namespace TraineeTracker.Services.TraineeLessonStates {

    /// <summary>
    /// Factory responsible for creating concrete implementations of <see cref="ITraineeLessonState"/>
    /// based on the current enum <see cref="TraineeLessonState"/>.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Simon Hinterreiter (hintsimo)
    /// </remarks>
    public class TraineeLessonStateFactory {

        // ----------------------------------------------
        /// <summary>
        /// Creates a concrete <see cref="ITraineeLessonState"/> instance for the given <paramref name="state"/>.
        /// </summary>
        /// <param name="state">The current logical state of the trainee lesson.</param>
        /// <returns>An instance of a concrete <see cref="ITraineeLessonState"/> implementation.</returns>
        /// <exception cref="NotSupportedException">
        /// Thrown when the given <paramref name="state"/> is not supported by the factory.
        /// </exception>
        /// <remarks>
        /// Code Ownership: Simon Hinterreiter (hintsimo)
        /// </remarks>
        public ITraineeLessonState Create(TraineeLessonState state) {
            return state switch {
                TraineeLessonState.Open => new OpenState(),
                TraineeLessonState.Started => new StartedState(),
                TraineeLessonState.Finished => new FinishedState(),
                TraineeLessonState.Accepted => new AcceptedState(),
                TraineeLessonState.Rejected => new RejectedState(),
                TraineeLessonState.Skipped => new SkippedState(),
                TraineeLessonState.Rated => new RatedState(),
                _ => throw new NotSupportedException($"State '{state}' is not supported.")
            };
        }

        // ----------------------------------------------
    } 
}
