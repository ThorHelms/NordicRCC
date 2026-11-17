using NordicRCC.AcceptanceTests.Drivers.Api;
using Reqnroll;
using Reqnroll.BoDi;
using Reqnroll.UnitTestProvider;

namespace NordicRCC.AcceptanceTests.Drivers;

/// <summary>
/// Registers the drivers of the protocol chosen by <c>ACCEPTANCE_TEST_DRIVER</c> (default <c>Api</c>),
/// and skips scenarios that aren't tagged with that protocol.
/// This is the only place that refers to concrete driver classes.
/// </summary>
[Binding]
public sealed class DriverRegistration(
    IObjectContainer container,
    ScenarioContext scenarioContext,
    IUnitTestRuntimeProvider unitTestRuntimeProvider)
{
    private const string ProtocolVariable = "ACCEPTANCE_TEST_DRIVER";

    [BeforeScenario(Order = 0)]
    public void RegisterDrivers()
    {
        var protocol = SelectedProtocol();
        var tags = scenarioContext.ScenarioInfo.CombinedTags;
        var protocolNames = Enum.GetNames<DriverProtocol>();

        if (!tags.Any(tag => protocolNames.Contains(tag, StringComparer.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                $"The scenario '{scenarioContext.ScenarioInfo.Title}' isn't tagged with any protocol. " +
                $"Tag it, or its feature, with the protocols that it is implemented for: " +
                $"{string.Join(", ", protocolNames.Select(name => "@" + name))}.");
        }

        if (!tags.Contains(protocol.ToString(), StringComparer.OrdinalIgnoreCase))
        {
            unitTestRuntimeProvider.TestIgnore(
                $"The scenario isn't implemented for the {protocol} protocol.");
            return;
        }

        switch (protocol)
        {
            case DriverProtocol.Api:
                container.RegisterTypeAs<ApiSystemDriver, ISystemDriver>();
                container.RegisterTypeAs<ApiTimeSeriesDataDriver, ITimeSeriesDataDriver>();
                break;
            default:
                throw new InvalidOperationException($"No drivers are registered for the {protocol} protocol.");
        }
    }

    private static DriverProtocol SelectedProtocol()
    {
        var value = Environment.GetEnvironmentVariable(ProtocolVariable);

        if (string.IsNullOrWhiteSpace(value))
        {
            return DriverProtocol.Api;
        }

        if (Enum.TryParse<DriverProtocol>(value, ignoreCase: true, out var protocol)
            && Enum.IsDefined(protocol))
        {
            return protocol;
        }

        throw new InvalidOperationException(
            $"{ProtocolVariable} is '{value}', which isn't a supported protocol. " +
            $"Supported protocols: {string.Join(", ", Enum.GetNames<DriverProtocol>())}.");
    }
}
