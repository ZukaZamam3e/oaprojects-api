namespace OAProjects.Models.ShowLogger.Models.UnlinkedShow;
public class UnlinkedShowModel
{
    public string ShowName { get; set; }

    public int ShowTypeId { get; set; }

    public string? ShowTypeIdZ { get; set; }

    public DateTime LastWatched { get; set; }

    public int WatchCount { get; set; }

    public DateTime? AirDate { get; set; }

    public DateTime? LastDataRefresh { get; set; }

    public int? InfoId { get; set; }

    public bool InShowLoggerIndc { get; set; }
}
