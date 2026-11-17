@Api
Feature: Filtering time series data on bidding zone
  Time series data describes the electricity production, consumption and
  cross-border electricity flow of a bidding zone at a point in time.
  Anyone can fetch the time series data of a single bidding zone, so that
  they only receive the data that is relevant to them.

Background:
  Given the system is running
  And the following time series data has been recorded
    | Id  | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 101 | 2026-11-17T10:00:00Z | NO1          | 4512.75    | 3980.20     | -532.55           |
    | 102 | 2026-11-17T10:00:00Z | SE3          | 9120.00    | 10340.50    | 1220.50           |
    | 103 | 2026-11-17T11:00:00Z | NO1          | 4601.10    | 4015.00     | -586.10           |
    | 104 | 2026-11-17T11:00:00Z | DK1          | 2875.40    | 2640.85     | -234.55           |

Scenario: Time series data is fetched for a bidding zone
  When the time series data for bidding zone "NO1" is fetched
  Then the following time series data is returned
    | Id  | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 101 | 2026-11-17T10:00:00Z | NO1          | 4512.75    | 3980.20     | -532.55           |
    | 103 | 2026-11-17T11:00:00Z | NO1          | 4601.10    | 4015.00     | -586.10           |

Scenario: No time series data has been recorded for the bidding zone
  When the time series data for bidding zone "FI" is fetched
  Then no time series data is returned
