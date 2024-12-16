using System.ComponentModel.DataAnnotations.Schema;

namespace OAProjects.Data.CatanLogger.Entities;
public class CL_GROUP
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int GROUP_ID { get; set; }

    public string GROUP_NAME { get; set; }

    public DateTime DATE_ADDED { get; set; }

    public ICollection<CL_GAME> GAMES { get; set; }

    public ICollection<CL_GROUP_USER> GROUP_USERS { get; set; }
}
