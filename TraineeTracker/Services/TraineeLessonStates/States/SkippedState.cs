using System.Security.Claims;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Services.TraineeLessonStates.States {

    /// <summary>
    /// Represents the 'Skipped' state of a trainee lesson and defines the allowed transitions from this state.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Simon Hinterreiter (hintsimo)
    /// </remarks>
    public class SkippedState : ITraineeLessonState {

        // ----------------------------------------------
        /// <summary>
        /// Returns a list of lesson states that the current user is allowed to transition to from the 'Skipped' state.
        /// </summary>
        /// <param name="user">The user attempting the state transition.</param>
        /// <returns>
        /// A list of <see cref="TraineeLessonState"/> values representing valid target states
        /// from the current 'Skipped' state based on the user's role.
        /// </returns>
        /// <remarks>
        /// Code Ownership: Simon Hinterreiter (hintsimo)
        /// </remarks>
        public List<TraineeLessonState> GetAllowedLessonStateTransitions(ClaimsPrincipal user) {

            var transitions = new List<TraineeLessonState>();
            if (user.IsInRole("Mentor") || user.IsInRole("Admin")) {
                // Mentor/Admin can go to any state
                transitions.Add(TraineeLessonState.Open);
            }

            if (user.IsInRole("Trainee")) {
                // Nothing
            }

            return transitions;
        }

        // ----------------------------------------------
        /// <summary>
        /// Attempts to transition from the 'Skipped' state to the specified target state, 
        /// verifying that the transition is allowed for the given user.
        /// </summary>
        /// <param name="targetState">The desired target state to transition to.</param>
        /// <param name="user">The user attempting the transition.</param>
        /// <returns>The target state, if the transition is allowed.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the transition to <paramref name="targetState"/> is not allowed for the user's role.
        /// </exception>
        /// <remarks>
        /// Code Ownership: Simon Hinterreiter (hintsimo)
        /// </remarks>
        public TraineeLessonState TransitionTo(TraineeLessonState targetState, ClaimsPrincipal user) {
            var allowed = GetAllowedLessonStateTransitions(user);

            if (!allowed.Contains(targetState)) {
                throw new InvalidOperationException($"Transition from 'Skipped' to '{targetState}' is not allowed for this user.");
            }
            return targetState;
        }

        // ----------------------------------------------
    }
}