using OAProjects.Models.ShowLogger.Models.CodeValue;
using OAProjects.Models.ShowLogger.Models.WatchList;

namespace OAProjects.API.Responses.ShowLogger.WatchList;

public class WatchListGetResponse
{
    public IEnumerable<WatchListModel> WatchLists { get; set; }

    public int Count { get; set; }
}
