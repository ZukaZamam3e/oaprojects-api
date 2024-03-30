using System.ComponentModel.DataAnnotations.Schema;

namespace OAProjects.Data.ShowLogger.Entities;
public class SL_FRIEND_REQUEST
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int FRIEND_REQUEST_ID { get; set; }

    public int SENT_USER_ID { get; set; }

    public int RECEIVED_USER_ID { get; set; }


    public DateTime DATE_SENT { get; set; }
}
