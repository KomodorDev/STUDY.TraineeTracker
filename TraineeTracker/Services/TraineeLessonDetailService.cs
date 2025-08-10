using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

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

using TraineeTracker.Data;

namespace TraineeTracker.Services {
    public class TraineeLessonDetailService {

        /// <summary>
        /// Repository for managing Lessons.
        /// </summary>
        private ILessonRepository _databaseLessonRepository;

        /// <summary>
        /// Repository for managing trainee lessons, including retrieval and updates.
        /// </summary>
        private ITraineeLessonRepository _databaseTraineeLessonRepository;

        /// <summary>
        /// Repository for managing log entries related to trainee lesson state changes.
        /// </summary>
        private ITraineeLessonLogEntryRepository _databaseTraineeLessonLogEntryRepository;

        /// <summary>
        /// Repository for handling feedback related to lessons.
        /// </summary>
        private IFeedbackRepository _databaseFeedbackrepository;


        /// <summary>
        /// Repository for managing application users, e.g. retrieval based on ClaimsPrincipal
        /// </summary>
        private IApplicationUserRepository _databaseApplicationUserRepository;

        /// <summary>
        /// Factory for creating scoped service providers, useful for background operations or resolving scoped services manually.
        /// Here used for not blocking the thread when sending state or feedback change emails.
        /// </summary>
        private readonly IServiceScopeFactory _scopeFactory;

        /// <summary>
        /// Service for marking feedbacks as unread after they have been changed
        /// </summary>
        private FeedbackService _feedbackService;

        // ------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="TraineeLessonDetailService"/> class.
        /// </summary>
        /// <param name="databaseLessonRepository">Service used to send emails.</param>
        /// <param name="databaseTraineeLessonRepository">Repository to access application user data.</param>
        /// <param name="databaseTraineeLessonLogEntryRepository"></param>
        /// <param name="databaseFeedbackRepository"></param>
        /// <param name="databaseApplicationUserRepository"></param>
        /// <param name="scopeFactory"></param>
        /// <remarks>
        /// Code Ownership: Alexander Schlemmer (schleale)
        /// </remarks>
        public TraineeLessonDetailService(ILessonRepository databaseLessonRepository,
                                            ITraineeLessonRepository databaseTraineeLessonRepository,
                                            ITraineeLessonLogEntryRepository databaseTraineeLessonLogEntryRepository,
                                            IFeedbackRepository databaseFeedbackRepository,
                                            IApplicationUserRepository databaseApplicationUserRepository,
                                            IServiceScopeFactory scopeFactory,
                                            FeedbackService feedbackService) {
            _databaseLessonRepository = databaseLessonRepository;
            _databaseTraineeLessonLogEntryRepository = databaseTraineeLessonLogEntryRepository;
            _databaseTraineeLessonRepository = databaseTraineeLessonRepository;
            _databaseFeedbackrepository = databaseFeedbackRepository;
            _databaseApplicationUserRepository = databaseApplicationUserRepository;
            _scopeFactory = scopeFactory;
            _feedbackService = feedbackService;
        }

