using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Serilog;

namespace SeleniumFramework.Core;

public abstract class BasePage
{
    protected readonly IWebDriver Driver;
    protected readonly WebDriverWait Wait;

    protected BasePage(IWebDriver driver)
    {
        Driver = driver;
        Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
        Wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException), typeof(ElementClickInterceptedException));
    }

    protected IWebElement WaitForClickable(By locator)
    {
        Log.Debug("Waiting for clickable: {Locator}", locator);
        return Wait.Until(d =>
        {
            var el = d.FindElement(locator);
            if (!el.Displayed)
                throw new InvalidOperationException($"Element '{locator}' was found but is not displayed");
            if (!el.Enabled)
                throw new InvalidOperationException($"Element '{locator}' is displayed but is not enabled");
            return el;
        });
    }

    protected IWebElement WaitForVisible(By locator)
    {
        Log.Debug("Waiting for visible: {Locator}", locator);
        return Wait.Until(d =>
        {
            var el = d.FindElement(locator);
            return el;
        });
    }

    protected IWebElement WaitForPresent(By locator)
    {
        Log.Debug("Waiting for present: {Locator}", locator);
        return Wait.Until(d => d.FindElement(locator));
    }

    protected IReadOnlyCollection<IWebElement> WaitForElements(By locator)
    {
        Log.Debug("Waiting for elements: {Locator}", locator);
        return Wait.Until(d => d.FindElements(locator));
    }

    protected void ScrollIntoView(IWebElement element) =>
        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", element);

    protected void JavaScriptClick(IWebElement element) =>
        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", element);

    // Finds, scrolls to, and clicks an element in one atomic retry loop.
    // Retries on StaleElementReferenceException and ElementClickInterceptedException
    // (both configured on Wait) so a React re-render between scroll and click doesn't fail the test.
    protected void ScrollAndClick(By locator)
    {
        Log.Debug("ScrollAndClick: {Locator}", locator);
        Wait.Until(d =>
        {
            var el = d.FindElement(locator);
            if (!el.Displayed || !el.Enabled) return false;
            ((IJavaScriptExecutor)d).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", el);
            el.Click();
            return true;
        });
    }

    // Cookie banner is optional — swallowing WebDriverTimeoutException here is intentional.
    protected void DismissCookieBanner()
    {
        Log.Debug("Checking for cookie banner");
        try
        {
            var shortWait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
            var acceptBtn = shortWait.Until(d =>
            {
                var el = d.FindElement(By.Id("onetrust-accept-btn-handler"));
                return (el.Displayed && el.Enabled) ? el : null;
            });
            Log.Information("Dismissing cookie banner");
            JavaScriptClick(acceptBtn!);
            shortWait.Until(d =>
            {
                var banners = d.FindElements(By.Id("onetrust-banner-sdk"));
                return banners.Count == 0 || !banners[0].Displayed;
            });
        }
        catch (WebDriverTimeoutException)
        {
            Log.Debug("Cookie banner not present, continuing");
        }
    }
}
