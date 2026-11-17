@Api
Feature: Fetching a time series data entry
  Time series data describes the electricity production, consumption and
  cross-border electricity flow of a bidding zone at a point in time.
  Each time series data entry has an id that the system assigns, and that
  identifies the entry. Anyone can fetch a single time series data entry
  by giving its id.

Background:
  Given the system is running
  And the following time series data has been recorded
    | Id  | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 101 | 2026-11-17T10:00:00Z | NO1          | 4512.75    | 3980.20     | -532.55           |
    | 102 | 2026-11-17T10:00:00Z | SE3          | 9120.00    | 10340.50    | 1220.50           |
    | 103 | 2026-11-17T11:00:00Z | NO1          | 4601.10    | 4015.00     | -586.10           |

Scenario: A time series data entry is fetched
  When the time series data entry with id 102 is fetched
  Then the following time series data entry is returned
    | Id  | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 102 | 2026-11-17T10:00:00Z | SE3          | 9120.00    | 10340.50    | 1220.50           |

Scenario: No time series data entry has the id
  When the time series data entry with id 999 is fetched
  Then no time series data entry is found
