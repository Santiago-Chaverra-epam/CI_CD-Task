Feature: Insights Carousel
  In order to read EPAM insights
  As a website visitor
  I want to navigate the insights carousel and open articles

  Scenario: Article title on detail page matches the carousel title
    Given I navigate to the EPAM home page
    When I click on Insights
    And I advance the carousel to the next article
    And I advance the carousel to the next article
    And I note the active article title from the carousel
    And I click Read More on the active article
    Then the article detail page title should contain the carousel title
