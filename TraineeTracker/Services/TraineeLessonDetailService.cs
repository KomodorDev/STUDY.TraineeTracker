// class by schleale

using System.Security.Claims;

using TraineeTracker.Data.Feedbacks;
using TraineeTracker.Data.Lessons;
using TraineeTracker.Data.TraineeLessons;
using TraineeTracker.Data.TraineeLessonLog;
using TraineeTracker.Data.ApplicationUsers;

using TraineeTracker.Models.Domain;
using TraineeTracker.Models.Dtos;
using TraineeTracker.Models.ViewModels;

using TraineeTracker.Exceptions;
using TraineeTracker.Services.TraineeLessonStates;
using TraineeTracker.Services.Email;

namespace TraineeTracker.Services
{
    public class TraineeLessonDetailService {

        private ILessonRepository _databaseLessonRepository;
        private ITraineeLessonRepository _databaseTraineeLessonRepository;
        private ITraineeLessonLogEntryRepository _databaseTraineeLessonLogEntryRepository;
        private IFeedbackRepository _databaseFeedbackrepository;
        // the following are not included in the viewmodel, because i dont't think we need them there?
        private IApplicationUserRepository _databaseApplicationUserRepository;
        private EmailNotificationService _emailNotificationService;

        public TraineeLessonDetailService(ILessonRepository databaseLessonRepository,
                                            ITraineeLessonRepository databaseTraineeLessonRepository,
                                            ITraineeLessonLogEntryRepository databaseTraineeLessonLogEntryRepository,
                                            IFeedbackRepository databaseFeedbackRepository,
                                            IApplicationUserRepository databaseApplicationUserRepository,
                                            EmailNotificationService emailNotificationService) {
            _databaseLessonRepository = databaseLessonRepository;
            _databaseTraineeLessonLogEntryRepository = databaseTraineeLessonLogEntryRepository;
            _databaseTraineeLessonRepository = databaseTraineeLessonRepository;
            _databaseFeedbackrepository = databaseFeedbackRepository;
            _databaseApplicationUserRepository = databaseApplicationUserRepository;
            _emailNotificationService = emailNotificationService;
        }

        private async Task CheckHasAccess(ClaimsPrincipal user, int traineeLessonId) {
            if (user == null)
                throw new UserNotFoundException();

            var traineeLesson = await _databaseTraineeLessonRepository.GetTraineeLessonByIdWithLessonAsync(traineeLessonId) ?? throw new TraineeLessonNotFoundException(traineeLessonId);

            // looks through ClaimsPrincipal user for a claim with the type ClaimTypes.NameIdentifier, which should be the UserId
            var userId = (user.FindFirst(ClaimTypes.NameIdentifier)?.Value) ?? throw new Exception("ClaimTypes.NameIdentifier of user not found.");

            // allows access, if Role is Admin or Mentor
            if (user.IsInRole("Admin") || user.IsInRole("Mentor"))
                return;

            // allows access, if the correct Trainee tries to access
            if (!(userId == traineeLesson.TraineeId))
                throw new UnauthorizedAccessException("You can only access your own TraineeLessons.");
        }

        public async Task<TraineeLessonDetailViewModel> BuildTraineeLessonDetailViewModel(int traineeLessonId, ClaimsPrincipal user) {
            await CheckHasAccess(user, traineeLessonId);

            var tl = await _databaseTraineeLessonRepository.GetTraineeLessonByIdWithLessonAsync(traineeLessonId) ?? throw new TraineeLessonNotFoundException(traineeLessonId);
            var l = await _databaseLessonRepository.GetLessonByIdAsync(tl.LessonId) ?? throw new LessonNotFoundException(tl.LessonId);
            var tll = _databaseTraineeLessonLogEntryRepository.GetAllLogsForTraineeLesson(traineeLessonId);
            var f = await _databaseFeedbackrepository.GetAllFeedbacksForLessonWithLessonAndAuthorAndReadByUsersAsync(l);

            return new TraineeLessonDetailViewModel {
                TraineeLesson = tl,
                Lesson = l,
                LogEntries = tll,
                Feedbacks = f
            };
        }

        public async Task SaveTraineeLessonStateChange(TraineeLessonDto traineeLessonUpdate, ClaimsPrincipal user) {
            await CheckHasAccess(user, traineeLessonUpdate.TraineeLessonId);

            var oldTraineeLesson = await _databaseTraineeLessonRepository.GetTraineeLessonByIdWithLessonAsync(traineeLessonUpdate.TraineeLessonId) ?? throw new TraineeLessonNotFoundException(traineeLessonUpdate.TraineeLessonId);
            var oldState = oldTraineeLesson.State;

            // returns true if TargetStateName could be parsed into targetState
            TraineeLessonState targetState;
            if (!Enum.TryParse<TraineeLessonState>(traineeLessonUpdate.TargetStateName, out targetState))
                throw new Exception("Inalid target state in TraineeLessonDto.");

            // changes state, if allowed - and checks if rejection reason is present, if needed
            TraineeLessonStateFactory factory = new();
            oldTraineeLesson.State = factory.Create(oldTraineeLesson.State).TransitionTo(targetState, user);
            if (oldTraineeLesson.State == TraineeLessonState.Rejected)
                if (String.IsNullOrWhiteSpace(traineeLessonUpdate.RejectionReason))
                    throw new ArgumentException("Rejection reason must be provided for transitioning to rejected.", nameof(traineeLessonUpdate));

            // adds rejection reason and updates db
            oldTraineeLesson.RejectionReason = traineeLessonUpdate.RejectionReason;
            await _databaseTraineeLessonRepository.UpdateAsync(oldTraineeLesson);

            // sends email and creates log
            await _emailNotificationService.NotifyAboutStateChangeAsync(oldTraineeLesson, oldState, targetState);
            await LogStatusChange(oldTraineeLesson, oldState, targetState, user);
        }

