using OpenQA.Selenium;
using Tutorial_project.E2ETests;

namespace TraineeTracker.E2ETests.ImportTests {
    /// <summary>
    /// End-to-end test for the Import Dashboard page.
    /// Verifies that the import form components are present and functional.
    /// </summary>
    public class ImportPageTests : IClassFixture<BrowserFixture> {
        private readonly IWebDriver _driver;
        private const string BaseUrl = "http://localhost:5079";

        public ImportPageTests(BrowserFixture fixture) {
            _driver = fixture.Driver;
        }

        [Fact]
        public void ImportDashboard_ShouldDisplayImportForm() {
            // 1. Navigate to login page
            _driver.Navigate().GoToUrl(BaseUrl + "/Identity/Account/Login?ReturnUrl=%2FTeachingPlan%2FDashboard");
            Thread.Sleep(1000);

            // 2. Login
            _driver.FindElement(By.Id("Input_Email")).SendKeys("simon.hinterreiter@uni-a.de");
            _driver.FindElement(By.Id("Input_Password")).SendKeys("Sopro.2025");
            _driver.FindElement(By.Id("login-submit")).Click();
            Thread.Sleep(2000);

            // 3. Navigate to Import Dashboard
            _driver.Navigate().GoToUrl(BaseUrl + "/TeachingPlan/Dashboard");
            Assert.Equal(BaseUrl + "/TeachingPlan/Dashboard", _driver.Url);

            // 4. Locate the import form
            var importForm = _driver.FindElement(By.CssSelector("form[action='/TeachingPlan/ImportNewTeachingPlan']"));
            Assert.NotNull(importForm);

            // 5. Verify form elements are present
            var nameInput = importForm.FindElement(By.Name("NewPlanName"));
            Assert.True(nameInput.Displayed, "Name input should be visible");

            var fileInput = importForm.FindElement(By.CssSelector("input[type='file'][name='NewPlanFile']"));
            Assert.True(fileInput.Displayed, "File input should be visible");

            var submitButton = importForm.FindElement(By.CssSelector("button[type='submit']"));
            Assert.True(submitButton.Displayed, "Import button should be visible");
        }
    }
}

