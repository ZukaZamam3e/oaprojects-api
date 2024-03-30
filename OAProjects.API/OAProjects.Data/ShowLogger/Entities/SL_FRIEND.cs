using System.ComponentModel.DataAnnotations.Schema;

namespace OAProjects.Data.ShowLogger.Entities;
public class SL_FRIEND
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int FRIEND_ID { get; set; }

    public int USER_ID { get; set; }

    public int FRIEND_USER_ID { get; set; }

    public DateTime CREATED_DATE { get; set; }
}

