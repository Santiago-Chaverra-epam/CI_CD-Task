using OpenQA.Selenium;
using Serilog;
using AutomationFramework.Core;

namespace AutomationFramework.Pages;

public class ServicesPage : BasePage
{
    private static readonly By PageTitle = By.XPath(
        "//span[contains(@class,'rte-text-gradient')]/span[contains(@class,'gradient-text')]");
    private static readonly By RelatedExpertiseHeading = By.XPath(
        "//span[contains(@class,'museo-sans-light') and contains(normalize-space(.), 'Our Related Expertise')]");

    public ServicesPage(IWebDriver driver) : base(driver) { }

    public string GetPageTitle()
    {
        var title = WaitForVisible(PageTitle).Text;
        Log.Information("Services page title: {Title}", title);
        return title;
    }

    public bool IsRelatedExpertiseSectionDisplayed()
    {
        Log.Information("Checking for 'Our Related Expertise' section");
        return WaitForVisible(RelatedExpertiseHeading).Displayed;
    }
}
