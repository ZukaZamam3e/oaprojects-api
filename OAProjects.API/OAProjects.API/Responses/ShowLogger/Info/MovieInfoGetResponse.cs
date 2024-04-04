using OAProjects.Models.ShowLogger.Models.Info;

namespace OAProjects.API.Responses.ShowLogger.Info;

public class MovieInfoGetResponse
{
    public IEnumerable<MovieInfoModel> MovieInfos { get; set; }

    public int Count { get; set; }
}
