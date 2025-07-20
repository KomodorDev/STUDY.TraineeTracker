using System.Security.Claims;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Services.TraineeLessonStates.States {
    public class SkippedState : ITraineeLessonState {
        
        // ----------------------------------------------
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
        public TraineeLessonState TransitionTo(TraineeLessonState targetState, ClaimsPrincipal user) {
            var allowed = GetAllowedLessonStateTransitions(user);

            if (!allowed.Contains(targetState)) {
                throw new InvalidOperationException($"Transition from 'Skipped' to '{targetState}' is not allowed for this user.");
            }
            return targetState;
        }
    }
}