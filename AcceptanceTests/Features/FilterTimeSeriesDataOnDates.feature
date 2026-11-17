@Api
Feature: Filtering time series data on dates
  Time series data describes the electricity production, consumption and
  cross-border electricity flow of a bidding zone at a point in time.
  Anyone can fetch the time series data within a period, so that they only
  receive the data that is relevant to them. The period is given by a from
  time and a to time, both of which are optional. The from time is
  inclusive and the to time is exclusive, so that consecutive periods
  never overlap.

Background:
  Given the system is running
  And the following time series data has been recorded
    | Id  | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 101 | 2026-11-17T10:00:00Z | NO1          | 4512.75    | 3980.20     | -532.55           |
    | 102 | 2026-11-17T11:00:00Z | SE3          | 9120.00    | 10340.50    | 1220.50           |
    | 103 | 2026-11-17T12:00:00Z | DK1          | 2875.40    | 2640.85     | -234.55           |
    | 104 | 2026-11-17T13:00:00Z | NO1          | 4601.10    | 4015.00     | -586.10           |

Scenario: Time series data is fetched between a from time and a to time
  When the time series data from "2026-11-17T11:00:00Z" to "2026-11-17T13:00:00Z" is fetched
  Then the following time series data is returned
    | Id  | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 102 | 2026-11-17T11:00:00Z | SE3          | 9120.00    | 10340.50    | 1220.50           |
    | 103 | 2026-11-17T12:00:00Z | DK1          | 2875.40    | 2640.85     | -234.55           |

Scenario: Time series data is fetched from a time onwards
  When the time series data from "2026-11-17T12:00:00Z" is fetched
  Then the following time series data is returned
    | Id  | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 103 | 2026-11-17T12:00:00Z | DK1          | 2875.40    | 2640.85     | -234.55           |
    | 104 | 2026-11-17T13:00:00Z | NO1          | 4601.10    | 4015.00     | -586.10           |

Scenario: Time series data is fetched up to a time
  When the time series data up to "2026-11-17T12:00:00Z" is fetched
  Then the following time series data is returned
    | Id  | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 101 | 2026-11-17T10:00:00Z | NO1          | 4512.75    | 3980.20     | -532.55           |
    | 102 | 2026-11-17T11:00:00Z | SE3          | 9120.00    | 10340.50    | 1220.50           |

Scenario: Time series data is fetched without a from time or a to time
  When all time series data is fetched
  Then the following time series data is returned
    | Id  | Timestamp            | Bidding zone | Production | Consumption | Cross-border flow |
    | 101 | 2026-11-17T10:00:00Z | NO1          | 4512.75    | 3980.20     | -532.55           |
    | 102 | 2026-11-17T11:00:00Z | SE3          | 9120.00    | 10340.50    | 1220.50           |
    | 103 | 2026-11-17T12:00:00Z | DK1          | 2875.40    | 2640.85     | -234.55           |
    | 104 | 2026-11-17T13:00:00Z | NO1          | 4601.10    | 4015.00     | -586.10           |

Scenario: No time series data has been recorded within the period
  When the time series data from "2026-11-17T11:15:00Z" to "2026-11-17T11:45:00Z" is fetched
  Then no time series data is returned
