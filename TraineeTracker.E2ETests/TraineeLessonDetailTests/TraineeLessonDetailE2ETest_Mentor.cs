using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Tutorial_project.E2ETests;

namespace TraineeTracker.E2ETests.TraineeLessonDetailTests {

    /// <summary>
    /// A class for End-2-End tests, testing state changes, executed by a mentor.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexander Schlemmer (schleale)
    /// </remarks>
    public class TraineeLessonDetailE2ETest_Mentor : IClassFixture<BrowserFixture> {

        private readonly IWebDriver _driver;

        // ------------------------------------------------------
        public TraineeLessonDetailE2ETest_Mentor(BrowserFixture fixture) {
            _driver = fixture.Driver;
        }

        // ------------------------------------------------------
        [Fact]
        public void SaveTraineeLessonStateChange_FullWorkflow() {
            NavigateToDashboard_Mentor();

            // 1. Find the first row with "Open" status
            var openLessonRow = _driver.FindElements(By.CssSelector("tr.sopro-card"))
                .FirstOrDefault(row => {
                    try {
                        var span = row.FindElement(By.CssSelector("span.sopro-tag.open"));
                        return span != null && span.Text.Trim() == "Open";
                    } catch (NoSuchElementException) {
                        return false;
                    }
                });

            // Abort if there are no open lessons
            if (openLessonRow == null) {
                Assert.Fail("[TEST ABORT] No open lesson found.");
            }

            // Get trainee lesson id
            var traineeLessonId = openLessonRow.GetAttribute("data-lesson-id");
            Console.WriteLine($"[DEBUG] Found Open lesson with ID: {traineeLessonId}");

            // Scroll and click (if needed)
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", openLessonRow);
            Thread.Sleep(300);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", openLessonRow);

            // Scroll to the row
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", openLessonRow);
            Thread.Sleep(300);

            // Click via JavaScript
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", openLessonRow);

            // Wait for modal (adjust selector as needed)
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
            var modal = wait.Until(drv => drv.FindElement(By.CssSelector(".your-modal-selector")));

            // Do something with the modal
            Console.WriteLine("[DEBUG] Modal is visible. Interacting...");
/*
            Assert.Equal(traineeLesson.State.ToString(), "Open");

            SaveTraineeLessonStateChange_OpenToSkipped();
            Assert.Equal(traineeLesson.State.ToString(), "Skipped");

            SaveTraineeLessonStateChange_SkippedToOpen();
            Assert.Equal(traineeLesson.State.ToString(), "Open");

            SaveTraineeLessonStateChange_OpenToStarted();
            Assert.Equal(traineeLesson.State.ToString(), "Started");

            SaveTraineeLessonStateChange_StartedToOpen();
            Assert.Equal(traineeLesson.State.ToString(), "Open");

            SaveTraineeLessonStateChange_OpenToStarted();
            SaveTraineeLessonStateChange_StartedToFinished();
            Assert.Equal(traineeLesson.State.ToString(), "Finished");

            SaveTraineeLessonStateChange_FinishedToStarted();
            Assert.Equal(traineeLesson.State.ToString(), "Started");

            SaveTraineeLessonStateChange_StartedToFinished();
            SaveTraineeLessonStateChange_FinishedToRejected();
            Assert.Equal(traineeLesson.State.ToString(), "Rejected");

            SaveTraineeLessonStateChange_RejectedToFinished();
            Assert.Equal(traineeLesson.State.ToString(), "Finished");

            SaveTraineeLessonStateChange_FinishedToAccepted();
            Assert.Equal(traineeLesson.State.ToString(), "Accepted");

            SaveTraineeLessonStateChange_AcceptedToFinished();
            Assert.Equal(traineeLesson.State.ToString(), "Finished");

            SaveTraineeLessonStateChange_FinishedToAccepted();
            SaveTraineeLessonStateChange_AcceptedToRated();
            Assert.Equal(traineeLesson.State.ToString(), "Rated");*/
        }

        // ------------------------------------------------------
        // ------------------------------------------------------
        // ------------------------------------------------------
        private void SaveTraineeLessonStateChange_OpenToSkipped() {
        }

        private void SaveTraineeLessonStateChange_SkippedToOpen() {
        }

        // ------------------------------------------------------
        private void SaveTraineeLessonStateChange_OpenToStarted() {
        }

        private void SaveTraineeLessonStateChange_StartedToOpen() {
        }

        // ------------------------------------------------------
        private void SaveTraineeLessonStateChange_StartedToFinished() {
        }

        private void SaveTraineeLessonStateChange_FinishedToStarted() {
        }

        // ------------------------------------------------------
        private void SaveTraineeLessonStateChange_FinishedToRejected() {
        }

        private void SaveTraineeLessonStateChange_RejectedToFinished() {
        }

        // ------------------------------------------------------
        private void SaveTraineeLessonStateChange_FinishedToAccepted() {
        }

        private void SaveTraineeLessonStateChange_AcceptedToFinished() {
        }

        // ------------------------------------------------------
        private void SaveTraineeLessonStateChange_AcceptedToRated() {
        }

        // ------------------------------------------------------
        // ------------------------------------------------------
        // ------------------------------------------------------
        
        /// <summary>
        /// A method to enable reuse of login, navigate to dashboard and open a trainee lesson.
        /// </summary>
        /// <param name="webDriver"></param>
        /// <remarks>
        /// Code Ownership: Simon Hinterreiter (hintsimo)
        private void NavigateToDashboard_Mentor() {
            // 1. Navigate to login page
            _driver.Navigate().GoToUrl("http://localhost:5079/Identity/Account/Login?ReturnUrl=%2FDashboard");

            Thread.Sleep(1000);

            // 2. Login as Mentor
            _driver.FindElement(By.Id("Input_Email")).SendKeys("simon.hinterreiter@uni-a.de");
            _driver.FindElement(By.Id("Input_Password")).SendKeys("Sopro.2025");
            _driver.FindElement(By.Id("login-submit")).Click();

            Thread.Sleep(2000);

            // 3. Navigate to Dashboard
            _driver.Navigate().GoToUrl("http://localhost:5079/Dashboard");

            if (_driver.Url != "http://localhost:5079/Dashboard")
                throw new Exception("Navigation to Dashboard failed.");
        }
    }
}