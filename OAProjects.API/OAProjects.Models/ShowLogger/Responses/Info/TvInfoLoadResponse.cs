using OAProjects.Models.ShowLogger.Models.Info;

namespace OAProjects.Models.ShowLogger.Responses.Info;

public class TvInfoLoadResponse
{
    public IEnumerable<TvInfoModel> TvInfos { get; set; }

    public int Count { get; set; }
}
