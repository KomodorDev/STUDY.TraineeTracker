using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Tutorial_project.E2ETests;

namespace TraineeTracker.E2ETests.AdminTests {

    /// <summary>
    /// Contains end-to-end tests for admin functionalities in the TraineeTracker application.
    /// </summary>
    /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
    public class AdminTest : IClassFixture<BrowserFixture> {
        IWebDriver _driver;

        // ------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="AdminTest"/> class with the specified browser fixture.
        /// </summary>
        /// <param name="fixture">The browser fixture providing the WebDriver instance.</param>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
        public AdminTest(BrowserFixture fixture) {
            _driver = fixture.Driver;
        }

        // ------------------------------------------------------
        /// <summary>
        /// Verifies that creating a user with valid data results in the user being visible in the admin dashboard.
        /// The test performs the following steps:
        /// <list type="number">
        /// <item>Logs in as an admin user.</item>
        /// <item>Navigates to the user creation page and fills out the form with valid data.</item>
        /// <item>Submits the form to create a new user.</item>
        /// <item>Navigates to the admin dashboard and searches for the newly created user, paging if necessary.</item>
        /// <item>Asserts that the user is found in the dashboard.</item>
        /// </list>
        /// </summary>
        /// <remarks>Code Ownership: Paul Schweizer (schwepau)</remarks>
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
            var teachingPlanSelect = new SelectElement(wait.Until(d => d.FindElement(By.Id("User_TeachingPlanId"))));
            teachingPlanSelect.SelectByValue("1");

            wait.Until(d => d.FindElement(By.Id("Create_User"))).Click();

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