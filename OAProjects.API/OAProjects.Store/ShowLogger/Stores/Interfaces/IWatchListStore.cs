using OAProjects.Models.ShowLogger.Models.Show;
using OAProjects.Models.ShowLogger;
using System.Linq.Expressions;
using OAProjects.Models.ShowLogger.Models.WatchList;

namespace OAProjects.Store.ShowLogger.Stores.Interfaces;
public interface IWatchListStore
{
    IEnumerable<WatchListModel> GetWatchLists(Expression<Func<WatchListModel, bool>>? predicate = null);

    IEnumerable<WatchListModel> SearchWatchLists(int userId, string text);

    int CreateWatchList(int userId, WatchListModel model);

    int UpdateWatchList(int userId, WatchListModel model);

    bool DeleteWatchList(int userId, int watchListId);

    bool MoveToShows(int userId, int watchListId, DateTime dateWatched);
}
