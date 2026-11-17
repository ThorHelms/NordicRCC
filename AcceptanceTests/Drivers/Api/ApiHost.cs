using Microsoft.AspNetCore.Mvc.Testing;

namespace NordicRCC.AcceptanceTests.Drivers.Api;

/// <summary>
/// Hosts <c>Application.Api</c> in memory for one scenario.
/// </summary>
public sealed class ApiHost : IDisposable
{
    private WebApplicationFactory<Program>? _factory;
    private HttpClient? _client;

    public HttpClient Client => _client ?? throw NotStarted();

    public IServiceProvider Services => _factory?.Services ?? throw NotStarted();

    public void Start()
    {
        _factory ??= new WebApplicationFactory<Program>();
        _client ??= _factory.CreateClient();
    }

    public void Dispose()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    private static InvalidOperationException NotStarted() =>
        new("The system hasn't been started. Add 'Given the system is running' to the scenario.");
}
