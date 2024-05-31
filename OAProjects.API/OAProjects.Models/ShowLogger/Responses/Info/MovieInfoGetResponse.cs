using OAProjects.Models.ShowLogger.Models.Info;

namespace OAProjects.Models.ShowLogger.Responses.Info;

public class MovieInfoGetResponse
{
    public IEnumerable<MovieInfoModel> MovieInfos { get; set; }

    public int Count { get; set; }
}
