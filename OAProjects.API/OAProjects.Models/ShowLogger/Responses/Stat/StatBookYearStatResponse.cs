using OAProjects.Models.ShowLogger.Models.Show;
using OAProjects.Models.ShowLogger.Models.Stat;

namespace OAProjects.Models.ShowLogger.Responses.Stat;

public class StatBookYearStatResponse
{
    public IEnumerable<BookYearStatModel> BookYearStats { get; set; }

    public int Count { get; set; }
}
