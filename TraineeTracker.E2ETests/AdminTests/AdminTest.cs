using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Tutorial_project.E2ETests;

namespace TraineeTracker.E2ETests.AdminTests {
    public class AdminTest : IClassFixture<BrowserFixture> {
        IWebDriver _driver;

        public AdminTest(BrowserFixture fixture) {
            _driver = fixture.Driver;
        }

        [Fact]
        public void CreateUser_WithValidData_UserIsVisibleInAdminDashboard() {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(60));

            _driver.Navigate().GoToUrl("http://localhost:5079/Identity/Account/Login");
            wait.Until(d => d.FindElement(By.Id("Input_Email"))).SendKeys("simon.hinterreiter@uni-a.de");
            wait.Until(d => d.FindElement(By.Id("Input_Password"))).SendKeys("Sopro.2025");
            wait.Until(d => d.FindElement(By.Id("login-submit"))).Click();

            wait.Until(d => d.FindElement(By.Id("Nav_Admin")));
            _driver.Navigate().GoToUrl("http://localhost:5079/Admin/CreateUser");
            var roleSelect = new SelectElement(wait.Until(d => d.FindElement(By.Id("User_Role"))));
            roleSelect.SelectByValue("Trainee");

            wait.Until(d => d.FindElement(By.Id("User_Email"))).SendKeys("test.user@makandra.de");
            wait.Until(d => d.FindElement(By.Id("User_TraineeStartDate"))).SendKeys("01-08-2025");
            wait.Until(d => d.FindElement(By.Id("User_TraineeEndDate"))).SendKeys("31-12-2025");
            var teachingPlanSelect = new SelectElement(wait.Until(d => d.FindElement(By.Id("User_TeachingPlanId"))));
            teachingPlanSelect.SelectByValue("1");

            var resultsDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "TestResults"));

            if (!Directory.Exists(resultsDir))
                Directory.CreateDirectory(resultsDir);
            var screenshot = ((ITakesScreenshot)_driver).GetScreenshot();
            screenshot.SaveAsFile(Path.Combine(resultsDir, $"Screenshot_AdminTest_{DateTime.Now:ddMMyyyy_HHmmss}.png"));

        }
    }
}