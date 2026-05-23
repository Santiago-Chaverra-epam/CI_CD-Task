using NUnit.Framework;
using SeleniumFramework.Pages;

namespace SeleniumFramework.Tests;

[TestFixture]
public class AboutPageTests : BaseTest
{
    [Test]
    public void ValidateEpamAtAGlanceSectionIsVisible()
    {
        var homePage = new HomePage(Driver);
        homePage.NavigateTo();

        var aboutPage = homePage.ClickAbout();

        Assert.That(
            aboutPage.IsEpamAtAGlanceSectionVisible(),
            Is.True,
            "Expected 'EPAM at a Glance' section to be visible on the About page."
        );
    }
}
