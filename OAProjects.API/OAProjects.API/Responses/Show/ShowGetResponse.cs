using OAProjects.Models.ShowLogger.Models;

namespace OAProjects.API.Responses.Show;

public class ShowGetResponse
{
    public IEnumerable<ShowModel> Shows { get; set; }
}
