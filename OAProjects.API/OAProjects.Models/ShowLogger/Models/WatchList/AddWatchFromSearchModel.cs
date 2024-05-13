using OAProjects.Models.ShowLogger.Models.Info;

namespace OAProjects.Models.ShowLogger.Models.WatchList;
public class AddWatchListFromSearchModel
{
    public INFO_API API { get; set; }

    public INFO_TYPE Type { get; set; }

    public string Id { get; set; }

    public string ShowName { get; set; }

    public int ShowTypeId { get; set; }

    public int? SeasonNumber { get; set; }

    public int? EpisodeNumber { get; set; }

    public DateTime DateAdded { get; set; }

    public string? ShowNotes { get; set; }

}
