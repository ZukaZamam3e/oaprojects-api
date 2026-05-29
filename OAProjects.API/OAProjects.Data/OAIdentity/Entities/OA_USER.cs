using System.ComponentModel.DataAnnotations.Schema;

namespace OAProjects.Data.OAIdentity.Entities;
public class OA_USER
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int USER_ID { get; set; }

    public string USER_NAME { get; set; }

    public string FIRST_NAME { get; set; }

    public string? LAST_NAME { get; set; }

    public string EMAIL { get; set; }

    public DateTime DATE_ADDED { get; set; }
}
