using OpenQA.Selenium;

namespace AutomationFramework.Drivers;

public interface IBrowserFactory
{
    IWebDriver Create();
}
