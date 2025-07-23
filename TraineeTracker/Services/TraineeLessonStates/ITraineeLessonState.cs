using System.Security.Claims;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Services.TraineeLessonStates {

    /// <summary>
    /// Defines the contract for lesson state behavior, including allowed transitions
    /// and enforcement of valid state changes based on user roles.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Simon Hinterreiter (hintsimo)
    /// </remarks>
    public interface ITraineeLessonState {
        /// <summary>
        /// Returns a list of allowed target states based on the current user.
        /// </summary>
        /// <param name="user">The current user (including roles)</param>
        /// <returns>List of valid state transitions</returns>
        List<TraineeLessonState> GetAllowedLessonStateTransitions(ClaimsPrincipal user);

        /// <summary>
        /// Applies a transition to the given target state and returns the resulting state.
        /// Throws if the transition is not allowed.
        /// </summary>
        TraineeLessonState TransitionTo(TraineeLessonState targetState, ClaimsPrincipal user);
    }
}
