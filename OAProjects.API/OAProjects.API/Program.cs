using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using OAProjects.API.Middleware;
using OAProjects.API.Requirements;
using OAProjects.API.Setup;
using OAProjects.Models.ShowLogger.Models.Config;
using Scalar.AspNetCore;
using Serilog;
using System.Diagnostics;
using System.Security.Claims;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

if (Debugger.IsAttached)
{
    builder.Configuration.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);
}

// Configure Serilog with LogBaseDirectory
var logBaseDirectory = builder.Configuration.GetValue<string>("LogBaseDirectory");

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
    .WriteTo.File(Path.Join(logBaseDirectory, "log-.txt"), rollingInterval: RollingInterval.Day));

var config = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add global exception handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddOAIdentityDb(builder.Configuration);
builder.Services.AddShowLoggerDb(builder.Configuration);
builder.Services.AddFinanceTrackerDb(builder.Configuration);

ApisConfig apisConfig = new ApisConfig();
builder.Configuration.GetSection("Apis").Bind(apisConfig);
builder.Services.AddSingleton(apisConfig);

Auth0Config auth0APIConfig = new Auth0Config();
config.GetSection("Auth0").Bind(auth0APIConfig);
builder.Services.AddSingleton(auth0APIConfig);

string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
string domain = $"https://{builder.Configuration["Auth0:Domain"]}/";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
{
    options.Authority = domain;
    options.Audience = builder.Configuration["Auth0:Audience"];
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = ClaimTypes.NameIdentifier,
        ValidateLifetime = !Debugger.IsAttached
    };
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("User.ReadWrite", policy => policy.Requirements.Add(new HasScopeRequirement("User.ReadWrite", domain)))
    .AddPolicy("Batch.ReadWrite", policy => policy.Requirements.Add(new HasScopeRequirement("Batch.ReadWrite", domain)))
    .AddPolicy("Info.ReadWrite", policy => policy.Requirements.Add(new HasScopeRequirement("Info.ReadWrite", domain)));

builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        builder =>
        {
            builder.WithOrigins("https://show-logger.oaprojects.net", "https://finance-tracker.oaprojects.net", "https://oaprojects.net", "http://localhost:5173")
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

builder.Services.AddHttpClient("Auth0", httpClient =>
{
    httpClient.BaseAddress = new Uri(builder.Configuration["Auth0:Url"]);
});

builder.Services.AddMemoryCache();

var app = builder.Build();

// Add Serilog request logging
app.UseSerilogRequestLogging();

// Add global exception handler
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "openapi/{documentName}.json";
    });
    //app.UseSwaggerUI();

    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("OA Projects API")
            .WithTheme(ScalarTheme.DeepSpace)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

string listeningPort = builder.Configuration.GetValue<string>("ListeningPort");

app.Urls.Add(listeningPort);

try
{
    Log.Information("Starting OAProjects API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}