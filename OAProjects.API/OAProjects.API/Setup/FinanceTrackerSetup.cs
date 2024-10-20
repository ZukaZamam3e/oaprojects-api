using Microsoft.EntityFrameworkCore;
using OAProjects.Data.FinanceTracker.Context;
using OAProjects.Data.ShowLogger.Context;

namespace OAProjects.API.Setup;

public static class FinanceTrackerSetup
{
    public static IServiceCollection AddFinanceTrackerDb(this IServiceCollection services, ConfigurationManager configuration)
    {
        string? financeTrackerConnectionString = configuration.GetConnectionString("FinanceTrackerConnection");
        services.AddDbContext<FinanceTrackerDbContext>(m => m.UseMySql(financeTrackerConnectionString, ServerVersion.AutoDetect(financeTrackerConnectionString), m => m.MigrationsHistoryTable("__FT_EFMigrationsHistory")), ServiceLifetime.Transient);

        return services;
    }
}
