using OAProjects.Models.ShowLogger.Models.Show;
using OAProjects.Models.ShowLogger.Models.Stat;

namespace OAProjects.Models.ShowLogger.Responses.Stat;

public class StatYearStatResponse
{
    public IEnumerable<YearStatModel> YearStats { get; set; }

    public int Count { get; set; }
}
