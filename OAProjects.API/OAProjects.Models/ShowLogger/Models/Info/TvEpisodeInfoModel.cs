namespace OAProjects.Models.ShowLogger.Models.Info;
public class TvEpisodeInfoModel
{
    public int TvEpisodeInfoId { get; set; }

    public int TvInfoId { get; set; }

    public int? ApiType { get; set; }

    public string? ApiId { get; set; }

    public string? SeasonName { get; set; }

    public string? EpisodeName { get; set; }

    public int? SeasonNumber { get; set; }

    public int? EpisodeNumber { get; set; }

    public string SeasonEpisode => SeasonNumber != null && EpisodeNumber != null ? $"s{SeasonNumber.Value.ToString().PadLeft(2, '0')}e{EpisodeNumber.Value.ToString().PadLeft(2, '0')}" : "";

    public string EpisodeOverview { get; set; }

    public int? Runtime { get; set; }

    public DateTime? AirDate { get; set; }

    public string ImageUrl { get; set; }

    public int OverallEpisodeNumber { get; set; }
}
