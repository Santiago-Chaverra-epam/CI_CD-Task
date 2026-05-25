using NUnit.Framework;
using AutomationFramework.Pages;

namespace AutomationFramework.UITests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class CareersSearchTests : BaseTest
{
    [TestCase("Python", "All Locations")]
    [TestCase("Java", "All Locations")]
    public void ValidateUserCanSearchForPosition(string programmingLanguage, string location)
    {
        var homePage = new HomePage(Driver);
        homePage.NavigateTo();

        var careersPage = homePage.ClickCareers();
        careersPage.ClickStartSearch();
        careersPage.WaitForPageLoaderToDisappear();
        careersPage.SelectLocation(location);
        careersPage.SelectRemoteWorkplace();
        careersPage.EnterKeyword(programmingLanguage);
        careersPage.ClickSearch();
        careersPage.WaitForResultsToLoad();
        careersPage.ScrollToLatestResult();

        var title = careersPage.GetLatestVacancyTitle();

        Assert.That(
            title,
            Does.Contain(programmingLanguage).IgnoreCase,
            $"Expected latest vacancy title to contain '{programmingLanguage}' but was: '{title}'"
        );
    }
}
