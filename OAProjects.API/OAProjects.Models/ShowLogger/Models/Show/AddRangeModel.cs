namespace OAProjects.Models.ShowLogger.Models.Show;
public class AddRangeModel
{
    public string ShowName { get; set; }

    public int? SeasonNumber { get; set; }

    public int StartEpisode { get; set; }

    public int? EndEpisode { get; set; }

    public DateTime DateWatched { get; set; }
}
