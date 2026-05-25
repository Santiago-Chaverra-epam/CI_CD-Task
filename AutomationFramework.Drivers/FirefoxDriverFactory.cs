using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;

namespace AutomationFramework.Drivers;

public class FirefoxDriverFactory : IBrowserFactory
{
    public IWebDriver Create()
    {
        new DriverManager().SetUpDriver(new FirefoxConfig(), VersionResolveStrategy.Latest);

        var options = new FirefoxOptions();
        options.SetPreference("dom.webnotifications.enabled", false);

        var driver = new FirefoxDriver(options);
        driver.Manage().Window.Maximize();
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

        return driver;
    }
}
