using TraineeTracker.Models.Domain;
using TraineeTracker.Services;
using TraineeTracker.Models.Dtos;

namespace TraineeTracker.UnitTests.TraineeLessonTests {

        /// <summary>
        /// A class for testing functionality of the TraineeLessonDetailService.cs
        /// </summary>
        /// <remarks>
        /// Code Ownership: Alexander Schlemmer (schleale)
        /// </remarks>
        public class TraineeLessonDetailServiceTests {
                // ----------------------------------------------------
                [Fact]
                public async Task SaveTraineeLessonStateChangeTest_FinishedToAccepted_ByMentor() {
                        var service = new TraineeLessonDetailService();

                        // ---
                        var trainee = TestDataFactoryTraineeLessons.CreateTestApplicationUser(1);
                        var user;

                        var lesson = TestDataFactoryTraineeLessons.CreateTestLesson(1);
                        var traineeLesson = TestDataFactoryTraineeLessons.CreateTestTraineeLesson(trainee, lesson);

                        var traineeLessonDto = (new TraineeLessonDto {
                                TraineeLessonId = traineeLesson.TraineeLessonId,
                                TraineeId = trainee.Id,
                                LessonId = lesson.LessonId,
                                TargetStateName = TraineeLessonState.Accepted.ToString(),
                                RejectionReason = null
                        }

                        // ---
                        await service.SaveTraineeLessonStateChange(traineeLessonDto, user);
                }

                // ----------------------------------------------------
                [Fact]
                public async Task SaveFeedbackTest_FeedbackDoesNotExist() {
                        var service = new TraineeLessonDetailService();

                        // ---
                        var trainee = TestDataFactoryTraineeLessons.CreateTestApplicationUser(1);
                        var user;

                        var lesson = TestDataFactoryTraineeLessons.CreateTestLesson(1);
                        var traineeLesson = TestDataFactoryTraineeLessons.CreateTestTraineeLesson(trainee, lesson);

                        var feedbackDto = new FeedbackDto {
                                Difficulty = LessonDifficulty.VeryEasy,
                                PreviousKnowledge = PreviousKnowledgeLevel.None,
                                HoursOfEffort = (float) 1.1,
                                TraineeLessonId = traineeLesson.TraineeLessonId
                        };

                        await service.SaveFeedback(feedbackDto, user);
                }
        }
}
