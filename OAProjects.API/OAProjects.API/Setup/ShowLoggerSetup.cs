using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OAProjects.Data.ShowLogger.Context;
using OAProjects.Models.ShowLogger.Models;
using OAProjects.API.Validators.ShowLogger;
using OAProjects.Store.ShowLogger.Stores;
using OAProjects.Store.ShowLogger.Stores.Interfaces;

namespace OAProjects.API.Setup;

public static class ShowLoggerSetup
{
    public static IServiceCollection AddShowLoggerDb(this IServiceCollection services, ConfigurationManager configuration)
    {
        string? showLoggerConnectionString = configuration.GetConnectionString("ShowLoggerConnection");
        services.AddDbContext<ShowLoggerDbContext>(m => m.UseSqlServer(showLoggerConnectionString, m => m.MigrationsHistoryTable("__SL_EFMigrationsHistory")), ServiceLifetime.Transient);
        services.AddTransient<IShowStore, ShowStore>();

        services.AddScoped<IValidator<ShowModel>, ShowValidator>();

        return services;
    }
}
