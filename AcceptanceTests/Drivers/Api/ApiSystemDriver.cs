namespace NordicRCC.AcceptanceTests.Drivers.Api;

public sealed class ApiSystemDriver(ApiHost host) : ISystemDriver
{
    public Task StartAsync()
    {
        host.Start();
        return Task.CompletedTask;
    }
}
