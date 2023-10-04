namespace OAProjects.Models.ShowLogger;

public class ShowModel
{
    public int UserId { get; set; }

    public int ShowId { get; set; }

    public string ShowName { get; set; }

    public int ShowTypeId { get; set; }

    public string? ShowTypeIdZ { get; set; }

    public int? SeasonNumber { get; set; }

    public int? EpisodeNumber { get; set; }

    public DateTime DateWatched { get; set; }

    public string? ShowNotes { get; set; }
}
