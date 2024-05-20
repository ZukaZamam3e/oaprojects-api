using OAProjects.Models.ShowLogger.Models.Info;

namespace OAProjects.Models.ShowLogger.Responses.Info;

public class TvInfoGetResponse
{
    public IEnumerable<TvInfoModel> TvInfos { get; set; }

    public int Count { get; set; }
}
