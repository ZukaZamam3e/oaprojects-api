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

        services.AddScoped<IValidator<ShowModel>, ShowValidator>();
        services.AddScoped<IValidator<FriendIdRequest>, FriendIdValidator>();
        services.AddScoped<IValidator<FriendRequestIdRequest>, FriendRequestIdValidator>();
        services.AddScoped<IValidator<AddFriendModel>, AddFriendValidator>();

        services.AddScoped<IValidator<WatchListIdRequest>, WatchListIdValidator>();
        services.AddScoped<IValidator<WatchListModel>, WatchListValidator>();

        return services;
    }
}
