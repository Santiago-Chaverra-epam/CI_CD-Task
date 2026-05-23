Feature: Global Search
  In order to find relevant content
  As a website visitor
  I want to use the global search feature

  Scenario Outline: Global search returns relevant results
    Given I navigate to the EPAM home page
    When I click the search icon
    And I type "<searchTerm>" into the search box
    And I click the Find button
    Then at least one search result title should contain "<searchTerm>"

    Examples:
      | searchTerm |
      | BLOCKCHAIN |
      | Cloud      |
      | Automation |