        private async Task LogStatusChange(TraineeLesson traineeLesson, TraineeLessonState oldState, TraineeLessonState newState, ClaimsPrincipal user) {
            if (traineeLesson == null)
                throw new TraineeLessonNotFoundException();

            var lesson = await _databaseLessonRepository.GetLessonByIdAsync(traineeLesson.LessonId) ?? throw new LessonNotFoundException(traineeLesson.LessonId);

            _databaseTraineeLessonLogEntryRepository.Create(
                new TraineeLessonLogEntry {
                    TraineeLessonId = traineeLesson.TraineeLessonId,
                    LessonName = lesson.Title,
                    UserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("ClaimTypes.NameIdentifier of user not found."),
                    UserName = user.FindFirst(ClaimTypes.Name)?.Value ?? throw new Exception("ClaimTypes.Name of user not found"),
                    OldState = oldState.ToString(),
                    NewState = newState.ToString(),
                    Timestamp = DateTime.UtcNow
                });
        }

        public async Task SaveFeedback(FeedbackDto feedbackDto, ClaimsPrincipal user) {
            await CheckHasAccess(user, feedbackDto.TraineeLessonId);    // it is basically a state change, hence checking this beforehand

            if (feedbackDto == null)
                throw new Exception("FeedbackDto is null");

            var correspondingTraineeLesson = await _databaseTraineeLessonRepository.GetTraineeLessonByIdWithLessonAsync(feedbackDto.TraineeLessonId) ?? throw new TraineeLessonNotFoundException(feedbackDto.TraineeLessonId);
            var existingFeedback = await _databaseFeedbackrepository.GetFeedbackOfTraineeLessonWithLessonAndAuthorAndReadByUsersAsync(correspondingTraineeLesson);

            if (existingFeedback != null) {
                // feedback exists

                existingFeedback.Difficulty = feedbackDto.Difficulty ?? existingFeedback.Difficulty;
                existingFeedback.PreviousKnowledge = feedbackDto.PreviousKnowledge ?? existingFeedback.PreviousKnowledge;
                existingFeedback.HoursOfEffort = feedbackDto.HoursOfEffort ?? existingFeedback.HoursOfEffort;

                await _databaseFeedbackrepository.UpdateAsync(existingFeedback);
            } else {
                // feedback doesn't exist

                if (!user.IsInRole("Trainee"))
                    throw new UnauthorizedAccessException("You cannot create a feedback as a Mentor or Admin");

                if (correspondingTraineeLesson.State != TraineeLessonState.Accepted)
                    throw new UnauthorizedAccessException("You can write a feedback once your TraineeLesson has been accepted.");

                var authorId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("ClaimTypes.NameIdentifier of user not found.");

                await _databaseFeedbackrepository.CreateAsync(new Feedback {
                    Difficulty = feedbackDto.Difficulty ?? throw new ArgumentNullException(nameof(feedbackDto), "Difficulty cannot be null."),
                    PreviousKnowledge = feedbackDto.PreviousKnowledge ?? throw new ArgumentNullException(nameof(feedbackDto), "PreviousKnowledge cannot be null."),
                    HoursOfEffort = feedbackDto.HoursOfEffort ?? throw new ArgumentNullException(nameof(feedbackDto), "HoursOfEffort cannot be null."),
                    Comment = feedbackDto.Comment,

                    // relations
                    LessonId = correspondingTraineeLesson.LessonId,
                    Lesson = await _databaseLessonRepository.GetLessonByIdAsync(correspondingTraineeLesson.LessonId) ?? throw new LessonNotFoundException(correspondingTraineeLesson.LessonId),
                    AuthorId = authorId,
                    Author = await _databaseApplicationUserRepository.FindByIdAsync(authorId) ?? throw new UserNotFoundException($"User with id {authorId} not found."),
                    ReadByUsers = new List<ApplicationUser>()
                });
            }

            await SaveTraineeLessonStateChange(new TraineeLessonDto {
                TraineeLessonId = feedbackDto.TraineeLessonId,
                TargetStateName = TraineeLessonState.Rated.ToString()
            }, user);
        }

        public async Task DeleteFeedback(ClaimsPrincipal user, int feedbackId) {
            if (user.IsInRole("Trainee"))
                throw new UnauthorizedAccessException("Trainees cannot delete feedbacks.");
            if (!await _databaseFeedbackrepository.ExistsAsync(feedbackId))
                throw new FeedbackNotFoundException(feedbackId);

            await _databaseFeedbackrepository.DeleteAsync(feedbackId);
        }
    }
}