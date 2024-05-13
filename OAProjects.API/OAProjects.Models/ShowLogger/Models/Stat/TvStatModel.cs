namespace OAProjects.Models.ShowLogger.Models.Stat;
public class TvStatModel
{
    public int UserId { get; set; }

    public int ShowId { get; set; }

    public string ShowName { get; set; }

    public int EpisodesWatched { get; set; }

    public int? StartingSeasonNumber { get; set; }

    public int? StartingEpisodeNumber { get; set; }

    public string StartingWatched => StartingSeasonNumber != null && StartingEpisodeNumber != null ? $"s{StartingSeasonNumber.Value.ToString().PadLeft(2, '0')}e{StartingEpisodeNumber.Value.ToString().PadLeft(2, '0')}" : "";

    public int? LatestSeasonNumber { get; set; }

    public int? LatestEpisodeNumber { get; set; }

    public string LatestWatched => LatestSeasonNumber != null && LatestEpisodeNumber != null ? $"s{LatestSeasonNumber.Value.ToString().PadLeft(2, '0')}e{LatestEpisodeNumber.Value.ToString().PadLeft(2, '0')}" : "";

    public string Watched => $"{StartingWatched} - {LatestWatched}";

    public DateTime FirstWatched { get; set; }

    public DateTime LastWatched { get; set; }

    public decimal EpisodesPerDay => Math.Max(Math.Round(EpisodesWatched / (decimal)DaysSinceStarting, 2), 1);

    public int DaysSinceStarting => Math.Max(Convert.ToInt32((LastWatched.Date - FirstWatched.Date.AddDays(-1)).TotalDays), 1);

    public int? InfoId { get; set; }

    public string? InfoBackdropUrl { get; set; }

    public string? InfoUrl { get; set; }

    public int? NextEpisodeInfoId { get; set; }

    public string? NextEpisodeName { get; set; }

    public int? NextSeasonNumber { get; set; }

    public int? NextEpisodeNumber { get; set; }

    public string? NextInfoUrl { get; set; }

    public DateTime? NextAirDate { get; set; }

    public int? EpisodesLeft { get; set; }

    public string NextWatched => NextSeasonNumber != null && NextSeasonNumber != null ? $"s{NextSeasonNumber.Value.ToString().PadLeft(2, '0')}e{NextEpisodeNumber.Value.ToString().PadLeft(2, '0')}" : "";

}
