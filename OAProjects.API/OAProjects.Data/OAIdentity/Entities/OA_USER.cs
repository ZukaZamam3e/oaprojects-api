using System.ComponentModel.DataAnnotations.Schema;

namespace OAProjects.Data.OAIdentity.Entities;
public class OA_USER
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int USER_ID { get; set; }

    public string USER_GUID { get; set; }

    public string USER_NAME { get; set; }

    public string USER_LOGIN_TYPE { get; set; }

    public DateTime DATE_ADDED { get; set; }
}
