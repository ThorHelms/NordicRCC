@Api
Feature: Value limits of updated time series data
  The production and consumption given in an update of a time series data
  entry must not be negative. The cross-border flow may be negative, as
  electricity can flow both into and out of a bidding zone. No value may
  have more than three decimal places. An update that breaks these limits
  is rejected, and the entry keeps the values it had.

Background:
  Given the system is running
  And the following time series data has been recorded
    | Id  | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 101 | 2026-11-17T10:00:00Z | DK1          | 2875.40    | 2640.85     | -234.55           |

Scenario Outline: An update at the limits is accepted
  When the time series data entry with id 101 is updated with the following values
    | Production   | Consumption   | Cross-border flow   |
    | <Production> | <Consumption> | <Cross-border flow> |
  Then the update of the time series data entry is accepted
  And the system holds the following time series data
    | Id  | Timestamp            | Bidding zone | Production   | Consumption   | Cross-border flow   |
    | 101 | 2026-11-17T10:00:00Z | DK1          | <Production> | <Consumption> | <Cross-border flow> |

  Examples:
    | Case                 | Production | Consumption | Cross-border flow |
    | no production        | 0          | 2640.85     | 2640.85           |
    | no consumption       | 2875.40    | 0           | -2875.40          |
    | no cross-border flow | 2875.40    | 2875.40     | 0                 |
    | three decimal places | 2875.404   | 2640.851    | -234.553          |

Scenario Outline: An update with a negative production or consumption is rejected
  When the time series data entry with id 101 is updated with the following values
    | Production   | Consumption   | Cross-border flow |
    | <Production> | <Consumption> | -234.55           |
  Then the update of the time series data entry is rejected because "<Reason>"
  And the system holds the following time series data
    | Id  | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 101 | 2026-11-17T10:00:00Z | DK1          | 2875.40    | 2640.85     | -234.55           |

  Examples:
    | Production | Consumption | Reason                      |
    | -0.001     | 2640.85     | the production is negative  |
    | 2875.40    | -0.001      | the consumption is negative |

Scenario Outline: An update with more than three decimal places is rejected
  When the time series data entry with id 101 is updated with the following values
    | Production   | Consumption   | Cross-border flow   |
    | <Production> | <Consumption> | <Cross-border flow> |
  Then the update of the time series data entry is rejected because "<Reason>"
  And the system holds the following time series data
    | Id  | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 101 | 2026-11-17T10:00:00Z | DK1          | 2875.40    | 2640.85     | -234.55           |

  Examples:
    | Production | Consumption | Cross-border flow | Reason                                                   |
    | 2875.4001  | 2640.85     | -234.55           | the production has more than three decimal places        |
    | 2875.40    | 2640.8501   | -234.55           | the consumption has more than three decimal places       |
    | 2875.40    | 2640.85     | -234.5501         | the cross-border flow has more than three decimal places |
