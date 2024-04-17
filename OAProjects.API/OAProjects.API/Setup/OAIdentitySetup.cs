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
        string? oaIdentityConnectionString = configuration.GetConnectionString("OAIdentityConnection");
        services.AddDbContext<OAIdentityDbContext>(m => m.UseSqlServer(oaIdentityConnectionString, m => m.MigrationsHistoryTable("__OA_EFMigrationsHistory")), ServiceLifetime.Transient);
        services.AddTransient<IUserStore, UserStore>();
        return services;
    }
}
