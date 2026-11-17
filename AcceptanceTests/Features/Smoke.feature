Feature: Smoke
  Verifies that the acceptance test setup is working.

Scenario: Acceptance tests can run
  Given the acceptance test project is set up
  When a scenario is executed
  Then it passes
