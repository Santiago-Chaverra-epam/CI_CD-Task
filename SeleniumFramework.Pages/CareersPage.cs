using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Serilog;
using SeleniumFramework.Core;

namespace SeleniumFramework.Pages;

public class CareersPage : BasePage
{
    private static readonly By StartSearchButton = By.XPath(
        "//span[contains(@class,'uppercase-text') and contains(normalize-space(.), 'Start Your Search Here') and not(ancestor::*[contains(@class, 'pinned-link-hide')])]");
    private static readonly By KeywordsInput = By.XPath("//input[@data-testid='search-input']");
    private static readonly By LocationDropdown = By.XPath("//div[contains(@data-testid,'country-dropdown')]");
    private static readonly By LocationClearButton = By.CssSelector("[data-testid*='country-dropdown'] .dropdown__clear-indicator");
    private static readonly By RemoteCheckbox = By.XPath("//input[@name='vacancy_type-Remote']");
    private static readonly By SearchButton = By.XPath("//button[@name='submit_search_box_button']");
    private static readonly By AnyResultContainer = By.XPath("//div[@data-testid='accordion-section-container']");
    private static readonly By LatestResultTitle = By.XPath(
        "(//div[@data-testid='accordion-section-container'])[last()]//a[@data-event-content='vacancy_title']");
    private static readonly By FullPagePreloader = By.CssSelector("[class*='Preloader_fullSize']");

    public CareersPage(IWebDriver driver) : base(driver) { }

    public void ClickStartSearch()
    {
        Log.Information("Clicking Start Your Search Here");
        ScrollAndClick(StartSearchButton);
        DismissCookieBanner();
    }

    public void EnterKeyword(string keyword)
    {
        Log.Information("Entering keyword: {Keyword}", keyword);
        Wait.Until(d =>
        {
            var input = d.FindElement(KeywordsInput);
            if (!input.Displayed || !input.Enabled) return false;
            input.Clear();
            input.SendKeys(keyword);
            return true;
        });
    }

    public void SelectLocation(string location)
    {
        Log.Information("Selecting location: {Location}", location);

        // Geo-IP auto-selects the user's country; wait for that to settle before acting.
        WaitForLocationToStabilize();

        if (location.Equals("All Locations", StringComparison.OrdinalIgnoreCase))
        {
            // "All Locations" is the cleared state. Only click × if a country was auto-selected.
            var clearBtns = Driver.FindElements(LocationClearButton);
            if (clearBtns.Count > 0 && clearBtns[0].Displayed)
            {
                Log.Debug("Clearing auto-detected location");
                ScrollAndClick(LocationClearButton);
            }
            return;
        }

        ScrollAndClick(LocationDropdown);

        var option = WaitForClickable(By.XPath($"(//*[@role='option'] | //li)[normalize-space(.)='{location}']"));
        option.Click();
    }

    private void WaitForLocationToStabilize()
    {
        // Geo-IP may or may not fire; swallowing the timeout is intentional.
        Log.Debug("Waiting for geo-IP location to stabilise");
        var shortWait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
        shortWait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
        try
        {
            shortWait.Until(d =>
            {
                var text = d.FindElement(LocationDropdown).Text?.Trim() ?? string.Empty;
                return !string.IsNullOrEmpty(text) && text != "All Locations";
            });
        }
        catch (WebDriverTimeoutException) { }
    }

    public void SelectRemoteWorkplace()
    {
        Log.Information("Selecting Remote workplace option");
        // Checkbox is visually hidden by CSS — interact via JS to avoid interactability issues.
        Wait.Until(d =>
        {
            var js = (IJavaScriptExecutor)d;
            var el = d.FindElement(RemoteCheckbox);
            js.ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", el);
            var isChecked = (bool)js.ExecuteScript("return arguments[0].checked;", el);
            if (!isChecked)
                js.ExecuteScript("arguments[0].click();", el);
            return true;
        });
    }

    public void ClickSearch()
    {
        Log.Information("Clicking Search button");
        ScrollAndClick(SearchButton);
    }

    public void WaitForPageLoaderToDisappear()
    {
        Log.Debug("Waiting for page loader to disappear");
        Wait.Until(d =>
        {
            var loaders = d.FindElements(By.XPath(
                "//*[contains(concat(' ', normalize-space(@class), ' '), ' loader ') or " +
                "contains(concat(' ', normalize-space(@class), ' '), ' Loader ') or " +
                "contains(concat(' ', normalize-space(@class), ' '), ' preloader ') or " +
                "contains(@class, 'Preloader_fullSize') or " +
                "@data-testid='preloader']"));
            return loaders.All(l => !l.Displayed);
        });
    }

    // Old results stay visible during load; waiting for the preloader cycle is the reliable signal
    // that the new result set is in the DOM. The first wait is optional — the preloader may appear
    // and vanish before we get here, so its timeout is intentionally swallowed.
    public void WaitForResultsToLoad()
    {
        Log.Debug("Waiting for search results to load");
        var shortWait = new WebDriverWait(Driver, TimeSpan.FromSeconds(5));
        try
        {
            shortWait.Until(d => d.FindElements(FullPagePreloader).Any(e => e.Displayed));
        }
        catch (WebDriverTimeoutException) { }

        Wait.Until(d => d.FindElements(FullPagePreloader).All(e => !e.Displayed));
        WaitForVisible(AnyResultContainer);
        Log.Debug("Search results loaded");
    }

    public void ScrollToLatestResult()
    {
        Log.Debug("Scrolling to latest result");
        Wait.Until(d =>
        {
            var el = d.FindElement(LatestResultTitle);
            ((IJavaScriptExecutor)d).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", el);
            return el.Displayed;
        });

        // Scrolling can trigger lazy-load; wait for the count to stabilise before reading [last()].
        WaitForResultCountToStabilise();
    }

    private void WaitForResultCountToStabilise()
    {
        var shortWait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
        int lastCount = -1;
        shortWait.Until(d =>
        {
            int current = d.FindElements(AnyResultContainer).Count;
            if (current == lastCount) return true;
            lastCount = current;
            return false;
        });
        Log.Debug("Result count stabilised at {Count}", lastCount);
    }

    public string GetLatestVacancyTitle()
    {
        var title = WaitForVisible(LatestResultTitle).Text;
        Log.Information("Latest vacancy title: {Title}", title);
        return title;
    }
}
