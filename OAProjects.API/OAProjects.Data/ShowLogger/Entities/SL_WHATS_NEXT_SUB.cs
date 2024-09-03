using System.ComponentModel.DataAnnotations.Schema;

namespace OAProjects.Data.ShowLogger.Entities;
public class SL_WHATS_NEXT_SUB
{
    public int WHATS_NEXT_SUB_ID { get; set; }

    public int USER_ID { get; set; }

    public int TV_INFO_ID { get; set; }

    public bool INCLUDE_SPECIALS { get; set; }

    public DateTime SUBSCRIBE_DATE { get; set; }
}
