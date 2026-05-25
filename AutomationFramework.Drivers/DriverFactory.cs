namespace AutomationFramework.Drivers;

public static class DriverFactory
{
    public static IBrowserFactory GetFactory(string browser) =>
        browser.ToLowerInvariant() switch
        {
            "chrome"  => new ChromeDriverFactory(),
            "firefox" => new FirefoxDriverFactory(),
            _ => throw new NotSupportedException(
                $"Browser '{browser}' is not supported. Supported values: chrome, firefox.")
        };
}
