using OAProjects.Models.ShowLogger.Models.FriendHistory;
using OAProjects.Models.ShowLogger.Models.Stat;

namespace OAProjects.Models.ShowLogger.Responses.FriendHistory;

public class ShowFriendHistoryGetResponse
{
    public IEnumerable<ShowFriendHistoryModel> ShowFriendHistory { get; set; }

    public int Count { get; set; }
}
