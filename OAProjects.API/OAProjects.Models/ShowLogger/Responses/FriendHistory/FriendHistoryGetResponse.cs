using OAProjects.Models.ShowLogger.Models.FriendHistory;
using OAProjects.Models.ShowLogger.Models.Stat;

namespace OAProjects.Models.ShowLogger.Responses.FriendHistory;

public class FriendHistoryGetResponse
{
    public IEnumerable<FriendHistoryModel> FriendHistory { get; set; }

    public int Count { get; set; }
}
