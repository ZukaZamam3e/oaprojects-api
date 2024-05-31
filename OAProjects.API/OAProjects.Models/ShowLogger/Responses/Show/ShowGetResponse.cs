using OAProjects.Models.ShowLogger.Models.Show;

namespace OAProjects.Models.ShowLogger.Responses.Show;

public class ShowGetResponse
{
    public IEnumerable<DetailedShowModel> Shows { get; set; }

    public int Count { get; set; }
}
