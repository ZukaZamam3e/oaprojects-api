using Microsoft.EntityFrameworkCore;
using OAProjects.Data.FinanceTracker.Context;
using OAProjects.Store.FinanceTracker.Stores;
using OAProjects.Store.FinanceTracker.Stores.Interfaces;
using FluentValidation;
using OAProjects.Models.FinanceTracker.Models;
using OAProjects.API.Validators.FinanceTracker.Calendar;

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

        services.AddScoped<IValidator<TransactionModel>, TransactionValidator>();

        return services;
    }
}
