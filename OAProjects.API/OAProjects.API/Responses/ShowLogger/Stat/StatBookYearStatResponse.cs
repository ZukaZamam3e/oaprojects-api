using OAProjects.Models.ShowLogger.Models.Show;
using OAProjects.Models.ShowLogger.Models.Stat;

namespace OAProjects.API.Responses.ShowLogger.Stat;

public class StatBookYearStatResponse
{
    public IEnumerable<BookYearStatModel> BookYearStats { get; set; }

    public int Count { get; set; }
}
