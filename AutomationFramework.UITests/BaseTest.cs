using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using Serilog;
using AutomationFramework.Drivers;

namespace AutomationFramework.UITests;

public abstract class BaseTest
{
    // ThreadLocal ensures each parallel test thread owns its own browser instance,
    // regardless of whether NUnit reuses or creates new fixture instances.
    private static readonly ThreadLocal<IWebDriver?> _driverHolder = new();
    private static IBrowserFactory _browserFactory = null!;

    protected IWebDriver Driver => _driverHolder.Value!;

    [OneTimeSetUp]
    public void InitialiseTestRun()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: "logs/tests-.log",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        var browser = config["Browser"] ?? "Chrome";
        _browserFactory = DriverFactory.GetFactory(browser);

        Log.Information("Logging initialised — browser: {Browser}", browser);
    }

    [SetUp]
    public void SetUp()
    {
        Log.Information("=== Test start: {TestName} ===", TestContext.CurrentContext.Test.Name);
        _driverHolder.Value = _browserFactory.Create();
        Log.Debug("Browser launched");
    }

    [TearDown]
    public void TearDown()
    {
        var result = TestContext.CurrentContext.Result;
        var status = result.Outcome.Status;

        if (status == TestStatus.Failed)
        {
            Log.Error("Test failed: {Message}", result.Message);
            TakeFailureScreenshot();
        }

        Log.Information("=== Test end: {TestName} — {Status} ===",
            TestContext.CurrentContext.Test.Name, status);

        _driverHolder.Value?.Quit();
        _driverHolder.Value = null;
        Log.Debug("Browser closed");
    }

    [OneTimeTearDown]
    public void FlushLogging()
    {
        Log.Information("Test run complete");
        Log.CloseAndFlush();
    }

    private void TakeFailureScreenshot()
    {
        if (Driver is not ITakesScreenshot screenshotDriver) return;

        var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        var safeName = string.Concat(
            TestContext.CurrentContext.Test.Name
                .Split(Path.GetInvalidFileNameChars()))
            .Replace("\"", "").Replace("(", "").Replace(")", "").Replace(",", "_");
        var dir = Path.Combine(TestContext.CurrentContext.WorkDirectory, "screenshots");
        Directory.CreateDirectory(dir);
        var filePath = Path.Combine(dir, $"{safeName}_{timestamp}.png");

        screenshotDriver.GetScreenshot().SaveAsFile(filePath);
        TestContext.AddTestAttachment(filePath, "Screenshot on failure");
        Log.Error("Screenshot saved: {Path}", filePath);
    }
}
