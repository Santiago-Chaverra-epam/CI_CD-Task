Feature: About Page
  In order to learn about EPAM
  As a website visitor
  I want to view the About page content

  Scenario: EPAM at a Glance section is visible
    Given I navigate to the EPAM home page
    When I click on the About navigation link
    Then the EPAM at a Glance section should be visible
