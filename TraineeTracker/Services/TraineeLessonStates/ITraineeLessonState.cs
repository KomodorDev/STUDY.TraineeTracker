using System.Security.Claims;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Services.TraineeLessonStates {
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
