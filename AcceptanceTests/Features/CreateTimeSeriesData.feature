@Api
Feature: Creating time series data
  Time series data describes the electricity production, consumption and
  cross-border electricity flow of a bidding zone at a point in time.
  Anyone can create a new time series data entry, so that the system holds
  data that has not been imported from an external source.

  An entry is only accepted when it is valid. The rules that an entry must
  follow are described in their own features. A rejected entry leaves the
  time series data that the system holds unchanged.

Background:
  Given the system is running

Scenario: A time series data entry is created
  When the following time series data entry is created
    | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 2026-11-17T10:00:00Z | NO1          | 4512.75    | 3980.20     | -532.55           |
  Then the time series data entry is accepted
  And the system holds the following time series data
    | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 2026-11-17T10:00:00Z | NO1          | 4512.75    | 3980.20     | -532.55           |
