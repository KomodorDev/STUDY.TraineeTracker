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

            wait.Until(d => d.FindElement(By.Id("Create_User"))).Click();

            wait.Until(d => d.FindElement(By.Id("Admin_Table")));
            _driver.Navigate().GoToUrl("http://localhost:5079/Admin/Dashboard");
            int page = 1;
            bool found = false;
            while (!found) {
                try {
                    wait.Until(d => d.FindElement(By.Id("Admin_Table")));
                    _driver.FindElement(By.XPath("//td[text()='test.user@makandra.de']"));
                    found = true;
                    break;
                }
                catch (NoSuchElementException) {
                    try {
                        _driver.FindElement(By.Id("Next_Page"));
                        _driver.Navigate().GoToUrl($"http://localhost:5079/Admin/Dashboard?page={++page}");
                    }
                    catch (NoSuchElementException) {
                        break;
                    }
                }
            }
            Assert.True(found);
        }
    }
}