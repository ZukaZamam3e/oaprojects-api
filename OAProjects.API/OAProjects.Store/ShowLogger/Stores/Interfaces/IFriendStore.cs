using OAProjects.Models.OAIdentity;
using OAProjects.Models.ShowLogger.Models.Friend;
using OAProjects.Store.Stores.Interfaces;

namespace OAProjects.Store.ShowLogger.Stores.Interfaces;
public interface IFriendStore : IStore
{
    IEnumerable<FriendModel> GetFriends(int userId, Dictionary<int, UserModel> users);

    bool SendFriendRequest(int userId, int friendId);

    bool AcceptFriendRequest(int userId, int friendRequestId);

    bool DenyFriendRequest(int userId, int friendRequestId);

    bool DeleteFriend(int userId, int friendId);
}
