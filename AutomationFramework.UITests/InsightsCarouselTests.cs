using NUnit.Framework;
using AutomationFramework.Pages;

namespace AutomationFramework.UITests;

[TestFixture]
public class InsightsCarouselTests : BaseTest
{
    [Test]
    public void ValidateTitleOfArticleMatchesTitleInCarousel()
    {
        var homePage = new HomePage(Driver);
        homePage.NavigateTo();

        var insightsPage = homePage.ClickInsights();
        insightsPage.ClickCarouselNext();
        insightsPage.ClickCarouselNext();

        var carouselTitle = insightsPage.GetActiveArticleTitle();
        var articlePage = insightsPage.ClickReadMore();
        var articleTitle = articlePage.GetArticleTitle();

        Assert.That(
            articleTitle.Contains(carouselTitle, StringComparison.OrdinalIgnoreCase),
            Is.True,
            $"Expected article page title to match the carousel title. " +
            $"Carousel: '{carouselTitle}', Article page: '{articleTitle}'"
        );
    }
}
