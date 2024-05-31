using OAProjects.Models.ShowLogger.Models.Show;
using OAProjects.Models.ShowLogger.Models.Stat;

namespace OAProjects.Models.ShowLogger.Responses.Stat;

public class StatTvStatResponse
{
    public IEnumerable<TvStatModel> TvStats { get; set; }

    public int Count { get; set; }
}
