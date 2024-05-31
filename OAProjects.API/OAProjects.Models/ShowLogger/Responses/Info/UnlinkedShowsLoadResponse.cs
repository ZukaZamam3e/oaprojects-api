using OAProjects.Models.ShowLogger.Models.UnlinkedShow;

namespace OAProjects.Models.ShowLogger.Responses.Info;

public class UnlinkedShowsLoadResponse
{
    public IEnumerable<UnlinkedShowModel> UnlinkedShows { get; set; }

    public int Count { get; set; }
}
