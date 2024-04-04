using System.ComponentModel.DataAnnotations.Schema;

namespace OAProjects.Data.ShowLogger.Entities;
public class SL_TV_INFO
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TV_INFO_ID { get; set; }

    public string SHOW_NAME { get; set; }

    public string SHOW_OVERVIEW { get; set; }

    public int? API_TYPE { get; set; }

    public string? API_ID { get; set; }

    public DateTime LAST_DATA_REFRESH { get; set; }

    public DateTime LAST_UPDATED { get; set; }

    public string? IMAGE_URL { get; set; }

    public ICollection<SL_TV_EPISODE_INFO> EPISODE_INFOS { get; set; }
}
