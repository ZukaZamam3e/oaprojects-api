using Microsoft.EntityFrameworkCore;
using OAProjects.Data.FinanceTracker.Context;
using OAProjects.Store.FinanceTracker.Stores;
using OAProjects.Store.FinanceTracker.Stores.Interfaces;
using FluentValidation;
using OAProjects.Models.FinanceTracker.Models;
using OAProjects.API.Validators.FinanceTracker.Calendar;
using OAProjects.Models.FinanceTracker.Requests.Calendar;
using OAProjects.API.Validators.FinanceTracker.Account;
using OAProjects.Models.FinanceTracker.Requests.Account;

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

        services.AddScoped<IValidator<FTTransactionModel>, TransactionValidator>();
        services.AddScoped<IValidator<DeleteTransactionRequest>, DeleteTransactionValidator>();
        services.AddScoped<IValidator<AccountModel>, AccountValidator>();
        services.AddScoped<IValidator<AccountIdRequest>, AccountIdValidator>();

        return services;
    }
}
