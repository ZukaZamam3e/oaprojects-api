﻿using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OAProjects.Data.ShowLogger.Context;
using OAProjects.Store.ShowLogger.Stores;
using OAProjects.Store.ShowLogger.Stores.Interfaces;
using OAProjects.Models.ShowLogger.Models.Show;
using OAProjects.API.Validators.ShowLogger.Show;
using OAProjects.API.Requests.Friend;
using OAProjects.API.Validators.ShowLogger.Friend;
using OAProjects.Models.ShowLogger.Models.Friend;
using OAProjects.API.Validators.ShowLogger.WatchList;
using OAProjects.API.Requests.WatchList;
using OAProjects.Models.ShowLogger.Models.WatchList;
using OAProjects.Models.ShowLogger.Models.Config;
using OAProjects.Models.ShowLogger.Models.Info;
using OAProjects.API.Validators.ShowLogger.Info;
using OAProjects.API.Responses.ShowLogger.Info;
using OAProjects.API.Validators.ShowLogger.UnlinkedShows;
using OAProjects.Models.ShowLogger.Models.UnlinkedShow;
using OAProjects.API.Requests.Show;
using OAProjects.Models.ShowLogger.Models.Transaction;
using OAProjects.API.Requests.Transaction;
using OAProjects.API.Validators.ShowLogger.Transaction;

namespace OAProjects.API.Setup;

public static class ShowLoggerSetup
{
    public static IServiceCollection AddShowLoggerDb(this IServiceCollection services, ConfigurationManager configuration)
    {
        string? showLoggerConnectionString = configuration.GetConnectionString("ShowLoggerConnection");
        services.AddDbContext<ShowLoggerDbContext>(m => m.UseSqlServer(showLoggerConnectionString, m => m.MigrationsHistoryTable("__SL_EFMigrationsHistory")), ServiceLifetime.Transient);
        services.AddTransient<IShowStore, ShowStore>();
        services.AddTransient<IFriendStore, FriendStore>();
        services.AddTransient<IWatchListStore, WatchListStore>();
        services.AddTransient<ICodeValueStore, CodeValueStore>();
        services.AddTransient<IInfoStore, InfoStore>();
        services.AddTransient<IUnlinkedShowStore, UnlinkedShowStore>();
        services.AddTransient<ITransactionStore, TransactionStore>();
        services.AddTransient<IStatStore, StatStore>();

        services.AddScoped<IValidator<ShowModel>, ShowValidator>();
        services.AddScoped<IValidator<ShowTransactionModel>, ShowTransactionValidator>();
        services.AddScoped<IValidator<ShowIdRequest>, ShowIdValidator>();
        services.AddScoped<IValidator<AddRangeModel>, AddRangeValidator>();
        services.AddScoped<IValidator<ShowAddNextEpisodeRequest>, ShowAddNextEspisodeValidator>();
        services.AddScoped<IValidator<AddWatchFromSearchModel>, AddWatchFromSearchValidator>();

        services.AddScoped<IValidator<TransactionModel>, TransactionValidator>();
        services.AddScoped<IValidator<TransactionIdRequest>, TransactionIdValidator>();

        services.AddScoped<IValidator<FriendIdRequest>, FriendIdValidator>();
        services.AddScoped<IValidator<FriendRequestIdRequest>, FriendRequestIdValidator>();
        services.AddScoped<IValidator<AddFriendModel>, AddFriendValidator>();

        services.AddScoped<IValidator<WatchListIdRequest>, WatchListIdValidator>();
        services.AddScoped<IValidator<WatchListMoveToShowsRequest>, WatchListMoveToShowsValidator>();
        services.AddScoped<IValidator<WatchListModel>, WatchListValidator>();

        services.AddScoped<IValidator<InfoApiSearchModel>, InfoApiSearchValidator>();
        services.AddScoped<IValidator<InfoApiDownloadModel>, InfoApiDownloadValidator>();
        services.AddScoped<IValidator<TvInfoIdRequest>, TvInfoIdValidator>();
        services.AddScoped<IValidator<MovieInfoIdRequest>, MovieInfoIdValidator>();

        services.AddScoped<IValidator<UpdateUnlinkedNameModel>, UpdateUnlinkedNameValidator>();
        services.AddScoped<IValidator<LinkShowModel>, LinkShowValidator>();

        return services;
    }
}
