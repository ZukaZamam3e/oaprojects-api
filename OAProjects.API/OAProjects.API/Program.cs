using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using OAProjects.API.Requirements;
using OAProjects.API.Setup;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOAIdentityDb(builder.Configuration);
builder.Services.AddShowLoggerDb(builder.Configuration);

string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
string domain = $"https://{builder.Configuration["Auth0:Domain"]}/";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = domain;
        options.Audience = builder.Configuration["Auth0:Audience"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = ClaimTypes.NameIdentifier
        };
    });
    //.AddMicrosoftIdentityWebApi(options =>
    //{
    //    builder.Configuration.Bind("AzureAd", options);
    //    options.TokenValidationParameters.NameClaimType = "name";
    //}, options => { builder.Configuration.Bind("AzureAd", options); });

//builder.Services.AddAuthorization(config =>
//{
//    config.AddPolicy("AuthZPolicy", policyBuilder =>
//            policyBuilder.Requirements.Add(new ScopeAuthorizationRequirement() { RequiredScopesConfigurationKey = $"AzureAd:Scopes" }));
//});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("User.ReadWrite", policy => policy.Requirements.Add(new HasScopeRequirement("User.ReadWrite", domain)));
});

builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        builder =>
        {
            builder.WithOrigins("https://oaprojects.net", "http://localhost:3000")
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

string listeningPort = builder.Configuration.GetValue<string>("ListeningPort");

app.Urls.Add(listeningPort);

app.Run();