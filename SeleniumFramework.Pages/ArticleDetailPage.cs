using OpenQA.Selenium;
using SeleniumFramework.Core;

namespace SeleniumFramework.Pages;

public class ArticleDetailPage : BasePage
{
    private static readonly By ArticleHeading = By.XPath("//h1");

    public ArticleDetailPage(IWebDriver driver) : base(driver) { }

    public string GetArticleTitle() => WaitForVisible(ArticleHeading).Text.Trim();
}
