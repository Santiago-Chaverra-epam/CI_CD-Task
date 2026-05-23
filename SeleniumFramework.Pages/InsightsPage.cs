using OpenQA.Selenium;
using SeleniumFramework.Core;

namespace SeleniumFramework.Pages;

public class InsightsPage : BasePage
{
    private static readonly By CarouselNextButton = By.XPath(
        "(//button[contains(@class,'slider__right-arrow')])[1]");

    private static readonly By ActiveSlideTitle = By.XPath(
        "(//div[contains(@class,'owl-item') and contains(@class,'active')])[1]" +
        "//p[contains(@class,'scaling-of-text-wrapper')]");

    private static readonly By ActiveSlideReadMore = By.XPath(
        "(//div[contains(@class,'owl-item active')])[1]//a[contains(@class,'link-with-bottom-arrow')]");

    public InsightsPage(IWebDriver driver) : base(driver) { }

    public void ClickCarouselNext() => ScrollAndClick(CarouselNextButton);

    public string GetActiveArticleTitle() => WaitForVisible(ActiveSlideTitle).Text.Trim();

    public ArticleDetailPage ClickReadMore()
    {
        ScrollAndClick(ActiveSlideReadMore);
        return new ArticleDetailPage(Driver);
    }
}
