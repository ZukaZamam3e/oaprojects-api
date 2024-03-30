using OAProjects.Models.ShowLogger.Models.Friend;

namespace OAProjects.API.Responses.Friend;

public class FriendLoadResponse
{
    public IEnumerable<FriendModel> Friends { get; set; }

    public int Count { get; set; }
}
