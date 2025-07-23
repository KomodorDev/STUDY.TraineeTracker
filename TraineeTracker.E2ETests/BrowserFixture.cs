using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Tutorial_project.E2ETests {
    // BrowserFixture stellt einen einmalig initialisierten Browser für alle Tests bereit
    public class BrowserFixture : IDisposable {
        // Property, um den Selenium-WebDriver (Chrome) allen Tests bereitzustellen
        public IWebDriver Driver { get; private set; }

        // Konstruktor: wird beim Erstellen der Fixture-Klasse einmalig aufgerufen
        public BrowserFixture() {
            // ChromeOptions erlaubt dir, den Browser „Headless“ zu starten (ohne GUI)
            var options = new ChromeOptions();
            options.AddArgument("--headless");  // Im Headless-Modus, wichtig für CI/CD
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--no-sandbox"); // Wichtige Option für CI/CD, damit keine Sandbox-Probleme auftreten
            options.AddArgument("--disable-gpu"); // Deaktiviert GPU-Beschleunigung für Headless-Modus (stabiler)
            options.AddArgument("--disable-dev-shm-usage"); // Verhindert Speicherprobleme in Container-Umgebungen

            // TempDir for User
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);
            options.AddArgument($"--user-data-dir={tempDir}");

            // Erstelle den WebDriver (Chrome), übergib ihm die Optionen
            Driver = new ChromeDriver(options);

            // Starte die Browser-Session und navigiere direkt zur App-Startseite
            // Hier: HTTP (lokal unter Port 5260)
            Driver.Navigate().GoToUrl("http://localhost:5079");  // Anpassen auf deine lokale/Produktiv-URL
        }

        // IDisposable-Implementierung: sorgt dafür, dass der Browser nach Tests wieder geschlossen wird
        public void Dispose() {
            Driver.Quit();    // Schließt alle Browserfenster und beendet ChromeDriver-Prozesse
            Driver.Dispose(); // Gibt auch alle Ressourcen des Drivers frei
        }
    }
}
