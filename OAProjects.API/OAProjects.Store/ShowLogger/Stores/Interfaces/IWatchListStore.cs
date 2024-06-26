﻿using OAProjects.Models.ShowLogger.Models.Show;
using OAProjects.Models.ShowLogger;
using System.Linq.Expressions;
using OAProjects.Models.ShowLogger.Models.WatchList;

namespace OAProjects.Store.ShowLogger.Stores.Interfaces;
public interface IWatchListStore
{
    IEnumerable<DetailedWatchListModel> GetWatchLists(Expression<Func<WatchListInfoModel, bool>>? predicate = null);

    IEnumerable<WatchListModel> SearchWatchLists(int userId, string text);

    int CreateWatchList(int userId, WatchListModel model, int? infoId = null);

    int UpdateWatchList(int userId, WatchListModel model);

    bool DeleteWatchList(int userId, int watchListId);

    bool MoveToShows(int userId, int watchListId, DateTime dateWatched);
}
