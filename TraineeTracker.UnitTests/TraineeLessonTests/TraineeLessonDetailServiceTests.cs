using TraineeTracker.Models.Domain;
using TraineeTracker.Services;
using TraineeTracker.Models.Dtos;
using System.Security.Claims;
using TraineeTracker.Data.TraineeLessons;
using TraineeTracker.Data.Lessons;
using TraineeTracker.Data.TraineeLessonLog;
using TraineeTracker.Data.Feedbacks;
using Microsoft.Extensions.DependencyInjection;
using TraineeTracker.Data.ApplicationUsers;

namespace TraineeTracker.UnitTests.TraineeLessonTests {

        /// <summary>
        /// A unit test class for testing functionality of the TraineeLessonDetailService.cs
        /// These tests test the correct state change from 'Finished' to 'Accepted', executed by a 'Mentor';
        /// as well as saving a feedback for the first time (i.e. transition from 'Accepted' to 'Rated')
        /// </summary>
        /// <remarks>
        /// Code Ownership: Alexander Schlemmer (schleale)
        /// </remarks>
        public class TraineeLessonDetailServiceTests {

                // -------------------------------------------------------------------------------------------------------------------
                [Fact]
                public async Task SaveTraineeLessonStateChangeTest_FinishedToAccepted_ByMentor() {
                        // Arrange
                        var trainee = TestDataFactoryTraineeLessons.CreateTestApplicationUser();
                        var lesson = TestDataFactoryTraineeLessons.CreateTestLesson(1);
                        var traineeLesson = TestDataFactoryTraineeLessons.CreateTestTraineeLesson(trainee, lesson);
                        traineeLesson.State = TraineeLessonState.Finished;
                        var user = TestDataFactoryTraineeLessons.CreateTestClaimsPrincipal_Mentor();

                        var service = new TestableTraineeLessonDetailService(
                                databaseLessonRepository: null!,
                                databaseTraineeLessonRepository: new FakeTraineeLessonRepository(traineeLesson),
                                databaseTraineeLessonLogEntryRepository: null!,
                                databaseFeedbackRepository: null!,
                                databaseApplicationUserRepository: null!,
                                scopeFactory: null!,
                                feedbackService: null!
                        );

                        // ----------
                        var traineeLessonDto = new TraineeLessonDto {
                                TraineeLessonId = traineeLesson.TraineeLessonId,
                                TraineeId = trainee.Id,
                                LessonId = lesson.LessonId,
                                TargetStateName = TraineeLessonState.Accepted.ToString(),
                                RejectionReason = null
                        };

                        // Act
                        await service.SaveTraineeLessonStateChange(traineeLessonDto, user);

                        // Assert
                        Assert.Equal(TraineeLessonState.Accepted, traineeLesson.State);

                        Console.WriteLine("[DEBUG] Trainee lesson test 1 succeeded");
                }

                // -------------------------------------------------------------------------------------------------------------------
                [Fact]
                public async Task SaveFeedbackTest_FeedbackDoesNotExist() {
                        // Arrange
                        var trainee = TestDataFactoryTraineeLessons.CreateTestApplicationUser();
                        var user = TestDataFactoryTraineeLessons.CreateTestClaimsPrincipal_Mentor();

                        var lesson = TestDataFactoryTraineeLessons.CreateTestLesson(1);
                        var traineeLesson = TestDataFactoryTraineeLessons.CreateTestTraineeLesson(trainee, lesson);
                        traineeLesson.State = TraineeLessonState.Accepted;

                        var fakeFeedbackRepo = new FakeFeedbackRepository();

                        var service = new TestableTraineeLessonDetailService2(
                                databaseLessonRepository: new FakeLessonRepository(lesson),
                                databaseTraineeLessonRepository: new FakeTraineeLessonRepository(traineeLesson),
                                databaseTraineeLessonLogEntryRepository: null!,
                                databaseFeedbackRepository: fakeFeedbackRepo,
                                databaseApplicationUserRepository: new FakeApplicationUserRepository(trainee),
                                scopeFactory: null!,
                                feedbackService: new FakeFeedbackService(null!, null!, null!, null!)
                        );

                        // ----------
                        var feedbackDto = new FeedbackDto {
                                Difficulty = LessonDifficulty.VeryEasy,
                                PreviousKnowledge = PreviousKnowledgeLevel.None,
                                HoursOfEffort = (float)1.1,
                                TraineeLessonId = traineeLesson.TraineeLessonId,
                                Comment = "The cake is a lie."
                        };

                        // Act
                        await service.SaveFeedback(feedbackDto, user);

                        // Assert
                        var createdFeedback = fakeFeedbackRepo.CreatedFeedback;
                        Assert.NotNull(createdFeedback);
                        Assert.Equal(feedbackDto.Difficulty, createdFeedback!.Difficulty);
                        Assert.Equal(feedbackDto.PreviousKnowledge, createdFeedback.PreviousKnowledge);
                        Assert.Equal(feedbackDto.HoursOfEffort, createdFeedback.HoursOfEffort);
                        Assert.Equal(feedbackDto.Comment, createdFeedback.Comment);

                        Console.WriteLine("[DEBUG] Trainee lesson test 2 succeeded");
                }

