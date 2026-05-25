using Reqnroll.BoDi;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using OpenQA.Selenium;
using Serilog;
using AutomationFramework.Drivers;
using Reqnroll;

namespace AutomationFramework.Specs.Hooks;

[Binding]
public sealed class TestHooks
{
    private readonly IObjectContainer _container;

    public TestHooks(IObjectContainer container)
    {
        _container = container;
    }

    [BeforeTestRun]
    public static void BeforeTestRun()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: "logs/specs-.log",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        Log.Information("Reqnroll test run started");
    }

    [BeforeScenario]
    public void BeforeScenario(ScenarioContext scenarioContext)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var browser = config["Browser"] ?? "Chrome";
        var driver = DriverFactory.GetFactory(browser).Create();

        _container.RegisterInstanceAs<IWebDriver>(driver);

        Log.Information("=== Scenario start: {ScenarioTitle} ===", scenarioContext.ScenarioInfo.Title);
    }

    [AfterScenario]
    public void AfterScenario(ScenarioContext scenarioContext)
    {
        var driver = _container.Resolve<IWebDriver>();

        if (scenarioContext.TestError != null)
        {
            Log.Error("Scenario failed: {Error}", scenarioContext.TestError.Message);
            TakeFailureScreenshot(driver, scenarioContext.ScenarioInfo.Title);
        }

        var status = scenarioContext.TestError == null ? "Passed" : "Failed";
        Log.Information("=== Scenario end: {ScenarioTitle} — {Status} ===",
            scenarioContext.ScenarioInfo.Title, status);

        driver?.Quit();
        driver?.Dispose();
    }

    private static void TakeFailureScreenshot(IWebDriver? driver, string scenarioTitle)
    {
        if (driver is not ITakesScreenshot screenshotDriver) return;

        var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        var safeName = string.Concat(scenarioTitle.Split(Path.GetInvalidFileNameChars()))
            .Replace("\"", "").Replace("(", "").Replace(")", "").Replace(",", "_");
        var dir = Path.Combine(TestContext.CurrentContext.WorkDirectory, "screenshots");
        Directory.CreateDirectory(dir);
        var filePath = Path.Combine(dir, $"{safeName}_{timestamp}.png");

        screenshotDriver.GetScreenshot().SaveAsFile(filePath);
        TestContext.AddTestAttachment(filePath, "Screenshot on failure");
        Log.Error("Screenshot saved: {Path}", filePath);
    }

    [AfterTestRun]
    public static void AfterTestRun()
    {
        Log.Information("Reqnroll test run complete");
        Log.CloseAndFlush();
    }
}
