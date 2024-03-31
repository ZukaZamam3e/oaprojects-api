using System.ComponentModel.DataAnnotations.Schema;

namespace OAProjects.Data.ShowLogger.Entities;
public class SL_TV_EPISODE_INFO
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TV_EPISODE_INFO_ID { get; set; }

    public int TV_INFO_ID { get; set; }

    public int? API_TYPE { get; set; }

    public string? API_ID { get; set; }

    public string? SEASON_NAME { get; set; }

    public string? EPISODE_NAME { get; set; }

    public int? SEASON_NUMBER { get; set; }

    public int? EPISODE_NUMBER { get; set; }

    public string? EPISODE_OVERVIEW { get; set; }

    public int? RUNTIME { get; set; }

    public DateTime? AIR_DATE { get; set; }

    public string? IMAGE_URL { get; set; }

    public SL_TV_INFO TV_INFO { get; set; }
}
