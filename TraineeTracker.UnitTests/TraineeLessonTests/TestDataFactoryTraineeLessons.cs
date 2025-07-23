using System.Security.Claims;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.UnitTests.TraineeLessonTests {
    
    /// <summary>
    /// A class to create test data required for testing.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexander Schlemmer (schleale)
    /// </remarks>
    public static class TestDataFactoryTraineeLessons {
        // ----------------------------------------------------
        public static ApplicationUser CreateTestApplicationUser() {
            int id = 1;

            return new ApplicationUser {
                Id = "test" + id,
                Email = "test" + id + "@example.com",
                EmailNotificationSetting = new EmailNotificationSetting()
            };
        }

        // ----------------------------------------------------
        public static Lesson CreateTestLesson(int id) {
            return new Lesson {
                LessonId = id,
                MakandraId = id.ToString(),

                TeachingPlanId = id,
                SortingIndex = id,

                Title = "Test " + id,
                EstimatedEffort = id + (id / 10),
                LinkUrl = "https://www.google.com/"
            };
        }

        // ----------------------------------------------------
        public static TraineeLesson CreateTestTraineeLesson(ApplicationUser trainee, Lesson lesson) {
            return new TraineeLesson {
                TraineeId = trainee.Id,
                Trainee = trainee,

                LessonId = lesson.LessonId,
                Lesson = lesson,

                State = TraineeLessonState.Open
            };
        }

        // ----------------------------------------------------
        public static ClaimsPrincipal CreateTestClaimsPrincipal_Mentor() {
            var claims = new List<Claim> {
                    new Claim(ClaimTypes.Role, "Mentor"),
                    new Claim(ClaimTypes.NameIdentifier, "mentor-id")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");

            return new ClaimsPrincipal(identity);
        }

        // ----------------------------------------------------
    }
}
