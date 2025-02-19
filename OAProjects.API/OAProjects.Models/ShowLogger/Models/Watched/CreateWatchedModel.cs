using OAProjects.Models.ShowLogger.Models.Info;

namespace OAProjects.Models.ShowLogger.Models.Watched;
public class CreateWatchedModel
{
    public DateTime? DateWatched { get; set; }

    public INFO_API API { get; set; }

    public INFO_TYPE Type { get; set; }

    public string Id { get; set; }
}
