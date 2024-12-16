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

public enum CL_RoleIds
{
    ADMIN = 1000,
    EDIT = 1001,
    READ = 1002
}

public enum CL_Status
{
    PENDING = 0,
    APPROVED = 1,
}
