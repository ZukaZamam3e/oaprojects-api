namespace OAProjects.Models.ShowLogger.Models.WhatsNext;
public class WhatsNextWatchEpisodeModel
{
    public int WhatsNextSubId { get; set; }

    public int UserId { get; set; }

    public int TvInfoId { get; set; }

    public string ShowName { get; set; }

    public DateTime SubscribeDate { get; set; }

    public bool IncludeSpecials { get; set; }

    public DateTime LastDataRefresh { get; set; }

    public string? PosterUrl { get; set; }

    public string? BackdropUrl { get; set; }

    public string? InfoUrl { get; set; }

    public string? Status { get; set; }
}
