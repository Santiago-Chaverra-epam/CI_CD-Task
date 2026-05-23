using SeleniumFramework.Pages;

namespace SeleniumFramework.Specs.Shared;

public class PageContext
{
    public HomePage? HomePage { get; set; }
    public CareersPage? CareersPage { get; set; }
    public AboutPage? AboutPage { get; set; }
    public InsightsPage? InsightsPage { get; set; }
    public ArticleDetailPage? ArticleDetailPage { get; set; }
    public GlobalSearchResultsPage? GlobalSearchResultsPage { get; set; }
    public ServicesPage? ServicesPage { get; set; }
    public string? CarouselTitle { get; set; }
}
