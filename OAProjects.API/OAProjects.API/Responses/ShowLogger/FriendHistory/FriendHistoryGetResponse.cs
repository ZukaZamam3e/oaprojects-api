using OAProjects.Models.ShowLogger.Models.FriendHistory;
using OAProjects.Models.ShowLogger.Models.Stat;

namespace OAProjects.API.Responses.ShowLogger.FriendHistory;

public class FriendHistoryGetResponse
{
    public IEnumerable<FriendHistoryModel> FriendHistory { get; set; }

    public int Count { get; set; }
}
