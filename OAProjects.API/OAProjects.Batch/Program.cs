// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OAProjects.Batch;
using OAProjects.Batch.Config;
using OAProjects.Batch.Processes;
using OAProjects.Batch.Processes.Interface;
using Serilog;
using Serilog.Events;
using System.Diagnostics;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

var configBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false);

if (Debugger.IsAttached)
{
    configBuilder.AddJsonFile("appsettings.local.json");
    //configBuilder.AddJsonFile("appsettings.development.json");
}

IConfiguration config = configBuilder.Build();

string? logFolderPath = config.GetValue<string>("LogFolderPath");

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .WriteTo.Console()
    .WriteTo.File(Path.Join(logFolderPath, "log-.txt"), rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

Auth0APIConfig auth0APIConfig = new Auth0APIConfig();
config.GetSection("Auth0API").Bind(auth0APIConfig);
builder.Services.AddSingleton(auth0APIConfig);

builder.Services.AddHttpClient("Auth0API", httpClient =>
{
    httpClient.BaseAddress = new Uri(auth0APIConfig.Auth0Url);
});

builder.Services.AddHttpClient("OAProjectsAPI", httpClient =>
{
    httpClient.BaseAddress = new Uri(config.GetValue<string>("OAProjectsAPI"));
    httpClient.Timeout = new TimeSpan(0, 5, 0);
});

builder.Services.AddLogging(logging =>
{
    logging.SetMinimumLevel(LogLevel.Debug);
    logging.AddSerilog(Log.Logger, true);
});

builder.Services.AddSerilog((services, lc) => lc
    .ReadFrom.Configuration(config)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.File(Path.Join(logFolderPath, "log-.txt"), rollingInterval: RollingInterval.Day)
    .WriteTo.Console());

builder.Services.AddTransient<IGetAuthTokenProcess, GetAuthTokenProcess>();
builder.Services.AddTransient<IRefreshRecurringTvShowsProcess, RefreshRecurringTvShowsProcess>();
builder.Services.AddSingleton(typeof(Serilog.ILogger), Log.Logger);

builder.Services.AddTransient<App>();

using IHost host = builder.Build();

RunApp(host.Services, args);

await host.RunAsync();

static void RunApp(IServiceProvider hostProvider, string[] arguments)
{
    using IServiceScope serviceScope = hostProvider.CreateScope();
    IServiceProvider provider = serviceScope.ServiceProvider;
    App app = provider.GetRequiredService<App>();


    app.Run(arguments);
}

