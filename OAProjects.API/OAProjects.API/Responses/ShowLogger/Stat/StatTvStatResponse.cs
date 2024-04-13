using OAProjects.Models.ShowLogger.Models.Show;
using OAProjects.Models.ShowLogger.Models.Stat;

namespace OAProjects.API.Responses.ShowLogger.Stat;

public class StatTvStatResponse
{
    public IEnumerable<TvStatModel> TvStats { get; set; }

    public int Count { get; set; }
}
