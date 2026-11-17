@Api
Feature: Required values of updated time series data
  An update of a time series data entry must have a production, a
  consumption and a cross-border flow. An update that is missing any of
  these values is rejected, and the entry keeps the values it had.

Background:
  Given the system is running
  And the following time series data has been recorded
    | Id  | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 101 | 2026-11-17T10:00:00Z | NO1          | 4512.75    | 3980.20     | -532.55           |

Scenario Outline: An update with a missing value is rejected
  When the time series data entry with id 101 is updated with the following values
    | Production   | Consumption   | Cross-border flow   |
    | <Production> | <Consumption> | <Cross-border flow> |
  Then the update of the time series data entry is rejected because "<Reason>"
  And the system holds the following time series data
    | Id  | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 101 | 2026-11-17T10:00:00Z | NO1          | 4512.75    | 3980.20     | -532.55           |

  Examples:
    | Production | Consumption | Cross-border flow | Reason                           |
    |            | 4015.00     | -586.10           | the production is missing        |
    | 4601.10    |             | -586.10           | the consumption is missing       |
    | 4601.10    | 4015.00     |                   | the cross-border flow is missing |
