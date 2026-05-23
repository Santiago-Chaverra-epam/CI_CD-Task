using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using Serilog;
using SeleniumFramework.Core;

namespace SeleniumFramework.Pages;

public class HomePage : BasePage
{
    private const string BaseUrl = "https://www.epam.com/";

    private static readonly By CareersLink = By.XPath(
        "//a[contains(@class,'top-navigation__item-link') and normalize-space(.)='Careers']");
    private static readonly By AboutLink = By.XPath(
        "//a[contains(@class,'top-navigation__item-link') and normalize-space(.)='About']");
    private static readonly By InsightsLink = By.XPath(
        "//a[contains(@class,'top-navigation__item-link') and normalize-space(.)='Insights']");
    private static readonly By ServicesLink = By.XPath(
        "//a[contains(@class,'top-navigation__item-link') and normalize-space(.)='Services']");
    private static readonly By SearchIcon = By.CssSelector("span.header-search__search-icon");
    private static readonly By SearchInput = By.Id("new_form_search");
    private static readonly By FindButton = By.XPath("//button[contains(@class,'custom-button')]");

    public HomePage(IWebDriver driver) : base(driver) { }

    public void NavigateTo()
    {
        Log.Information("Navigating to {Url}", BaseUrl);
        Driver.Navigate().GoToUrl(BaseUrl);
        DismissCookieBanner();
    }

    public CareersPage ClickCareers()
    {
        Log.Information("Clicking Careers link");
        WaitForVisible(CareersLink).Click();
        return new CareersPage(Driver);
    }

    public AboutPage ClickAbout()
    {
        Log.Information("Clicking About link");
        WaitForVisible(AboutLink).Click();
        return new AboutPage(Driver);
    }

    public InsightsPage ClickInsights()
    {
        Log.Information("Clicking Insights link");
        WaitForVisible(InsightsLink).Click();
        return new InsightsPage(Driver);
    }

    public void ClickSearchIcon()
    {
        Log.Information("Clicking search icon");
        WaitForVisible(SearchIcon).Click();
    }

    public void EnterSearchQuery(string query)
    {
        Log.Information("Entering search query: {Query}", query);
        var input = WaitForVisible(SearchInput);
        input.Clear();
        input.SendKeys(query);
    }

    public GlobalSearchResultsPage ClickFind()
    {
        Log.Information("Clicking Find button");
        WaitForClickable(FindButton).Click();
        return new GlobalSearchResultsPage(Driver);
    }

    public void HoverOverServices()
    {
        Log.Information("Hovering over Services link");
        var link = WaitForVisible(ServicesLink);
        new Actions(Driver).MoveToElement(link).Perform();
    }

    public ServicesPage ClickServicesSubLink(string category)
    {
        Log.Information("Clicking Services sub-link: {Category}", category);
        var subLink = WaitForClickable(By.XPath(
            $"//a[contains(@class,'top-navigation__sub-link') and normalize-space(.)='{category}']"));
        subLink.Click();
        return new ServicesPage(Driver);
    }
}
