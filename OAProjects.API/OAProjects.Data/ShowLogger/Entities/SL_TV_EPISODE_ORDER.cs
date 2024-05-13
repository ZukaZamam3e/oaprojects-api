using System.ComponentModel.DataAnnotations.Schema;

namespace OAProjects.Data.ShowLogger.Entities;
public class SL_TV_EPISODE_ORDER
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TV_EPISODE_ORDER_ID { get; set; }

    public int TV_INFO_ID { get; set; }

    public int TV_EPISODE_INFO_ID { get; set; }

    public int EPISODE_ORDER { get; set; }
}
