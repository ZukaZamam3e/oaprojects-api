using OAProjects.Models.ShowLogger.Models.FriendHistory;
using OAProjects.Store.Stores.Interfaces;

namespace OAProjects.Store.ShowLogger.Stores.Interfaces;
public interface IFriendHistoryStore : IStore
{
    IEnumerable<ShowFriendHistoryModel> GetShowFriendHistory(int userId, Dictionary<int, string> users);

    IEnumerable<BookFriendHistoryModel> GetBookFriendHistory(int userId, Dictionary<int, string> users);

}
