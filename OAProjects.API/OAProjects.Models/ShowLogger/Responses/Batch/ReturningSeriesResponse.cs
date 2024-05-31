using OAProjects.Models.ShowLogger.Models.Batch;

namespace OAProjects.Models.ShowLogger.Responses.Batch;

public class ReturningSeriesResponse
{
    public IEnumerable<ReturningSeriesModel> ReturningSeries { get; set; }
}