        // --------------------------------------------------
        /// <summary>
        /// Checks whether the given user has access to open the modal of a trainee's lesson.
        /// Grants access, if the trainee themselves or any Mentor or Admin tries to accesss.
        /// </summary>
        /// <param name="user">The current authenticated user.</param>
        /// <param name="traineeLessonId">The ID of the trainee lesson to check access for.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="UserNotFoundException">Thrown if the user was not found.</exception>
        /// <exception cref="TraineeLessonNotFoundException">Thrown if the trainee lesson does not exist.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown if access is denied.</exception>
        /// <remarks>
        /// Code Ownership: Alexander Schlemmer (schleale)
        /// </remarks>
        protected virtual async Task CheckHasAccess(ClaimsPrincipal user, int traineeLessonId) {
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

        // --------------------------------------------------
        /// <summary>
        /// Builds a detailed view model for a trainee lesson, including lesson info, log entries, feedback, and allowed state transitions.
        /// </summary>
        /// <param name="traineeLessonId">The ID of the trainee lesson to build the view model for.</param>
        /// <param name="user">The user trying to build the view model.</param>
        /// <returns>A <see cref="TraineeLessonDetailViewModel"/> representing detailed info about the trainee lesson.</returns>
        /// <exception cref="TraineeLessonNotFoundException">Thrown if the requested trainee lesson could not be found.</exception>
        /// <exception cref="LessonNotFoundException">Thrown if the trainee lesson has an invalid corresponding lesson.</exception>
        /// <remarks>
        /// Code Ownership: Alexander Schlemmer (schleale), tiny edit by Simon Hinterreiter (hintsimo)
        /// </remarks>
        public async Task<TraineeLessonDetailViewModel> BuildTraineeLessonDetailViewModel(int traineeLessonId, ClaimsPrincipal user) {
            await CheckHasAccess(user, traineeLessonId);

            var tl = await _databaseTraineeLessonRepository.GetTraineeLessonByIdWithLessonAsync(traineeLessonId) ?? throw new TraineeLessonNotFoundException(traineeLessonId);
            var l = await _databaseLessonRepository.GetLessonByIdAsync(tl.LessonId) ?? throw new LessonNotFoundException(tl.LessonId);
            var tll = _databaseTraineeLessonLogEntryRepository.GetAllLogsForTraineeLesson(traineeLessonId);
            var f = await _databaseFeedbackrepository.GetAllFeedbacksForLessonWithLessonAndAuthorAndReadByUsersAsync(l);
            var feedback = await _databaseFeedbackrepository.GetFeedbackOfTraineeLessonWithLessonAndAuthorAndReadByUsersAsync(tl);

            TraineeLessonStateFactory factory = new();

            return new TraineeLessonDetailViewModel {
                TraineeLesson = tl,
                Lesson = l,
                LogEntries = tll,
                Feedbacks = f
                    .OrderByDescending(x => x.CreateTime)
                    .Take(10)
                    .ToList(),
                AllowedStateTransitions = factory.Create(tl.State)
                                                    .GetAllowedLessonStateTransitions(user)
                                                    .Select(s => s.ToString())
                                                    .ToList(),
                ExistingFeedback = feedback,
                PreviousKnowledgeOptions = Enum.GetValues(typeof(PreviousKnowledgeLevel))
                    .Cast<PreviousKnowledgeLevel>()
                    .Select(e => new SelectListItem {
                        Value = e.ToString(),
                        Text = e.GetType()
                                .GetMember(e.ToString())
                                .First()
                                .GetCustomAttribute<DisplayAttribute>()?.Name ?? e.ToString()
                    }),
                DifficultyOptions = Enum.GetValues(typeof(LessonDifficulty))
                    .Cast<LessonDifficulty>()
                    .Select(e => new SelectListItem
                    {
                        Value = e.ToString(),
                        Text = e.GetType().GetMember(e.ToString())[0]
                            .GetCustomAttribute<DisplayAttribute>()?.Name ?? e.ToString()
                    }).ToList()
            };
        }

        // --------------------------------------------------
        /// <summary>
        /// Changes the state of a trainee lesson, if the user is allowed to do so.
        /// Validates the requested state change, updates dates accordingly, logs the change, and sends notification emails asynchronously.
        /// </summary>
        /// <param name="traineeLessonUpdate">Data transfer object (DTO) containing the necessary information for changing the state of a trainee lesson.</param>
        /// <param name="user">The user trying to change the state.</param>
        /// <returns>A task representing the asynchronous save operation.</returns>
        /// <exception cref="TraineeLessonNotFoundException">Thrown when the specified trainee lesson was not found.</exception>
        /// <exception cref="ArgumentException">Thrown when rejection reason is missing for transitioning to Rejected state.</exception>
        /// <exception cref="Exception">Thrown when the target state is invalid.</exception>
        /// <remarks>
        /// Code Ownership: Alexander Schlemmer (schleale)
        /// </remarks>
        public virtual async Task SaveTraineeLessonStateChange(TraineeLessonDto traineeLessonUpdate, ClaimsPrincipal user, Feedback? feedback = null) {
            await CheckHasAccess(user, traineeLessonUpdate.TraineeLessonId);

            var oldTraineeLesson = await _databaseTraineeLessonRepository.GetTraineeLessonByIdWithLessonAsync(traineeLessonUpdate.TraineeLessonId) ?? throw new TraineeLessonNotFoundException(traineeLessonUpdate.TraineeLessonId);
            var oldState = oldTraineeLesson.State;

            // returns true if TargetStateName could be parsed into targetState
            TraineeLessonState targetState;
            if (!Enum.TryParse<TraineeLessonState>(traineeLessonUpdate.TargetStateName, out targetState))
                throw new Exception("Invalid target state in TraineeLessonDto.");

            // checks for missing rejection reason
            if (targetState == TraineeLessonState.Rejected && String.IsNullOrWhiteSpace(traineeLessonUpdate.RejectionReason))
                throw new ArgumentException("Rejection reason must be provided for transitioning to rejected.", nameof(traineeLessonUpdate));

            // transitions, if allowed
            TraineeLessonStateFactory factory = new();
            oldTraineeLesson.State = factory.Create(oldTraineeLesson.State).TransitionTo(targetState, user);

            // changes dates
            if (oldTraineeLesson.State == TraineeLessonState.Rejected) {
                // add rejection reason & remove dayFinished if lesson rejected
                oldTraineeLesson.RejectionReason = traineeLessonUpdate.RejectionReason;
                oldTraineeLesson.DayFinished = null;

            } else if (oldTraineeLesson.State == TraineeLessonState.Started) {
                // add dayStarted if lesson started
                oldTraineeLesson.DayStarted = DateOnly.FromDateTime(DateTime.Today);
                oldTraineeLesson.DayFinished = null;

            } else if (oldTraineeLesson.State == TraineeLessonState.Open) {
                // remove dayStarted if lesson un-started
                oldTraineeLesson.DayStarted = null;

            } else if (oldTraineeLesson.State == TraineeLessonState.Finished) {
                // add dayFinished if lesson finished
                oldTraineeLesson.DayFinished = DateOnly.FromDateTime(DateTime.Today);
            }

            // update database
            await _databaseTraineeLessonRepository.UpdateAsync(oldTraineeLesson);

            // sends email (different thread)
            NotifyStateChange(oldTraineeLesson, oldState, targetState, feedback);            

            // creates log
            await LogStatusChange(oldTraineeLesson, oldState, targetState, user);
        }

        // --------------------------------------------------
        /// <summary>
        /// Sends emails about state changes, in a different thread.
        /// </summary>
        /// <param name="oldTraineeLesson">The trainee lesson that changed state.</param>
        /// <param name="oldState">The old state before the change.</param>
        /// <param name="targetState">The new state after the change.</param>
        /// <param name="feedback">The optional feedback, if the target State is Rated</param>
        /// <returns>A task representing the asynchronous logging operation.</returns>
        /// <remarks>
        /// Code Ownership: Alexander Schlemmer (schleale)
        /// </remarks>
        protected virtual void NotifyStateChange(TraineeLesson oldTraineeLesson, TraineeLessonState oldState, TraineeLessonState targetState, Feedback? feedback) {
            _ = Task.Run(async () => {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var emailService = scope.ServiceProvider.GetRequiredService<EmailNotificationService>();


                await emailService.NotifyAboutStateChangeAsync(oldTraineeLesson, oldState, targetState, feedback);
            });
        }

        // --------------------------------------------------
        /// <summary>
        /// Logs the status change of a trainee lesson.
        /// </summary>
        /// <param name="traineeLesson">The trainee lesson that changed state.</param>
        /// <param name="oldState">The old state before the change.</param>
        /// <param name="newState">The new state after the change.</param>
        /// <param name="user">The user who made the change.</param>
        /// <returns>A task representing the asynchronous logging operation.</returns>
        /// <exception cref="TraineeLessonNotFoundException">Thrown if the given trainee lesson could not be found.</exception>
        /// <exception cref="LessonNotFoundException">Thrown if the corresponding lesson could not be found.</exception>
        /// <exeption cref="UserNotFoundException">Thrown if the given user could not be found.</exception>
        /// <remarks>
        /// Code Ownership: Alexander Schlemmer (schleale)
        /// </remarks>
        protected virtual async Task LogStatusChange(TraineeLesson traineeLesson, TraineeLessonState oldState, TraineeLessonState newState, ClaimsPrincipal user) {
            if (traineeLesson == null)
                throw new TraineeLessonNotFoundException();

            var lesson = await _databaseLessonRepository.GetLessonByIdAsync(traineeLesson.LessonId) ?? throw new LessonNotFoundException(traineeLesson.LessonId);

            _databaseTraineeLessonLogEntryRepository.Create(
                new TraineeLessonLogEntry {
                    TraineeLessonId = traineeLesson.TraineeLessonId,
                    LessonName = lesson.Title,
                    UserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UserNotFoundException("ClaimTypes.NameIdentifier of user not found."),
                    UserName = user.FindFirst(ClaimTypes.Name)?.Value ?? throw new UserNotFoundException("ClaimTypes.Name of user not found"),
                    OldState = oldState.ToString(),
                    NewState = newState.ToString(),
                    Timestamp = DateTime.UtcNow
                });
        }

        // --------------------------------------------------
        /// <summary>
        /// Updates feedback for a trainee lesson and sends an email about the change,
        /// otherwise calls <see cref="SaveTraineeLessonStateChange"/> and saves the feedback if it is newly created.
        /// This method checks if the user has access to the trainee lesson before proceeding.
        /// </summary>
        /// <param name="feedbackDto">The feedback data transfer object containing feedback details.</param>
        /// <param name="user">The current user making the request, used for authorization.</param>
        /// <exception cref="ArgumentNullException">Thrown if required fields in <paramref name="feedbackDto"/> are null when creating new feedback.</exception>
        /// <exception cref="TraineeLessonNotFoundException">Thrown if the referenced trainee lesson does not exist.</exception>
        /// <exception cref="UserNotFoundException">Thrown if the trainee could not be found, or user claims are missing required data.</exception>
        /// <exception cref="LessonNotFoundException">Thrown if the lesson related to the trainee lesson does not exist.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown if the trainee lesson state is not accepted when creating new feedback.</exception>
        /// <remarks>
        /// Code Ownership: Alexander Schlemmer (schleale)
        /// </remarks>
        public async Task SaveFeedback(FeedbackDto feedbackDto, ClaimsPrincipal user) {
            // it is basically a state change, hence checking this beforehand
            await CheckHasAccess(user, feedbackDto.TraineeLessonId); 

            if (feedbackDto == null)
                throw new Exception("FeedbackDto is null");

            var correspondingTraineeLesson = await _databaseTraineeLessonRepository.GetTraineeLessonByIdWithLessonAsync(feedbackDto.TraineeLessonId) ?? throw new TraineeLessonNotFoundException(feedbackDto.TraineeLessonId);
            var traineeId = correspondingTraineeLesson.TraineeId;
            var trainee = await _databaseApplicationUserRepository.FindByIdAsync(traineeId) ?? throw new UserNotFoundException();
            var existingFeedback = await _databaseFeedbackrepository.GetFeedbackOfTraineeLessonWithLessonAndAuthorAndReadByUsersAsync(correspondingTraineeLesson);
            
            if (existingFeedback != null) {
                // -> feedback exists

                existingFeedback.Difficulty = feedbackDto.Difficulty;
                existingFeedback.PreviousKnowledge = feedbackDto.PreviousKnowledge;
                existingFeedback.HoursOfEffort = feedbackDto.HoursOfEffort;
                existingFeedback.Comment = feedbackDto.Comment;

                await _databaseFeedbackrepository.UpdateAsync(existingFeedback);

                // mark changed feedback as unread
                await _feedbackService.MarkFeedbackAsUnreadForEveryoneAsync(existingFeedback.FeedbackId);
                
                var applicationUser = await _databaseApplicationUserRepository.GetUserAsync(user) ?? throw new UserNotFoundException();

                // sends email (different thread)
                _ = Task.Run(async () => {
                    using var scope = _scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var emailService = scope.ServiceProvider.GetRequiredService<EmailNotificationService>();


                    await emailService.NotifyAboutFeedbackChangeAsync(existingFeedback, applicationUser);
                });
            } else {
                // -> feedback doesn't exist
                
                if (correspondingTraineeLesson.State != TraineeLessonState.Accepted &&
                    correspondingTraineeLesson.State != TraineeLessonState.Rated)
                    throw new UnauthorizedAccessException("You can write a feedback once your TraineeLesson has been accepted.");

                var feedback = new Feedback {
                    Difficulty = feedbackDto.Difficulty,
                    PreviousKnowledge = feedbackDto.PreviousKnowledge,
                    HoursOfEffort = feedbackDto.HoursOfEffort,
                    Comment = feedbackDto.Comment,

                    // relations
                    LessonId = correspondingTraineeLesson.LessonId,
                    Lesson = await _databaseLessonRepository.GetLessonByIdAsync(correspondingTraineeLesson.LessonId) ?? throw new LessonNotFoundException(correspondingTraineeLesson.LessonId),
                    AuthorId = traineeId,
                    Author = trainee,
                    ReadByUsers = new List<ApplicationUser>()
                };

                // create feedback
                await _databaseFeedbackrepository.CreateAsync(feedback);

                // update state to rated, also sends email and creates log
                await SaveTraineeLessonStateChange(new TraineeLessonDto {
                    TraineeLessonId = feedbackDto.TraineeLessonId,
                    TargetStateName = TraineeLessonState.Rated.ToString()
                }, user, feedback);
            }
        }

        // --------------------------------------------------
        /// <summary>
        /// Deletes a feedback entry identified by <paramref name="feedbackId"/>.
        /// Only users who are in the "Mentor" or "Admin" role are authorized to delete feedback.
        /// </summary>
        /// <param name="user">The current user attempting the deletion, used for authorization checks.</param>
        /// <param name="feedbackId">The ID of the feedback to delete.</param>
        /// <exception cref="UnauthorizedAccessException">Thrown if a trainee tries to delete the feedback of a different user.</exception>
        /// <exception cref="FeedbackNotFoundException">Thrown if no feedback with the specified ID exists.</exception>
        /// <exception cref="UserNotFoundException">Thrown if the user trying to delete the feedback couldn't be found.</exception>
        /// <remarks>
        /// Code Ownership: Alexander Schlemmer (schleale)
        /// </remarks>
        public async Task DeleteFeedback(ClaimsPrincipal user, int feedbackId) {
            // Checks
            var feedbackToDelete = await _databaseFeedbackrepository.GetFeedbackByIDWithLessonAndAuthorAndReadByUsersAsync(feedbackId)
                ?? throw new FeedbackNotFoundException(feedbackId);
            var accessingUser = await _databaseApplicationUserRepository.GetUserAsync(user)
                ?? throw new UserNotFoundException();

            if (!(user.IsInRole("Mentor") || user.IsInRole("Admin")))
                if (accessingUser != feedbackToDelete.Author)
                    throw new UnauthorizedAccessException("You can only delete your own feedbacks");

            // Delete feedback
            await _databaseFeedbackrepository.DeleteAsync(feedbackId);

            // Send email (different thread)
            _ = Task.Run(async () => {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var emailService = scope.ServiceProvider.GetRequiredService<EmailNotificationService>();

                await emailService.NotifyAboutFeedbackChangeAsync(feedbackToDelete, accessingUser, true);
            });
        }
    }
}