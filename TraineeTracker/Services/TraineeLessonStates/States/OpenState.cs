using System.Security.Claims;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Services.TraineeLessonStates.States {
    public class OpenState : ITraineeLessonState {
        
        // ----------------------------------------------
        public List<TraineeLessonState> GetAllowedLessonStateTransitions(ClaimsPrincipal user) {

            var transitions = new List<TraineeLessonState>();
            if (user.IsInRole("Mentor") || user.IsInRole("Admin")) {
                // Mentor/Admin can go to any state
                transitions.Add(TraineeLessonState.Started);
                transitions.Add(TraineeLessonState.Skipped);
            }

            if (user.IsInRole("Trainee")) {
                transitions.Add(TraineeLessonState.Started);
            }

            return transitions;
        }


        // ----------------------------------------------
        public TraineeLessonState TransitionTo(TraineeLessonState targetState, ClaimsPrincipal user) {
            var allowed = GetAllowedLessonStateTransitions(user);

            if (!allowed.Contains(targetState)) {
                throw new InvalidOperationException($"Transition from 'Open' to '{targetState}' is not allowed for this user.");
            }
            return targetState;
        }
    }
}