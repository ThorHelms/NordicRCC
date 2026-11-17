# NordicRCC

A web API for managing time series data per bidding zone.

## Project structure

- `Domain` – business logic and validation for time series data.
- `Infrastructure` – data access, using Entity Framework Core with an in-memory database.
- `Application.Api` – the ASP.NET Core minimal API that exposes the endpoints.
- `AcceptanceTests` – Reqnroll acceptance tests that run against the API.

## Prerequisites

- [.NET 11 SDK](https://dotnet.microsoft.com/download)
- Optional: [Docker](https://www.docker.com/), to run the API in a container

No database is needed: the API uses an in-memory database, so all data is lost when it stops.

## Running the API

From the repository root, the easiest way to run the API is:

```sh
dotnet watch --project Application.Api
```

This starts the API on http://localhost:5276, automatically opens the interactive API documentation in your browser, and reloads the API when the code changes.

Alternatively, run the API without opening a browser or reloading on changes:

```sh
dotnet run --project Application.Api
```

The API documentation is then available at http://localhost:5276/scalar.

To serve HTTPS as well (on https://localhost:7080), use the `https` launch profile:

```sh
dotnet run --project Application.Api --launch-profile https
```

## Running the tests

From the repository root:

```sh
dotnet test
```

The acceptance tests are written in Gherkin (`AcceptanceTests/Features`) using Reqnroll and TUnit, and host the API in memory, so nothing needs to be running beforehand.

## Development notes

During development, some assumptions were made, largely regarding the domain. These include the following:

- Time series data can be expressed as a single entity
- Cross-border electricity flow can be expressed as a single value
- Only some bidding zones are legal values in the system

Noteworthy architecture details:

- Three layer architecture, with basically input (Application.Api), logic (Domain), and output (Infrastructure)
- Testing with Gherkin feature files, via ReqNRoll
- In-memory database for now, to make it easier to run the system
- I have assumed a SQL database is used, with Entity Framework Core as ORM

As a final note: The system has been developed with the help of an AI, but the architecture and development process follows my personal
preferences regardless. In particular, the development process is Acceptance Test Driven Development, where the tests are writting in Gherkin
before any implementation, and the test implementation is following the _protocol driver_ pattern.
