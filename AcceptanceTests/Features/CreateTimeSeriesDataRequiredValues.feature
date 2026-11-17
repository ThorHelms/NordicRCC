@Api
Feature: Required values of new time series data
  A new time series data entry must have a timestamp, a bidding zone, a
  production, a consumption and a cross-border flow. An entry that is
  missing any of these values is rejected.

Background:
  Given the system is running

Scenario Outline: An entry with a missing value is rejected
  When the following time series data entry is created
    | Timestamp   | Bidding zone   | Production   | Consumption   | Cross-border flow   |
    | <Timestamp> | <Bidding zone> | <Production> | <Consumption> | <Cross-border flow> |
  Then the time series data entry is rejected because "<Reason>"
  And the system holds no time series data

  Examples:
    | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow | Reason                           |
    |                      | NO1          | 4512.75    | 3980.20     | -532.55           | the timestamp is missing         |
    | 2026-11-17T10:00:00Z |              | 4512.75    | 3980.20     | -532.55           | the bidding zone is missing      |
    | 2026-11-17T10:00:00Z | NO1          |            | 3980.20     | -532.55           | the production is missing        |
    | 2026-11-17T10:00:00Z | NO1          | 4512.75    |             | -532.55           | the consumption is missing       |
    | 2026-11-17T10:00:00Z | NO1          | 4512.75    | 3980.20     |                   | the cross-border flow is missing |
