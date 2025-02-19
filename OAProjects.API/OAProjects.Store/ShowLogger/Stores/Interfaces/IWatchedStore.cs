using OAProjects.Models.ShowLogger.Models.Watched;
using System.Linq.Expressions;

namespace OAProjects.Store.ShowLogger.Stores.Interfaces;
public interface IWatchedStore
{
    IEnumerable<WatchedModel> GetWatchedTV(int userId, Expression<Func<WatchedModel, bool>>? predicate = null);

    IEnumerable<WatchedModel> GetWatchedMovies(int userId, Expression<Func<WatchedModel, bool>>? predicate = null);

    IEnumerable<WatchedModel> GetWatched(Expression<Func<WatchedModel, bool>>? predicate = null);

    int CreateWatched(int userId, WatchedModel model, int? infoId = null);

    int UpdateWatched(int userId, WatchedModel model);

    bool DeleteWatched(int userId, int watchListId);
}