                // -------------------------------------------------------------------------------------------------------------------
                public class TestableTraineeLessonDetailService : TraineeLessonDetailService {
                        public TestableTraineeLessonDetailService(
                                ILessonRepository databaseLessonRepository,
                                ITraineeLessonRepository databaseTraineeLessonRepository,
                                ITraineeLessonLogEntryRepository databaseTraineeLessonLogEntryRepository,
                                IFeedbackRepository databaseFeedbackRepository,
                                IApplicationUserRepository databaseApplicationUserRepository,
                                IServiceScopeFactory scopeFactory,
                                FeedbackService feedbackService
                        ) : base(
                                databaseLessonRepository,
                                databaseTraineeLessonRepository,
                                databaseTraineeLessonLogEntryRepository,
                                databaseFeedbackRepository,
                                databaseApplicationUserRepository,
                                scopeFactory,
                                feedbackService) { }

                        protected override Task CheckHasAccess(ClaimsPrincipal user, int traineeLessonId) {
                                // Ignore access check in unit test
                                return Task.CompletedTask;
                        }

                        protected override Task LogStatusChange(TraineeLesson traineeLesson, TraineeLessonState oldState, TraineeLessonState newState, ClaimsPrincipal user) {
                                // Ignore logging
                                return Task.CompletedTask;
                        }
                        protected override void NotifyStateChange(TraineeLesson oldTraineeLesson, TraineeLessonState oldState, TraineeLessonState targetState, Feedback? feedback) {
                                // Do not send emails
                        }
                }

                // ------------------------------------------------

                public class TestableTraineeLessonDetailService2 : TraineeLessonDetailService {
                        public TestableTraineeLessonDetailService2(
                                ILessonRepository databaseLessonRepository,
                                ITraineeLessonRepository databaseTraineeLessonRepository,
                                ITraineeLessonLogEntryRepository databaseTraineeLessonLogEntryRepository,
                                IFeedbackRepository databaseFeedbackRepository,
                                IApplicationUserRepository databaseApplicationUserRepository,
                                IServiceScopeFactory scopeFactory,
                                FeedbackService feedbackService
                        ) : base(
                                databaseLessonRepository,
                                databaseTraineeLessonRepository,
                                databaseTraineeLessonLogEntryRepository,
                                databaseFeedbackRepository,
                                databaseApplicationUserRepository,
                                scopeFactory,
                                feedbackService) { }

                        public override Task SaveTraineeLessonStateChange(TraineeLessonDto traineeLessonUpdate, ClaimsPrincipal user, Feedback? feedback = null) {
                                // Ignore state change logic ONLY FOR SAVING FEEDBACKS
                                return Task.CompletedTask;
                        }
                        protected override Task CheckHasAccess(ClaimsPrincipal user, int traineeLessonId) {
                                // Ignore access check in unit test
                                return Task.CompletedTask;
                        }
                        protected override Task LogStatusChange(TraineeLesson traineeLesson, TraineeLessonState oldState, TraineeLessonState newState, ClaimsPrincipal user) {
                                // Ignore logging
                                return Task.CompletedTask;
                        }
                        protected override void NotifyStateChange(TraineeLesson oldTraineeLesson, TraineeLessonState oldState, TraineeLessonState targetState, Feedback? feedback) {
                                // Do not send emails
                        }
                }
        }
}
