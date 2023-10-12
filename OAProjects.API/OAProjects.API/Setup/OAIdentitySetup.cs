using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OAProjects.Data.OAIdentity.Context;
using OAProjects.Store.OAIdentity.Stores;
using OAProjects.Store.OAIdentity.Stores.Interfaces;

namespace OAProjects.API.Setup;

public static class OAIdentitySetup
{
    public static IServiceCollection AddOAIdentityDb(this IServiceCollection services, ConfigurationManager configuration)
    {
        string? showLoggerConnectionString = configuration.GetConnectionString("OAIdentityConnection");
        services.AddDbContext<OAIdentityDbContext>(m => m.UseSqlServer(showLoggerConnectionString, m => m.MigrationsHistoryTable("__OA_EFMigrationsHistory")), ServiceLifetime.Transient);
        services.AddTransient<IUserStore, UserStore>();
        return services;
    }
}
