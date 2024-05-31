using OAProjects.Models.ShowLogger.Models.CodeValue;
using OAProjects.Models.ShowLogger.Models.WatchList;

namespace OAProjects.Models.ShowLogger.Responses.WatchList;

public class WatchListLoadResponse
{
    public IEnumerable<SLCodeValueSimpleModel> ShowTypeIds { get; set; }

    public IEnumerable<DetailedWatchListModel> WatchLists { get; set; }

    public int Count { get; set; }
}
