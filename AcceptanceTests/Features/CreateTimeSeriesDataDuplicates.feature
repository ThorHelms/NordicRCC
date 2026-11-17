@Api
Feature: Duplicate time series data
  A bidding zone has one time series data entry per point in time. A new
  entry for a bidding zone and timestamp that the system already holds
  data for is rejected, and the existing data is kept.

Background:
  Given the system is running
  And the following time series data has been recorded
    | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 2026-11-17T10:00:00Z | NO1          | 4512.75    | 3980.20     | -532.55           |

Scenario Outline: An entry that differs in bidding zone or timestamp is accepted
  When the following time series data entry is created
    | Timestamp   | Bidding zone   | Production | Consumption | Cross-border flow |
    | <Timestamp> | <Bidding zone> | 9120.00    | 10340.50    | 1220.50           |
  Then the time series data entry is accepted
  And the system holds the following time series data
    | Timestamp            | Bidding zone   | Production | Consumption | Cross-border flow |
    | 2026-11-17T10:00:00Z | NO1            | 4512.75    | 3980.20     | -532.55           |
    | <Timestamp>          | <Bidding zone> | 9120.00    | 10340.50    | 1220.50           |

  Examples:
    | Case                                 | Timestamp            | Bidding zone |
    | same timestamp, another bidding zone | 2026-11-17T10:00:00Z | SE3          |
    | same bidding zone, another timestamp | 2026-11-17T11:00:00Z | NO1          |

Scenario: An entry for a bidding zone and timestamp that already has data is rejected
  When the following time series data entry is created
    | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 2026-11-17T10:00:00Z | NO1          | 4601.10    | 4015.00     | -586.10           |
  Then the time series data entry is rejected because "the bidding zone already has time series data at the timestamp"
  And the system holds the following time series data
    | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 2026-11-17T10:00:00Z | NO1          | 4512.75    | 3980.20     | -532.55           |
