using System.ComponentModel.DataAnnotations.Schema;

namespace OAProjects.Data.CatanLogger.Entities;
public class CL_GROUP_USER
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int GROUP_USER_ID { get; set; }

    public int GROUP_ID { get; set; }

    public int USER_ID { get; set; }

    public int ROLE_ID { get; set; }

    public DateTime DATE_ADDED { get; set; }

    public int GROUP_USER_STATUS { get; set; }

    public int CONFIRMED_USER_ID { get; set; }

    public CL_GROUP GROUP { get; set; }
}
