using NUnit.Framework;
using AutomationFramework.Pages;

namespace AutomationFramework.UITests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class GlobalSearchTests : BaseTest
{
    [TestCase("BLOCKCHAIN")]
    [TestCase("Cloud")]
    [TestCase("Automation")]
    public void ValidateGlobalSearchWorksAsExpected(string searchTerm)
    {
        var homePage = new HomePage(Driver);
        homePage.NavigateTo();
        homePage.ClickSearchIcon();
        homePage.EnterSearchQuery(searchTerm);

        var resultsPage = homePage.ClickFind();

        var titles = resultsPage.GetAllResultTitleTexts().ToList();

        Assert.That(titles, Is.Not.Empty, $"No search results found for '{searchTerm}'.");

        Assert.That(
            titles.Any(t => t.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)),
            Is.True,
            $"No result title contains '{searchTerm}'. Titles found: {string.Join(", ", titles)}"
        );
    }
}
