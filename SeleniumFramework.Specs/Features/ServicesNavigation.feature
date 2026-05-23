Feature: Services Navigation
  In order to explore EPAM's service offerings
  As a website visitor
  I want to navigate to specific AI service categories from the Services menu

  Scenario Outline: Navigate to a service sub-category from the Services menu
    Given I navigate to the EPAM home page
    When I hover over the Services link in the navigation menu
    And I click "<category>" in the Services dropdown
    Then the services page title should contain "<category>"
    And the "Our Related Expertise" section should be displayed

    Examples:
      | category       |
      | Generative AI  |
      | Responsible AI |
