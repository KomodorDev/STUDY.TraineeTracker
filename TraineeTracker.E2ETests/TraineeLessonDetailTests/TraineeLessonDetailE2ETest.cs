using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Tutorial_project.E2ETests;

namespace TraineeTracker.E2ETests.TraineeLessonDetailTests {

    /// <summary>
    /// A class for two End-2-End tests - testing all the allowed state transitions for both trainees and mentors
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexander Schlemmer (schleale)
    /// </remarks>
    public class TraineeLessonDetailE2ETest : IClassFixture<BrowserFixture> {

        private readonly IWebDriver _driver;

        // ------------------------------------------------------
        public TraineeLessonDetailE2ETest(BrowserFixture fixture) {
            _driver = fixture.Driver;
        }

        // ------------------------------------------------------
        [Fact]
        public void SaveTraineeLessonStateChange_FullWorkflow_Mentor() {
            OpenTraineeLesson(isMentor: true, state: "Open");
            if (!CheckIfIsCurrentState("Open"))
                throw new Exception();

            // Open -> Skipped
            ClickOnState("Skipped");
            Assert.True(CheckIfIsCurrentState("Skipped"));

            // Open <- Skipped
            ClickOnState("Open");
            Assert.True(CheckIfIsCurrentState("Open"));

            // Open -> Started
            ClickOnState("Started");
            Assert.True(CheckIfIsCurrentState("Started"));

            // Open <- Started
            ClickOnState("Open");
            Assert.True(CheckIfIsCurrentState("Open"));

            // (Open ->) Started -> Finished
            ClickOnState("Started");
            ClickOnState("Finished");
            Assert.True(CheckIfIsCurrentState("Finished"));

            // Started <- Finished
            ClickOnState("Started");
            Assert.True(CheckIfIsCurrentState("Started"));

            // (Started ->) Finished -> Rejected
            ClickOnState("Finished");
            ClickOnState("Rejected");
            Assert.True(CheckIfIsCurrentState("Rejected"));

            // Finished <- Rejected
            ClickOnState("Finished");
            Assert.True(CheckIfIsCurrentState("Finished"));

            // Finished -> Accepted
            ClickOnState("Accepted");
            Assert.True(CheckIfIsCurrentState("Accepted"));

            // Finished <- Accepted
            ClickOnState("Finished");
            Assert.True(CheckIfIsCurrentState("Finished"));

            // (Finished ->) Accepted -> Rated
            ClickOnState("Accepted");
            ClickOnState("Rated");
            Assert.True(CheckIfIsCurrentState("Rated"));
        }

        // ------------------------------------------------------
        [Fact]
        public void SaveTraineeLessonStateChange_FullWorkflow_Trainee() {
            OpenTraineeLesson(isMentor: false, state: "Open");
            if (!CheckIfIsCurrentState("Open"))
                throw new Exception();
        
            // Test Open <-> Started <-> Finished
            ClickOnState("Started");
            Assert.True(CheckIfIsCurrentState("Started"));

            ClickOnState("Open");
            Assert.True(CheckIfIsCurrentState("Open"));

            ClickOnState("Started");
            ClickOnState("Finished");
            Assert.True(CheckIfIsCurrentState("Finished"));

            ClickOnState("Started");
            Assert.True(CheckIfIsCurrentState("Started"));

            ClickOnState("Finished");

            // Intermission: Mentor accepts lesson
            OpenTraineeLesson(isMentor: true, state: "Finished");
            if (!CheckIfIsCurrentState("Finished"))
                throw new Exception();

            ClickOnState("Accepted");

            // Test Accepted -> Rated
            OpenTraineeLesson(isMentor: false, state: "Accepted");
            if (!CheckIfIsCurrentState("Accepted"))
                throw new Exception();

            ClickOnState("Rated");
            Assert.True(CheckIfIsCurrentState("Rated"));
        }

