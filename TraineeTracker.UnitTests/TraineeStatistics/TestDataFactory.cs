using TraineeTracker.Models.Domain;

namespace TraineeTracker.UnitTests.TraineeStatistics.TestDataFactory {

    /// <summary>
    /// Provides reusable test data for unit tests involving trainees and their lessons.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Nikita Stefan (stefanni)
    /// </remarks>
    public static class TestDataFactory {

        // ------------------------------------------------------
        /// <summary>
        /// The default trainee ID used in test data.
        /// </summary>
        public static string DefaultTraineeId => "trainee-123";

        // ------------------------------------------------------
        /// <summary>
        /// Creates a test <see cref="ApplicationUser"/> representing a trainee with a default ID and email.
        /// </summary>
        /// <returns>A configured <see cref="ApplicationUser"/> instance.</returns>
        public static ApplicationUser CreateTestTrainee() {
            return new ApplicationUser {
                Id = DefaultTraineeId,
                Email = "test@example.com",
                EmailNotificationSetting = new EmailNotificationSetting()
            };
        }

        // ------------------------------------------------------
        /// <summary>
        /// Creates a list of <see cref="TraineeLesson"/> instances with varying states and estimated efforts
        /// for use in unit tests.
        /// </summary>
        /// <param name="trainee">The trainee for whom the lessons are created.</param>
        /// <returns>A list of <see cref="TraineeLesson"/> objects.</returns>
        public static List<TraineeLesson> CreateTestTraineeLessons(ApplicationUser trainee) {
            return new List<TraineeLesson> {
                new TraineeLesson {
                    TraineeId = trainee.Id,
                    Trainee = trainee,
                    LessonId = 1,
                    Lesson = new Lesson {
                        LessonId = 1,
                        EstimatedEffort = 5,
                        MakandraId = "1",
                        TeachingPlanId = 1,
                        SortingIndex = 1,
                        Title = "Lesson 1",
                        LinkUrl = "http://example.com/1"
                    },
                    State = TraineeLessonState.Finished
                },
                new TraineeLesson {
                    TraineeId = trainee.Id,
                    Trainee = trainee,
                    LessonId = 2,
                    Lesson = new Lesson {
                        LessonId = 2,
                        EstimatedEffort = 4,
                        MakandraId = "2",
                        TeachingPlanId = 1,
                        SortingIndex = 2,
                        Title = "Lesson 2",
                        LinkUrl = "http://example.com/2"
                    },
                    State = TraineeLessonState.Accepted
                },
                new TraineeLesson {
                    TraineeId = trainee.Id,
                    Trainee = trainee,
                    LessonId = 3,
                    Lesson = new Lesson {
                        LessonId = 3,
                        EstimatedEffort = 3,
                        MakandraId = "3",
                        TeachingPlanId = 1,
                        SortingIndex = 3,
                        Title = "Lesson 3",
                        LinkUrl = "http://example.com/3"
                    },
                    State = TraineeLessonState.Rejected
                },
                new TraineeLesson {
                    TraineeId = trainee.Id,
                    Trainee = trainee,
                    LessonId = 4,
                    Lesson = new Lesson {
                        LessonId = 4,
                        EstimatedEffort = 2,
                        MakandraId = "4",
                        TeachingPlanId = 1,
                        SortingIndex = 4,
                        Title = "Lesson 4",
                        LinkUrl = "http://example.com/4"
                    },
                    State = TraineeLessonState.Rated
                },
                new TraineeLesson {
                    TraineeId = trainee.Id,
                    Trainee = trainee,
                    LessonId = 5,
                    Lesson = new Lesson {
                        LessonId = 5,
                        EstimatedEffort = 10,
                        MakandraId = "5",
                        TeachingPlanId = 1,
                        SortingIndex = 5,
                        Title = "Lesson 5",
                        LinkUrl = "http://example.com/5"
                    },
                    State = TraineeLessonState.Open
                },
                new TraineeLesson {
                    TraineeId = trainee.Id,
                    Trainee = trainee,
                    LessonId = 6,
                    Lesson = new Lesson {
                        LessonId = 6,
                        EstimatedEffort = 99,
                        MakandraId = "6",
                        TeachingPlanId = 1,
                        SortingIndex = 6,
                        Title = "Lesson 6",
                        LinkUrl = "http://example.com/6"
                    },
                    State = TraineeLessonState.Skipped
                }
            };
        }
        
        // ------------------------------------------------------
    }
}