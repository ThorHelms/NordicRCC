using NordicRCC.AcceptanceTests.Drivers;
using Reqnroll;

namespace NordicRCC.AcceptanceTests.StepDefinitions;

[Binding]
public sealed class SystemStepDefinitions(ISystemDriver system)
{
    [Given("the system is running")]
    public Task GivenTheSystemIsRunning() => system.StartAsync();
}
