namespace OAProjects.Models.ShowLogger.Models.Show;
public class ShowInfoModel : DetailedShowModel
{
    public int? InfoSeasonNumber { get; set; }
    public int? InfoEpisodeNumber { get; set; }
    public int? InfoApiType { get; set; }
    public string? InfoApiId { get; set; }
    public string? InfoImageUrl { get; set; }
}
