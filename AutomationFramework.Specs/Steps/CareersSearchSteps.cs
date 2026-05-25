using NUnit.Framework;
using AutomationFramework.Specs.Shared;
using TechTalk.SpecFlow;

namespace AutomationFramework.Specs.Steps;

[Binding]
public class CareersSearchSteps
{
    private readonly PageContext _pages;

    public CareersSearchSteps(PageContext pages)
    {
        _pages = pages;
    }

    [When("I click on Careers")]
    public void WhenIClickOnCareers()
    {
        _pages.CareersPage = _pages.HomePage!.ClickCareers();
    }

    [When("I click Start Your Search")]
    public void WhenIClickStartYourSearch()
    {
        _pages.CareersPage!.ClickStartSearch();
    }

    [When(@"I select the location ""(.*)""")]
    public void WhenISelectTheLocation(string location)
    {
        _pages.CareersPage!.SelectLocation(location);
    }

    [When("I select the Remote workplace option")]
    public void WhenISelectTheRemoteWorkplaceOption()
    {
        _pages.CareersPage!.SelectRemoteWorkplace();
    }

    [When(@"I enter the search keyword ""(.*)""")]
    public void WhenIEnterTheSearchKeyword(string keyword)
    {
        _pages.CareersPage!.EnterKeyword(keyword);
    }

    [When("I click the Search button")]
    public void WhenIClickTheSearchButton()
    {
        _pages.CareersPage!.ClickSearch();
        _pages.CareersPage!.WaitForResultsToLoad();
        _pages.CareersPage!.ScrollToLatestResult();
    }

    [Then(@"the latest vacancy title should contain ""(.*)""")]
    public void ThenTheLatestVacancyTitleShouldContain(string keyword)
    {
        var title = _pages.CareersPage!.GetLatestVacancyTitle();
        Assert.That(title, Does.Contain(keyword).IgnoreCase);
    }
}
