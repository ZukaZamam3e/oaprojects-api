using OAProjects.Models.ShowLogger.Models.UnlinkedShow;

namespace OAProjects.API.Responses.ShowLogger.Info;

public class UnlinkedShowsGetResponse
{
    public IEnumerable<UnlinkedShowModel> UnlinkedShows { get; set; }

    public int Count { get; set; }
}
