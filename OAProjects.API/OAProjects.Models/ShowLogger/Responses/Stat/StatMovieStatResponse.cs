using OAProjects.Models.ShowLogger.Models.Show;
using OAProjects.Models.ShowLogger.Models.Stat;

namespace OAProjects.Models.ShowLogger.Responses.Stat;

public class StatMovieStatResponse
{
    public IEnumerable<MovieStatModel> MovieStats { get; set; }

    public int Count { get; set; }
}
