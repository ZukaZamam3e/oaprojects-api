using Microsoft.EntityFrameworkCore;
using OAProjects.Data.FinanceTracker.Context;
using OAProjects.Data.ShowLogger.Context;
using OAProjects.Store.ShowLogger.Stores.Interfaces;
using OAProjects.Store.ShowLogger.Stores;
using OAProjects.Store.FinanceTracker.Stores;
using OAProjects.Store.FinanceTracker.Stores.Interfaces;

namespace OAProjects.API.Setup;

public static class FinanceTrackerSetup
{
    public static IServiceCollection AddFinanceTrackerDb(this IServiceCollection services, ConfigurationManager configuration)
    {
        string? financeTrackerConnectionString = configuration.GetConnectionString("FinanceTrackerConnection");
        services.AddDbContext<FinanceTrackerDbContext>(m => m.UseMySql(financeTrackerConnectionString, ServerVersion.AutoDetect(financeTrackerConnectionString), m => m.MigrationsHistoryTable("__FT_EFMigrationsHistory")), ServiceLifetime.Transient);

        services.AddTransient<IFTAccountStore, FTAccountStore>();
        services.AddTransient<IFTCodeValueStore, FTCodeValueStore>();
        services.AddTransient<IFTTransactionOffsetStore, FTTransactionOffsetStore>();
        services.AddTransient<IFTTransactionStore, FTTransactionStore>();

        return services;
    }
}
