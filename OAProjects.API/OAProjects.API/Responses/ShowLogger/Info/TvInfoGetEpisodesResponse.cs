using OAProjects.Models.ShowLogger.Models.Info;

namespace OAProjects.API.Responses.ShowLogger.Info;

public class TvInfoGetEpisodesResponse
{
    public IEnumerable<TvEpisodeInfoModel> TvEpisodeInfos { get; set; }

    public int Count { get; set; }
}
