using NUnit.Framework;
using AutomationFramework.Specs.Shared;
using Reqnroll;

namespace AutomationFramework.Specs.Steps;

[Binding]
public class GlobalSearchSteps
{
    private readonly PageContext _pages;

    public GlobalSearchSteps(PageContext pages)
    {
        _pages = pages;
    }

    [When("I click the search icon")]
    public void WhenIClickTheSearchIcon()
    {
        _pages.HomePage!.ClickSearchIcon();
    }

    [When(@"I type ""(.*)"" into the search box")]
    public void WhenITypeIntoTheSearchBox(string searchTerm)
    {
        _pages.HomePage!.EnterSearchQuery(searchTerm);
    }

    [When("I click the Find button")]
    public void WhenIClickTheFindButton()
    {
        _pages.GlobalSearchResultsPage = _pages.HomePage!.ClickFind();
    }

    [Then(@"at least one search result title should contain ""(.*)""")]
    public void ThenAtLeastOneSearchResultTitleShouldContain(string searchTerm)
    {
        var titles = _pages.GlobalSearchResultsPage!.GetAllResultTitleTexts();
        Assert.That(
            titles.Any(t => t.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)),
            Is.True,
            $"No result title contains '{searchTerm}'");
    }
}
