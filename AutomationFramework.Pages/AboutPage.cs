using OpenQA.Selenium;
using AutomationFramework.Core;

namespace AutomationFramework.Pages;

public class AboutPage : BasePage
{
    private static readonly By EpamAtAGlanceSection = By.XPath("//p[contains(normalize-space(.), 'EPAM at a Glance')]");

    public AboutPage(IWebDriver driver) : base(driver) { }

    public bool IsEpamAtAGlanceSectionVisible()
    {
        var section = WaitForPresent(EpamAtAGlanceSection);
        ScrollIntoView(section);
        return section.Displayed;
    }
}
