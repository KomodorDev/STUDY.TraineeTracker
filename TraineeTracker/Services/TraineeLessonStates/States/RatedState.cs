using System.Security.Claims;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Services.TraineeLessonStates.States {

    /// <summary>
    /// Represents the 'Rated' state of a trainee lesson. No further transitions are allowed from this state.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Simon Hinterreiter (hintsimo)
    /// </remarks>
    public class RatedState : ITraineeLessonState {

        // ----------------------------------------------
        /// <summary>
        /// Returns an empty list, as no transitions are allowed from the 'Rated' state.
        /// </summary>
        /// <param name="user">The user attempting the state transition.</param>
        /// <returns>
        /// An empty list of <see cref="TraineeLessonState"/> values, indicating no valid transitions.
        /// </returns>
        /// <remarks>
        /// Code Ownership: Simon Hinterreiter (hintsimo)
        /// </remarks>
        public List<TraineeLessonState> GetAllowedLessonStateTransitions(ClaimsPrincipal user) {

            var transitions = new List<TraineeLessonState>();
            if (user.IsInRole("Mentor") || user.IsInRole("Admin")) {
                // Nothing
            }

            if (user.IsInRole("Trainee")) {
                // Nothing
            }

            return transitions;
        }

        // ----------------------------------------------
        /// <summary>
        /// Attempts to transition from the 'Rated' state to another state.
        /// This always fails, since no transitions are allowed from 'Rated'.
        /// </summary>
        /// <param name="targetState">The target state to transition to.</param>
        /// <param name="user">The user attempting the transition.</param>
        /// <returns>This method does not return normally; it always throws.</returns>
        /// <exception cref="InvalidOperationException">
        /// Always thrown because no transitions are allowed from the 'Rated' state.
        /// </exception>
        /// <remarks>
        /// Code Ownership: Simon Hinterreiter (hintsimo)
        /// </remarks>
        public TraineeLessonState TransitionTo(TraineeLessonState targetState, ClaimsPrincipal user) {
            var allowed = GetAllowedLessonStateTransitions(user);

            if (!allowed.Contains(targetState)) {
                throw new InvalidOperationException($"Transition from 'Rated' to '{targetState}' is not allowed for this user.");
            }
            return targetState;
        }

        // ----------------------------------------------
    }
}