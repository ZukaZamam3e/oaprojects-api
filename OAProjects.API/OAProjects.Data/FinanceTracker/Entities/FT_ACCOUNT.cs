using System.ComponentModel.DataAnnotations.Schema;

namespace OAProjects.Data.FinanceTracker.Entities;
public class FT_ACCOUNT
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ACCOUNT_ID { get; set; }

    public int USER_ID { get; set; }

    public string ACCOUNT_NAME { get; set; }

    public bool DEFAULT_INDC { get; set; }

}
