using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Tutorial_project.E2ETests;
using System.Threading;

namespace TraineeTracker.E2ETests
{
    public class StatisticsSnapshotFallbackTests : IClassFixture<BrowserFixture>
    {
        private readonly IWebDriver _driver;

        public StatisticsSnapshotFallbackTests(BrowserFixture fixture)
        {
            _driver = fixture.Driver;
        }

        [Fact]
        public void Tooltip_ShouldAppear_WhenHoveringOverInfoIcon()
        {
            // 1. Navigate to login page
            _driver.Navigate().GoToUrl("http://localhost:5079/Identity/Account/Login?ReturnUrl=%2FDashboard");
            Thread.Sleep(1000);

            // 2. Login
            _driver.FindElement(By.Id("Input_Email")).SendKeys("vanessa.vital@makandra.de");
            _driver.FindElement(By.Id("Input_Password")).SendKeys("VVital13!");
            _driver.FindElement(By.Id("login-submit")).Click();

            Thread.Sleep(2000);

            // 3. Click the statistics button to open the modal
            _driver.FindElement(By.Id("openStatisticsModalBtn")).Click();

            Thread.Sleep(2000);

            // 4. Locate the modal and the info icon (which contains the tooltip)
            var modal = _driver.FindElement(By.Id("traineeStatisticsModal"));
            var infoWrapper = modal.FindElement(By.CssSelector(".metric-tile-new .info-icon-wrapper"));
            var tooltipBox = infoWrapper.FindElement(By.CssSelector(".tooltip-box"));

            // 5. Assert that the tooltip is not visible before hovering
            Assert.False(tooltipBox.Displayed || tooltipBox.Text.Trim().Length > 0, "Tooltip should not be visible initially.");

            // 6. Simulate a mouse hover over the info icon
            var actions = new OpenQA.Selenium.Interactions.Actions(_driver);
            actions.MoveToElement(infoWrapper).Perform();

            Thread.Sleep(1000);

            // 7. Assert that the tooltip is now visible and contains the expected text
            Assert.True(tooltipBox.Displayed, "Tooltip should be visible.");
            Assert.Contains("snapshot", tooltipBox.Text.ToLower());
        }
    }
}
