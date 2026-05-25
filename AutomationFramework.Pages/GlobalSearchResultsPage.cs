using OpenQA.Selenium;
using Serilog;
using AutomationFramework.Core;

namespace AutomationFramework.Pages;

public class GlobalSearchResultsPage : BasePage
{
    private static readonly By ResultTitleLinks = By.CssSelector("a.search-results__title-link");

    public GlobalSearchResultsPage(IWebDriver driver) : base(driver) { }

    public IEnumerable<string> GetAllResultTitleTexts()
    {
        Log.Information("Retrieving search result titles");
        var elements = WaitForElements(ResultTitleLinks);
        var titles = elements.Select(e => e.Text);
        Log.Information("Found {Count} result(s)", titles.Count());
        return titles;
    }
}
