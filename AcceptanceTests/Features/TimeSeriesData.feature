@Api
Feature: Fetching time series data
  Time series data describes the electricity production, consumption and
  cross-border electricity flow of a bidding zone at a point in time.
  Anyone can fetch all of the time series data that the system holds.

Background:
  Given the system is running

Scenario: All time series data is fetched
  Given the following time series data has been recorded
    | Id  | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 101 | 2026-11-17T10:00:00Z | NO1          | 4512.75    | 3980.20     | -532.55           |
    | 102 | 2026-11-17T10:00:00Z | SE3          | 9120.00    | 10340.50    | 1220.50           |
    | 103 | 2026-11-17T11:00:00Z | NO1          | 4601.10    | 4015.00     | -586.10           |
    | 104 | 2026-11-17T11:00:00Z | DK1          | 2875.40    | 2640.85     | -234.55           |
  When all time series data is fetched
  Then the following time series data is returned
    | Id  | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 101 | 2026-11-17T10:00:00Z | NO1          | 4512.75    | 3980.20     | -532.55           |
    | 102 | 2026-11-17T10:00:00Z | SE3          | 9120.00    | 10340.50    | 1220.50           |
    | 103 | 2026-11-17T11:00:00Z | NO1          | 4601.10    | 4015.00     | -586.10           |
    | 104 | 2026-11-17T11:00:00Z | DK1          | 2875.40    | 2640.85     | -234.55           |

Scenario: No time series data has been recorded
  Given no time series data has been recorded
  When all time series data is fetched
  Then no time series data is returned
