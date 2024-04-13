using OAProjects.Models.ShowLogger.Models.CodeValue;
using OAProjects.Models.ShowLogger.Models.Show;
using OAProjects.Store.Stores.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Store.ShowLogger.Stores.Interfaces;

public interface IShowStore : IStore
{
    IEnumerable<SLCodeValueModel> GetCodeValues(Expression<Func<SLCodeValueModel, bool>>? predicate = null);

    IEnumerable<DetailedShowModel> GetShows(Expression<Func<ShowInfoModel, bool>>? predicate = null);

    int CreateShow(int userId, ShowModel model, int? infoId = null);

    int UpdateShow(int userId, ShowModel model);

    int AddNextEpisode(int userId, int showId, DateTime dateWatched);

    bool DeleteShow(int userId, int showId);

    bool AddOneDay(int userId, int showId);

    bool SubtractOneDay(int userId, int showId);

    bool AddRange(int userId, AddRangeModel model);

    IEnumerable<ShowTransactionModel> GetShowTransactions(int userId, int showId);

    int DeleteAllShowTransactions(int userId, int showId);
    
    int SaveShowTransactions(int userId, int showId, DateTime dateWatched, IEnumerable<ShowTransactionModel>? transactions);

    IEnumerable<FriendWatchHistoryModel> GetFriendWatchHistory(int userId, Dictionary<int, string> users);
}
