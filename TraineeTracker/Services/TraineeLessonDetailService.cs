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

        private ILessonRepository _databaseLessonRepository;
        private ITraineeLessonRepository _databaseTraineeLessonRepository;
        private ITraineeLessonLogEntryRepository _databaseTraineeLessonLogEntryRepository;
        private IFeedbackRepository _databaseFeedbackrepository;
        // the following are not included in the viewmodel, because i dont't think we need them there?
        private IApplicationUserRepository _databaseApplicationUserRepository;

        private readonly IServiceScopeFactory _scopeFactory;

        public TraineeLessonDetailService(ILessonRepository databaseLessonRepository,
                                            ITraineeLessonRepository databaseTraineeLessonRepository,
                                            ITraineeLessonLogEntryRepository databaseTraineeLessonLogEntryRepository,
                                            IFeedbackRepository databaseFeedbackRepository,
                                            IApplicationUserRepository databaseApplicationUserRepository,
                                            IServiceScopeFactory scopeFactory) {
            _databaseLessonRepository = databaseLessonRepository;
            _databaseTraineeLessonLogEntryRepository = databaseTraineeLessonLogEntryRepository;
            _databaseTraineeLessonRepository = databaseTraineeLessonRepository;
            _databaseFeedbackrepository = databaseFeedbackRepository;
            _databaseApplicationUserRepository = databaseApplicationUserRepository;
            _scopeFactory = scopeFactory;
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

        public async Task SaveTraineeLessonStateChange(TraineeLessonDto traineeLessonUpdate, ClaimsPrincipal user, Feedback? feedback = null) {
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
            _ = Task.Run(async () => {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var emailService = scope.ServiceProvider.GetRequiredService<EmailNotificationService>();


                await emailService.NotifyAboutStateChangeAsync(oldTraineeLesson, oldState, targetState, feedback);
            });
            
            // creates log
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
            var oldState = correspondingTraineeLesson.State;
            var traineeId = correspondingTraineeLesson.TraineeId;
            var trainee = await _databaseApplicationUserRepository.FindByIdAsync(traineeId) ?? throw new UserNotFoundException();
            var existingFeedback = await _databaseFeedbackrepository.GetFeedbackOfTraineeLessonWithLessonAndAuthorAndReadByUsersAsync(correspondingTraineeLesson);
            
            if (existingFeedback != null) {
                // -> feedback exists

                existingFeedback.Difficulty = feedbackDto.Difficulty ?? existingFeedback.Difficulty;
                existingFeedback.PreviousKnowledge = feedbackDto.PreviousKnowledge ?? existingFeedback.PreviousKnowledge;
                existingFeedback.HoursOfEffort = feedbackDto.HoursOfEffort ?? existingFeedback.HoursOfEffort;

                await _databaseFeedbackrepository.UpdateAsync(existingFeedback);

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
                
                if (correspondingTraineeLesson.State != TraineeLessonState.Accepted)
                    throw new UnauthorizedAccessException("You can write a feedback once your TraineeLesson has been accepted.");

                var authorId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("ClaimTypes.NameIdentifier of user not found.");

                var feedback = new Feedback {
                    Difficulty = feedbackDto.Difficulty ?? throw new ArgumentNullException(nameof(feedbackDto), "Difficulty cannot be null."),
                    PreviousKnowledge = feedbackDto.PreviousKnowledge ?? throw new ArgumentNullException(nameof(feedbackDto), "PreviousKnowledge cannot be null."),
                    HoursOfEffort = feedbackDto.HoursOfEffort ?? throw new ArgumentNullException(nameof(feedbackDto), "HoursOfEffort cannot be null."),
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

        public async Task DeleteFeedback(ClaimsPrincipal user, int feedbackId) {
            if (user.IsInRole("Trainee"))
                throw new UnauthorizedAccessException("Trainees cannot delete feedbacks.");

            if (!await _databaseFeedbackrepository.ExistsAsync(feedbackId))
                throw new FeedbackNotFoundException(feedbackId);

            var deletedFeedback = await _databaseFeedbackrepository.GetFeedbackByIDWithLessonAndAuthorAndReadByUsersAsync(feedbackId) ?? throw new FeedbackNotFoundException();

            await _databaseFeedbackrepository.DeleteAsync(feedbackId);

            // sends email (different thread)
            _ = Task.Run(async () => {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var emailService = scope.ServiceProvider.GetRequiredService<EmailNotificationService>();

                await emailService.NotifyAboutFeedbackChangeAsync(deletedFeedback, await _databaseApplicationUserRepository.GetUserAsync(user) ?? throw new UserNotFoundException(), true);
            });
        }
    }
}