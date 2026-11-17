@Api
Feature: Creating time series data
  Time series data describes the electricity production, consumption and
  cross-border electricity flow of a bidding zone at a point in time.
  Anyone can create a new time series data entry, so that the system holds
  data that has not been imported from an external source.

  The system gives each accepted entry a new id, which identifies the
  entry, and returns the id to whoever created the entry. In the
  scenarios, "the new id" stands for the id that the system returned.

  An entry is only accepted when it is valid. The rules that an entry must
  follow are described in their own features. A rejected entry is not
  given an id, and leaves the time series data that the system holds
  unchanged.

Background:
  Given the system is running

Scenario: A time series data entry is created
  When the following time series data entry is created
    | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 2026-11-17T10:00:00Z | NO1          | 4512.75    | 3980.20     | -532.55           |
  Then the time series data entry is accepted and given a new id
  And the system holds the following time series data
    | Id         | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | the new id | 2026-11-17T10:00:00Z | NO1          | 4512.75    | 3980.20     | -532.55           |

Scenario: A created time series data entry is fetched by its new id
  When the following time series data entry is created
    | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 2026-11-17T10:00:00Z | NO1          | 4512.75    | 3980.20     | -532.55           |
  And the time series data entry with the new id is fetched
  Then the following time series data entry is returned
    | Id         | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | the new id | 2026-11-17T10:00:00Z | NO1          | 4512.75    | 3980.20     | -532.55           |

Scenario: A created time series data entry is given an id that no other entry has
  Given the following time series data has been recorded
    | Id  | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 101 | 2026-11-17T10:00:00Z | NO1          | 4512.75    | 3980.20     | -532.55           |
  When the following time series data entry is created
    | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 2026-11-17T10:00:00Z | SE3          | 9120.00    | 10340.50    | 1220.50           |
  Then the time series data entry is accepted and given a new id
  And the system holds the following time series data
    | Id         | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 101        | 2026-11-17T10:00:00Z | NO1          | 4512.75    | 3980.20     | -532.55           |
    | the new id | 2026-11-17T10:00:00Z | SE3          | 9120.00    | 10340.50    | 1220.50           |
