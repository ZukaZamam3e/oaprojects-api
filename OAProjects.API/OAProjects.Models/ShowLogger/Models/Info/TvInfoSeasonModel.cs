namespace OAProjects.Models.ShowLogger.Models.Info;
public class TvInfoSeasonModel
{
    public int TvInfoId { get; set; }

    public int SeasonNumber { get; set; }

    public string SeasonName { get; set; }

    public int EpisodeCount { get; set; }

    public string SeasonNumberZ => $"Season {SeasonNumber}";
}
