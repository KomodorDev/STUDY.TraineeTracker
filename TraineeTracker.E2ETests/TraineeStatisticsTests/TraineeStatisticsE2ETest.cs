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
            _driver.Navigate().GoToUrl("http://localhost:5079/Identity/Account/Login?ReturnUrl=%2FDashboard");
            Thread.Sleep(1000);

            _driver.FindElement(By.Id("Input_Email")).SendKeys("vanessa.vital@makandra.de");
            _driver.FindElement(By.Id("Input_Password")).SendKeys("VVital13!");
            _driver.FindElement(By.Id("login-submit")).Click();

            Thread.Sleep(2000);

            Thread.Sleep(1000);
            _driver.FindElement(By.Id("openStatisticsModalBtn")).Click();

            Thread.Sleep(2000);

            var modal = _driver.FindElement(By.Id("traineeStatisticsModal"));
            var infoWrapper = modal.FindElement(By.CssSelector(".metric-tile-new .info-icon-wrapper"));
            var tooltipBox = infoWrapper.FindElement(By.CssSelector(".tooltip-box"));

            Assert.False(tooltipBox.Displayed || tooltipBox.Text.Trim().Length > 0, "Tooltip should not be visible initially.");

            var actions = new OpenQA.Selenium.Interactions.Actions(_driver);
            actions.MoveToElement(infoWrapper).Perform();

            Thread.Sleep(1000);

            Assert.True(tooltipBox.Displayed, "Tooltip should be visible.");
            Assert.Contains("snapshot", tooltipBox.Text.ToLower());
        }
    }
}
