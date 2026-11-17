@Api
Feature: Bidding zone of new time series data
  A new time series data entry must be for one of the Nordic bidding
  zones: NO1, NO2, NO3, NO4, NO5, SE1, SE2, SE3, SE4, DK1, DK2 or FI. An
  entry for any other bidding zone is rejected.

Background:
  Given the system is running

Scenario Outline: An entry for a Nordic bidding zone is accepted
  When the following time series data entry is created
    | Timestamp            | Bidding zone   | Production | Consumption | Cross-border flow |
    | 2026-11-17T10:00:00Z | <Bidding zone> | 1000.00    | 900.00      | -100.00           |
  Then the time series data entry is accepted

  Examples:
    | Bidding zone |
    | NO1          |
    | NO2          |
    | NO3          |
    | NO4          |
    | NO5          |
    | SE1          |
    | SE2          |
    | SE3          |
    | SE4          |
    | DK1          |
    | DK2          |
    | FI           |

Scenario Outline: An entry for an unknown bidding zone is rejected
  When the following time series data entry is created
    | Timestamp            | Bidding zone   | Production | Consumption | Cross-border flow |
    | 2026-11-17T10:00:00Z | <Bidding zone> | 4512.75    | 3980.20     | -532.55           |
  Then the time series data entry is rejected because "the bidding zone is unknown"
  And the system holds no time series data

  Examples:
    | Case                         | Bidding zone |
    | outside the Nordic countries | DE1          |
    | beyond the Norwegian zones   | NO6          |
    | beyond the Swedish zones     | SE5          |
    | a country rather than a zone | SE           |
