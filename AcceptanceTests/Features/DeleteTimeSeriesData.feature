@Api
Feature: Deleting time series data
  Time series data describes the electricity production, consumption and
  cross-border electricity flow of a bidding zone at a point in time.
  Anyone can delete a time series data entry that the system holds, by
  giving its id, so that data that should not have been recorded can be
  removed.

  A deleted entry is removed from the time series data that the system
  holds, and can no longer be fetched or updated. Deleting an entry leaves
  all other entries unchanged.

Background:
  Given the system is running
  And the following time series data has been recorded
    | Id  | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 101 | 2026-11-17T10:00:00Z | NO1          | 4512.75    | 3980.20     | -532.55           |
    | 102 | 2026-11-17T10:00:00Z | SE3          | 9120.00    | 10340.50    | 1220.50           |

Scenario: A time series data entry is deleted
  When the time series data entry with id 101 is deleted
  Then the deletion of the time series data entry is accepted
  And the system holds the following time series data
    | Id  | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 102 | 2026-11-17T10:00:00Z | SE3          | 9120.00    | 10340.50    | 1220.50           |

Scenario: The last time series data entry is deleted
  When the time series data entry with id 101 is deleted
  And the time series data entry with id 102 is deleted
  Then the system holds no time series data

Scenario: A deleted time series data entry cannot be fetched
  When the time series data entry with id 101 is deleted
  And the time series data entry with id 101 is fetched
  Then no time series data entry is found

Scenario: A deleted time series data entry cannot be updated
  When the time series data entry with id 101 is deleted
  And the time series data entry with id 101 is updated with the following values
    | Production | Consumption | Cross-border flow |
    | 4601.10    | 4015.00     | -586.10           |
  Then no time series data entry is found
  And the system holds the following time series data
    | Id  | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 102 | 2026-11-17T10:00:00Z | SE3          | 9120.00    | 10340.50    | 1220.50           |

Scenario: A deleted time series data entry cannot be deleted again
  When the time series data entry with id 101 is deleted
  And the time series data entry with id 101 is deleted
  Then no time series data entry is found
  And the system holds the following time series data
    | Id  | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 102 | 2026-11-17T10:00:00Z | SE3          | 9120.00    | 10340.50    | 1220.50           |

Scenario: No time series data entry has the id
  When the time series data entry with id 999 is deleted
  Then no time series data entry is found
  And the system holds the following time series data
    | Id  | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 101 | 2026-11-17T10:00:00Z | NO1          | 4512.75    | 3980.20     | -532.55           |
    | 102 | 2026-11-17T10:00:00Z | SE3          | 9120.00    | 10340.50    | 1220.50           |
