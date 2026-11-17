@Api
Feature: Value limits of new time series data
  The production and consumption of a new time series data entry must not
  be negative. The cross-border flow may be negative, as electricity can
  flow both into and out of a bidding zone. No value may have more than
  three decimal places. An entry that breaks these limits is rejected.

Background:
  Given the system is running

Scenario Outline: An entry at the limits is accepted
  When the following time series data entry is created
    | Timestamp            | Bidding zone | Production   | Consumption   | Cross-border flow   |
    | 2026-11-17T10:00:00Z | DK1          | <Production> | <Consumption> | <Cross-border flow> |
  Then the time series data entry is accepted and given a new id
  And the system holds the following time series data
    | Id         | Timestamp            | Bidding zone | Production   | Consumption   | Cross-border flow   |
    | the new id | 2026-11-17T10:00:00Z | DK1          | <Production> | <Consumption> | <Cross-border flow> |

  Examples:
    | Case                 | Production | Consumption | Cross-border flow |
    | no production        | 0          | 2640.85     | 2640.85           |
    | no consumption       | 2875.40    | 0           | -2875.40          |
    | no cross-border flow | 2875.40    | 2875.40     | 0                 |
    | three decimal places | 2875.404   | 2640.851    | -234.553          |

Scenario Outline: An entry with a negative production or consumption is rejected
  When the following time series data entry is created
    | Timestamp            | Bidding zone | Production   | Consumption   | Cross-border flow |
    | 2026-11-17T10:00:00Z | NO1          | <Production> | <Consumption> | -532.55           |
  Then the time series data entry is rejected because "<Reason>"
  And the system holds no time series data

  Examples:
    | Production | Consumption | Reason                      |
    | -0.001     | 3980.20     | the production is negative  |
    | 4512.75    | -0.001      | the consumption is negative |

Scenario Outline: An entry with more than three decimal places is rejected
  When the following time series data entry is created
    | Timestamp            | Bidding zone | Production   | Consumption   | Cross-border flow   |
    | 2026-11-17T10:00:00Z | NO1          | <Production> | <Consumption> | <Cross-border flow> |
  Then the time series data entry is rejected because "<Reason>"
  And the system holds no time series data

  Examples:
    | Production | Consumption | Cross-border flow | Reason                                                   |
    | 4512.7501  | 3980.20     | -532.55           | the production has more than three decimal places        |
    | 4512.75    | 3980.2001   | -532.55           | the consumption has more than three decimal places       |
    | 4512.75    | 3980.20     | -532.5501         | the cross-border flow has more than three decimal places |