        // ------------------------------------------------------
        // ------------------------------------------------------
        // ------------------------------------------------------
        /// <summary>
        /// A method to enable reuse of login, navigate to dashboard and open the first trainee lesson that is in the given state.
        /// </summary>
        /// <param name="isMentor">Defines whether the user is a mentor or trainee.</param>
        /// <param name="state">The state, that the trainee lesson is supposed to have.</param>
        /// <remarks>
        /// Code Ownership: Simon Hinterreiter (hintsimo) & Alexander Schlemmer (schleale)
        /// </remarks>
        private void OpenTraineeLesson(bool isMentor = true, string state = "Open") {
            // 1. Navigate to login page
            _driver.Navigate().GoToUrl("http://localhost:5079/Identity/Account/Login?ReturnUrl=%2FDashboard");

            Thread.Sleep(1000);

            // 2. Login
            if (isMentor) {
                _driver.FindElement(By.Id("Input_Email")).SendKeys("simon.hinterreiter@uni-a.de");
            } else {
                _driver.FindElement(By.Id("Input_Email")).SendKeys("alexandros.blaskTEST@uni-a.de");
            }
            _driver.FindElement(By.Id("Input_Password")).SendKeys("Sopro.2025");
            _driver.FindElement(By.Id("login-submit")).Click();

            Thread.Sleep(2000);

            // 3. Navigate to Dashboard
            _driver.Navigate().GoToUrl("http://localhost:5079/Dashboard");

            if (_driver.Url != "http://localhost:5079/Dashboard")
                throw new Exception("Navigation to Dashboard failed.");

            // 4. Navigate to alexandros.blaskTEST
            if (isMentor) {
                var traineeSelect = new SelectElement(_driver.FindElement(By.Name("traineeId")));

                var targetOption = traineeSelect.Options
                    .FirstOrDefault(opt => opt.Text.Trim().Equals("alexandros.blaskTEST", StringComparison.OrdinalIgnoreCase))
                    ?? throw new Exception("Trainee 'alexandros.blaskTEST' not found in the dropdown.");

                traineeSelect.SelectByText("alexandros.blaskTEST");

                Thread.Sleep(500);
            }

            // 5. Find the first trainee lesson row with state
            var lessonRow = _driver.FindElements(By.CssSelector("tr.sopro-card"))
                .FirstOrDefault(row => {
                    try {
                        var span = row.FindElement(By.CssSelector($"span.sopro-tag.{state.ToLower()}"));
                        return true;
                    }
                    catch (NoSuchElementException) {
                        return false;
                    }
                }) ?? throw new Exception($"No lesson in state {state} found.");

            // 6. Scroll to row and click
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", lessonRow);
            Thread.Sleep(300);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", lessonRow);

            // 7. Wait for modal to open
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            var modal = wait.Until(drv => drv.FindElement(By.Id("lessonDetailModal")));
        }

        // ------------------------------------------------------
        private bool CheckIfIsCurrentState(string state) {
            // Locate the button by XPath using text match (whitespace ignored)
            Thread.Sleep(500);
            IWebElement openStateButton = _driver.FindElement(By.XPath($"//button[normalize-space(text())='{state}']"));

            // Get the class attribute value
            string classAttr = openStateButton.GetAttribute("class") ?? throw new Exception($"{state} button does not have a class");

            // Check if it has 'current' class -> it's the current state
            return classAttr.Contains("current");
        }

        // ------------------------------------------------------
        private void ClickOnState(string targetState) {
            // Press the state button by XPath using text match (whitespace ignored)
            IWebElement targetStateButton = _driver.FindElement(By.XPath($"//button[normalize-space(text())='{targetState}']"));

            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", targetStateButton);

            // Additional inputs if the state is either one of these two
            if (targetState == "Rejected") {
                // Wait for the rejection form to become visible
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
                wait.Until(driver => {
                    var formDiv = driver.FindElement(By.Id("rejectionForm"));
                    return formDiv.Displayed && formDiv.GetCssValue("display") != "none";
                });

                // Find and fill in the rejection reason
                var rejectionTextarea = _driver.FindElement(By.Name("rejectionReason"));
                rejectionTextarea.SendKeys("Reason for rejection entered by E2E test");

                // Submit the form
                var submitButton = _driver.FindElement(By.CssSelector("#rejectionForm button[type='submit']"));
                submitButton.Click();
            } else if (targetState == "Rated") {
                // Wait for the feedback form to be visible
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
                wait.Until(driver => {
                    var formDiv = driver.FindElement(By.Id("feedbackForm"));
                    return formDiv.Displayed && formDiv.GetCssValue("display") != "none";
                });

                // 1. Select the first option (not the default placeholder) in "Difficulty"
                var difficultyDropdown = new SelectElement(_driver.FindElement(By.Name("Difficulty")));
                difficultyDropdown.SelectByIndex(1); // index 0 is usually the placeholder "-- Select --"

                // 2. Select the first option in "Previous Knowledge"
                var previousKnowledgeDropdown = new SelectElement(_driver.FindElement(By.Name("PreviousKnowledge")));
                previousKnowledgeDropdown.SelectByIndex(1);

                // 3. Fill in "Hours of Effort" with 1.0
                var hoursOfEffortNumberfield = _driver.FindElement(By.Name("hoursOfEffort"));
                hoursOfEffortNumberfield.SendKeys("1.0");

                // Optional: fill comment or leave blank
                var commentField = _driver.FindElement(By.Name("comment"));
                commentField.SendKeys("Automated test input");

                // 4. Submit the form
                var submitButton = _driver.FindElement(By.CssSelector("#feedbackForm button[type='submit']"));
                submitButton.Click();
            }

            // Wait until the modal refreshes, i.e. the targetStatus button has the class "current"
            var wait2 = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait2.Until(driver =>
            {
                try
                {
                    var currentButton = driver.FindElements(By.CssSelector("button.sopro-status-button"))
                        .FirstOrDefault(b => b.GetAttribute("class")?.Contains("current") == true);

                    return currentButton != null && currentButton.Text.Trim() == targetState;
                }
                catch (StaleElementReferenceException)
                {
                    return false;
                }
            });
        }
    }
}