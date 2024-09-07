using OAProjects.Models.ShowLogger.Models.WhatsNext;
using System.Linq.Expressions;

namespace OAProjects.Store.ShowLogger.Stores.Interfaces;
public interface IWhatsNextStore
{
    IEnumerable<WhatsNextShowModel> GetWhatsNext(int userId, Expression<Func<WhatsNextShowModel, bool>> predicate);

    IEnumerable<WhatsNextWatchEpisodeModel> GetWhatsNextSubs(Expression<Func<WhatsNextWatchEpisodeModel, bool>>? predicate);

    int CreateWhatsNextSub(int userId, WhatsNextWatchEpisodeModel model);

    int UpdateWhatsNextSub(int userId, WhatsNextWatchEpisodeModel model);

    bool DeleteWhatsNextSub(int userId, int whatsNextSubId);

    int WatchEpisode(int userId, int tvEpisodeInfoId, DateTime dateWatched);
}
