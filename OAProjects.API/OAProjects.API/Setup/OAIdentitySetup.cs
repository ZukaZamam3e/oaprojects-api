using Microsoft.EntityFrameworkCore;
using OAProjects.Data.OAIdentity.Context;

namespace OAProjects.API.Setup;

public static class OAIdentitySetup
{
    public static IServiceCollection AddOAIdentityDb(this IServiceCollection services, ConfigurationManager configuration)
    {
        string? showLoggerConnectionString = configuration.GetConnectionString("OAIdentityConnection");
        services.AddDbContext<OAIdentityDbContext>(m => m.UseSqlServer(showLoggerConnectionString), ServiceLifetime.Transient);


        return services;
    }
}
