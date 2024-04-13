using OAProjects.Models.ShowLogger.Models.Show;
using OAProjects.Models.ShowLogger.Models.Stat;

namespace OAProjects.API.Responses.ShowLogger.Stat;

public class StatYearStatResponse
{
    public IEnumerable<YearStatModel> YearStats { get; set; }

    public int Count { get; set; }
}
