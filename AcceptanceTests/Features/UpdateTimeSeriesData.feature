@Api
Feature: Updating time series data
  Time series data describes the electricity production, consumption and
  cross-border electricity flow of a bidding zone at a point in time.
  Anyone can update a time series data entry that the system holds, by
  giving its id, so that mistakes in the data can be corrected.

  An update gives a new production, consumption and cross-border flow for
  the entry. The timestamp and the bidding zone identify what the entry
  describes, so they cannot be updated: they stay as they were when the
  entry was recorded. The entry also keeps its id.

  An update is only accepted when the new values are valid. The rules that
  the new values must follow are described in their own features. A
  rejected update leaves the time series data that the system holds
  unchanged.

Background:
  Given the system is running
  And the following time series data has been recorded
    | Id  | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 101 | 2026-11-17T10:00:00Z | NO1          | 4512.75    | 3980.20     | -532.55           |
    | 102 | 2026-11-17T10:00:00Z | SE3          | 9120.00    | 10340.50    | 1220.50           |

Scenario: A time series data entry is updated
  When the time series data entry with id 101 is updated with the following values
    | Production | Consumption | Cross-border flow |
    | 4601.10    | 4015.00     | -586.10           |
  Then the update of the time series data entry is accepted
  And the system holds the following time series data
    | Id  | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 101 | 2026-11-17T10:00:00Z | NO1          | 4601.10    | 4015.00     | -586.10           |
    | 102 | 2026-11-17T10:00:00Z | SE3          | 9120.00    | 10340.50    | 1220.50           |

Scenario: An updated time series data entry is fetched by its id
  When the time series data entry with id 101 is updated with the following values
    | Production | Consumption | Cross-border flow |
    | 4601.10    | 4015.00     | -586.10           |
  And the time series data entry with id 101 is fetched
  Then the following time series data entry is returned
    | Id  | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 101 | 2026-11-17T10:00:00Z | NO1          | 4601.10    | 4015.00     | -586.10           |

Scenario: An update with unchanged values is accepted
  When the time series data entry with id 101 is updated with the following values
    | Production | Consumption | Cross-border flow |
    | 4512.75    | 3980.20     | -532.55           |
  Then the update of the time series data entry is accepted
  And the system holds the following time series data
    | Id  | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 101 | 2026-11-17T10:00:00Z | NO1          | 4512.75    | 3980.20     | -532.55           |
    | 102 | 2026-11-17T10:00:00Z | SE3          | 9120.00    | 10340.50    | 1220.50           |

Scenario: No time series data entry has the id
  When the time series data entry with id 999 is updated with the following values
    | Production | Consumption | Cross-border flow |
    | 4601.10    | 4015.00     | -586.10           |
  Then no time series data entry is found
  And the system holds the following time series data
    | Id  | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 101 | 2026-11-17T10:00:00Z | NO1          | 4512.75    | 3980.20     | -532.55           |
    | 102 | 2026-11-17T10:00:00Z | SE3          | 9120.00    | 10340.50    | 1220.50           |
