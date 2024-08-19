using OAProjects.Models.ShowLogger.Models.FriendHistory;

namespace OAProjects.Models.ShowLogger.Responses.FriendHistory;
public class BookFriendHistoryResponse
{
    public IEnumerable<BookFriendHistoryModel> BookFriendHistory { get; set; }

    public int Count { get; set; }
}
