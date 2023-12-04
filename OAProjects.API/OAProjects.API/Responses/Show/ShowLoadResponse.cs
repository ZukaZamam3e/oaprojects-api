using OAProjects.Models.ShowLogger;
using OAProjects.Models.ShowLogger.Models;

namespace OAProjects.API.Responses.Show;

public class ShowLoadResponse
{
    public IEnumerable<SLCodeValueSimpleModel> ShowTypeIds { get; set; }

    public IEnumerable<ShowModel> Shows { get; set; }

    public int Count { get; set; }
}
