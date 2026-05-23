using NUnit.Framework;
using SeleniumFramework.Specs.Shared;
using TechTalk.SpecFlow;

namespace SeleniumFramework.Specs.Steps;

[Binding]
public class ServicesNavigationSteps
{
    private readonly PageContext _pages;

    public ServicesNavigationSteps(PageContext pages)
    {
        _pages = pages;
    }

    [When("I hover over the Services link in the navigation menu")]
    public void WhenIHoverOverTheServicesLink()
    {
        _pages.HomePage!.HoverOverServices();
    }

    [When(@"I click ""(.*)"" in the Services dropdown")]
    public void WhenIClickInTheServicesDropdown(string category)
    {
        _pages.ServicesPage = _pages.HomePage!.ClickServicesSubLink(category);
    }

    [Then(@"the services page title should contain ""(.*)""")]
    public void ThenTheServicesPageTitleShouldContain(string expectedTitle)
    {
        var title = _pages.ServicesPage!.GetPageTitle();
        Assert.That(title, Does.Contain(expectedTitle).IgnoreCase);
    }

    [Then(@"the ""Our Related Expertise"" section should be displayed")]
    public void ThenTheRelatedExpertiseSectionShouldBeDisplayed()
    {
        Assert.That(_pages.ServicesPage!.IsRelatedExpertiseSectionDisplayed(), Is.True);
    }
}
