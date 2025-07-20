using TraineeTracker.Models.Domain;
using TraineeTracker.Services.TraineeLessonStates.States;

namespace TraineeTracker.Services.TraineeLessonStates {
    public class TraineeLessonStateFactory {

        // ----------------------------------------------
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
    }
}
