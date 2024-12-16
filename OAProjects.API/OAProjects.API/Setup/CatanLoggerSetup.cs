using Microsoft.EntityFrameworkCore;
using OAProjects.Data.CatanLogger.Context;
using OAProjects.Store.CatanLogger.Stores.Interfaces;
using OAProjects.Store.CatanLogger.Stores;
using FluentValidation;
using OAProjects.API.Validators.CatanLogger.Game;
using OAProjects.Models.CatanLogger.Requests;
using OAProjects.Models.CatanLogger.Models;

namespace OAProjects.API.Setup;

public static class CatanLoggerSetup
{
    public static IServiceCollection AddCatanLoggerDb(this IServiceCollection services, ConfigurationManager configuration)
    {
        string? catanLoggerConnectionString = configuration.GetConnectionString("CatanLoggerConnection");
        services.AddDbContext<CatanLoggerDbContext>(m => m.UseMySql(catanLoggerConnectionString, ServerVersion.AutoDetect(catanLoggerConnectionString), m => m.MigrationsHistoryTable("__CL_EFMigrationsHistory")), ServiceLifetime.Transient);

        services.AddTransient<IGroupStore, GroupStore>();
        services.AddTransient<IGameStore, GameStore>();

        services.AddScoped<IValidator<GroupModel>, GroupValidator>();
        services.AddScoped<IValidator<GameModel>, GameValidator>();
        services.AddScoped<IValidator<GameIdRequest>, GameIdValidator>();

        return services;
    }
}
