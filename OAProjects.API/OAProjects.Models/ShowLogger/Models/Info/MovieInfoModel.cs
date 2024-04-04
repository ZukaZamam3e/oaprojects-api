namespace OAProjects.Models.ShowLogger.Models.Info;
public class MovieInfoModel
{
    public int MovieInfoId { get; set; }

    public string MovieName { get; set; }

    public string? MovieOverview { get; set; }

    public int? ApiType { get; set; }

    public string? ApiId { get; set; }

    public int? Runtime { get; set; }

    public DateTime? AirDate { get; set; }

    public DateTime LastDataRefresh { get; set; }

    public DateTime LastUpdated { get; set; }

    public string? ImageURL { get; set; }
}
