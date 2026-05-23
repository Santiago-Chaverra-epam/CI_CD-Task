Feature: Careers Search
  In order to find job opportunities
  As a job seeker
  I want to search for positions on the EPAM careers page

  Scenario Outline: User can search for a position by keyword
    Given I navigate to the EPAM home page
    When I click on Careers
    And I click Start Your Search
    And I select the location "<location>"
    And I select the Remote workplace option
    And I enter the search keyword "<keyword>"
    And I click the Search button
    Then the latest vacancy title should contain "<keyword>"

    Examples:
      | keyword | location      |
      | Python  | All Locations |
      | Java    | All Locations |
