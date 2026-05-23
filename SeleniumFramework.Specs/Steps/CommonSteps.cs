using OpenQA.Selenium;
using SeleniumFramework.Pages;
using SeleniumFramework.Specs.Shared;
using TechTalk.SpecFlow;

namespace SeleniumFramework.Specs.Steps;

[Binding]
public class CommonSteps
{
    private readonly IWebDriver _driver;
    private readonly PageContext _pages;

    public CommonSteps(IWebDriver driver, PageContext pages)
    {
        _driver = driver;
        _pages = pages;
    }

    [Given("I navigate to the EPAM home page")]
    public void GivenINavigateToTheEpamHomePage()
    {
        _pages.HomePage = new HomePage(_driver);
        _pages.HomePage.NavigateTo();
    }
}
