using System.ComponentModel.DataAnnotations.Schema;

namespace OAProjects.Data.ShowLogger.Entities;
public class SL_WATCHLIST
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int WATCHLIST_ID { get; set; }

    public int USER_ID { get; set; }

    public string SHOW_NAME { get; set; }

    public int SHOW_TYPE_ID { get; set; }

    public int? SEASON_NUMBER { get; set; }

    public int? EPISODE_NUMBER { get; set; }

    public DateTime DATE_ADDED { get; set; }

    public string? SHOW_NOTES { get; set; }

    public int? INFO_ID { get; set; }
}
