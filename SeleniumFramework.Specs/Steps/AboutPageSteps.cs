using NUnit.Framework;
using SeleniumFramework.Specs.Shared;
using TechTalk.SpecFlow;

namespace SeleniumFramework.Specs.Steps;

[Binding]
public class AboutPageSteps
{
    private readonly PageContext _pages;

    public AboutPageSteps(PageContext pages)
    {
        _pages = pages;
    }

    [When("I click on the About navigation link")]
    public void WhenIClickOnTheAboutNavigationLink()
    {
        _pages.AboutPage = _pages.HomePage!.ClickAbout();
    }

    [Then("the EPAM at a Glance section should be visible")]
    public void ThenTheEpamAtAGlanceSectionShouldBeVisible()
    {
        Assert.That(_pages.AboutPage!.IsEpamAtAGlanceSectionVisible(), Is.True);
    }
}
