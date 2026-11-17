# Acceptance tests: agent instructions

These instructions apply to everything in the `AcceptanceTests` project. The tests use [Reqnroll](https://reqnroll.net) on [TUnit](https://tunit.dev). Run them with `dotnet test --project AcceptanceTests`.

The project follows a four-layer structure. Each layer has one job, and implementation details live only in the bottom layer:

| Layer | Folder | Knows about |
| --- | --- | --- |
| Feature files | `Features/` | The business domain only |
| Step definitions | `StepDefinitions/` | The domain, and the protocol driver interfaces |
| Protocol driver interfaces | `Drivers/` | The domain only |
| Protocol driver implementations | `Drivers/<Protocol>/`, for example `Drivers/Api/` | How to talk to the system: HTTP requests, serialization, and so on |

## 1. Feature files are implementation-agnostic

Write scenarios in the language of the business domain. Describe *what* the user or system achieves, never *how* it is done.

A feature file must not refer to:

- the API, endpoints, URLs, routes, HTTP methods, status codes, headers or JSON
- pages, screens, buttons, links, fields, forms, menus, clicks or typing
- databases, tables, queues, services or other infrastructure
- class names, method names or any other code

A good test: the same feature file must read naturally, and stay true, whether the system is driven through the API, a web UI, an AI assistant or a person at a terminal.

Bad:

```gherkin
Given the API is running
When I request the health endpoint
Then the response status code is 200
And the response body is "Healthy"
```

Good:

```gherkin
Given the system is running
When I check the health of the system
Then the system reports that it is healthy
```

Other rules:

- Tag every feature, or scenario, with the protocols that it is implemented for, such as `@Api` (see [section 4](#4-switching-protocol-drivers)). The tags are the only place where a feature file names a protocol.
- Write in the third person or as a named role (`Given an analyst has recorded time series data`), not about the test tooling.
- Keep each scenario focused on one behavior. Use `Background` for shared setup and `Scenario Outline` for variations of the same behavior.
- Express data with domain terms. Tables should have domain column names, not field or property names.

## 2. Step definitions are implementation-agnostic

Step definitions translate Gherkin into calls on a protocol driver. They must not contain implementation details.

A step definition must not:

- use `HttpClient`, `HttpResponseMessage`, `WebApplicationFactory`, Playwright/Selenium types, or any other transport or UI type
- contain URLs, routes, selectors, status codes or serialization code
- reference a concrete driver class. Inject the driver **interface** through the constructor.

A step definition should:

- parse the step's arguments into domain values
- call one driver method per step, as far as possible
- assert on the domain result returned by the driver, using TUnit assertions
- keep state between steps in the step definition class or a shared scenario context class, never in static fields

```csharp
[Binding]
public sealed class HealthStepDefinitions(IHealthDriver health)
{
    private HealthStatus _status;

    [When("I check the health of the system")]
    public async Task WhenICheckTheHealthOfTheSystem() => _status = await health.CheckHealthAsync();

    [Then("the system reports that it is healthy")]
    public async Task ThenTheSystemReportsThatItIsHealthy() =>
        await Assert.That(_status).IsEqualTo(HealthStatus.Healthy);
}
```

## 3. Protocol drivers hold the implementation details

A protocol driver is the only code that knows how to talk to the system under test.

- Define one driver interface per area of the domain in `Drivers/`, for example `IHealthDriver`. Name its methods after domain actions (`CheckHealthAsync`, `FetchAllTimeSeriesDataAsync`), not after the transport (`GetAsync`, `ClickButton`).
- Driver methods take and return domain types defined in the test project (for example `HealthStatus` or a `TimeSeriesData` record). They never return transport types such as `HttpResponseMessage` or page objects.
- Translate transport outcomes into domain outcomes inside the driver. For example, the API driver maps `200 OK` with body `Healthy` to `HealthStatus.Healthy`, and maps an expected error status to a domain result or a domain exception.
- Put each implementation in a folder named after its protocol, for example `Drivers/Api/ApiHealthDriver.cs`.
- Infrastructure shared by the drivers of one protocol lives in that protocol's folder and is injected into the drivers. For the API, `Drivers/Api/ApiHost.cs` hosts `Application.Api` in memory with `WebApplicationFactory` and provides the `HttpClient`.
- Starting the system is itself a driver action: `ISystemDriver.StartAsync`, bound to `Given the system is running`.
- Drivers and their shared infrastructure own the lifetime of what they start (in-memory host, containers, and so on) and release it in `Dispose`. Reqnroll creates one instance per scenario and disposes it when the scenario ends.
- Infrastructure that is slow to start, such as a database in a container, may be shared by the whole test run instead, and stopped in an `[AfterTestRun]` hook. Keep scenarios independent anyway: give each scenario its own data, for example its own uniquely named database, and never rely on data that another scenario created.
- When a Given step sets up data that the system doesn't let users add, such as data imported from an external source, the drivers add it to the system's database directly, through `INordicRccDbContext` in a scope created from `ApiHost.Services`, so that the scenario doesn't depend on that import. Steps that view or change data still go through the protocol.
- Assertions belong in step definitions, not in drivers. A driver may throw if the system behaves in a way it cannot translate into a domain result.

## 4. Switching protocol drivers

The protocol is chosen per test run, and the **API driver is the default**.

- The protocol is selected by the `ACCEPTANCE_TEST_DRIVER` environment variable, whose values are the members of the `DriverProtocol` enum (case-insensitive). When it is unset, `Api` is used. Unsupported values fail every scenario with a message listing the supported protocols.
- Only `Api` exists today. To add a protocol, such as `Ui` or `Mcp`, add it to `DriverProtocol`, implement every driver interface in a new `Drivers/<Protocol>/` folder, add a `case` to `DriverRegistration`, and tag the features that it implements with the protocol's name.
- A single hook class, `Drivers/DriverRegistration.cs`, reads the setting in a `[BeforeScenario]` hook and registers the matching implementation of every driver interface in Reqnroll's `IObjectContainer`, for example `container.RegisterTypeAs<ApiHealthDriver, IHealthDriver>()`. This is the only place that refers to concrete driver classes.
- When adding a new driver interface, add an implementation for the API protocol and register it for every protocol. If a protocol cannot support it yet, register an implementation that throws `NotSupportedException` with an explanatory message, rather than leaving it unregistered.
- Each feature, or scenario, is tagged with the protocols that it is implemented for, named after the members of `DriverProtocol`, such as `@Api`. `DriverRegistration` skips a scenario for a protocol that it isn't tagged with, and fails a scenario that isn't tagged with any protocol, so that no scenario is skipped by mistake. Implement a feature for every protocol where you can, and leave a tag out only when the feature makes no sense for that protocol.

Example of choosing the protocol explicitly:

```shell
ACCEPTANCE_TEST_DRIVER=Api dotnet test --project AcceptanceTests
```

## Checklist for changes

Before finishing work in this project, check that:

- [ ] no `.feature` file mentions APIs, endpoints, status codes, pages, buttons or other implementation details
- [ ] no step definition uses transport or UI types, or a concrete driver class
- [ ] every feature is tagged with the protocols that it is implemented for
- [ ] every new driver interface has an implementation for every protocol, and all are registered in `DriverRegistration`
- [ ] `dotnet test --project AcceptanceTests` passes with the default (API) driver
