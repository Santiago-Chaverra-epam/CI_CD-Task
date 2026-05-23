using OpenQA.Selenium;

namespace SeleniumFramework.Drivers;

public interface IBrowserFactory
{
    IWebDriver Create();
}
