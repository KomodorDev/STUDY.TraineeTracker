using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Tutorial_project.E2ETests {

    /// <summary>
    /// Provides a shared, pre-configured Selenium Chrome WebDriver for all end-to-end tests.
    /// Ensures consistent browser setup and handles cleanup after test execution.
    /// </summary>
    /// <remarks>Code Ownership: Simon Hinterreiter (hintsimo)</remarks>
    public class BrowserFixture : IDisposable {

        /// <summary>
        /// The Selenium WebDriver instance (Chrome) that can be used by all tests.
        /// </summary>
        public IWebDriver Driver { get; private set; }

        // ------------------------------------------------------
        /// <summary>
        /// Initializes a new headless ChromeDriver instance with CI/CD-safe options,
        /// including isolated user data directory and window size.
        /// </summary>
        public BrowserFixture() {

            var options = new ChromeOptions();

            // Run Chrome in headless mode (no GUI)
            options.AddArgument("--headless");

            // Standard resolution
            options.AddArgument("--window-size=1920,1080");

            // Required in some CI/CD containers
            options.AddArgument("--no-sandbox");

            // Disable GPU usage for stability
            options.AddArgument("--disable-gpu");

            // Prevent shared memory issues in Docker
            options.AddArgument("--disable-dev-shm-usage");

            // Use isolated user data directory
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);
            options.AddArgument($"--user-data-dir={tempDir}");

            // Erstelle den WebDriver (Chrome), übergib ihm die Optionen
            Driver = new ChromeDriver(options);

            // Navigate immediately to the app's base URL
            Driver.Navigate().GoToUrl("http://localhost:5079");
        }

        // ------------------------------------------------------
        /// <summary>
        /// Cleans up ChromeDriver and all associated browser processes after test execution.
        /// </summary>
        public void Dispose() {
            Driver.Quit();
            Driver.Dispose();
        }

        // ------------------------------------------------------
    }
}
