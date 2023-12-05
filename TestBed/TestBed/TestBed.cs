using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestBed.Mocks;


namespace TestBed;


public delegate void OneTimeSetupDelegate();

public class TestBed<TEntryPoint> : WebApplicationFactory<TEntryPoint>, IDisposable 
    where TEntryPoint : class
{
    private static readonly object _lockObject = new object();
    private static bool _firstExecution = true;
    private WebApplicationFactory<TEntryPoint> _factory;

    public HttpClient Client { get; private set; }
    public IServiceProvider? Services { get; private set; }
    public OneTimeSetupDelegate OneTimeSetupHandleCallback { get; set; }
    public string AppSettingsFile { get; set; }
    public IMockStrategy[] MockServices { get; set; }


    public TestBed()
    {
        _factory = new WebApplicationFactory<TEntryPoint>();
    }

    public void Initializer() 
    {
        Client = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Test");
            builder.ConfigureTestServices(services =>
            {
                // Register fake/mocks elements dynamically
                MockServices
                    .ToList()
                    .ForEach(ms => ms.RegisterMockService(services));

                // Create custom Callback
                var scope = services.BuildServiceProvider().CreateScope();
                if (IsFirstExecution() && OneTimeSetupHandleCallback is not null)
                    OneTimeSetupHandleCallback();

                Services = scope.ServiceProvider;
            });
            builder.ConfigureAppConfiguration((context, config) =>
            {
                if (!string.IsNullOrWhiteSpace(AppSettingsFile))
                    config.AddJsonFile(AppSettingsFile);
                config.AddEnvironmentVariables();
            });
        }).CreateClient();
    }

    private static bool IsFirstExecution()
    {
        // Test can execute simultaneously, some actions as clean the DB cannot be done on each test as the DB is shared.
        lock (_lockObject)
        {
            if (_firstExecution)
            {
                _firstExecution = false;
                return true;
            }

            return false;
        }
    }

    public void Dispose()
    {
        Client?.Dispose();
        GC.SuppressFinalize(this);
    }
}