namespace OAProjects.Models.ShowLogger.Models.Watched;
public class WatchedModel
{
    public int WatchedId { get; set; }

    public int InfoId { get; set; }

    public int UserId { get; set; }

    public int InfoType { get; set; }

    public string? InfoTypeIdZ { get; set; }

    public string Name { get; set; }

    public DateTime? DateWatched { get; set; }

    public string InfoUrl { get; set; }

    public string BackdropUrl { get; set; }
}

public class WatchedInfoModel : WatchedModel
{
    public int? InfoApiType { get; set; }
    public string? InfoApiId { get; set; }
    public string? InfoBackdropUrl { get; set; }
}
