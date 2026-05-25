using NUnit.Framework;
using AutomationFramework.Specs.Shared;
using Reqnroll;

namespace AutomationFramework.Specs.Steps;

[Binding]
public class InsightsCarouselSteps
{
    private readonly PageContext _pages;

    public InsightsCarouselSteps(PageContext pages)
    {
        _pages = pages;
    }

    [When("I click on Insights")]
    public void WhenIClickOnInsights()
    {
        _pages.InsightsPage = _pages.HomePage!.ClickInsights();
    }

    [When("I advance the carousel to the next article")]
    public void WhenIAdvanceTheCarouselToTheNextArticle()
    {
        _pages.InsightsPage!.ClickCarouselNext();
    }

    [When("I note the active article title from the carousel")]
    public void WhenINoteTheActiveArticleTitleFromTheCarousel()
    {
        _pages.CarouselTitle = _pages.InsightsPage!.GetActiveArticleTitle();
    }

    [When("I click Read More on the active article")]
    public void WhenIClickReadMoreOnTheActiveArticle()
    {
        _pages.ArticleDetailPage = _pages.InsightsPage!.ClickReadMore();
    }

    [Then("the article detail page title should contain the carousel title")]
    public void ThenTheArticleDetailPageTitleShouldContainTheCarouselTitle()
    {
        var articleTitle = _pages.ArticleDetailPage!.GetArticleTitle();
        Assert.That(articleTitle, Does.Contain(_pages.CarouselTitle!).IgnoreCase);
    }
}
