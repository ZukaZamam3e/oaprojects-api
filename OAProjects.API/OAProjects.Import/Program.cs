// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OAProjects.Data.OAIdentity.Context;
using OAProjects.Data.ShowLogger.Context;
using OAProjects.Import;
using OAProjects.Import.Config;
using OAProjects.Import.Imports;
using System.Diagnostics;

Console.WriteLine("Hello, World!");
HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

var configBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false);

if (Debugger.IsAttached)
{
    //configBuilder.AddJsonFile("appsettings.local.json");
    configBuilder.AddJsonFile("appsettings.development.json");
}

IConfiguration config = configBuilder.Build();

string? showLoggerConnectionString = config.GetConnectionString("ShowLoggerConnection");
builder.Services.AddDbContext<ShowLoggerDbContext>(m => m.UseSqlServer(showLoggerConnectionString, m => m.MigrationsHistoryTable("__SL_EFMigrationsHistory")), ServiceLifetime.Transient);

string? oaIdentityConnectionString = config.GetConnectionString("OAIdentityConnection");
builder.Services.AddDbContext<OAIdentityDbContext>(m => m.UseSqlServer(oaIdentityConnectionString, m => m.MigrationsHistoryTable("__OA_EFMigrationsHistory")), ServiceLifetime.Transient);

DataConfig dataConfig = new DataConfig();
config.GetSection("Data").Bind(dataConfig);
builder.Services.AddSingleton(dataConfig);

ApiConfig apiConfig = new ApiConfig();
config.GetSection("Apis").Bind(apiConfig);
builder.Services.AddSingleton(apiConfig);

builder.Services.AddTransient<IRestartImport, RestartImport>();
builder.Services.AddTransient<IUserImport, UserImport>();
builder.Services.AddTransient<IInfoImport, InfoImport>();
builder.Services.AddTransient<IShowImport, ShowImport>();
builder.Services.AddTransient<ITransactionImport, TransactionImport>();
builder.Services.AddTransient<IFriendImport, FriendImport>();
builder.Services.AddTransient<IUserPrefImport, UserPrefImport>();
builder.Services.AddTransient<IWatchListImport, WatchListImport>();
builder.Services.AddTransient<IBookImport, BookImport>();
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

   