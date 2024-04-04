using OAProjects.Models.ShowLogger.Models.Show;

namespace OAProjects.API.Responses.ShowLogger.Show;

public class ShowGetResponse
{
    public IEnumerable<ShowModel> Shows { get; set; }

    public int Count { get; set; }
}
