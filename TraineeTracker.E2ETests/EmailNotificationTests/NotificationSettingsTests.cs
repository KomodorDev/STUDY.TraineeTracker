using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace TraineeTracker.E2ETests {

    /// <summary>
    /// End-to-end test for the notification settings page.
    /// </summary>
    /// <remarks>Code Ownnership: Simon Hinterreiter (hintsimo)</remarks>
    public class NotificationSettingsTests : IDisposable {

        private readonly IWebDriver _driver;

        public NotificationSettingsTests() {
            var options = new ChromeOptions();
            options.AddArgument("--headless"); // Optional: ohne GUI
            _driver = new ChromeDriver(options);
        }

        // ------------------------------------------------------
        [Fact]
        public void SaveNotificationSetting_ShouldPersistAfterReload() {

            // 1. Navigate to login page
            _driver.Navigate().GoToUrl("http://localhost:5079/Identity/Account/Login?ReturnUrl=%2FDashboard");

            Thread.Sleep(1000);

            // 2. Login
            _driver.FindElement(By.Id("Input_Email")).SendKeys("simon.hinterreiter@uni-a.de");
            _driver.FindElement(By.Id("Input_Password")).SendKeys("Sopro.2025");
            _driver.FindElement(By.Id("login-submit")).Click();

            Thread.Sleep(2000);

            // 3. Navigate to Notification Settings
            _driver.Navigate().GoToUrl("http://localhost:5079/Notifications");

            Assert.Equal("http://localhost:5079/Notifications", _driver.Url);

            // 4. Find a checkbox and toggle it
            var checkbox = _driver.FindElement(By.Id("ReceiveStartedNotifications"));
            bool initialState = checkbox.Selected; // Store initialState
            checkbox.Click(); // toggle

            // 5. Click save button
            var saveButton = _driver.FindElement(By.Id("saveNotificationsButton"));

            // Scroll
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", saveButton);
            Thread.Sleep(300);

            // Click via JavaScript
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", saveButton);

            // 6. Wait for reload (or poll DOM)
            Thread.Sleep(2000);

            // 7. Check if checkbox state persisted
            var newCheckbox = _driver.FindElement(By.Id("ReceiveStartedNotifications"));

            // Assert - Test 1
            Assert.Equal(!initialState, newCheckbox.Selected);
        }

        public void Dispose() {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}
