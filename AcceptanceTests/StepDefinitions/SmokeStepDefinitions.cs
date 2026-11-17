using Reqnroll;

namespace NordicRCC.AcceptanceTests.StepDefinitions;

[Binding]
public sealed class SmokeStepDefinitions
{
    private bool _executed;

    [Given("the acceptance test project is set up")]
    public void GivenTheAcceptanceTestProjectIsSetUp()
    {
    }

    [When("a scenario is executed")]
    public void WhenAScenarioIsExecuted()
    {
        _executed = true;
    }

    [Then("it passes")]
    public async Task ThenItPasses()
    {
        await Assert.That(_executed).IsTrue();
    }
}
