using System.Security.Claims;
using TraineeTracker.Data.Feedbacks;
using TraineeTracker.Data.Lessons;
using TraineeTracker.Data.TraineeLessonLog;
using TraineeTracker.Data.TraineeLessons;
using TraineeTracker.Models.Domain;
using TraineeTracker.Models.Dtos;

namespace TraineeTracker.Services
{
    public class TraineeLessonDetailService {
        public TraineeLessonDetailService(ILessonRepository iLessonRepository,
                                            ITraineeLessonRepository iTraineeLessonRepository,
                                            ITraineeLessonLogEntryRepository iTraineeLessonLogEntryRepository,
                                            IFeedbackRepository iFeedbackRepository) {
            // dothings();
        }

        public bool CheckHasAccess(ClaimsPrincipal user, int traineeLessonId) {

        }

        public TraineeLessonDetailViewModel BuildTraineeLessonDetailViewModel(int traineeLessonId, ClaimsPrincipal user) {
            if (CheckHasAccess(user, traineeLessonId)) {

            } else {

            }
        }

        public void SaveTraineeLessonStateChange(TraineeLessonUpdateDto traineeLessonUpdateDto, ClaimsPrincipal user) {

        }

        private void LogStatusChange(TraineeLesson traineeLesson, TraineeLessonState oldState, TraineeLessonState newState, ClaimsPrincipal user) {

        }

        public void SaveFeedback(FeedbackDto feedbackDto, ClaimsPrincipal user, int lessonId) {
            
        }
    }
}